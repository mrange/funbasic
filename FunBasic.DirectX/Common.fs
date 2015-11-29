module internal FunBasic.DirectX.Common

open System
open System.Collections.Generic

open SharpDX

[<Measure>]
type BitmapMeasure
[<Measure>]
type BrushMeasure
[<Measure>]
type TextFormatMeasure
[<Measure>]
type VisualMeasure

type BitmapId     = int<BitmapMeasure>
type BrushId      = int<BrushMeasure>
type TextFormatId = int<TextFormatMeasure>
type VisualId     = int<VisualMeasure>

let inline createBitmapId i     = i*1<BitmapMeasure>
let inline createBrushId i      = i*1<BrushMeasure>
let inline createTextFormatId i = i*1<TextFormatMeasure>
let inline createVisualId i     = i*1<VisualMeasure>

let inline refEqual (l : 'T) (r : 'T) : bool = Object.ReferenceEquals (l, r)

let traceException (e : #Exception) : unit = 
  printfn "EXCEPTION: %s" e.Message

let tryWith (dv : 'T ) (a : unit -> 'T) : 'T =
  try
    a ()
  with
  | e -> 
    traceException e
    dv

let dispose (d : IDisposable) =
  if d <> null then
    try
      d.Dispose ()
    with
    | e ->
      traceException e


let disposeResourceDictionary (d : IDictionary<_, _*#IDisposable>) : unit =
  try
    for kv in d do
      dispose (snd kv.Value)
  finally
      d.Clear ()

let inline getDescriptor (k : 'K) (d : Dictionary<'K, 'U*'V>) : 'U option =
  let ok, uv = d.TryGetValue k
  if ok then
    let u, _ = uv
    Some u
  else
    None

let inline getResource (k : 'K) (dv : 'V) (creator : 'K -> 'U -> 'V) (d : Dictionary<'K, 'U*'V>) : 'V =
  let ok, uv = d.TryGetValue k
  if ok then
    let u, v = uv
    if v = null then
      let nv = creator k u
      d.[k] <- (u, nv)
      nv
    else
      v
  else
    dv

let inline reserveResource (k : 'K) (u : 'U) (d : Dictionary<'K, 'U*'V>) : unit =
  let ok, uv = d.TryGetValue k
  if ok then
    let pu, v = uv
    if refEqual u pu then
      ()
    else
    let nuv = u, null
    d.[k] <- nuv
    dispose v
  else
    let nuv = u, null
    d.[k] <- nuv

type ActionDisposable(a : unit -> unit) =
  interface IDisposable with
    member x.Dispose () =
      try
        a ()
      with
      | e -> 
        traceException e

let onExit (a : unit -> unit) : IDisposable = upcast new ActionDisposable (a)

type Direct2D1.Brush with
  member x.IsVisible =
    if x = null then
      false
    else
      x.Opacity > 0.0f

type IDictionary<'K,'V> with
  member inline x.GetOrCreate (k : 'K) (u : 'U) (creator : 'K -> 'U -> 'V) : 'V =
    let ok, v = x.TryGetValue k
    if ok then
      v
    else
      let nv = creator k u
      x.Add (k, nv)
      nv

  member inline x.CreateOrUpdate (k : 'K) (u : 'U) (creator : 'K -> 'U -> 'V) (updater : 'K -> 'V -> 'U -> 'V) : 'V =
    let ok, v = x.TryGetValue k
    if ok then
      let nv = updater k v u
      if not <| refEqual nv v then
        x.[k] <- nv
      nv
    else
      let nv = creator k u
      x.[k] <- nv
      nv

let inline v2 x y             = Vector2 (x, y)
let inline size2 w h          = Size2F (w, h)
let inline rectf x y w h      = RectangleF (x, y, w, h)
let inline ellipsef x y rx ry = Direct2D1.Ellipse (v2 x y, rx, ry)

let inline ecreate (x : float) (y : float) (w : float) (h : float) : Direct2D1.Ellipse =
  Direct2D1.Ellipse (v2 (float32 x) (float32 y), float32 (w / 2.0), float32 (h / 2.0))

let inline rcreate (x : float) (y : float) (w : float) (h : float) : RectangleF = 
  rectf (float32 (x - w / 2.0)) (float32 (y - h / 2.0)) (float32 w) (float32 h)

let inline rmove2   (v : Vector2) (r : RectangleF) = rectf (v.X - r.Width / 2.0F) (v.Y - r.Height / 2.0F) r.Width r.Height
let inline rresize2 (v : Vector2) (r : RectangleF) = rectf r.X r.Y v.X v.Y

let inline emove2   (v : Vector2) (e : Direct2D1.Ellipse) = ellipsef v.X v.Y e.RadiusX e.RadiusY
let inline eresize2 (v : Vector2) (e : Direct2D1.Ellipse) = ellipsef e.Point.X e.Point.Y (v.X / 2.0F) (v.Y / 2.0F)

let inline expand (w : float32) (h : float32) (rect : RectangleF) : RectangleF =
  let hw = w / 2.0F
  let hh = h / 2.0F
  RectangleF (rect.X - hw, rect.Y - hh, rect.Width + w, rect.Height + h)

let bounds (v1 : Vector2) (v2 : Vector2) =
    let min = Vector2.Min (v1, v2)
    let max = Vector2.Max (v1, v2)
    rectf min.X min.Y (max.X - min.X) (max.Y - min.Y)

type Direct2D1.RenderTarget with
    member x.Clear (c : Color) =
        x.Clear (Nullable<_> (c.ToColor4 ()))

let parseColor (color : string) : Color4 =
  let red = Color.Red

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
    
