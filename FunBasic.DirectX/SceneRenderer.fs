module internal FunBasic.DirectX.SceneRenderer

open FunBasic.DirectX.Common
open FunBasic.DirectX.Direct2d
open FunBasic.DirectX.ApiModel

open SharpDX

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Text.RegularExpressions

type Vector       = float*float
type BoundingBox  = float*float*float*float

type Visual =
  | EmptyVisual
                              // Bitmap   Opacity InterpolationMode                 Bounds
  | BitmapVisual              of BitmapId*float32*Direct2D1.BitmapInterpolationMode*RectangleF
                              // Width   Stroke  Fill    Bounds            BrushTransform
  | EllipseVisual             of float32*BrushId*BrushId*Direct2D1.Ellipse*Matrix3x2
                              // Width   Stroke  Fill    Bounds     BrushTransform
  | RectangleVisual           of float32*BrushId*BrushId*RectangleF*Matrix3x2
                              // Fill    TextFormat   Text   Bounds     BrushTransform
  | TextVisual                of BrushId*TextFormatId*string*RectangleF*Matrix3x2

let rec renderVisual (v : Visual) (d : Device) (rt : Direct2D1.RenderTarget) : unit =
  match v with
  | EmptyVisual ->
    ()
  | BitmapVisual (b, o, bim, r) ->
    let bb = d.GetBitmap b
    if bb <> null && o > 0.0f then
      rt.DrawBitmap (bb, r, o, bim)
  | EllipseVisual (sw, s, f, e, bt) ->
    let fb = d.GetBrush f
    let sb = d.GetBrush s
    if fb.IsVisible then
      fb.Transform <- bt
      rt.FillEllipse (e, fb)
    if sb.IsVisible && sw > 0.0f then
      sb.Transform <- bt
      rt.DrawEllipse (e, sb, sw)
  | RectangleVisual (sw, s, f, r, bt) ->
    let fb = d.GetBrush f
    let sb = d.GetBrush s
    if fb.IsVisible then
      fb.Transform <- bt
      rt.FillRectangle (r, fb)
    if sb.IsVisible && sw > 0.0f then
      sb.Transform <- bt
      rt.DrawRectangle (r, sb, sw)
  | TextVisual (f, tf, t, r, bt) ->
    let fb  = d.GetBrush f
    let tfd = d.GetTextFormat tf
    if fb.IsVisible && tfd <> null then
      fb.Transform <- bt
      rt.DrawText (t, tfd, r, fb)

let onInit (d : Device) : unit =
  let brushes =
    [|
      0,      Color.Black
      1,      Color.White
      2,      Color.Red
      3,      Color.Orange
      4,      Color.Yellow
      5,      Color.Green
      6,      Color.Blue
      7,      Color.Indigo
      8,      Color.Violet
    |]

  for bid, color in brushes do
    d.ReserveBrush (createBrushId bid) <| SolidBrush (color.ToColor4 ())

let onKeyUp _ : unit =
  ()

let regexExtendMode =
  Regex ( @"^\s*((?<Clamp>clamp)|(?<Mirror>mirror)|(?<Wrap>wrap))\s*$"
        , RegexOptions.IgnoreCase ||| RegexOptions.Compiled ||| RegexOptions.CultureInvariant
        )

let parseExtendMode (em : string) : Direct2D1.ExtendMode =
  let em = fixString em
  let m = regexExtendMode.Match em
  if m.Groups.["Clamp"].Success then
    Direct2D1.ExtendMode.Clamp
  elif m.Groups.["Mirror"].Success then
    Direct2D1.ExtendMode.Mirror
  elif m.Groups.["Wrap"].Success then
    Direct2D1.ExtendMode.Wrap
  else
    Direct2D1.ExtendMode.Clamp

