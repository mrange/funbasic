namespace FunBasic.DirectX.Scene

open FunBasic.DirectX.ApiModel
open FunBasic.DirectX.Common
open FunBasic.DirectX.SceneRenderer

open System
open System.Threading

type Scene() =
  let refreshLock           = new ManualResetEvent false

  let downloadLock          = obj ()
  let mutable downloadCount = 0

  let onRender () =
    ignore <| refreshLock.Set ()

  let renderThread, renderQueue = createRenderer onRender

  let hasLock_incrementDownloadCount () =
    downloadCount <- downloadCount + 1

  let incrementDownloadCount () =
    lock downloadLock hasLock_incrementDownloadCount

  let hasLock_decrementDownloadCount () =
    downloadCount <- downloadCount - 1
    Monitor.Pulse downloadLock

  let decrementDownloadCount () =
    lock downloadLock hasLock_decrementDownloadCount

  let hasLock_waitForDownloadsToComplete () =
    while downloadCount > 0 do
      ignore <| Monitor.Wait downloadLock

  let waitForDownloadsToComplete () =
    lock downloadLock hasLock_waitForDownloadsToComplete

  let downloadBitmap (bitmapId :int) (uri : string) : Async<unit> =
    async {
      try
        try
          use wc    = new System.Net.WebClient ()
          let uri   = fixString uri
          let uri   = Uri uri
          let! bits = wc.AsyncDownloadData <| uri

          renderQueue.Enqueue <| InternalInput (CreateBitmapFromBits (bitmapId, bits))
        with
        | e ->
          traceException e
      finally
        decrementDownloadCount ()
    }

  member x.SendInput (input : Input) : unit =
    match input with
    | GlobalInput (WaitForDownloads) ->
      waitForDownloadsToComplete ()
    | GlobalInput (WaitForRefresh) ->
      // Improve on wait for refresh to support command buffering to ensure 60fps
      ignore <| refreshLock.Reset ()
      ignore <| refreshLock.WaitOne ()
    | BitmapInput (bitmapId, DownloadBitmap uri) ->
      // Download the bits before passing this to the render thread
      incrementDownloadCount ()
      Async.Start <| downloadBitmap bitmapId uri
    | _ ->
      renderQueue.Enqueue input

  interface IDisposable with
    member x.Dispose () =
      renderQueue.Enqueue <| InternalInput DiscardWindow  // Kills thread
      renderThread.Join ()
      dispose refreshLock
