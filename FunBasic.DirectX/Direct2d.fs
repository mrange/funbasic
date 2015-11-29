module internal FunBasic.DirectX.Direct2d

open System
open System.Collections.Generic
open System.IO
open System.Threading

open SharpDX

open FunBasic.DirectX.Common

type BitmapDescriptor =
  | BitmapBits of byte[]

type BrushDescriptor =
  | Transparent
  | SolidBrush          of  Color4
                        //  Start   End     ExtendMode           Stops
  | LinearGradientBrush of  Vector2*Vector2*Direct2D1.ExtendMode*(Color4*float32) []
                        //  Center  Radius  Origin  ExtendMode           Stops
  | RadialGradientBrush of  Vector2*Vector2*Vector2*Direct2D1.ExtendMode*(Color4*float32) []

  member x.IsVisible =
    match x with
    | Transparent -> false
    | _           -> true

type TextFormatDescriptor =
  | SimpleTextFormat of string * float32

[<AllowNullLiteral>]
type BitmapSource ( stream : Stream
                  , decoder   : WIC.BitmapDecoder
                  , source    : WIC.BitmapSource
                  , converter : WIC.FormatConverter
  ) =

  member x.Converter  = converter
  member x.Source     = source
  member x.Decoder    = decoder
  member x.Stream     = stream

  interface IDisposable with
    member x.Dispose () =
      dispose converter
      dispose source
      dispose decoder
      dispose stream

type DeviceIndependentResources() =
  let imagingFactory  = new WIC.ImagingFactory ()

  let bitmaps         = Dictionary<BitmapId, BitmapDescriptor*BitmapSource> ()

  let createBitmap (bid : BitmapId) (bd : BitmapDescriptor) : BitmapSource =
    match bd with
    | BitmapBits bytes ->
      try
        let ms    = new MemoryStream (bytes)
        let dec   = new WIC.BitmapDecoder (imagingFactory, ms, WIC.DecodeOptions.CacheOnLoad)
        let fc    = dec.FrameCount
        if fc > 0 then
          let f = dec.GetFrame 0
          let c = new WIC.FormatConverter (imagingFactory)
          c.Initialize (f, WIC.PixelFormat.Format32bppPRGBA)
          let bs = new BitmapSource (ms, dec, f, c)
          bs
        else
          null
      with
      | e ->
        traceException e
        null

  member x.ReserverBitmap (bid : BitmapId) (bd : BitmapDescriptor) : unit =
    reserveResource bid bd bitmaps

  member x.GetBitmap (bid : BitmapId) : BitmapSource =
    getResource bid null createBitmap bitmaps

  interface IDisposable with
    member x.Dispose () =
      disposeResourceDictionary bitmaps
      dispose imagingFactory

type DeviceInit =
  {
    Bitmaps     : (BitmapId*BitmapDescriptor) []
    Brushes     : (BrushId*BrushDescriptor) []
    TextFormats : (TextFormatId*TextFormatDescriptor) []
  }

