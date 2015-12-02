





namespace PowerShell.DirectX
{
  using FunBasic.DirectX.ApiModel;

  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Management.Automation;
  using System.Management.Automation.Runspaces;

  static class AllCmdLets
  {
    public static Collection<CmdletConfigurationEntry> CmdLets
    {
      get
      {
        var coll = new Collection<CmdletConfigurationEntry> ()
        {
          // ------------------------------------------------------------------------
          // Prefix: BitmapInput
          // ------------------------------------------------------------------------

          new CmdletConfigurationEntry ("Download-D2DBitmap", typeof(DownloadBitmapCommand), ""),
          // ------------------------------------------------------------------------
          // Prefix: BrushInput
          // ------------------------------------------------------------------------

          new CmdletConfigurationEntry ("Create-D2DGradientStopForBrush", typeof(CreateGradientStopForBrushCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DLinearGradientBrush", typeof(CreateLinearGradientBrushCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DRadialGradientBrush", typeof(CreateRadialGradientBrushCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DSolidBrush", typeof(CreateSolidBrushCommand), ""),
          // ------------------------------------------------------------------------
          // Prefix: GlobalInput
          // ------------------------------------------------------------------------

          new CmdletConfigurationEntry ("Clear-D2DVisuals", typeof(ClearVisualsCommand), ""),
          new CmdletConfigurationEntry ("Hide-D2DWindow", typeof(HideWindowCommand), ""),
          new CmdletConfigurationEntry ("Set-D2DBackground", typeof(SetBackgroundCommand), ""),
          new CmdletConfigurationEntry ("Show-D2DWindow", typeof(ShowWindowCommand), ""),
          new CmdletConfigurationEntry ("Wait-D2DForDownloads", typeof(WaitForDownloadsCommand), ""),
          new CmdletConfigurationEntry ("Wait-D2DForRefresh", typeof(WaitForRefreshCommand), ""),
          // ------------------------------------------------------------------------
          // Prefix: TextFormatInput
          // ------------------------------------------------------------------------

          new CmdletConfigurationEntry ("Create-D2DTextFormat", typeof(CreateTextFormatCommand), ""),
          // ------------------------------------------------------------------------
          // Prefix: VisualInput
          // ------------------------------------------------------------------------

          new CmdletConfigurationEntry ("Clone-D2DVisual", typeof(CloneVisualCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DBitmapVisual", typeof(CreateBitmapVisualCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DEllipseVisual", typeof(CreateEllipseVisualCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DRectangleVisual", typeof(CreateRectangleVisualCommand), ""),
          new CmdletConfigurationEntry ("Create-D2DTextVisual", typeof(CreateTextVisualCommand), ""),
          new CmdletConfigurationEntry ("Move-D2DVisual", typeof(MoveVisualCommand), ""),
          new CmdletConfigurationEntry ("Move-D2DVisualToBottom", typeof(MoveVisualToBottomCommand), ""),
          new CmdletConfigurationEntry ("Move-D2DVisualToTop", typeof(MoveVisualToTopCommand), ""),
          new CmdletConfigurationEntry ("Remove-D2DVisual", typeof(RemoveVisualCommand), ""),
          new CmdletConfigurationEntry ("Resize-D2DVisual", typeof(ResizeVisualCommand), ""),
        };

        return coll;
      }
    }
  }

  // ------------------------------------------------------------------------
  // Prefix: BitmapInput
  // ------------------------------------------------------------------------

  // Method: DownloadBitmap (bitmapId, uri)
  sealed class DownloadBitmapCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        BitmapId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public string     Uri
    {
      get;
      set;
    }
      = "http://cliparts.co/cliparts/BTg/ERG/BTgERG4oc.png";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewBitmapInput (
            BitmapId
          , BitmapInput.NewDownloadBitmap (
              Uri
            )
          )
        );
    }
  }

  // ------------------------------------------------------------------------
  // Prefix: BrushInput
  // ------------------------------------------------------------------------

  // Method: CreateGradientStopForBrush (brushId, color, offset)
  sealed class CreateGradientStopForBrushCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        BrushId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public string     Color
    {
      get;
      set;
    }
      = "#fff";

    [Parameter(Mandatory = false)]
    public double     Offset
    {
      get;
      set;
    }
      = 0.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewBrushInput (
            BrushId
          , BrushInput.NewCreateGradientStopForBrush (
              Color
            , Offset
            )
          )
        );
    }
  }

  // Method: CreateLinearGradientBrush (brushId, startX, startY, endX, endY, extendMode)
  sealed class CreateLinearGradientBrushCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        BrushId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public double     StartX
    {
      get;
      set;
    }
      = 0.0;