let createRenderer (onRefresh :  unit -> unit) =

  let inline v (x : float) (y : float) =
    Vector2 (float32 x, float32 y)

  let inline ecreate (x : float) (y : float) (w : float) (h : float) : Direct2D1.Ellipse =
    Direct2D1.Ellipse (v2 (float32 x) (float32 y), float32 (w / 2.0), float32 (h / 2.0))
  let inline rcreate (x : float) (y : float) (w : float) (h : float) : RectangleF =
    rectf (float32 (x - w / 2.0)) (float32 (y - h / 2.0)) (float32 w) (float32 h)

  let inline rmove2 (x : float) (y : float) (r : RectangleF) =
    rectf (float32 x - r.Width / 2.0F) (float32 y - r.Height / 2.0F) r.Width r.Height
  let inline rresize2 (w : float) (h : float) (r : RectangleF) =
    rectf r.X r.Y (float32 w) (float32 h)

  let inline emove2 (x : float) (y : float) (e : Direct2D1.Ellipse) =
    ellipsef (float32 x) (float32 y) e.RadiusX e.RadiusY
  let inline eresize2 (w : float) (h : float) (e : Direct2D1.Ellipse) =
    ellipsef e.Point.X e.Point.Y (float32 w / 2.0F) (float32 h / 2.0F)

  let inline btFromRect (r : RectangleF) : Matrix3x2 =
    matrix32 r.Width 0.0F r.X 0.0F r.Height r.Y

  let inline btFromEllipse (e : Direct2D1.Ellipse) : Matrix3x2 =
    matrix32 (2.0F * e.RadiusX) 0.0F (e.Point.X - e.RadiusX) 0.0F (2.0F * e.RadiusY) (e.Point.Y - e.RadiusY)

  let input     = ConcurrentQueue<Input> ()
  let visuals   = OrderedDictionary<VisualId, Visual> ()
  let background= ref <| Color.Black.ToColor4 ()

  let createVisual k u =
    match u with
    | CloneVisual (cloneVisualId) ->
      visuals.GetValue (createVisualId cloneVisualId) EmptyVisual
    // Remove during create results in an empty visual
    | RemoveVisual
    // Not supported on creation
    | MoveVisual _
    | ResizeVisual _
    | MoveVisualToBottom
    | MoveVisualToTop     ->
      EmptyVisual
    | CreateBitmapVisual (bitmapId, opacity, centerX, centerY, width, height) ->
      BitmapVisual (createBitmapId bitmapId, float32 opacity, Direct2D1.BitmapInterpolationMode.NearestNeighbor, rcreate centerX centerY width height)
    | CreateEllipseVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height) ->
      let e   = ecreate centerX centerY width height
      let bt  = btFromEllipse e
      EllipseVisual (float32 strokeWidth, createBrushId strokeBrushId, createBrushId fillBrushId, e, bt)
    | CreateRectangleVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height) ->
      let r   = rcreate centerX centerY width height
      let bt  = btFromRect r
      RectangleVisual (float32 strokeWidth, createBrushId strokeBrushId, createBrushId fillBrushId, r, bt)
    | CreateTextVisual (fillBrushId, textFormatId, centerX, centerY, width, height, text) ->
      let r   = rcreate centerX centerY width height
      let bt  = btFromRect r
      let text= fixString text
      TextVisual (createBrushId fillBrushId, createTextFormatId textFormatId, text, r, bt)

  let updateVisual k v u =
    match u with
    | RemoveVisual ->
      EmptyVisual
    | MoveVisual (x, y) ->
      match v with
      | EmptyVisual ->
        EmptyVisual
      | BitmapVisual  (bid, o, bim, r) ->
        BitmapVisual  (bid, o, bim, rmove2 x y r)
      | EllipseVisual (sw, s, f, e, _) ->
        let ne  = emove2 x y e
        let nbt = btFromEllipse ne
        EllipseVisual (sw, s, f, ne, nbt)
      | RectangleVisual (sw, s, f, r, _) ->
        let nr  = rmove2 x y r
        let nbt = btFromRect nr
        RectangleVisual (sw, s, f, nr, nbt)
      | TextVisual (bd, tfd, t, r, _) ->
        let nr  = rmove2 x y r
        let nbt  = btFromRect nr
        TextVisual (bd, tfd, t, nr, nbt)
    | ResizeVisual (w, h) ->
      match v with
      | EmptyVisual ->
        EmptyVisual
      | BitmapVisual  (bid, o, bim, r) ->
        BitmapVisual  (bid, o, bim, rresize2 w h r)
      | EllipseVisual (sw, s, f, r, _) ->
        let ne  = eresize2 w h r
        let nbt = btFromEllipse ne
        EllipseVisual (sw, s, f, ne, nbt)
      | RectangleVisual (sw, s, f, r, _) ->
        let nr  = rresize2 w h r
        let nbt = btFromRect nr
        RectangleVisual (sw, s, f, nr , nbt)
      | TextVisual (bd, tfd, t, r, _) ->
        let nr  = rresize2 w h r
        let nbt = btFromRect nr
        TextVisual (bd, tfd, t, nr, nbt)
    | MoveVisualToBottom ->
      visuals.Move k -10000 EmptyVisual
    | MoveVisualToTop     ->
      visuals.Move k 10000 EmptyVisual
    | _ ->
      createVisual k u

  let doNothing = InternalInput DoNothing

  let onRender (delay : (unit -> unit) -> unit) (d : Device) (rt : Direct2D1.RenderTarget) : bool =
    let mutable cont  = true
    let mutable i     = doNothing
    while input.TryDequeue (&i) do
      match i with
      | InternalInput ii ->
        match ii with
        | DiscardWindow ->
          cont <- false
        | DoNothing ->
          ()
        | CreateBitmapFromBits (bitmapId, bits) ->
          if bits = null || bits.Length = 0 then
            // TODO: Trace null bits
            ()
          else
            d.ReserveBitmap (createBitmapId bitmapId) (BitmapBits bits)
      | GlobalInput gi ->
        match gi with
        | WaitForDownloads
        | WaitForRefresh ->
          // Not expected here, handled by Scene
          ()
        | ClearVisuals ->
          visuals.Clear ()
        | HideWindow ->
          // TODO:
          ()
        | SetBackground color ->
          background := parseColor color
        | ShowWindow ->
          // TODO:
          ()
      | BitmapInput (bid, (DownloadBitmap _)) ->
        // Not expected here, handled by Scene
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
          let color   = parseColor color
          let offset  = float32 offset
          let ogd     = d.GetBrushDescriptor bid
          match ogd with
          | Some (LinearGradientBrush (s, e, em, stops)) ->
            d.ReserveBrush bid (LinearGradientBrush (s, e, em, stops |> addToArray (color, float32 offset)))
          | Some (RadialGradientBrush (c, r, o, em, stops)) ->
            d.ReserveBrush bid (RadialGradientBrush (c, r, o, em, stops |> addToArray (color, float32 offset)))
          | _ ->
            ()
      | TextFormatInput (tfid, (CreateTextFormat (fontFamily, fontSize))) ->
        let tfid        = createTextFormatId tfid
        let fontFamily  = fixString fontFamily
        d.ReserveTextFormat tfid (SimpleTextFormat (fontFamily, float32 fontSize))
      | VisualInput (vid, vi) ->
        ignore <| visuals.CreateOrUpdate (createVisualId vid) vi createVisual updateVisual

    onRefresh ()

    rt.Clear <| Nullable<_> !background

    visuals.VisitAllValues <| fun i v ->
      renderVisual v d rt

    cont

  let uiProc () =
    try
      Window.show "FunBasic Direct2D" 1024 768 onInit onKeyUp onRender
    with
    | e ->
      traceException e

  let uiThread =
    let t = Thread (ThreadStart uiProc)
    t.SetApartmentState ApartmentState.STA
    t.Priority          <- ThreadPriority.AboveNormal
    t.IsBackground      <- true
    t.Start ()
    t
  uiThread, input

