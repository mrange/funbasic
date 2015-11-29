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

let inline bitmapId i     = i*1<BitmapMeasure>
let inline brushId i      = i*1<BrushMeasure>
let inline textFormatId i = i*1<TextFormatMeasure>
let inline visualId i     = i*1<VisualMeasure>

(*
let invalidVisualId       = visualId -1
let invalidBrushId        = brushId -1
let invalidTextFormatId   = textFormatId -1
*)

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

let inline findResource (k : 'K) (dv : 'V) (d : Dictionary<'K, 'U*'V>) : 'V =
  let ok, uv = d.TryGetValue k
  if ok then
    snd uv
  else
    dv

let makeResourceUpdater (creator : 'K -> 'U -> 'U*'V ) : 'K  -> 'U*'V -> 'U -> 'U*'V =
  fun k uv u ->
    let pu, v = uv
    if Object.ReferenceEquals (u, v) then 
      uv
    else
      let nv = creator k u
      dispose v
      nv


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
      if not <| Object.ReferenceEquals (nv, v) then
        x.[k] <- nv
      nv
    else
      let nv = creator k u
      x.[k] <- nv
      nv

let inline v2 x y             = Vector2 (x,y)
let inline size2 w h          = Size2F (w,h)
let inline rectf x y w h      = RectangleF (x,y,w,h)
let inline ellipsef x y rx ry = Direct2D1.Ellipse (v2 x y, rx, ry)

let inline rmove2   (v : Vector2) (r : RectangleF) = rectf v.X v.Y r.Width r.Height
let inline rresize2 (v : Vector2) (r : RectangleF) = rectf r.X r.Y v.X v.Y

let inline emove2   (v : Vector2) (e : Direct2D1.Ellipse) = ellipsef v.X v.Y e.RadiusX e.RadiusY
let inline eresize2 (v : Vector2) (e : Direct2D1.Ellipse) = ellipsef e.Point.X e.Point.Y v.X v.Y


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


