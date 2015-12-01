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

let inline clamp v b e = 
  if v > e then e
  elif v < b then b
  else v

let inline refEqual (l : 'T) (r : 'T) : bool = Object.ReferenceEquals (l, r)

let inline addToArray (v : 'T) (vs : 'T []) : 'T [] =
  let mutable nvs = vs
  System.Array.Resize (&nvs, vs.Length + 1)
  nvs.[vs.Length] <- v
  nvs

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

let inline v2 x y                           = Vector2 (x, y)
let inline size2 w h                        = Size2F (w, h)
let inline rectf x y w h                    = RectangleF (x, y, w, h)
let inline ellipsef x y rx ry               = Direct2D1.Ellipse (v2 x y, rx, ry)
let inline matrix32 m11 m21 m31 m12 m22 m32 = Matrix3x2 (m11, m12, m21, m22, m31, m32)

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

type Direct2D1.Brush with
  member x.IsVisible =
    if x = null then
      false
    else
      x.Opacity > 0.0f

[<ReferenceEquality>]
[<NoComparison>]
type Payload<'V> =
  {
    mutable Value : 'V
  }

  static member New v = { Value = v }

type OrderedDictionary<'K, 'V when 'K : equality>() =
  let kvs = Dictionary<'K, Payload<'V>> ()
  let vs  = ResizeArray<Payload<'V>> ()

  let move f t =
    let e = vs.Count - 1
    let f = clamp f 0 e
    let t = clamp t 0 e
    if f < t then
      let s = vs.[f]
      for i = f to (t - 1) do
        vs.[i] <- vs.[i + 1]
      vs.[t] <- s
    elif f > t then
      let s = vs.[f]
      for i = f downto (t + 1) do
        vs.[i] <- vs.[i - 1]
      vs.[t] <- s
    else
      ()

  member x.Clear () =
    kvs.Clear ()
    vs.Clear ()

  member x.GetValue (k : 'K) (dv : 'V) : 'V =
    let ok, p = kvs.TryGetValue k
    if ok then
      p.Value
    else
      dv

  member x.CreateOrUpdate (k : 'K) (u : 'U) (creator : 'K -> 'U -> 'V) (updater : 'K -> 'V -> 'U -> 'V) : 'V =
    let ok, p = kvs.TryGetValue k
    if ok then
      let v = p.Value
      let nv = updater k v u
      p.Value <- nv
      nv
    else
      let nv = creator k u
      let np = Payload<_>.New nv
      kvs.[k] <- np
      vs.Add np
      nv

  member x.Move (k : 'K) (o : int) (dv : 'V) : 'V =
    let ok, p = kvs.TryGetValue k
    if ok && o = 0 then
      p.Value
    elif ok then
      let c = vs.Count
      let f = vs.IndexOf p
      if f > -1 then
        move f (f + o)

      p.Value
    else
      dv

  member x.VisitAllValues (visitor : int -> 'V -> unit) : unit =
    let e = vs.Count - 1
    for i = 0 to e do
      let p = vs.[i]
      visitor i p.Value
