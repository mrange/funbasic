
open FunBasic.Interpreter
open FunBasic.Library
open FunBasic.Runtime

open Microsoft.FSharp.Core.Printf

open System
open System.Collections.Generic
open System.Globalization
open System.Reflection
open System.Threading

let code = """
D2D.ShowWindow ()

D2D.ClearVisuals ()

' Numbers used to assign different D2D objects
'   TextFormats, Bitmaps, Brushes and Visuals all have their own
'   number series
textFormat_std  = 0001

bitmap_ufo      = 0001

brush_fill      = 0001
brush_stroke    = 0002
brush_radialfill= 0003
brush_radialfill= 0004

visual_rect     = 0001
visual_ellipse  = 0002
visual_text     = 0003
visual_ufo1     = 0004
visual_ufo2     = 0005
visual_ufo3     = 0006

' Sets the background to a darkblue (#rgb)
D2D.SetBackground               ("#003")

' Download a nice ufo
'   Note that this starts the download process but doesn't block
'   This enables you to download multiple bitmaps simultanenously
D2D.DownloadBitmap              (bitmap_ufo       , "http://blitzetc.ru/images/5/5a/BMax-UFO.png")

' Create a text format that will be used to render text
'   If no text format can be found a default will be used
D2D.CreateTextFormat            (textFormat_std   , "Tahoma", 24)

' Create some solid brushes
D2D.CreateSolidBrush            (brush_fill       , "#0FF")
D2D.CreateSolidBrush            (brush_stroke     , "#00FF7f")

' Create a radial brush to make some nice looking spheres
'                                                   CenterX CenterY RadiusX RadiusY OffsetX OffsetY ExtendMode
D2D.CreateRadialGradientBrush   (brush_radialfill , 0.5   , 0.5   , 1,      1,      -0.25 , -0.25,  "clamp")
' Create a gradient stop                            Color   Offset
D2D.CreateGradientStopForBrush  (brush_radialfill , "#fff", 0)
D2D.CreateGradientStopForBrush  (brush_radialfill , "#000", 1)

' Create a linear brush to make some nice looking bars
'                                                   StartX  StartY  EndX  EndY ExtendMode
D2D.CreateLinearGradientBrush   (brush_linearfill , 0     , 0     , 0   , 1,  "clamp")
' Create a gradient stop                            Color   Offset
D2D.CreateGradientStopForBrush  (brush_linearfill , "#400", 0)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#A00", 0.25)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#F00", 0.5)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#A00", 0.75)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#400", 1)

' Wait for all downloads to complete
D2D.WaitForDownloadsToComplete  ()

' Create a rectangle visual using the linear gradient brush
D2D.CreateRectangleVisual       (visual_rect    , brush_linearfill, 0, 0, 0, 0, 400, 40)
' Create a text visual
D2D.CreateTextVisual            (visual_text    , textFormat_std , 1, 200, 400, 200, 100, "Direct2D + FunBasic")
' Create a bitmap visual of the downloaded UFO
D2D.CreateBitmapVisual          (visual_ufo1    , bitmap_ufo, 1, 200, 300, 160, 128)
' Create a bitmap visual of the downloaded UFO (transparent)
D2D.CreateBitmapVisual          (visual_ufo2    , bitmap_ufo, 0.75, 400, 300, 160, 128)
' Create a bitmap visual of the downloaded UFO (transparent)
D2D.CreateBitmapVisual          (visual_ufo3    , bitmap_ufo, 0.5, 600, 300, 160, 128)
' Create a ellipse visual using the radial gradient brush
D2D.CreateEllipseVisual         (visual_ellipse , brush_radialfill, brush_stroke, 3, 200, 200, 125, 125)

' Make sure the rect visual is rendered on top of all other visuals
'   Usually the order of visual is decided by the creation order
D2D.MoveVisualToTop             (visual_rect)

' Make sure the ellipse visual is rendered below all other visuals
D2D.MoveVisualToBottom          (visual_ellipse)

x = 0
y = 0

While D2D.LastKey = ""
  x = x + 2
  y = y + 1

  ' Move the rectangle
  D2D.MoveVisual (visual_rect, x, y)

  ' Wait for screen refresh
  D2D.WaitForRefresh ()
EndWhile

D2D.CloseWindow ()

"""

