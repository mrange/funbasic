namespace FunBasic.DirectX.Scene

open FunBasic.DirectX.Common
open FunBasic.DirectX.Direct2d
open FunBasic.DirectX.ApiModel

open SharpDX

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Text.RegularExpressions

module internal SceneRenderer =
  type Vector       = float*float
  type BoundingBox  = float*float*float*float

  type Visual =
    | EmptyVisual
                                // Bitmap   Opacity InterpolationMode                 Bounds
    | BitmapVisual              of BitmapId*float32*Direct2D1.BitmapInterpolationMode*RectangleF
                                // Width   Stroke  Fill    Bounds
    | EllipseVisual             of float32*BrushId*BrushId*Direct2D1.Ellipse
                                // Width   Stroke  Fill    Bounds
    | RectangleVisual           of float32*BrushId*BrushId*RectangleF
                                // Fill    TextFormat   Bounds     Text
    | TextVisual                of BrushId*TextFormatId*RectangleF*string

  let rec renderVisual (v : Visual) (d : Device) (rt : Direct2D1.RenderTarget) : unit =
    match v with
    | EmptyVisual ->
      ()
    | BitmapVisual (b, o, bim, r) ->
      let bb = d.GetBitmap b
      if bb <> null && o > 0.0f then
        rt.DrawBitmap (bb, r, o, bim)
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

  let onKeyUp _ = 
    ()

  let regexExtendMode = 
    Regex ( @"^\s*((?<Clamp>clamp)|(?<Mirror>mirror)|(?<Wrap>wrap))\s*$"
          , RegexOptions.IgnoreCase ||| RegexOptions.Compiled ||| RegexOptions.CultureInvariant
          )

  let createRenderer (onRefresh :  unit -> unit) =
    let inline createBitmapId i     = i*1<BitmapMeasure>
    let inline createBrushId i      = i*1<BrushMeasure>
    let inline createTextFormatId i = i*1<TextFormatMeasure>
    let inline createVisualId i     = i*1<VisualMeasure>

    let parseExtendMode (em : string) : Direct2D1.ExtendMode =
      let m = regexExtendMode.Match em
      if m.Groups.["Clamp"].Success then
        Direct2D1.ExtendMode.Clamp
      elif m.Groups.["Mirror"].Success then
        Direct2D1.ExtendMode.Mirror
      elif m.Groups.["Wrap"].Success then
        Direct2D1.ExtendMode.Wrap
      else
        Direct2D1.ExtendMode.Clamp

        

    let inline v (x : float) (y : float) =
      Vector2 (float32 x, float32 y)

    let inline ecreate (x : float) (y : float) (w : float) (h : float) : Direct2D1.Ellipse =
      Direct2D1.Ellipse (v2 (float32 x) (float32 y), float32 (w / 2.0), float32 (h / 2.0))
    let inline rcreate (x : float) (y : float) (w : float) (h : float) : RectangleF = 
      rectf (float32 (x - w / 2.0)) (float32 (y - h / 2.0)) (float32 w) (float32 h)

    let inline rmove2 (x : float) (y : float) (r : RectangleF) = 
      rectf (float32 x - r.Width / 2.0F) (float32 x - r.Height / 2.0F) r.Width r.Height
    let inline rresize2 (w : float) (h : float) (r : RectangleF) = 
      rectf r.X r.Y (float32 w) (float32 h)

    let inline emove2 (x : float) (y : float) (e : Direct2D1.Ellipse) = 
      ellipsef (float32 x) (float32 y) e.RadiusX e.RadiusY
    let inline eresize2 (w : float) (h : float) (e : Direct2D1.Ellipse) = 
      ellipsef e.Point.X e.Point.Y (float32 w / 2.0F) (float32 h / 2.0F)

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
      | CreateBitmapVisual (bitmapId, opacity, centerX, centerY, width, height) ->
        BitmapVisual (createBitmapId bitmapId, float32 opacity, Direct2D1.BitmapInterpolationMode.NearestNeighbor, rcreate centerX centerY width height)
      | CreateEllipseVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height) ->
        EllipseVisual (float32 strokeWidth, createBrushId strokeBrushId, createBrushId fillBrushId, ecreate centerX centerY width height)
      | CreateRectangleVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height) ->
        RectangleVisual (float32 strokeWidth, createBrushId strokeBrushId, createBrushId fillBrushId, rcreate centerX centerY width height)
      | CreateTextVisual (fillBrushId, textFormatId, centerX, centerY, width, height, text) ->
        TextVisual (createBrushId fillBrushId, createTextFormatId textFormatId, rcreate centerX centerY width height, text)

    let updateVisual k v u = 
      match u with
      | MoveVisual (x, y) ->
        match v with
        | EmptyVisual -> 
          EmptyVisual
        | BitmapVisual  (bid, o, bim, r) ->
          BitmapVisual  (bid, o, bim, rmove2 x y r)
        | EllipseVisual (sw, s, f, r) ->
          EllipseVisual (sw, s, f, emove2 x y r)
        | RectangleVisual (sw, s, f, r) ->
          RectangleVisual (sw, s, f, rmove2 x y r)
        | TextVisual (bd, tfd, r, t) ->
          TextVisual (bd, tfd, rmove2 x y r, t)
      | ResizeVisual (w, h) ->
        match v with
        | EmptyVisual -> 
          EmptyVisual
        | BitmapVisual  (bid, o, bim, r) ->
          BitmapVisual  (bid, o, bim, rresize2 w h r)
        | EllipseVisual (sw, s, f, r) ->
          EllipseVisual (sw, s, f, eresize2 w h r)
        | RectangleVisual (sw, s, f, r) ->
          RectangleVisual (sw, s, f, rresize2 w h r)
        | TextVisual (bd, tfd, r, t) ->
          TextVisual (bd, tfd, rresize2 w h r, t)
      | _ ->
        createVisual k u

    let doNothing = GlobalInput DoNothing
    let onRender (delay : (unit -> unit) -> unit) (d : Device) (rt : Direct2D1.RenderTarget) : bool =
      let mutable cont  = true
      let mutable i     = doNothing 
      while input.TryDequeue (&i) do
        match i with
        | GlobalInput gi ->
          match gi with
          | ClearVisuals ->
            visuals.Clear ()
          | CloseWindow ->
            cont <- false
          | DoNothing ->
            ()
          | HideWindow ->
            // TODO:
            ()
          | SetBackground color ->
            background := parseColor color
          | ShowWindow ->
            // TODO:
            ()
        | BitmapInput (bid, (DownloadBitmap url)) ->
          // TODO:
          // d.ReserveBitmap bid (BitmapBits bytes)
          ()
        | BrushInput (bid, bi) ->
          let bid = createBrushId bid
          match bi with
          | CreateSolidBrush color ->
            d.ReserveBrush bid (SolidBrush (parseColor color))
          | CreateLinearGradientBrush (startX, startY, endX, endY, extendMode) ->
            d.ReserveBrush bid (LinearGradientBrush (v startX startY, v endX endY, parseExtendMode extendMode, [||]))
          | CreateRadialGradientBrush (centerX, centerY, radiusX, radiusY, offsetX, offsetY, extendMode) ->
            d.ReserveBrush bid (RadialGradientBrush (v centerX centerY, v radiusX radiusY, v offsetX offsetY, parseExtendMode extendMode, [||]))
          | CreateGradientStopForBrush (color, offset) ->
            let c = parseColor color
            let ogd = d.GetBrushDescriptor bid
            match ogd with
            | Some (LinearGradientBrush (s, e, em, stops)) ->
              d.ReserveBrush bid (LinearGradientBrush (s, e, em, stops |> Array.append [|c, float32 offset|]))
            | Some (RadialGradientBrush (c, r, o, em, stops)) ->
              d.ReserveBrush bid (RadialGradientBrush (c, r, o, em, stops |> Array.append [|c, float32 offset|]))
            | _ ->
              ()
        | TextFormatInput (tfid, (CreateTextFormat (fontFamily, fontSize))) ->
          let tfid = createTextFormatId tfid
          d.ReserveTextFormat tfid (SimpleTextFormat (fontFamily, float32 fontSize))
        | VisualInput (vid, vi) ->
          let vid = createVisualId vid
          ignore <| visuals.CreateOrUpdate vid vi createVisual updateVisual

      onRefresh ()

      rt.Clear <| Nullable<_> !background

      for kv in visuals do
        renderVisual kv.Value d rt

      cont

    let uiProc () =
      try
        Window.show "FunBasic Direct2D" 1024 768 onKeyUp onRender
      with
      | e -> 
        traceException e

    let uiThread =
      let t = Thread (ThreadStart uiProc)
      t.SetApartmentState ApartmentState.STA
      t.IsBackground    <- true
      t.Start ()
      t  
    uiThread, input

open SceneRenderer

type Scene() =
  let refreshLock = new ManualResetEvent false

  let onRender () =
    ignore <| refreshLock.Set ()

  let renderThread, renderQueue = SceneRenderer.createRenderer onRender

  member x.WaitForRefresh () : unit =
    ignore <| refreshLock.Reset ()
    ignore <| refreshLock.WaitOne ()

(*
  member x.DownloadBitmap (bitmapId : int, url : string) : unit =
    try
      use wc = new System.Net.WebClient ()
      let bytes = wc.DownloadData url
      CreateBitmapFromBits bytes
      |> enqueueBitmapInput bitmapId
    with
    | e -> 
      traceException e

*)

  member x.SendInput (input : Input) : unit =
    renderQueue.Enqueue input

  interface IDisposable with
    member x.Dispose () =
      renderQueue.Enqueue <| GlobalInput CloseWindow  // Kills thread
      renderThread.Join ()
      dispose refreshLock