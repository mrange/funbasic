namespace FunBasic.DirectX.Scene

open FunBasic.DirectX.Common
open FunBasic.DirectX.Direct2d
open FunBasic.DirectX.ApiModel

open SharpDX

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
(*
type VisualInput =
                              // X       Y
  | MoveVisual                of float32*float32
                              // Width   Height
  | ResizeVisual              of float32*float32
                              // Bitmap   Opacity InterpolationMode                 Bounds
  | CreateBitmap              of BitmapId*float32*Direct2D1.BitmapInterpolationMode*RectangleF
                              // Width   Stroke  Fill    Bounds
  | CreateEllipse             of float32*BrushId*BrushId*Direct2D1.Ellipse
                              // Width   Stroke  Fill    Bounds
  | CreateRectangle           of float32*BrushId*BrushId*RectangleF
                              // Fill    TextFormat   Bounds     Text
  | CreateText                of BrushId*TextFormatId*RectangleF*string

type BrushInput =
  | CreateSolidBrush          of Color4
                              // Start   End     ExtendMode           
  | CreateLinearGradientBrush of Vector2*Vector2*Direct2D1.ExtendMode
                              // Center  Radius  Origin  ExtendMode           
  | CreateRadialGradientBrush of Vector2*Vector2*Vector2*Direct2D1.ExtendMode
  | CreateGradientStop        of Color4*float32

type BitmapInput =
  | CreateBitmapFromBits      of byte[]

type TextFormatInput =
  | CreateTextFormat          of string * float32

type Input = 
  | DoNothing
  | Exit
  | Clear
  | Background                of Color4
  | BitmapInput               of BitmapId*BitmapInput
  | BrushInput                of BrushId*BrushInput
  | TextFormatInput           of TextFormatId*TextFormatInput
  | VisualInput               of VisualId*VisualInput
*)

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
      | CreateBitmapVisual (bitmapId, opacity, centerX, centerY, width, height) ->
        BitmapVisual (createBitmapId bitmapId, float32 opacity, Direct2D1.BitmapInterpolationMode.NearestNeighbor, rcreate centerX centerY width height)
      | CreateEllipseVisual (sw, s, f, e) ->
        EllipseVisual (sw, s, f, e)
      | CreateRectangleVisual (sw, s, f, r) ->
        RectangleVisual (sw, s, f, r)
      | CreateTextVisual (bd, tfd, r, t) ->
        TextVisual (bd, tfd, r, t)

    let updateVisual k v u = 
      match u with
      | MoveVisual (x, y) ->
        let pos = v2 x y
        match v with
        | EmptyVisual -> 
          EmptyVisual
        | BitmapVisual  (bid, o, bim, r) ->
          BitmapVisual  (bid, o, bim, rmove2 pos r)
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
        | BitmapVisual  (bid, o, bim, r) ->
          BitmapVisual  (bid, o, bim, rresize2 sz r)
        | EllipseVisual (sw, s, f, r) ->
          EllipseVisual (sw, s, f, eresize2 sz r)
        | RectangleVisual (sw, s, f, r) ->
          RectangleVisual (sw, s, f, rresize2 sz r)
        | TextVisual (bd, tfd, r, t) ->
          TextVisual (bd, tfd, rresize2 sz r, t)
      | _ ->
        createVisual k u

    let doNothing = GlobalInput (DoNothing ())
    let onRender (delay : (unit -> unit) -> unit) (d : Device) (rt : Direct2D1.RenderTarget) : bool =
      let mutable cont  = true
      let mutable i     = doNothing 
      while input.TryDequeue (&i) do
        match i with
        | GlobalInput gi ->
          match gi with
          | ClearVisuals _ ->
            visuals.Clear ()
          | CloseWindow _ ->
            // TODO:
            cont <- false
          | DoNothing _ ->
            ()
          | HideWindow _ ->
            // TODO:
            ()
          | SetBackground color ->
            background := parseColor color
          | ShowWindow _ ->
            // TODO:
            ()
        | BitmapInput (bid, (CreateBitmapFromBits bytes)) ->
          d.ReserveBitmap bid (BitmapBits bytes)
        | BrushInput (bid, bi) ->
          match bi with
          | CreateSolidBrush c ->
            d.ReserveBrush bid (SolidBrush c)
          | CreateLinearGradientBrush (s, e, em) ->
            d.ReserveBrush bid (LinearGradientBrush (s, e, em, [||]))
          | CreateRadialGradientBrush (c, r, o, em) ->
            d.ReserveBrush bid (RadialGradientBrush (c, r, o, em, [||]))
          | CreateGradientStop (color, offset) ->
            let ogd = d.GetBrushDescriptor bid
            match ogd with
            | Some (LinearGradientBrush (s, e, em, stops)) ->
              d.ReserveBrush bid (LinearGradientBrush (s, e, em, stops |> Array.append [|color, offset|]))
            | Some (RadialGradientBrush (c, r, o, em, stops)) ->
              d.ReserveBrush bid (RadialGradientBrush (c, r, o, em, stops |> Array.append [|color, offset|]))
            | _ ->
              ()
        | TextFormatInput (tfid, (CreateTextFormat (fontFamily, fontSize))) ->
          d.ReserveTextFormat tfid (SimpleTextFormat (fontFamily, fontSize))
        | VisualInput (vid, vi) ->
          ignore <| visuals.CreateOrUpdate vid vi createVisual updateVisual

      onRefresh ()

      rt.Clear <| Nullable<_> !background

      for kv in visuals do
        renderVisual kv.Value d rt

      cont

    let uiProc () =
      try
        Window.show "FunBasic Direct2D" 1024 768 doNothing onRender
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