let colorprint (cc : ConsoleColor) (msg : string) : unit =
  let saved = Console.ForegroundColor
  try
    Console.ForegroundColor <- cc
    Console.WriteLine msg
  finally
    Console.ForegroundColor <- saved

let hilight   msg   = colorprint ConsoleColor.White msg
let hilightf  fmt   = ksprintf hilight fmt

let error     msg   = colorprint ConsoleColor.Red msg
let errorf    fmt   = ksprintf error fmt

let initLibrary () =
  let console =
    { new IConsole with
      member x.WriteLine value = Console.WriteLine value
    }

  let surface =
    { new ISurface with
      member x.Width
        with get () = 800.0
        and  set v  = ()
      member x.Height
        with get () = 600.0
        and  set v  = ()
      member x.BackgroundColor
        with get () = "White"
        and  set v  = ()
      member x.Clear () =
        ()
      member x.ShowMessage (content, title) =
        ()
    }

  let style =
    { new IStyle with
      member x.PenWidth
        with get () = 2.0
        and  set v  = ()
      member x.PenColor
        with get () = "Black"
        and  set v  = ()
      member x.BrushColor
        with get () = "White"
        and  set v  = ()
      member x.FontSize
        with get () = 24.0
        and  set v  = ()
      member x.FontName
        with get () = "Tahoma"
        and  set v  = ()
      member x.FontItalic
        with get () = false
        and  set v  = ()
      member x.FontBold
        with get () = false
        and  set v  = ()
    }

  let shapes =
    { new IShapes with
      member x.AddText(text) =
        ""
      member x.AddEllipse(width, height) =
        ""
      member x.AddLine(x1, y1, x2, y2) =
        ""
      member x.AddTriangle(x1, y1, x2, y2, x3, y3) =
        ""
      member x.AddRectangle(width, height) =
        ""
      member x.AddImage(url) =
        ""
      member x.HideShape(name) =
        ()
      member x.ShowShape(name) =
        ()
      member x.Remove(name) =
        ()
      member x.GetLeft(name) =
        0.0
      member x.GetTop(name) =
        0.0
      member x.Move(name, x1, y1) =
        ()
      member x.Animate(name, x1, y1, duration) =
        ()
      member x.Rotate(name, angle) =
        ()
      member x.Zoom(name, scaleX, scaleY) =
        ()
      member x.GetOpacity(name) =
        0
      member x.SetOpacity(name, opacity) =
        ()
      member x.SetText(name, text) =
        ()
    }

  let timer =
    let timerEvent  = Event<EventHandler> ()
    let d = timerEvent.Publish
    { new ITimer with
      member x.Interval
        with get () = 100
        and  set v  = ()
      member x.add_Tick handler =
        ()
      member x.remove_Tick handler =
        ()
      member x.Pause() =
        ()
      member x.Resume() =
        ()
    }

  let keyboard =
    { new IKeyboard with
      member x.LastKey
        with get () =
          if Console.KeyAvailable then
            let k = Console.ReadKey()
            k.KeyChar.ToString ()
          else
            ""
      member x.add_KeyDown handler =
        ()
      member x.remove_KeyDown handler =
        ()
      member x.add_KeyUp handler =
        ()
      member x.remove_KeyUp handler =
        ()
    }

  let cts = new CancellationTokenSource ()
  let ct  = cts.Token

  _Library.Initialize (console, surface, style, null, shapes, null, null, null, keyboard, null, timer, null, null, ct)

  cts

let toDictionary (vs : seq<'TKey*'TValue>) : Dictionary<'TKey, 'TValue> =
  let d = Dictionary<_, _> ()
  for (k,v) in vs do
    d.[k] <- v
  d

