namespace FunBasic.DirectX.Scene

open FunBasic.DirectX.Common
open FunBasic.DirectX.Direct2d

open SharpDX

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading

module internal SceneRenderer =
  type Vector       = float*float
  type BoundingBox  = float*float*float*float

  type Visual =
    | EmptyVisual
    | EllipseVisual   of float32*BrushId*BrushId*Direct2D1.Ellipse
    | RectangleVisual of float32*BrushId*BrushId*RectangleF
    | TextVisual      of BrushId*TextFormatId*RectangleF*string

  type VisualInput =
    | MoveVisual      of float32*float32
    | ResizeVisual    of float32*float32
    | CreateEllipse   of float32*BrushId*BrushId*Direct2D1.Ellipse
    | CreateRectangle of float32*BrushId*BrushId*RectangleF
    | CreateText      of BrushId*TextFormatId*RectangleF*string

  type Input = 
    | DoNothing
    | Exit
    | Background  of Color4
    | BrushInput  of BrushId*BrushDescriptor
    | VisualInput of VisualId*VisualInput

  let rec renderVisual (v : Visual) (d : Device) (rt : Direct2D1.RenderTarget) : unit =
    match v with
    | EmptyVisual ->
      ()
    | EllipseVisual (sw, s, f, e) ->
      let fb = d.GetBrush f
      let sb = d.GetBrush s
      if fb.IsVisible then
        rt.FillEllipse (e, fb)
      if sb.IsVisible && sw > 0.0f then
        rt.DrawEllipse (e, sb, sw)
    | RectangleVisual (sw, s, f, r) ->
      let fb = d.GetBrush f
      let sb = d.GetBrush s
      if fb.IsVisible then
        rt.FillRectangle (r, fb)
      if sb.IsVisible && sw > 0.0f then
        rt.DrawRectangle (r, sb, sw)
    | TextVisual (f, tf, r, t) ->
      let fb  = d.GetBrush f
      let tfd = d.GetTextFormat tf
      if fb.IsVisible && tfd <> null then
        rt.DrawText (t, tfd, r, fb)

  let doNothing _ = 
    ()

  let createRenderer (onRefresh :  unit -> unit) =
    let input     = ConcurrentQueue<Input> ()
    let visuals   = Dictionary<VisualId, Visual> ()
    let background= ref <| Color.Black.ToColor4 ()

    let createVisual k u = 
      match u with
      | MoveVisual _ ->
        // Not supported on creation
        EmptyVisual
      | ResizeVisual _ ->
        // Not supported on creation
        EmptyVisual
      | CreateEllipse (sw, s, f, e) ->
        EllipseVisual (sw, s, f, e)
      | CreateRectangle (sw, s, f, r) ->
        RectangleVisual (sw, s, f, r)
      | CreateText (bd, tfd, r, t) ->
        TextVisual (bd, tfd, r, t)

    let updateVisual k v u = 
      match u with
      | MoveVisual (x, y) ->
        let pos = v2 x y
        match v with
        | EmptyVisual -> 
          EmptyVisual
        | EllipseVisual (sw, s, f, r) ->
          EllipseVisual (sw, s, f, emove2 pos r)
        | RectangleVisual (sw, s, f, r) ->
          RectangleVisual (sw, s, f, rmove2 pos r)
        | TextVisual (bd, tfd, r, t) ->
          TextVisual (bd, tfd, rmove2 pos r, t)
      | ResizeVisual (w, h) ->
        let sz = v2 w h
        match v with
        | EmptyVisual -> 
          EmptyVisual
        | EllipseVisual (sw, s, f, r) ->
          EllipseVisual (sw, s, f, eresize2 sz r)
        | RectangleVisual (sw, s, f, r) ->
          RectangleVisual (sw, s, f, rresize2 sz r)
        | TextVisual (bd, tfd, r, t) ->
          TextVisual (bd, tfd, rresize2 sz r, t)
      | _ ->
        createVisual k u

    let onRender (d : Device) (rt : Direct2D1.RenderTarget) : bool =
      let mutable cont  = true
      let mutable i     = DoNothing
      while input.TryDequeue (&i) do
        match i with
        | DoNothing -> 
          ()
        | Background bkg ->
          background := bkg
        | Exit ->
          cont <- false
        | VisualInput (vid, vi) ->
          ignore <| visuals.CreateOrUpdate vid vi createVisual updateVisual
        | BrushInput (bid, bd) ->
          ignore <| d.CreateBrush bid bd

      onRefresh ()

      rt.Clear <| Nullable<_> !background

      for kv in visuals do
        renderVisual kv.Value d rt

      cont

    let uiProc () =
      try
        Window.show "FunBasic Direct2D" 1024 768 doNothing onRender
      with
      | e -> () // TODO: Trace

    let uiThread =
      let t = Thread (ThreadStart uiProc)
      t.SetApartmentState ApartmentState.STA
      t.IsBackground    <- true
      t.Start ()
      t  
    uiThread, input

open SceneRenderer