type Device (dir : DeviceIndependentResources, form : Windows.RenderForm) =

  let getDeviceAndSwapChain (form : Windows.RenderForm) =
    let width               = form.ClientSize.Width
    let height              = form.ClientSize.Height

    let mutable desc        = DXGI.SwapChainDescription ()
    desc.BufferCount        <- 2
    desc.ModeDescription    <- SharpDX.DXGI.ModeDescription (
                                width                           ,
                                height                          ,
                                DXGI.Rational (60, 1)           ,
                                DXGI.Format.R8G8B8A8_UNorm
                                )
    desc.IsWindowed         <- Bool true
    desc.OutputHandle       <- form.Handle
    desc.SampleDescription  <- DXGI.SampleDescription (1,0)
    desc.SwapEffect         <- DXGI.SwapEffect.Sequential
    desc.Usage              <- DXGI.Usage.RenderTargetOutput

    let featureLevels       =
      [|
        Direct3D.FeatureLevel.Level_11_0
        Direct3D.FeatureLevel.Level_10_1
        Direct3D.FeatureLevel.Level_10_0
        Direct3D.FeatureLevel.Level_9_3
        Direct3D.FeatureLevel.Level_9_2
        Direct3D.FeatureLevel.Level_9_1
      |]

    Direct3D11.Device.CreateWithSwapChain (
      Direct3D.DriverType.Hardware                ,
      Direct3D11.DeviceCreationFlags.BgraSupport  ,
      featureLevels                               ,
      desc
      )

  let width               = float32 form.ClientSize.Width
  let height              = float32 form.ClientSize.Height

  let wicf                = new WIC.ImagingFactory ()
  let dwFactory           = new DirectWrite.Factory (DirectWrite.FactoryType.Isolated)
  let d2dFactory          = new Direct2D1.Factory (Direct2D1.FactoryType.SingleThreaded)
  let device, swapChain   = getDeviceAndSwapChain form
  let factory             = swapChain.GetParent<DXGI.Factory> ()

  let associateWithWindow = factory.MakeWindowAssociation (form.Handle, DXGI.WindowAssociationFlags.IgnoreAll)

  let backBuffer          = Direct3D11.Texture2D.FromSwapChain<Direct3D11.Texture2D> (swapChain, 0)
  let surface             = backBuffer.QueryInterface<SharpDX.DXGI.Surface> ();
  let d2dRenderTarget     = new Direct2D1.RenderTarget (
                              d2dFactory                          ,
                              surface                             ,
                              Direct2D1.RenderTargetProperties (
                                Direct2D1.PixelFormat (
                                  DXGI.Format.Unknown         ,
                                  Direct2D1.AlphaMode.Premultiplied
                                  )
                                )
                              )

  let defaultBrush        = new Direct2D1.SolidColorBrush (d2dRenderTarget, Color.Red.ToColor4 ()) :> Direct2D1.Brush
  let defaultTextFormat   = new DirectWrite.TextFormat (dwFactory, "Tahoma", 24.0f)

  let bitmaps             = Dictionary<BitmapId, BitmapDescriptor*Direct2D1.Bitmap> ()
  let brushes             = Dictionary<BrushId, BrushDescriptor*Direct2D1.Brush> ()
  let textFormats         = Dictionary<TextFormatId, TextFormatDescriptor*DirectWrite.TextFormat> ()

  let disposeResources () : unit =
    disposeResourceDictionary textFormats
    disposeResourceDictionary brushes
    disposeResourceDictionary bitmaps

  let gradientStop (c : Color4) (p : float32) : Direct2D1.GradientStop =
    let mutable gs = Direct2D1.GradientStop ()
    gs.Color    <- c
    gs.Position <- p
    gs

  let gradientStopCollection (em : Direct2D1.ExtendMode) (stops : (Color4*float32) []) : Direct2D1.GradientStopCollection =
    let gstops  = stops |> Array.map (fun (c,p) -> gradientStop c p)
    new Direct2D1.GradientStopCollection (d2dRenderTarget, gstops, em)

  let createBrush (bid : BrushId) (bd : BrushDescriptor) : Direct2D1.Brush =
    match bd with
    | Transparent                             ->
      null
    | SolidBrush c                            ->
      upcast new Direct2D1.SolidColorBrush (d2dRenderTarget, c)
    | LinearGradientBrush (_, _, _, stops)
    | RadialGradientBrush (_, _, _, _, stops) when stops.Length = 0 ->
      null
    | LinearGradientBrush (sp, ep, em, stops) ->
      let mutable props = Direct2D1.LinearGradientBrushProperties ()
      props.StartPoint  <- sp
      props.EndPoint    <- ep
      use gcoll         = gradientStopCollection em stops
      upcast new Direct2D1.LinearGradientBrush (d2dRenderTarget, props, gcoll)
    | RadialGradientBrush (c, r, o, em, stops) ->
      let mutable props           = Direct2D1.RadialGradientBrushProperties ()
      props.Center                <- c
      props.RadiusX               <- r.X
      props.RadiusY               <- r.Y
      props.GradientOriginOffset  <- o
      use gcoll                   = gradientStopCollection em stops
      upcast new Direct2D1.RadialGradientBrush (d2dRenderTarget, props, gcoll)

  let createBitmap (bid : BitmapId) (bd : BitmapDescriptor) : Direct2D1.Bitmap =
    try
      dir.ReserverBitmap bid bd
      let bitmapSource = dir.GetBitmap bid
      if bitmapSource <> null then
        let p = Direct2D1.BitmapProperties <| Direct2D1.PixelFormat (DXGI.Format.R8G8B8A8_UNorm, Direct2D1.AlphaMode.Premultiplied)
        Direct2D1.Bitmap.FromWicBitmap (d2dRenderTarget, bitmapSource.Converter, p)
      else
        null
    with
    | e ->
      traceException e
      null

  let createTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor) : DirectWrite.TextFormat =
    match tfd with
    | SimpleTextFormat (fontFamilyName, fontSize) ->
        new DirectWrite.TextFormat (dwFactory, fontFamilyName, fontSize)

  member x.InitializeResources (di : DeviceInit) : unit =
    disposeResources ()

    for bid, bd in di.Bitmaps do
      x.ReserveBitmap bid bd

    for bid, bd in di.Brushes do
      x.ReserveBrush bid bd

    for tfid, tfd in di.TextFormats do
      x.ReserveTextFormat tfid tfd

  member x.CreateDeviceInit () : DeviceInit =
    let inline stripResource (s : Dictionary<'K, 'U*'V>) =
      s
      |> Seq.map (fun kv -> kv.Key, fst kv.Value)
      |> Seq.toArray

    let bmps =
      bitmaps
      |> stripResource

    let bs =
      brushes
      |> stripResource
    let tfs =
      textFormats
      |> stripResource

    { Bitmaps = bmps; Brushes = bs; TextFormats = tfs }

  member x.ReserveBitmap (bid : BitmapId) (bd : BitmapDescriptor) : unit =
    reserveResource bid bd bitmaps

  member x.GetBitmap (bid : BitmapId) : Direct2D1.Bitmap =
    getResource bid null createBitmap bitmaps

  member x.GetBitmapDescriptor (bid : BitmapId) : BitmapDescriptor option =
    getDescriptor bid bitmaps

  member x.ReserveBrush (bid : BrushId) (bd : BrushDescriptor) : unit =
    reserveResource bid bd brushes

  member x.GetBrush (bid : BrushId) : Direct2D1.Brush =
    getResource bid defaultBrush createBrush brushes

  member x.GetBrushDescriptor (bid : BrushId) : BrushDescriptor option =
    getDescriptor bid brushes

  member x.ReserveTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor)  : unit =
    reserveResource tfid tfd textFormats

  member x.GetTextFormat (tfid : TextFormatId) : DirectWrite.TextFormat =
    getResource tfid defaultTextFormat createTextFormat textFormats

  member x.GetTextFormatDescriptor (tfid : TextFormatId) : TextFormatDescriptor option =
    getDescriptor tfid textFormats

  member x.Width                  = width
  member x.Height                 = height
  member x.ClientSize             = size2 (float32 width) (float32 height)

  member x.Draw (a : Direct2D1.RenderTarget -> bool) : bool =
    d2dRenderTarget.BeginDraw ()
    try
      a d2dRenderTarget
    finally
        d2dRenderTarget.EndDraw ()
        swapChain.Present (1, DXGI.PresentFlags.None)

  interface IDisposable with
    member x.Dispose () =
      disposeResources ()

      dispose defaultTextFormat
      dispose defaultBrush
      dispose d2dRenderTarget
      dispose surface
      dispose backBuffer
      dispose factory
      dispose swapChain
      dispose device
      dispose d2dFactory
      dispose dwFactory

