module internal FunBasic.DirectX.Direct2d

open System
open System.Collections.Generic
open System.IO
open System.Threading

open SharpDX

open FunBasic.DirectX.Common

type BitmapDescriptor =
  | Bitmap of byte[]

type BrushDescriptor =
  | Transparent
  | SolidBrush of Color4

  member x.IsVisible =
    match x with
    | Transparent -> false
    | _           -> true

type TextFormatDescriptor =
  | SimpleTextFormat of string * float32

type BitmapSource =
  {
    Stream    : Stream
    Decoder   : WIC.BitmapDecoder
    Source    : WIC.BitmapSource
    Converter : WIC.FormatConverter
  }

  member x.HasValue =
    x.Stream        <> null
    && x.Decoder    <> null
    && x.Source     <> null
    && x.Converter  <> null

  interface IDisposable with
    member x.Dispose () =
      dispose x.Converter
      dispose x.Source
      dispose x.Decoder
      dispose x.Stream

let noSource : BitmapSource = 
  { Stream = null; Decoder = null; Source = null; Converter = null}

type DeviceIndependentResources() = 

  let imagingFactory  = new WIC.ImagingFactory ()

  let bitmaps         = Dictionary<BitmapId, BitmapDescriptor*BitmapSource> ()

  let createBitmap (bid : BitmapId) (bd : BitmapDescriptor) : BitmapDescriptor*BitmapSource = 
    match bd with 
    | Bitmap bytes ->
      try
        // TODO:
        let ms    = new MemoryStream (bytes)
        let dec   = new WIC.BitmapDecoder (imagingFactory, ms, WIC.DecodeOptions.CacheOnLoad)
        let fc    = dec.FrameCount
        if fc > 0 then
          let f = dec.GetFrame 0
          let c = new WIC.FormatConverter (imagingFactory)
          c.Initialize (f, WIC.PixelFormat.Format32bppPRGBA)
          let bs = { Stream = ms; Decoder = dec; Source = f; Converter = c }
          bd, bs
        else
          bd, noSource
      with 
      | e -> 
        traceException e
        bd, noSource

  let updateBitmap      = makeResourceUpdater createBitmap

  member x.CreateBitmap (bid : BitmapId) (bd : BitmapDescriptor) : BitmapSource =
    snd <| bitmaps.CreateOrUpdate bid bd createBitmap updateBitmap

  member x.GetBitmap (bid : BitmapId) : BitmapSource =
    findResource bid noSource bitmaps
  
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

  let bitmaps             = Dictionary<BitmapId, BitmapDescriptor*Direct2D1.Bitmap> ()
  let brushes             = Dictionary<BrushId, BrushDescriptor*Direct2D1.Brush> ()
  let textFormats         = Dictionary<TextFormatId, TextFormatDescriptor*DirectWrite.TextFormat> ()

  let disposeResources () : unit =
    disposeResourceDictionary textFormats
    disposeResourceDictionary brushes
    disposeResourceDictionary bitmaps

  let createBrush (bid : BrushId) (bd : BrushDescriptor) : BrushDescriptor*Direct2D1.Brush =
    match bd with
    | Transparent   -> bd, null
    | SolidBrush c  -> bd, upcast new Direct2D1.SolidColorBrush (d2dRenderTarget, c)

  let createBitmap (bid : BitmapId) (bd : BitmapDescriptor) : BitmapDescriptor*Direct2D1.Bitmap =
    match bd with
    | Bitmap bytes  -> 
      try
        let bitmapSource = dir.CreateBitmap bid bd
        if bitmapSource.HasValue then
          let p = Direct2D1.BitmapProperties <| Direct2D1.PixelFormat (DXGI.Format.R8G8B8A8_UNorm, Direct2D1.AlphaMode.Premultiplied)
          bd, Direct2D1.Bitmap.FromWicBitmap (d2dRenderTarget, bitmapSource.Converter, p)
        else
          bd, null
      with
      | e -> 
        traceException e
        bd, null

  let createTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor) : TextFormatDescriptor*DirectWrite.TextFormat =
    match tfd with
    | SimpleTextFormat (fontFamilyName, fontSize) ->
        tfd, new DirectWrite.TextFormat (dwFactory, fontFamilyName, fontSize)

  let updateBitmap      = makeResourceUpdater createBitmap
  let updateBrush       = makeResourceUpdater createBrush
  let updateTextFormat  = makeResourceUpdater createTextFormat

  member x.InitializeResources (di : DeviceInit) : unit =
    disposeResources ()

    // TODO: Rewrite

    for bid, bd in di.Bitmaps do
      ignore <| bitmaps.GetOrCreate bid bd createBitmap

    for bid, bd in di.Brushes do
      ignore <| brushes.GetOrCreate bid bd createBrush

    for tfid, tfd in di.TextFormats do
      ignore <| textFormats.GetOrCreate tfid tfd createTextFormat

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

  member x.CreateBitmap (bid : BitmapId) (bd : BitmapDescriptor) : Direct2D1.Bitmap =
    snd <| bitmaps.CreateOrUpdate bid bd createBitmap updateBitmap

  member x.GetBitmap (bid : BitmapId) : Direct2D1.Bitmap =
    findResource bid null bitmaps

  member x.CreateBrush (bid : BrushId) (bd : BrushDescriptor) : Direct2D1.Brush =
    snd <| brushes.CreateOrUpdate bid bd createBrush updateBrush

  member x.GetBrush (bid : BrushId) : Direct2D1.Brush =
    findResource bid null brushes

  member x.CreateTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor)  : DirectWrite.TextFormat =
    snd <| textFormats.CreateOrUpdate tfid tfd createTextFormat updateTextFormat

  member x.GetTextFormat (tfid : TextFormatId) : DirectWrite.TextFormat =
    findResource tfid null textFormats

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