let namespaces =
  typeof<_Library>.Assembly.DefinedTypes
  |> Seq.filter (fun t -> t.IsAbstract && t.IsSealed) // Static class
  |> Seq.map (fun t ->
    let methods =
      t.DeclaredMethods
      |> Seq.filter (fun m -> m.IsStatic && m.IsPublic)
      |> Seq.toArray
    let getProperties =
      t.DeclaredProperties
      |> Seq.map (fun p -> p,p.GetMethod)
      |> Seq.filter (fun (_,m) -> m <> null && (m.GetParameters ()).Length = 0)
      |> Seq.toArray
    let setProperties =
      t.DeclaredProperties
      |> Seq.map (fun p -> p,p.SetMethod)
      |> Seq.filter (fun (_,m) -> m <> null && (m.GetParameters ()).Length = 1)
      |> Seq.toArray
    t,methods,getProperties,setProperties
    )
  |> Array.ofSeq

let methods =
  namespaces
  |> Seq.collect (fun (_, methods,_ , _) -> methods)
  |> Seq.map (fun m -> (m.DeclaringType.Name, m.Name), m)
  |> toDictionary

let getters =
  namespaces
  |> Seq.collect (fun (_, _,getters , _) -> getters)
  |> Seq.map (fun (p, m) -> (p.DeclaringType.Name, p.Name), m)
  |> toDictionary

let setters =
  namespaces
  |> Seq.collect (fun (_, _, _, setters) -> setters)
  |> Seq.map (fun (p, m) -> (p.DeclaringType.Name, p.Name), m)
  |> toDictionary


let converters =
  let inline converter (f : obj -> 'T) : obj -> obj =
    fun o -> box <| f o
  [|
    typeof<string>, (box ""    , converter Convert.ToString   )
    typeof<int>   , (box 0     , converter Convert.ToInt32    )
    typeof<float> , (box 0.0   , converter Convert.ToDouble   )
    typeof<bool>  , (box false , converter Convert.ToBoolean  )
  |] |> toDictionary

let convertArg (ty : Type) (arg : obj) : obj =
  let inline isEmpty (o : obj) =
    match o with
    | :? string as s -> s.Length = 0
    | _ -> false
  let ok,v = converters.TryGetValue ty
  if ok then
    let dv,converter = v
    let isEmpty =
      match arg with
      | :? string as s -> s.Length = 0
      | _ -> false
    if isEmpty then dv
    else converter arg
  else
    arg

let invokeMethod (m : MethodInfo) (values : obj []) : obj =
  let parameters = m.GetParameters ()
  if parameters.Length = values.Length then
    let cvalues =
      [|
        for i = 0 to values.Length - 1 do
          yield convertArg parameters.[i].ParameterType values.[i]
      |]
    m.Invoke (null, cvalues)
  else
    failwithf
      "Method arguments don't match, expected: %d, found: %d: %s.%s"
        parameters.Length
        values.Length
        m.DeclaringType.Name
        m.Name


[<EntryPoint>]
let main argv =
  hilight "FunBasic Interpreter"
  try
    let ct = Thread.CurrentThread
    ct.CurrentCulture <- CultureInfo.InvariantCulture

    use cts = initLibrary ()

    let ffi =
      { new IFFI with
        member x.MethodInvoke (ns, name, values)  =
          let k     = ns, name
          let ok, m = methods.TryGetValue k
          if ok then
            invokeMethod m values
          else
            failwithf "Method not found: %s.%s" ns name
        member x.PropertyGet  (ns, name)         =
          let k     = ns, name
          let ok, m = getters.TryGetValue k
          if ok then
            invokeMethod m [||]
          else
            failwithf "Property (Get) not found: %s.%s" ns name
        member x.PropertySet  (ns, name, value)  =
          let k     = ns, name
          let ok, m = setters.TryGetValue k
          if ok then
            ignore <| invokeMethod m [|value|]
          else
            failwithf "Property (Set) not found: %s.%s" ns name
        member x.EventAdd     (ns, name, handler)=
          printfn "EventAdd: %s.%s - %A" ns name handler
          ()
      }

    let ict  = CancelToken ()
    Run (code, ffi, ict)
    0
  with
  | e ->
    errorf "Exception: %s" e.Message
    999