module Window =
  let show
    (title      : string                                                              )
    (width      : int                                                                 )
    (height     : int                                                                 )
    (onKeyUp    : int -> unit                                                         )
    (onRender   : ((unit -> unit) -> unit) -> Device -> Direct2D1.RenderTarget -> bool) =
    use syncContext         = new Windows.Forms.WindowsFormsSynchronizationContext ()
    SynchronizationContext.SetSynchronizationContext syncContext

    use form                = new Windows.RenderForm (title)

    form.ClientSize         <- Drawing.Size (width,height)

    use dir                 = new DeviceIndependentResources ()
    let device              = ref <| new Device (dir, form)

    let disposeDevice ()    = dispose !device
    let recreateDevice ()   =
      let di = (!device).CreateDeviceInit ()
      disposeDevice ()
      device := new Device (dir, form)
      (!device).InitializeResources di

    use onExitDisposeDevice = onExit disposeDevice

    let resizer             = EventHandler (fun o e -> recreateDevice ())
    let keyUp               = Windows.Forms.KeyEventHandler (fun o e -> onKeyUp e.KeyValue)

    form.Resize.AddHandler resizer
    form.KeyUp.AddHandler keyUp

    use onExitRemoveHandler = onExit <| fun () -> form.Resize.RemoveHandler resizer

    let delay (a : unit -> unit) : unit =
      SynchronizationContext.Current.Post (SendOrPostCallback (fun _ -> a ()), null)

    let render () =
      let d = !device

      let cont = d.Draw <| fun d2dRenderTarget -> onRender delay d d2dRenderTarget

      if not cont then
        form.Close ()

    Windows.RenderLoop.Run (form, render)
