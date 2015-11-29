namespace FunBasic.DirectX.ApiModel






  // Prefix: BitmapInput
  type BitmapInput =
  // Method: DownloadBitmap (uri)
  | DownloadBitmap       of uri:string

  // Prefix: BrushInput
  type BrushInput =
  // Method: CreateGradientStopForBrush (color, offset)
  | CreateGradientStopForBrush of color:string*offset:double
  // Method: CreateLinearGradientBrush (startX, startY, endX, endY, extendMode)
  | CreateLinearGradientBrush of startX:double*startY:double*endX:double*endY:double*extendMode:string
  // Method: CreateRadialGradientBrush (centerX, centerY, radiusX, radiusY, offsetX, offsetY, extendMode)
  | CreateRadialGradientBrush of centerX:double*centerY:double*radiusX:double*radiusY:double*offsetX:double*offsetY:double*extendMode:string
  // Method: CreateSolidBrush (color)
  | CreateSolidBrush     of color:string

  // Prefix: GlobalInput
  type GlobalInput =
  // Method: ClearVisuals 
  | ClearVisuals         
  // Method: HideWindow 
  | HideWindow           
  // Method: SetBackground (color)
  | SetBackground        of color:string
  // Method: ShowWindow 
  | ShowWindow           

  // Prefix: InternalInput
  type InternalInput =
  // Method: CreateBitmapFromBits (bitmapId, bits)
  | CreateBitmapFromBits of bitmapId:int*bits:byte[]
  // Method: DiscardWindow 
  | DiscardWindow        
  // Method: DoNothing 
  | DoNothing            

  // Prefix: TextFormatInput
  type TextFormatInput =
  // Method: CreateTextFormat (fontFamily, fontSize)
  | CreateTextFormat     of fontFamily:string*fontSize:double

  // Prefix: VisualInput
  type VisualInput =
  // Method: CloneVisual (cloneVisualId)
  | CloneVisual          of cloneVisualId:int
  // Method: CreateBitmapVisual (bitmapId, opacity, centerX, centerY, width, height)
  | CreateBitmapVisual   of bitmapId:int*opacity:double*centerX:double*centerY:double*width:double*height:double
  // Method: CreateEllipseVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
  | CreateEllipseVisual  of fillBrushId:int*strokeBrushId:int*strokeWidth:double*centerX:double*centerY:double*width:double*height:double
  // Method: CreateRectangleVisual (fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
  | CreateRectangleVisual of fillBrushId:int*strokeBrushId:int*strokeWidth:double*centerX:double*centerY:double*width:double*height:double
  // Method: CreateTextVisual (fillBrushId, textFormatId, centerX, centerY, width, height, text)
  | CreateTextVisual     of fillBrushId:int*textFormatId:int*centerX:double*centerY:double*width:double*height:double*text:string
  // Method: MoveVisual (x, y)
  | MoveVisual           of x:double*y:double
  // Method: ResizeVisual (width, height)
  | ResizeVisual         of width:double*height:double


  type Input =
  // Prefix: BitmapInput (bitmapId, payload)
  | BitmapInput          of bitmapId:int*payload:BitmapInput
  // Prefix: BrushInput (brushId, payload)
  | BrushInput           of brushId:int*payload:BrushInput
  // Prefix: GlobalInput (payload)
  | GlobalInput          of payload:GlobalInput
  // Prefix: InternalInput (payload)
  | InternalInput        of payload:InternalInput
  // Prefix: TextFormatInput (textFormatId, payload)
  | TextFormatInput      of textFormatId:int*payload:TextFormatInput
  // Prefix: VisualInput (visualId, payload)
  | VisualInput          of visualId:int*payload:VisualInput

