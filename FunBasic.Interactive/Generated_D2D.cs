





namespace FunBasic.Library
{
  static partial class D2D
  {
    // Method: DownloadBitmap
    public static void DownloadBitmap (
        int        bitmapId
      , string     url
      )
    {
      Scene.SendInput (
        new Input.NewBitmapInput (
            bitmapId
          , new BitmapInput.NewDownloadBitmap (
              url
            )));
    }

    // Method: CreateSolidBrush
    public static void CreateSolidBrush (
        int        brushId
      , string     color
      )
    {
      Scene.SendInput (
        new Input.NewBrushInput (
            brushId
          , new BrushInput.NewCreateSolidBrush (
              color
            )));
    }

    // Method: ClearVisuals
    public static void ClearVisuals (
      )
    {
      Scene.SendInput (
        new Input.NewGlobalInput (
            new GlobalInput.NewClearVisuals (
            )));
    }

    // Method: HideWindow
    public static void HideWindow (
      )
    {
      Scene.SendInput (
        new Input.NewGlobalInput (
            new GlobalInput.NewHideWindow (
            )));
    }

    // Method: SetBackground
    public static void SetBackground (
        string     color
      )
    {
      Scene.SendInput (
        new Input.NewGlobalInput (
            new GlobalInput.NewSetBackground (
              color
            )));
    }

    // Method: ShowWindow
    public static void ShowWindow (
      )
    {
      Scene.SendInput (
        new Input.NewGlobalInput (
            new GlobalInput.NewShowWindow (
            )));
    }

    // Method: CreateTextFormat
    public static void CreateTextFormat (
        int        textFormatId
      , string     fontFamily
      , double     fontSize
      )
    {
      Scene.SendInput (
        new Input.NewTextFormatInput (
            textFormatId
          , new TextFormatInput.NewCreateTextFormat (
              fontFamily
            , fontSize
            )));
    }

    // Method: CreateBitmapVisual
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
        new Input.NewVisualInput (
            visualId
          , new VisualInput.NewCreateBitmapVisual (
              bitmapId
            , opacity
            , centerX 
            , centerY
            , width
            , height
            )));
    }

    // Method: CreateEllipseVisual
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
        new Input.NewVisualInput (
            visualId
          , new VisualInput.NewCreateEllipseVisual (
              fillBrushId
            , strokeBrushId
            , strokeWidth
            , centerX
            , centerY
            , width
            , height
            )));
    }

    // Method: CreateRectangleVisual
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
        new Input.NewVisualInput (
            visualId
          , new VisualInput.NewCreateRectangleVisual (
              fillBrushId
            , strokeBrushId
            , strokeWidth
            , centerX
            , centerY
            , width
            , height
            )));
    }

    // Method: CreateTextVisual
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
        new Input.NewVisualInput (
            visualId
          , new VisualInput.NewCreateTextVisual (
              fillBrushId
            , textFormatId
            , centerX
            , centerY
            , width
            , height
            , text
            )));
    }

    // Method: MoveVisual
    public static void MoveVisual (
        int        visualId
      , double     x
      , double     y
      )
    {
      Scene.SendInput (
        new Input.NewVisualInput (
            visualId
          , new VisualInput.NewMoveVisual (
              x
            , y
            )));
    }

  }
}
