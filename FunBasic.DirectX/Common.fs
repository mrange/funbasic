module FunBasic.DirectX.Common

open System
open System.Collections.Generic

open SharpDX

let dispose (d : IDisposable) =
  if d <> null then
    try
      d.Dispose ()
    with
    | e -> () // TODO: Trace

let disposeValuesInDictionary (d : IDictionary<'K, #IDisposable>) : unit =
  try
    for kv in d do
      dispose kv.Value
  finally
      d.Clear ()

type ActionDisposable(a : unit -> unit) =
  interface IDisposable with
    member x.Dispose () =
      try
        a ()
      with
      | e -> () // TODO: Trace


let onExit (a : unit -> unit) : IDisposable = upcast new ActionDisposable (a)

type IDictionary<'K,'V> with
  member inline x.GetOrCreate (k : 'K) (creator : 'K -> 'V) =
    let ok, v = x.TryGetValue k
    if ok then
      v
    else
      let nv = creator k
      x.Add (k, nv)
      nv

let inline size2 w h = Size2F (w,h)

let inline rectf x y w h = RectangleF (x,y,w,h)

let inline expand (w : float32) (h : float32) (rect : RectangleF) : RectangleF =
  let hw = w / 2.0F
  let hh = h / 2.0F
  RectangleF (rect.X - hw, rect.Y - hh, rect.Width + w, rect.Height + h)

let inline v2 x y  = Vector2 (x,y)

let bounds (v1 : Vector2) (v2 : Vector2) =
    let min = Vector2.Min (v1, v2)
    let max = Vector2.Max (v1, v2)
    rectf min.X min.Y (max.X - min.X) (max.Y - min.Y)

type Direct2D1.RenderTarget with
    member x.Clear (c : Color) =
        x.Clear (Nullable<_> (c.ToColor4 ()))


