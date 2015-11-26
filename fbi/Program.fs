﻿
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
number = 100
While (number > 1)
  TextWindow.WriteLine(number)
  number = number / 2
EndWhile
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
      member x.add_Tick (handler) =
        ()
      member x.remove_Tick (handler) =
        ()
      member x.Pause() =
        ()
      member x.Resume() =
        ()
    }

  let cts = new CancellationTokenSource ()
  let ct  = cts.Token

  _Library.Initialize (console, surface, style, null, shapes, null, null, null, null, null, timer, null, null, ct)

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
