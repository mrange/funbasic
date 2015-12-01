





namespace FunBasic.Library
{
  using FunBasic.DirectX.ApiModel;

  static partial class D2D
  {
    // ------------------------------------------------------------------------
    // Prefix: BitmapInput
    // ------------------------------------------------------------------------

    // Method: DownloadBitmap (bitmapId, uri)
    public static void DownloadBitmap (
        int        bitmapId
      , string     uri
      )
    {
      Scene.SendInput (
        Input.NewBitmapInput (
            bitmapId
          , BitmapInput.NewDownloadBitmap (
              uri
            )
          )
        );
    }

    // ------------------------------------------------------------------------
    // Prefix: BrushInput
    // ------------------------------------------------------------------------

    // Method: CreateGradientStopForBrush (brushId, color, offset)
    public static void CreateGradientStopForBrush (
        int        brushId
      , string     color
      , double     offset
      )
    {
      Scene.SendInput (
        Input.NewBrushInput (
            brushId
          , BrushInput.NewCreateGradientStopForBrush (
              color
            , offset
            )
          )
        );
    }

    // Method: CreateLinearGradientBrush (brushId, startX, startY, endX, endY, extendMode)
    public static void CreateLinearGradientBrush (
        int        brushId
      , double     startX
      , double     startY
      , double     endX
      , double     endY
      , string     extendMode
      )
    {
      Scene.SendInput (
        Input.NewBrushInput (
            brushId
          , BrushInput.NewCreateLinearGradientBrush (
              startX
            , startY
            , endX
            , endY
            , extendMode
            )
          )
        );
    }

    // Method: CreateRadialGradientBrush (brushId, centerX, centerY, radiusX, radiusY, offsetX, offsetY, extendMode)
    public static void CreateRadialGradientBrush (
        int        brushId
      , double     centerX
      , double     centerY
      , double     radiusX
      , double     radiusY
      , double     offsetX
      , double     offsetY
      , string     extendMode
      )
    {
      Scene.SendInput (
        Input.NewBrushInput (
            brushId
          , BrushInput.NewCreateRadialGradientBrush (
              centerX
            , centerY
            , radiusX
            , radiusY
            , offsetX
            , offsetY
            , extendMode
            )
          )
        );
    }

    // Method: CreateSolidBrush (brushId, color)
    public static void CreateSolidBrush (
        int        brushId
      , string     color
      )
    {
      Scene.SendInput (
        Input.NewBrushInput (
            brushId
          , BrushInput.NewCreateSolidBrush (
              color
            )
          )
        );
    }

    // ------------------------------------------------------------------------
    // Prefix: GlobalInput
    // ------------------------------------------------------------------------

    // Method: ClearVisuals 
    public static void ClearVisuals (
      )
    {
      Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.ClearVisuals
          )
        );
    }

    // Method: HideWindow 
    public static void HideWindow (
      )
    {
      Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.HideWindow
          )
        );
    }

    // Method: SetBackground (color)
    public static void SetBackground (
        string     color
      )
    {
      Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.NewSetBackground (
              color
            )
          )
        );
    }

    // Method: ShowWindow 
    public static void ShowWindow (
      )
    {
      Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.ShowWindow
          )
        );
    }

    // ------------------------------------------------------------------------
    // Prefix: TextFormatInput
    // ------------------------------------------------------------------------

    // Method: CreateTextFormat (textFormatId, fontFamily, fontSize)
    public static void CreateTextFormat (
        int        textFormatId
      , string     fontFamily
      , double     fontSize
      )
    {
      Scene.SendInput (
        Input.NewTextFormatInput (
            textFormatId
          , TextFormatInput.NewCreateTextFormat (
              fontFamily
            , fontSize
            )
          )
        );
    }

    // ------------------------------------------------------------------------
    // Prefix: VisualInput
    // ------------------------------------------------------------------------

    // Method: CloneVisual (visualId, cloneVisualId)
    public static void CloneVisual (
        int        visualId
      , int        cloneVisualId
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewCloneVisual (
              cloneVisualId
            )
          )
        );
    }

    // Method: CreateBitmapVisual (visualId, bitmapId, opacity, centerX, centerY, width, height)
    public static void CreateBitmapVisual (
        int        visualId
      , int        bitmapId
      , double     opacity
      , double     centerX
      , double     centerY
      , double     width
      , double     height
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewCreateBitmapVisual (
              bitmapId
            , opacity
            , centerX
            , centerY
            , width
            , height
            )
          )
        );
    }

    // Method: CreateEllipseVisual (visualId, fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
    public static void CreateEllipseVisual (
        int        visualId
      , int        fillBrushId
      , int        strokeBrushId
      , double     strokeWidth
      , double     centerX
      , double     centerY
      , double     width
      , double     height
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewCreateEllipseVisual (
              fillBrushId
            , strokeBrushId
            , strokeWidth
            , centerX
            , centerY
            , width
            , height
            )
          )
        );
    }

    // Method: CreateRectangleVisual (visualId, fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
    public static void CreateRectangleVisual (
        int        visualId
      , int        fillBrushId
      , int        strokeBrushId
      , double     strokeWidth
      , double     centerX
      , double     centerY
      , double     width
      , double     height
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewCreateRectangleVisual (
              fillBrushId
            , strokeBrushId
            , strokeWidth
            , centerX
            , centerY
            , width
            , height
            )
          )
        );
    }

    // Method: CreateTextVisual (visualId, fillBrushId, textFormatId, centerX, centerY, width, height, text)
    public static void CreateTextVisual (
        int        visualId
      , int        fillBrushId
      , int        textFormatId
      , double     centerX
      , double     centerY
      , double     width
      , double     height
      , string     text
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewCreateTextVisual (
              fillBrushId
            , textFormatId
            , centerX
            , centerY
            , width
            , height
            , text
            )
          )
        );
    }

    // Method: MoveVisual (visualId, x, y)
    public static void MoveVisual (
        int        visualId
      , double     x
      , double     y
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewMoveVisual (
              x
            , y
            )
          )
        );
    }

    // Method: MoveVisualToBottom (visualId)
    public static void MoveVisualToBottom (
        int        visualId
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.MoveVisualToBottom
          )
        );
    }

    // Method: MoveVisualToTop (visualId)
    public static void MoveVisualToTop (
        int        visualId
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.MoveVisualToTop
          )
        );
    }

    // Method: RemoveVisual (visualId)
    public static void RemoveVisual (
        int        visualId
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.RemoveVisual
          )
        );
    }

    // Method: ResizeVisual (visualId, width, height)
    public static void ResizeVisual (
        int        visualId
      , double     width
      , double     height
      )
    {
      Scene.SendInput (
        Input.NewVisualInput (
            visualId
          , VisualInput.NewResizeVisual (
              width
            , height
            )
          )
        );
    }

  }
}
