# funbasic

The real repository for funbasic is [here](https://bitbucket.org/ptrelford/funbasic)

This place is just for my experiments and tweaks of funbasic

Aimed at @ptrelford
===================

So it's still in a bit rough shape, I cheated quite much just to get it run
on desktop. FunBasic.Runner.sln is the desktop solution I am fooling around in.
`fbi` is an embryo for a console application that runs FunBasic sources.

Anyway, I fooled around with Direct2D and SmallBasic before because I thought the
visuals in SmallBasic was way too slow. This is basically a port of that work
rewritten in F#. Direct2D are in many ways great as it has many high-level
graphic primitives (like brushes) while giving good performance.

If we are talking raw-performance Direct3D crushes Direct2D but then we are
basically pushing triangles and doing tesselation ourselves.

I am using Direct3D11 but it's quite possible to backport to Direct3D9.

API Ideas
=========

I think it's important that the API (and FunBasic is general) are forgiving.

No random crashes because of slightly wrong indata. Better to render what we got
as good as we can (using fallback brushes if necessary).

The API lets the developer create objects such as rectangles, ellipses,
 bitmaps and so on. These persists on screen until removed. I think this model
is easier to understand than redrawing each frame from scratch (which for real-life
applications are often better).

So the example program (lo-fi graphics here)

```
D2D.ShowWindow ()

D2D.ClearVisuals ()

' Numbers used to assign different D2D objects
'   TextFormats, Bitmaps, Brushes and Visuals all have their own
'   number series
textFormat_std  = 0001

bitmap_ufo      = 0001

brush_fill      = 0001
brush_stroke    = 0002
brush_radialfill= 0003
brush_radialfill= 0004

visual_rect     = 0001
visual_ellipse  = 0002
visual_text     = 0003
visual_ufos     = 1000

number_of_ufos  = 3

' Sets the background to a darkblue (#rgb)
D2D.SetBackground               ("#003")

' Download a nice ufo
'   Note that this starts the download process but doesn't block
'   This enables you to download multiple bitmaps simultanenously
D2D.DownloadBitmap              (bitmap_ufo       , "http://blitzetc.ru/images/5/5a/BMax-UFO.png")

' Create a text format that will be used to render text
'   If no text format can be found a default will be used
D2D.CreateTextFormat            (textFormat_std   , "Tahoma", 48)

' Create some solid brushes
D2D.CreateSolidBrush            (brush_fill       , "#0FF")
D2D.CreateSolidBrush            (brush_stroke     , "#00FF7f")

' Create a radial brush to make some nice looking spheres
'                                                   CenterX CenterY RadiusX RadiusY OffsetX OffsetY ExtendMode
D2D.CreateRadialGradientBrush   (brush_radialfill , 0.5   , 0.5   , 1,      1,      -0.25 , -0.25,  "clamp")
' Create a gradient stop                            Color   Offset
D2D.CreateGradientStopForBrush  (brush_radialfill , "#fff", 0)
D2D.CreateGradientStopForBrush  (brush_radialfill , "#07f", 0.15)
D2D.CreateGradientStopForBrush  (brush_radialfill , "#004", 1)

' Create a linear brush to make some nice looking bars
'                                                   StartX  StartY  EndX  EndY ExtendMode
D2D.CreateLinearGradientBrush   (brush_linearfill , 0     , 0     , 0   , 1,  "clamp")
' Create a gradient stop                            Color   Offset
D2D.CreateGradientStopForBrush  (brush_linearfill , "#024", 0)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#05A", 0.25)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#08F", 0.5)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#05A", 0.75)
D2D.CreateGradientStopForBrush  (brush_linearfill , "#024", 1)

' Wait for all downloads to complete
D2D.WaitForDownloadsToComplete  ()

' Create a text visual
D2D.CreateTextVisual            (visual_text    , textFormat_std , 1, 400, 400, 800, 60, "Direct2D + FunBasic")
' Create a rectangle visual using the linear gradient brush
D2D.CreateRectangleVisual       (visual_rect    , brush_linearfill, 0, 0, 400, 0, 800, 60)
' Create a ellipse visual using the radial gradient brush
D2D.CreateEllipseVisual         (visual_ellipse , brush_radialfill, brush_stroke, 3, 200, 200, 125, 125)
' Create a bitmap visual of the downloaded UFO
D2D.CreateBitmapVisual          (visual_ufos + 0, bitmap_ufo, 1, 200, 300, 160, 128)
' Clone the ufo visual a few times
For i = 1 To number_of_ufos
  D2D.CloneVisual (visual_ufos + i, visual_ufos + 0)
EndFor

' Make sure the ellipse visual is rendered on below all other visuals
'   Usually the order of visual is decided by the creation order
D2D.MoveVisualToBottom          (visual_ellipse)

' Make sure the text & rect visual are rendered below all other visuals
D2D.MoveVisualToTop             (visual_rect)
D2D.MoveVisualToTop             (visual_text)

i = 0
x = 0
y = 0

While D2D.LastKey = ""
  i = i + 1
  x = i * 2
  y = i * 1
  r = i * 0.03


  rx = 550.0 + 100.0*Math.Sin (3*ri)
  ry = 200.0 + 100.0*Math.Cos (ri)
  ' Move the rectangle & text
  D2D.MoveVisual (visual_rect, 400, ry)
  D2D.MoveVisual (visual_text, rx, ry)

  ' Move the ufos
  For ii = 0 To number_of_ufos
    ri = r + ii
    ux = 200.0 + 100.0*Math.Sin (ri)
    uy = 200.0 + 100.0*Math.Cos (ri)
    D2D.MoveVisual (visual_ufos + ii, ux, uy)
  EndFor

  ' Wait for screen refresh
  D2D.WaitForRefresh ()
EndWhile

D2D.CloseWindow ()

```
