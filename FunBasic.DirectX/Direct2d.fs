module FunBasic.DirectX.Direct2d

open System
open System.Collections.Generic

open SharpDX

open FunBasic.DirectX.Common

type BrushDescriptor =
  | Transparent
  | SolidBrush of Color

  member x.IsVisible =
    match x with
    | Transparent -> false
    | _           -> true

type TextFormatDescriptor =
  | SimpleTextFormat of string * float32


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

  let brushes             = Dictionary<BrushDescriptor, Direct2D1.Brush> ()

  let createBrush (bd : BrushDescriptor) : Direct2D1.Brush =
    match bd with
    | Transparent   -> null
    | SolidBrush c  -> upcast new Direct2D1.SolidColorBrush (d2dRenderTarget, c.ToColor4 ())

  let textFormats         = Dictionary<TextFormatDescriptor, DirectWrite.TextFormat> ()

  let createTextFormat (tfd : TextFormatDescriptor) : DirectWrite.TextFormat =
    match tfd with
    | SimpleTextFormat (fontFamilyName, fontSize) ->
        new DirectWrite.TextFormat (dwFactory, fontFamilyName, fontSize)

  member x.GetBrush (bd : BrushDescriptor) : Direct2D1.Brush =
    brushes.GetOrCreate bd createBrush

  member x.GetTextFormat (tfd : TextFormatDescriptor) : DirectWrite.TextFormat =
    textFormats.GetOrCreate tfd createTextFormat

  member x.Width                  = width
  member x.Height                 = height
  member x.ClientSize             = size2 (float32 width) (float32 height)

  member x.Draw (a : Direct2D1.RenderTarget->unit) =
    d2dRenderTarget.BeginDraw ()
    try
      a d2dRenderTarget
    finally
        d2dRenderTarget.EndDraw ()
        swapChain.Present (1, DXGI.PresentFlags.None)


  interface IDisposable with
    member x.Dispose () =
      disposeValuesInDictionary textFormats
      disposeValuesInDictionary brushes

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
    (onRender   : Device -> Direct2D1.RenderTarget -> unit) =
      use form                = new Windows.RenderForm (title)

      form.ClientSize         <- Drawing.Size (width,height)

      let device              = ref <| new Device (form)

      let disposeDevice ()    = dispose !device
      let recreateDevice ()   = disposeDevice ()
                                device := new Device (form)

      use onExitDisposeDevice = onExit disposeDevice

      let resizer             = EventHandler (fun o e -> recreateDevice ())
      let keyUp               = Windows.Forms.KeyEventHandler (fun o e -> onKeyUp e.KeyValue)

      form.Resize.AddHandler resizer
      form.KeyUp.AddHandler keyUp

      use onExitRemoveHandler = onExit <| fun () -> form.Resize.RemoveHandler resizer

      let render () =
        let d = !device

        d.Draw <| fun d2dRenderTarget -> onRender d d2dRenderTarget

      Windows.RenderLoop.Run (form, render)