type Scene() =

  let ellipse (x : float) (y : float) (radiusX : float) (radiusY : float) : Direct2D1.Ellipse =
    Direct2D1.Ellipse (v2 (float32 x) (float32 y), float32 radiusX, float32 radiusY)

  let rect (x : float) (y : float) (w : float) (h : float) : RectangleF = 
    RectangleF (float32 x,float32 y,float32 w,float32 h)

  let refreshLock = new ManualResetEvent false

  let onRender () =
    ignore <| refreshLock.Set ()

  let renderThread, renderQueue = SceneRenderer.createRenderer onRender

  let enqueueInput i =
    renderQueue.Enqueue i

  let enqueueBrushInput bid bd =
    BrushInput (brushId bid, bd)
    |> renderQueue.Enqueue

  let enqueueVisualInput vid vi =
    VisualInput (visualId vid, vi)
    |> renderQueue.Enqueue

  let red = Color.Red

  let parseColor (color : string) : Color4 =
    let parseColorChar(ch : char) : int =
      if ch >= '0' && ch <= '9' then 
        int ch - int '0'
      else if ch >= 'A' && ch <= 'F' then
        int ch - int 'A' + 10
      else if ch >= 'a' && ch <= 'f' then
        int ch - int 'a' + 10
      else
        -1

    let inline extendColor (i : int) : int = 
      let i = 0xF &&& i
      (i <<< 4) ||| i

    let inline mergeColor (u : int) (l : int) : int = 
      let u = 0xF &&& u
      let l = 0xF &&& l
      (u <<< 4) ||| l

    let inline isOk i = i > -1

    let parse (color : string) : Color =
      if color.Length = 0 || color.[0] <> '#' then 
        red
      else
        match color.Length with
        | 4 -> 
          // #rgb
          let r = parseColorChar color.[1]
          let g = parseColorChar color.[2]
          let b = parseColorChar color.[3]
          if isOk r && isOk g && isOk b then
            Color(extendColor r, extendColor g, extendColor b)
          else
            red
        | 5 -> 
          // #argb
          let a = parseColorChar color.[1]
          let r = parseColorChar color.[2]
          let g = parseColorChar color.[3]
          let b = parseColorChar color.[4]
          if isOk a && isOk r && isOk g && isOk b then
            Color(extendColor r, extendColor g, extendColor b, extendColor a)
          else
            red
        | 7 ->
          // #rrggbb
          let rh = parseColorChar color.[1]
          let rl = parseColorChar color.[2]
          let gh = parseColorChar color.[3]
          let gl = parseColorChar color.[4]
          let bh = parseColorChar color.[5]
          let bl = parseColorChar color.[6]
          if isOk rh && isOk rl && isOk gh && isOk gl && isOk bh && isOk bl then
            Color(mergeColor rh rl, mergeColor gh gl, mergeColor bh bl)
          else
            red
        | 9 ->
          // #rrggbb
          let ah = parseColorChar color.[1]
          let al = parseColorChar color.[2]
          let rh = parseColorChar color.[3]
          let rl = parseColorChar color.[4]
          let gh = parseColorChar color.[5]
          let gl = parseColorChar color.[6]
          let bh = parseColorChar color.[7]
          let bl = parseColorChar color.[8]
          if isOk ah && isOk al && isOk rh && isOk rl && isOk gh && isOk gl && isOk bh && isOk bl then
            Color(mergeColor rh rl, mergeColor gh gl, mergeColor bh bl, mergeColor ah al)
          else
            red
        | _ ->
          red

    let c = parse (color.Trim ())
    c.ToColor4 ()
    
  member x.Show           () : unit = ()
  member x.Close          () : unit = ()
  member x.Clear          () : unit = ()
  member x.WaitForRefresh () : unit =
    ignore <| refreshLock.Reset ()
    ignore <| refreshLock.WaitOne ()
  member x.BackGround     (color : string) : unit =
    Background (parseColor color)
    |> enqueueInput
  member x.SolidBrush     (brushId : int, color : string) : unit =
    SolidBrush (parseColor color)
    |> enqueueBrushInput brushId
  member x.Move           (id : int, x1 : float, y1 : float) : unit = 
    MoveVisual (float32 x1, float32 y1)
    |> enqueueVisualInput id
  member x.Rectangle      ( id            : int
                          , fillBrush     : int
                          , strokeBrush   : int
                          , strokeWidth   : float 
                          , left          : float 
                          , top           : float 
                          , width         : float 
                          , height        : float
                          ) : unit = 
    CreateRectangle (float32 strokeWidth, brushId strokeBrush, brushId fillBrush, rect left top width height)
    |> enqueueVisualInput id
  member x.Ellipse        ( id            : int
                          , fillBrush     : int   
                          , strokeBrush   : int   
                          , strokeWidth   : float 
                          , centerX       : float 
                          , centerY       : float 
                          , radiusX       : float 
                          , radiusY       : float
                          ) : unit = 
    CreateEllipse (float32 strokeWidth, brushId strokeBrush, brushId fillBrush, ellipse centerX centerY radiusX radiusY)
    |> enqueueVisualInput id
  interface IDisposable with
    member x.Dispose () =
      renderQueue.Enqueue Exit  // Kills the render thread
      renderThread.Join ()
      dispose refreshLock