    [Parameter(Mandatory = false)]
    public double     StartY
    {
      get;
      set;
    }
      = 0.0;

    [Parameter(Mandatory = false)]
    public double     EndX
    {
      get;
      set;
    }
      = 0.0;

    [Parameter(Mandatory = false)]
    public double     EndY
    {
      get;
      set;
    }
      = 1.0;

    [Parameter(Mandatory = false)]
    public string     ExtendMode
    {
      get;
      set;
    }
      = "mirror";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewBrushInput (
            BrushId
          , BrushInput.NewCreateLinearGradientBrush (
              StartX
            , StartY
            , EndX
            , EndY
            , ExtendMode
            )
          )
        );
    }
  }

  // Method: CreateRadialGradientBrush (brushId, centerX, centerY, radiusX, radiusY, offsetX, offsetY, extendMode)
  sealed class CreateRadialGradientBrushCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        BrushId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public double     CenterX
    {
      get;
      set;
    }
      = 0.5;

    [Parameter(Mandatory = false)]
    public double     CenterY
    {
      get;
      set;
    }
      = 0.5;

    [Parameter(Mandatory = false)]
    public double     RadiusX
    {
      get;
      set;
    }
      = 1.0;

    [Parameter(Mandatory = false)]
    public double     RadiusY
    {
      get;
      set;
    }
      = 1.0;

    [Parameter(Mandatory = false)]
    public double     OffsetX
    {
      get;
      set;
    }
      = -0.25;

    [Parameter(Mandatory = false)]
    public double     OffsetY
    {
      get;
      set;
    }
      = -0.25;

    [Parameter(Mandatory = false)]
    public string     ExtendMode
    {
      get;
      set;
    }
      = "mirror";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewBrushInput (
            BrushId
          , BrushInput.NewCreateRadialGradientBrush (
              CenterX
            , CenterY
            , RadiusX
            , RadiusY
            , OffsetX
            , OffsetY
            , ExtendMode
            )
          )
        );
    }
  }

  // Method: CreateSolidBrush (brushId, color)
  sealed class CreateSolidBrushCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        BrushId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public string     Color
    {
      get;
      set;
    }
      = "#fff";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewBrushInput (
            BrushId
          , BrushInput.NewCreateSolidBrush (
              Color
            )
          )
        );
    }
  }

  // ------------------------------------------------------------------------
  // Prefix: GlobalInput
  // ------------------------------------------------------------------------

  // Method: ClearVisuals 
  sealed class ClearVisualsCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.ClearVisuals
          )
        );
    }
  }

  // Method: HideWindow 
  sealed class HideWindowCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.HideWindow
          )
        );
    }
  }

  // Method: SetBackground (color)
  sealed class SetBackgroundCommand : Cmdlet
  {
    [Parameter(Mandatory = false)]
    public string     Color
    {
      get;
      set;
    }
      = "#004";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.NewSetBackground (
              Color
            )
          )
        );
    }
  }

  // Method: ShowWindow 
  sealed class ShowWindowCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.ShowWindow
          )
        );
    }
  }

  // Method: WaitForDownloads 
  sealed class WaitForDownloadsCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.WaitForDownloads
          )
        );
    }
  }

  // Method: WaitForRefresh 
  sealed class WaitForRefreshCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewGlobalInput (
            GlobalInput.WaitForRefresh
          )
        );
    }
  }

  // ------------------------------------------------------------------------
  // Prefix: TextFormatInput
  // ------------------------------------------------------------------------

  // Method: CreateTextFormat (textFormatId, fontFamily, fontSize)
  sealed class CreateTextFormatCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        TextFormatId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public string     FontFamily
    {
      get;
      set;
    }
      = "Tahoma";

    [Parameter(Mandatory = false)]
    public double     FontSize
    {
      get;
      set;
    }
      = 24.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewTextFormatInput (
            TextFormatId
          , TextFormatInput.NewCreateTextFormat (
              FontFamily
            , FontSize
            )
          )
        );
    }
  }

  // ------------------------------------------------------------------------
  // Prefix: VisualInput
  // ------------------------------------------------------------------------

  // Method: CloneVisual (visualId, cloneVisualId)
  sealed class CloneVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = true)]
    public int        CloneVisualId
    {
      get;
      set;
    }


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewCloneVisual (
              CloneVisualId
            )
          )
        );
    }
  }

  // Method: CreateBitmapVisual (visualId, bitmapId, opacity, centerX, centerY, width, height)
  sealed class CreateBitmapVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = true)]
    public int        BitmapId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public double     Opacity
    {
      get;
      set;
    }
      = 1.0;

    [Parameter(Mandatory = false)]
    public double     CenterX
    {
      get;
      set;
    }
      = 344.0;

    [Parameter(Mandatory = false)]
    public double     CenterY
    {
      get;
      set;
    }
      = 257.0;

    [Parameter(Mandatory = false)]
    public double     Width
    {
      get;
      set;
    }
      = 488.0;

    [Parameter(Mandatory = false)]
    public double     Height
    {
      get;
      set;
    }
      = 314.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewCreateBitmapVisual (
              BitmapId
            , Opacity
            , CenterX
            , CenterY
            , Width
            , Height
            )
          )
        );
    }
  }

  // Method: CreateEllipseVisual (visualId, fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
  sealed class CreateEllipseVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public int        FillBrushId
    {
      get;
      set;
    }
      = 4;

    [Parameter(Mandatory = false)]
    public int        StrokeBrushId
    {
      get;
      set;
    }
      = 1;

    [Parameter(Mandatory = false)]
    public double     StrokeWidth
    {
      get;
      set;
    }
      = 3.0;

    [Parameter(Mandatory = false)]
    public double     CenterX
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     CenterY
    {
      get;
      set;
    }
      = 200.0;

    [Parameter(Mandatory = false)]
    public double     Width
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Height
    {
      get;
      set;
    }
      = 100.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewCreateEllipseVisual (
              FillBrushId
            , StrokeBrushId
            , StrokeWidth
            , CenterX
            , CenterY
            , Width
            , Height
            )
          )
        );
    }
  }

  // Method: CreateRectangleVisual (visualId, fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, width, height)
  sealed class CreateRectangleVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public int        FillBrushId
    {
      get;
      set;
    }
      = 3;

    [Parameter(Mandatory = false)]
    public int        StrokeBrushId
    {
      get;
      set;
    }
      = 1;

    [Parameter(Mandatory = false)]
    public double     StrokeWidth
    {
      get;
      set;
    }
      = 3.0;

    [Parameter(Mandatory = false)]
    public double     CenterX
    {
      get;
      set;
    }
      = 200.0;

    [Parameter(Mandatory = false)]
    public double     CenterY
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Width
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Height
    {
      get;
      set;
    }
      = 100.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewCreateRectangleVisual (
              FillBrushId
            , StrokeBrushId
            , StrokeWidth
            , CenterX
            , CenterY
            , Width
            , Height
            )
          )
        );
    }
  }

  // Method: CreateTextVisual (visualId, fillBrushId, textFormatId, centerX, centerY, width, height, text)
  sealed class CreateTextVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public int        FillBrushId
    {
      get;
      set;
    }
      = 1;

    [Parameter(Mandatory = false)]
    public int        TextFormatId
    {
      get;
      set;
    }
      = 0;

    [Parameter(Mandatory = false)]
    public double     CenterX
    {
      get;
      set;
    }
      = 200.0;

    [Parameter(Mandatory = false)]
    public double     CenterY
    {
      get;
      set;
    }
      = 200.0;

    [Parameter(Mandatory = false)]
    public double     Width
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Height
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public string     Text
    {
      get;
      set;
    }
      = "Hello World!";


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewCreateTextVisual (
              FillBrushId
            , TextFormatId
            , CenterX
            , CenterY
            , Width
            , Height
            , Text
            )
          )
        );
    }
  }

  // Method: MoveVisual (visualId, x, y)
  sealed class MoveVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public double     X
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Y
    {
      get;
      set;
    }
      = 100.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewMoveVisual (
              X
            , Y
            )
          )
        );
    }
  }

  // Method: MoveVisualToBottom (visualId)
  sealed class MoveVisualToBottomCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.MoveVisualToBottom
          )
        );
    }
  }

  // Method: MoveVisualToTop (visualId)
  sealed class MoveVisualToTopCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.MoveVisualToTop
          )
        );
    }
  }

  // Method: RemoveVisual (visualId)
  sealed class RemoveVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.RemoveVisual
          )
        );
    }
  }

  // Method: ResizeVisual (visualId, width, height)
  sealed class ResizeVisualCommand : Cmdlet
  {
    [Parameter(Mandatory = true)]
    public int        VisualId
    {
      get;
      set;
    }

    [Parameter(Mandatory = false)]
    public double     Width
    {
      get;
      set;
    }
      = 100.0;

    [Parameter(Mandatory = false)]
    public double     Height
    {
      get;
      set;
    }
      = 100.0;


    protected override void ProcessRecord()
    {
      SceneManager.Scene.SendInput (
        Input.NewVisualInput (
            VisualId
          , VisualInput.NewResizeVisual (
              Width
            , Height
            )
          )
        );
    }
  }

}

