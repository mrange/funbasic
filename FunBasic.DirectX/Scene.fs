namespace FunBasic.DirectX.Scene

open FunBasic.DirectX.Common
open FunBasic.DirectX.Direct2d

open SharpDX

open System
open System.Threading

type Vector       = float*float
type BoundingBox  = float*float*float*float

type VisualTree =
  | Empty
  | Group           of VisualTree []
  | Rectangle       of float32*BrushDescriptor*BrushDescriptor*RectangleF
  | Text            of BrushDescriptor*TextFormatDescriptor*RectangleF*string

type Scene() =
  let rec renderVisualTree (vt : VisualTree) (d : Device) (rt : Direct2D1.RenderTarget) : unit =
    match vt with
    | Empty -> 
      ()
    | Group ivts -> 
      for ivt in ivts do
        renderVisualTree vt d rt
    | Rectangle (strokeWidth, stroke, fill, rect) ->
      if fill.IsVisible then
        rt.FillRectangle (rect, d.GetBrush fill)
      if stroke.IsVisible && strokeWidth > 0.0f then
        rt.DrawRectangle (rect, d.GetBrush stroke, strokeWidth)
    | Text (bd, tfd, rect, text) ->
      rt.DrawText (text, d.GetTextFormat tfd, rect, d.GetBrush bd)

  let refreshLock = new ManualResetEvent false

  let doNothing _ = 
    ()

  let white = SolidBrush Color.White
  let onRender (d : Device) (rt : Direct2D1.RenderTarget) : unit =

    rt.FillRectangle (rectf 0.0F 0.0F 100.0F 100.0F, d.GetBrush white)

    ignore <| refreshLock.Set ()

  let uiProc () =
    try
      Window.show "FunBasic" 1024 768 doNothing onRender
    with
    | e -> ()

  let uiThread =
    let t = Thread (ThreadStart uiProc)
    t.SetApartmentState ApartmentState.STA
    t.IsBackground    <- true
    t.Start ()
    t  

  member x.Discard        () : unit = 
    dispose refreshLock
  member x.Show           () : unit = ()
  member x.Close          () : unit = ()
  member x.Clear          () : unit = ()
  member x.WaitForRefresh () : unit =
    ignore <| refreshLock.Reset ()
    ignore <| refreshLock.WaitOne ()
  member x.Move           (id : int, x1 : float, y1 : float) : unit = ()
  member x.Rectangle      ( fillBrush     : obj   
                          , strokeBrush   : obj   
                          , strokeWidth   : float 
                          , x1            : float 
                          , y1            : float 
                          , w1            : float 
                          , h1            : float
                          ) : int = 
      0
    

  interface IDisposable with
    member x.Dispose () = ()

