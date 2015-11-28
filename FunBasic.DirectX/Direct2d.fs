module internal FunBasic.DirectX.Direct2d

open System
open System.Collections.Generic

open SharpDX

open FunBasic.DirectX.Common

type BrushDescriptor =
  | Transparent
  | SolidBrush of Color4

  member x.IsVisible =
    match x with
    | Transparent -> false
    | _           -> true

type TextFormatDescriptor =
  | SimpleTextFormat of string * float32

type DeviceInit =
  {
    Brushes     : (BrushId*BrushDescriptor) []
    TextFormats : (TextFormatId*TextFormatDescriptor) []
  }

type Device (form : Windows.RenderForm) =

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

  let brushes             = Dictionary<BrushId, BrushDescriptor*Direct2D1.Brush> ()
  let textFormats         = Dictionary<TextFormatId, TextFormatDescriptor*DirectWrite.TextFormat> ()

  let makeUpdater (creator : 'K -> 'U -> 'U*'V ) : 'K  -> 'U*'V -> 'U -> 'U*'V =
    fun k uv u ->
      let pu, v = uv
      if Object.ReferenceEquals (u, v) then 
        uv
      else
        let nv = creator k u
        dispose v
        nv

  let createBrush (bid : BrushId) (bd : BrushDescriptor) : BrushDescriptor*Direct2D1.Brush =
    match bd with
    | Transparent   -> bd, null
    | SolidBrush c  -> bd, upcast new Direct2D1.SolidColorBrush (d2dRenderTarget, c)

  let createTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor) : TextFormatDescriptor*DirectWrite.TextFormat =
    match tfd with
    | SimpleTextFormat (fontFamilyName, fontSize) ->
        tfd, new DirectWrite.TextFormat (dwFactory, fontFamilyName, fontSize)

  let updateBrush = makeUpdater createBrush

  let updateTextFormat = makeUpdater createTextFormat

  let disposeResourceDictionary (d : IDictionary<_, _*#IDisposable>) : unit =
    try
      for kv in d do
        dispose (snd kv.Value)
    finally
        d.Clear ()

  let disposeResources () : unit =
    disposeResourceDictionary brushes
    disposeResourceDictionary textFormats

  let findResource (k : 'K) (d : Dictionary<'K, 'U*'V>) : 'V =
    let ok, uv = d.TryGetValue k
    if ok then
      snd uv
    else
      null

  member x.InitializeResources (di : DeviceInit) : unit =
    disposeResources ()

    for bid, bd in di.Brushes do
      ignore <| brushes.GetOrCreate bid bd createBrush

    for tfid, tfd in di.TextFormats do
      ignore <| textFormats.GetOrCreate tfid tfd createTextFormat

  member x.CreateDeviceInit () : DeviceInit =
    let inline stripResource (s : Dictionary<'K, 'U*'V>) =
      s
      |> Seq.map (fun kv -> kv.Key, fst kv.Value)
      |> Seq.toArray

    let bs =
      brushes
      |> stripResource
    let tfs =
      textFormats
      |> stripResource

    { Brushes = bs; TextFormats = tfs }

  member x.CreateBrush (bid :  BrushId) (bd : BrushDescriptor) : Direct2D1.Brush =
    snd <| brushes.CreateOrUpdate bid bd createBrush updateBrush

  member x.GetBrush (bid : BrushId) : Direct2D1.Brush =
    findResource bid brushes

  member x.CreateTextFormat (tfid : TextFormatId) (tfd : TextFormatDescriptor)  : DirectWrite.TextFormat =
    snd <| textFormats.CreateOrUpdate tfid tfd createTextFormat updateTextFormat

  member x.GetTextFormat (tfid : TextFormatId) : DirectWrite.TextFormat =
    findResource tfid textFormats

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
    (title      : string                                  )
    (width      : int                                     )
    (height     : int                                     )
    (onKeyUp    : int -> unit                             )
    (onRender   : Device -> Direct2D1.RenderTarget -> bool) =
    use form                = new Windows.RenderForm (title)

    form.ClientSize         <- Drawing.Size (width,height)

    let device              = ref <| new Device (form)

    let disposeDevice ()    = dispose !device
    let recreateDevice ()   =
      let di = (!device).CreateDeviceInit ()
      disposeDevice ()
      device := new Device (form)
      (!device).InitializeResources di

    use onExitDisposeDevice = onExit disposeDevice

    let resizer             = EventHandler (fun o e -> recreateDevice ())
    let keyUp               = Windows.Forms.KeyEventHandler (fun o e -> onKeyUp e.KeyValue)

    form.Resize.AddHandler resizer
    form.KeyUp.AddHandler keyUp

    use onExitRemoveHandler = onExit <| fun () -> form.Resize.RemoveHandler resizer

    let render () =
      let d = !device

      let cont = d.Draw <| fun d2dRenderTarget -> onRender d d2dRenderTarget

      if not cont then
        form.Close ()

    Windows.RenderLoop.Run (form, render)
