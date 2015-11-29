namespace FunBasic.DirectX.ApiModel






    type BitmapInput =
    // Method: DownloadBitmap
    | DownloadBitmap       of url:string

    type BrushInput =
    // Method: CreateSolidBrush
    | CreateSolidBrush     of color:string

    type GlobalInput =
    // Method: ClearVisuals
    | ClearVisuals         of unit
    // Method: HideWindow
    | HideWindow           of unit
    // Method: SetBackground
    | SetBackground        of color:string
    // Method: ShowWindow
    | ShowWindow           of unit

    type TextFormatInput =
    // Method: CreateTextFormat
    | CreateTextFormat     of fontFamily:string*fontSize:double

    type VisualInput =
    // Method: CreateBitmapVisual
    | CreateBitmapVisual   of bitmapId:int*opacity:double*centerX :double*centerY:double*width:double*height:double
    // Method: CreateEllipseVisual
    | CreateEllipseVisual  of fillBrushId:int*strokeBrushId:int*strokeWidth:double*centerX:double*centerY:double*width:double*height:double
    // Method: CreateRectangleVisual
    | CreateRectangleVisual of fillBrushId:int*strokeBrushId:int*strokeWidth:double*centerX:double*centerY:double*width:double*height:double
    // Method: CreateTextVisual
    | CreateTextVisual     of fillBrushId:int*textFormatId:int*centerX:double*centerY:double*width:double*height:double*text:string
    // Method: MoveVisual
    | MoveVisual           of x:double*y:double


    type Input =
    | BitmapInput          of bitmapId:int*payload:BitmapInput
    | BrushInput           of brushId:int*payload:BrushInput
    | GlobalInput          of payload:GlobalInput
    | TextFormatInput      of textFormatId:int*payload:TextFormatInput
    | VisualInput          of visualId:int*payload:VisualInput

