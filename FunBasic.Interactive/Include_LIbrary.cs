



// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # text template file (.tt)                                                 #
// ############################################################################



// ############################################################################
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IFlickr.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISpeech.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Clock.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Controls.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Desktop.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Flickr.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\GraphicsWindow.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IConsole.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IControls.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IDrawings.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IImages.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IKeyboard.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\ImageList.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IMouse.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IShapes.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISounds.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IStyle.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISurface.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ITimer.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Math.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Mouse.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Program.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Shapes.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Sound.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Speech.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Stack.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Text.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\TextWindow.cs
// @@@ SKIPPING (Blacklisted): C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Web.cs
// @@@ SKIPPING (Blacklisted): C:\temp\GitHub\funbasic\FunBasic.Library\Properties\AssemblyInfo.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Timer.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Turtle.cs
// @@@ INCLUDING: C:\temp\GitHub\funbasic\FunBasic.Library\_Library.cs
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to top in order to work properly    
// ############################################################################
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IFlickr.cs
namespace FunBasic.Library
{
   public interface IFlickr
   {
      string GetInterestingPhoto();
      string GetTaggedPhoto(string tags);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IFlickr.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISpeech.cs
namespace FunBasic.Library
{
   public interface ISpeech
   {
      void Say(string text);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISpeech.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Clock.cs
namespace FunBasic.Library
{
   using System;
   using System.Globalization;

   public static class Clock
   {

      public static string Date
      {
         get
         {
            var format = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture);
            return DateTime.Now.ToString(format.ShortDatePattern, CultureInfo.CurrentUICulture);
         }
      }

      public static string Time
      {
         get
         {
            var format = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture);
            return DateTime.Now.ToString(format.LongTimePattern, CultureInfo.CurrentUICulture);
         }
      }

      public static int Year
      {
         get
         {
            return DateTime.Now.Year;
         }
      }

      public static int Month
      {
         get
         {
            return DateTime.Now.Month;
         }
      }

      public static int Day
      {
         get
         {
            return DateTime.Now.Day;
         }
      }

      public static int Hour
      {
         get
         {
            return DateTime.Now.Hour;
         }
      }

      public static int Minute
      {
         get
         {
            return DateTime.Now.Minute;
         }
      }

      public static int Second
      {
         get
         {
            return DateTime.Now.Second;
         }
      }

      public static int Millisecond
      {
         get
         {
            return DateTime.Now.Millisecond;
         }
      }

      public static double ElapsedMilliseconds
      {
         get
         {
            TimeSpan now = DateTime.Now - new DateTime(1900, 1, 1);
            return (double)now.TotalMilliseconds;
         }
      }

   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Clock.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Controls.cs
namespace FunBasic.Library
{
   using System;

   public static class Controls
   {
      private static IControls _controls;

      internal static void Init(IControls controls)
      {
         _controls = controls;
      }

      public static string AddButton(string caption, int x, int y)
      {
         return _controls.AddButton(caption, x, y);
      }

      public static string AddTextBox(int x, int y)
      {
         return _controls.AddTextBox(x, y);
      }

      public static string AddMultiLineTextBox(int x, int y)
      {
         return _controls.AddMultiLineTextBox(x,y);
      }

      public static string GetTextBoxText(string name)
      {
         return _controls.GetTextBoxText(name);
      }

      public static void SetTextBoxText(string name, string text)
      {
         _controls.SetTextBoxText(name, text);
      }

      public static void SetSize(string name, int width, int height)
      {
         _controls.SetSize(name, width, height);
      }

      public static string LastClickedButton
      {
         get { return _controls.LastClickedButton; }
      }

      public static string GetButtonCaption(string name)
      {
         return _controls.GetButtonCaption(name);
      }

      public static event EventHandler ButtonClicked
      {
         add { _controls.ButtonClicked += value; }
         remove { _controls.ButtonClicked -= value; }
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Controls.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Desktop.cs
namespace FunBasic.Library
{
   public static class Desktop
   {
      private static ISurface _surface;

      public static void Init(ISurface surface)
      {
         _surface = surface;
      }

      public static int Width {
         get { return (int)_surface.Width; }
      }

      public static int Height
      {
         get { return (int)_surface.Height; }
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Desktop.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Flickr.cs
namespace FunBasic.Library
{
   public static class Flickr
   {
      private static IFlickr _flickr;

      internal static void Init(IFlickr flickr)
      {
         _flickr = flickr;
      }

      public static string GetPictureOfMoment()
      {
         return _flickr.GetInterestingPhoto();
      }

      public static string GetRandomPicture(string tag)
      {
         return _flickr.GetTaggedPhoto(tag);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Flickr.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\GraphicsWindow.cs
namespace FunBasic.Library
{
   public static class GraphicsWindow
   {
      private static ISurface _surface;
      private static IDrawings _drawings;
      private static IKeyboard _keyboard;
      private static IMouse _mouse;
      private static System.Random _random = new System.Random();
      private static IStyle _style { get;set; }

      internal static void Init(
         IStyle style, 
         ISurface surface, 
         IDrawings graphics, 
         IKeyboard keyboard,      
         IMouse mouse)
      {
         _surface = surface;
         _drawings = graphics;
         _keyboard = keyboard;
         _style = style;
         _mouse = mouse;
         PenWidth = 2.0;
         BrushColor = "purple";
         PenColor = "black";
         FontSize = 12;
         FontName = "Tahoma";
      }

      public static string Title { get; set; }

      public static double Width
      {
         get { return (int) _surface.Width; }
         set { _surface.Width = value; }
      }

      public static double Height
      {
         get { return ((int ) _surface.Height / 2) * 2; }
         set { _surface.Height = value; }
      }

      public static double Top
      {
         get { return 0; }
         set { }
      }

      public static double Left
      {
         get { return 0; }
         set { }
      }

      public static bool CanResize
      {
         get { return true; }
         set { }
      }

      public static string BackgroundColor 
      {
         get { return _surface.BackgroundColor; }
         set { _surface.BackgroundColor = value; } 
      }
      
      public static string BrushColor 
      {
         get { return _style.BrushColor; }
         set { _style.BrushColor = value; }
      }

      public static string PenColor 
      {
         get { return _style.PenColor; }
         set { _style.PenColor = value; }
      }

      public static double PenWidth
      {
         get { return _style.PenWidth; }
         set { _style.PenWidth = value; }
      }

      public static double FontSize {
         get { return _style.FontSize; }
         set { _style.FontSize = value; }
      }

      public static string FontName 
      {
         get { return _style.FontName; }
         set { _style.FontName = value; }
      }

      public static bool FontItalic
      {
         get { return _style.FontItalic; }
         set { _style.FontItalic = value;  }
      }

      public static bool FontBold
      {
         get { return _style.FontBold; }
         set { _style.FontBold = value; }
      }

      public static string GetColorFromRGB(int r, int g, int b)
      {
         return string.Format("#{0:X2}{1:X2}{2:X2}",r,g,b);
      }

      public static string GetRandomColor()
      {
         return 
            string.Format("#{0:X2}{1:X2}{2:X2}", 
               _random.Next(256), 
               _random.Next(256), 
               _random.Next(256));
      }

      public static void Show()
      {
      }

      public static void Hide()
      {
      }

      public static void Clear()
      {
         _surface.Clear();
      }

      public static void ShowMessage(string content, string title)
      {
         _surface.ShowMessage(content, title);
      }

      #region Drawing
      public static void DrawText(double x, double y, string text)
      {
         _drawings.DrawText(x, y, text);
      }

      public static void DrawBoundText(double x, double y, double width, string text)
      {
         _drawings.DrawBoundText(x, y, width, text);
      }

      public static void DrawEllipse(double x, double y, double width, double height)
      {
         _drawings.DrawEllipse(x, y, width, height);
      }

      public static void DrawLine(double x1, double y1, double x2, double y2)
      {
         _drawings.DrawLine(x1, y1, x2, y2);
      }

      public static void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         _drawings.DrawTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void DrawRectangle(double x, double y, double width, double height)
      {
         _drawings.DrawRectangle(x, y, width, height);
      }

      public static void DrawImage(string imageName, double x, double y)
      {
         _drawings.DrawImage(imageName, x, y);
      }

      public static void DrawResizedImage(string imageName, double x, double y, double width, double height)
      {
         _drawings.DrawResizedImage(imageName, x, y, width, height);
      }

      public static string GetPixel(int x, int y)
      {
         return _drawings.GetPixel(x, y);
      }

      public static void SetPixel(int x, int y, string color)
      {
         _drawings.SetPixel(x, y, color);
      }
      #endregion

      #region Fill
      public static void FillEllipse(double x, double y, double width, double height)
      {
         _drawings.FillEllipse(x, y, width, height);
      }

      public static void FillTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         _drawings.FillTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void FillRectangle(double x1, double y1, double width, double height)
      {
         _drawings.FillRectangle(x1, y1, width, height);
      }
      #endregion

      #region Keyboard
      public static string LastKey
      {
         get { return _keyboard.LastKey; }
      }

      public static event System.EventHandler KeyDown
      {
         add { _keyboard.KeyDown += value; }
         remove { _keyboard.KeyDown -= value; }
      }

      public static event System.EventHandler KeyUp
      {
         add { _keyboard.KeyUp += value; }
         remove { _keyboard.KeyUp -= value; }
      }
      #endregion

      #region Mouse
      public static double MouseX
      {
         get { return _mouse.MouseX; }
      }

      public static double MouseY
      {
         get { return _mouse.MouseY; }
      }

      public static event System.EventHandler MouseDown
      {
         add { _mouse.MouseDown += value; }
         remove { _mouse.MouseDown -= value; }
      }

      public static event System.EventHandler MouseUp
      {
         add { _mouse.MouseUp += value; }
         remove { _mouse.MouseUp -= value; }
      }

      public static event System.EventHandler MouseMove
      {
         add { _mouse.MouseMove += value; }
         remove { _mouse.MouseMove -= value; }
      }
      #endregion
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\GraphicsWindow.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IConsole.cs
namespace FunBasic.Library
{
   public interface IConsole
   {
      void WriteLine(object value);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IConsole.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IControls.cs
namespace FunBasic.Library
{
   public interface IControls
   {
      #region Controls
      string AddButton(string text, int x, int y);
      string AddTextBox(int x, int y);
      string AddMultiLineTextBox(int x, int y);
      string GetTextBoxText(string name);
      void SetTextBoxText(string name, string text);
      void SetSize(string name, int width, int height);
      string LastClickedButton { get; }
      string GetButtonCaption(string name);
      event System.EventHandler ButtonClicked;
      #endregion
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IControls.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IDrawings.cs
namespace FunBasic.Library
{
   public interface IDrawings
   {
      string GetPixel(int x, int y);
      void SetPixel(int x, int y, string color);
      void DrawEllipse(double x, double y, double width, double height);
      void DrawLine(double x1, double y1, double x2, double y2);
      void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      void DrawRectangle(double x, double y, double width, double height);
      void DrawText(double x, double y, string text);
      void DrawBoundText(double x, double y, double width, string text);
      void DrawImage(string url, double x, double y);
      void DrawResizedImage(string url, double x, double y, double width, double height);
      void FillEllipse(double x, double y, double width, double height);
      void FillTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      void FillRectangle(double x, double y, double width, double height);      
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IDrawings.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IImages.cs
namespace FunBasic.Library
{
   public interface IImages
   {
      #region ImageList
      string LoadImage(string url);
      int GetImageWidth(string name);
      int GetImageHeight(string name);
      #endregion
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IImages.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IKeyboard.cs
namespace FunBasic.Library
{
   public interface IKeyboard
   {
      #region Keyboard
      string LastKey { get; }
      event System.EventHandler KeyDown;
      event System.EventHandler KeyUp;
      #endregion
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IKeyboard.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\ImageList.cs
namespace FunBasic.Library
{
   public static class ImageList
   {
      private static IImages _images;

      internal static void Init(IImages images)
      {
         _images = images;
      }

      public static int GetWidthOfImage(string name)
      {
         return _images.GetImageWidth(name);
      }

      public static int GetHeightOfImage(string name)
      {
         return _images.GetImageHeight(name);
      }

      public static string LoadImage(string url)
      {
         return _images.LoadImage(url);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\ImageList.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IMouse.cs
namespace FunBasic.Library
{
   public interface IMouse
   {
      #region Mouse
      double MouseX { get; }
      double MouseY { get; }
      event System.EventHandler MouseDown;
      event System.EventHandler MouseUp;
      event System.EventHandler MouseMove;
      #endregion
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IMouse.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IShapes.cs
namespace FunBasic.Library
{
   public interface IShapes
   {
      string AddText(string text);
      string AddEllipse(double width, double height);
      string AddLine(double x1, double y1, double x2, double y2);
      string AddTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      string AddRectangle(double width, double height);
      string AddImage(string url);
      void HideShape(string name);
      void ShowShape(string name);
      void Remove(string name);
      double GetLeft(string name);
      double GetTop(string name);
      void Move(string name, double x, double y);
      void Animate(string name, double x, double y, int duration);
      void Rotate(string name, double angle);
      void Zoom(string name, double scaleX, double scaleY);
      int GetOpacity(string name);
      void SetOpacity(string name, int opacity);
      void SetText(string name, string text);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IShapes.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISounds.cs

namespace FunBasic.Library
{
   public interface ISounds
   {
      void PlayStockSound(string name, bool wait);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISounds.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IStyle.cs
namespace FunBasic.Library
{
   public interface IStyle
   {
      double PenWidth { get; set; }
      string PenColor { get; set; }
      string BrushColor { get; set; }
      double FontSize { get; set; }
      string FontName { get; set; }
      bool FontItalic { get; set; }
      bool FontBold { get; set; }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IStyle.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISurface.cs
namespace FunBasic.Library
{
   public interface ISurface
   {
      double Width { get; set; }
      double Height { get; set; }
      string BackgroundColor { get; set; }
      void Clear();
      void ShowMessage(string content, string title);
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISurface.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ITimer.cs
namespace FunBasic.Library
{
   public interface ITimer
   {
      int Interval { get; set; }
      event System.EventHandler Tick;
      void Pause();
      void Resume();
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ITimer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Math.cs
namespace FunBasic.Library
{
   public static class Math
   {
      private static System.Random _random;

      public static double Pi
      {
         get { return 3.14159265358979; }
      }

      public static int GetRandomNumber(int maxNumber)
      {
         if (_random == null)
         {
            _random = new System.Random((int)System.DateTime.Now.Ticks);
         }
         return Math._random.Next(maxNumber) + 1;
      }

      public static int Remainder(int dividend, int divisor)
      {
         return dividend % divisor;
      }

      public static double Abs(double number)
      {
         return System.Math.Abs(number);
      }

      public static double Power(double baseNumber, double exponent)
      {
         return System.Math.Pow(baseNumber, exponent);
      }

      public static double SquareRoot(double number)
      {
         return System.Math.Sqrt(number);
      }

      public static double Floor(double number)
      {
         return System.Math.Floor(number);
      }

      public static double Ceiling(double number)
      {
         return System.Math.Ceiling(number);
      }

      public static double Round(double number)
      {
         return System.Math.Round(number);
      }

      public static double Max(double number1, double number2)
      {
         return System.Math.Max(number1, number2);
      }

      public static double Min(double number1, double number2)
      {
         return System.Math.Min(number1, number2);
      }

      public static double ArcTan(double number)
      {
         return System.Math.Atan(number);
      }

      public static double Cos(double number)
      {
         return System.Math.Cos(number);
      }

      public static double Sin(double number)
      {
         return System.Math.Sin(number);
      }

      public static double GetRadians(double degrees)
      {
         return (degrees % 360) * Pi / 180.0;
      }

      public static double GetDegrees(double radians)
      {
         return (180.0 * radians / Math.Pi) % 360;
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Math.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Mouse.cs
namespace FunBasic.Library
{
   public static class Mouse
   {
      public static bool IsLeftButtonDown { get; set; }
      public static bool IsRightButtonDown { get; set; }
      public static double MouseX { get; set; }
      public static double MouseY { get; set; }
      public static void HideCursor()
      {
      }
      public static void ShowCursor()
      {
      }
   }
}
 
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Mouse.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Program.cs
namespace FunBasic.Library
{
   using System.Threading;

   public static class Program
   {
      private static CancellationToken _token;

      internal static void Init(CancellationToken token)
      {
         _token = token;
      }

      [System.Diagnostics.DebuggerStepThrough]
      public static void Delay(int time)
      {
         try
         {
            System.Threading.Tasks.Task.Delay(time).Wait(_token);
         }
         catch (System.OperationCanceledException) { }
      }

      public static string Directory
      {
         get { return ""; }
      }

      public static void End()
      {
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Program.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Shapes.cs
namespace FunBasic.Library
{
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using System.Threading.Tasks;

   public static class Shapes
   {
      private static IShapes _shapes { get; set; }

      internal static void Init(IShapes shapes)
      {
         _shapes = shapes;
      }

      public static string AddText(string text)
      {
         return _shapes.AddText(text);
      }

      public static string AddEllipse(double width, double height)
      {
         return _shapes.AddEllipse(width, height);
      }

      public static string AddLine(double x1, double y1, double x2, double y2)
      {
         return _shapes.AddLine(x1, y1, x2, y2);
      }

      public static string AddTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         return _shapes.AddTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static string AddRectangle(double width, double height)
      {
         return _shapes.AddRectangle(width, height);
      }

      public static string AddImage(string url)
      {
         return _shapes.AddImage(url);
      }

      public static void HideShape(string name)
      {
         _shapes.HideShape(name);
      }

      public static void ShowShape(string name)
      {
         _shapes.ShowShape(name);
      }

      public static void Remove(string name)
      {
         _shapes.Remove(name);
      }

      public static int GetOpacity(string name)
      {
         return _shapes.GetOpacity(name);
      }

      public static void SetOpacity(string name, int value)
      {
         _shapes.SetOpacity(name, value);
      }

      public static void SetText(string name, string text)
      {
         _shapes.SetText(name, text);
      }

      public static double GetLeft(string name)
      {
         return _shapes.GetLeft(name);
      }

      public static double GetTop(string name)
      {
         return _shapes.GetTop(name);
      }

      public static void Move(string name, double x, double y)
      {
         _shapes.Move(name, x, y);
      }

      public static void Animate(string name, double x, double y, int duration)
      {
         _shapes.Animate(name, x, y, duration);
      }

      public static void Rotate(string name, int angle)
      {
         _shapes.Rotate(name, angle);
      }

      public static void Zoom(string name, double scaleX, double scaleY)
      {
         _shapes.Zoom(name, scaleX, scaleY);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Shapes.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Sound.cs
namespace FunBasic.Library
{
   public static class Sound
   {
      private static ISounds _sounds;

      internal static void Init(ISounds sounds)
      {
         _sounds = sounds;
      }

      public static void PlayBellRing()
      {
         _sounds.PlayStockSound("BellRing", false);
      }

      public static void PlayBellRingAndWait()
      {
         _sounds.PlayStockSound("BellRing", true);
      }

      public static void PlayChime()
      {
         _sounds.PlayStockSound("Chime", false);
      }

      public static void PlayChimeAndWait()
      {
         _sounds.PlayStockSound("Chime", true);
      }

      public static void PlayChimes()
      {
         _sounds.PlayStockSound("Chimes", false);
      }

      public static void PlayChimesAndWait()
      {
         _sounds.PlayStockSound("Chimes", true);
      }

      public static void PlayClick()
      {
         _sounds.PlayStockSound("Click", false);
      }

      public static void PlayClickAndWait()
      {
         _sounds.PlayStockSound("Click", true);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Sound.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Speech.cs
namespace FunBasic.Library
{
   public static class Speech
   {
      private static ISpeech _speech;

      internal static void Init(ISpeech speech)
      {
         _speech = speech;
      }

      public static void Say(string text)
      {
         _speech.Say(text);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Speech.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Stack.cs
namespace FunBasic.Library
{
   using System.Collections.Generic;

   public static class Stack
   {
      private static Dictionary<string, Stack<object>> _stackMap;

      internal static void Init()
      {
         _stackMap = new Dictionary<string, Stack<object>>();
      }

      public static object GetCount(string stackName)
      {
         Stack<object> values;
         if (!_stackMap.TryGetValue(stackName, out values))
         {
            values = new Stack<object>();
            _stackMap[stackName] = values;
         }
         return values.Count;
      }

      public static object PopValue(string stackName)
      {
         Stack<object> values;
         if (_stackMap.TryGetValue(stackName, out values))
         {
            return values.Pop();
         }
         return "";
      }

      public static void PushValue(string stackName, object value)
      {
         Stack<object> values;
         if (!_stackMap.TryGetValue(stackName, out values))
         {
            values = new Stack<object>();
            _stackMap[stackName] = values;
         }
         values.Push(value);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Stack.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Text.cs

namespace FunBasic.Library
{
   public static class Text
   {
      public static string Append(string a, string b)
      {
         return a + b;
      }

      public static int GetLength(string text)
      {
         return text.Length;
      }

      public static bool IsSubText(string text, string subText)
      {
         if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(subText)) return false;
         return text.Contains(subText);
      }

      public static string GetSubText(string text, int index, int length)
      {
         if (index > 0)
         {
            var startIndex = (int)Math.Min(text.Length + 1, index);
            var len = Math.Max(0, Math.Min(length, text.Length - startIndex + 1));
            return text.Substring(startIndex - 1, (int)len);
         }
         else
            return "";
      }

      public static string GetSubTextToEnd(string text, int index)
      {
         if (index > 0)
         {
            var startIndex = (int)Math.Min(text.Length + 1, index);            
            return text.Substring(startIndex - 1);
         }
         else
            return "";
      }

      public static int GetIndexOf(string text, string value)
      {
         return text.IndexOf(value) + 1;
      }

      public static int GetCharacterCode(string character)
      {
         return character.Length > 0
            ? (int)character[0]
            : 0;
      }

      public static string GetCharacter(int code)
      {
         return ((char)code).ToString();
      }

      public static string ConvertToLowerCase(string text)
      {
         return text.ToLowerInvariant();
      }

      public static string ConvertToUpperCase(string text)
      {
         return text.ToUpperInvariant();
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Text.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\TextWindow.cs
namespace FunBasic.Library
{
   public static class TextWindow
   {    
      private static IConsole _console;

      internal static void Init(IConsole console)
      {
         _console = console;
      }

      public static int Top { get; set; }
      public static int Left { get; set; }

      public static void WriteLine(object text)
      {
         _console.WriteLine(text);
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\TextWindow.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Timer.cs
namespace FunBasic.Library
{
   using System;

   public static class Timer
   {
      private static ITimer _timer;

      internal static void Init(ITimer timer)
      {
         _timer = timer;
         timer.Interval = -1;
      }

      public static int Interval
      {
         get { return _timer.Interval; }
         set { _timer.Interval = value; }
      }

      public static event EventHandler Tick
      {
         add { _timer.Tick += value; }
         remove { _timer.Tick -= value; }         
      }

      public static void Pause()
      {
         _timer.Pause();
      }

      public static void Resume()
      {
         _timer.Resume();
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Timer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Turtle.cs
namespace FunBasic.Library
{
   using System;

   public static class Turtle
   {
      private static IDrawings _graphics;
      private static IShapes _shapes;
      private static bool _isPenDown;
      private static bool _isVisible = false;

      internal static void Init(ISurface surface, IDrawings drawings, IShapes shapes)
      {
         _graphics = drawings;
         _shapes = shapes;
         X = surface.Width / 2.0;
         Y = surface.Height / 2.0;
         Angle = 0;
         _isPenDown = true;
         Hide();
      }

      public static double Angle { get; set; }
      public static double X { get; set; }
      public static double Y { get; set; }
      public static int Speed { get; set; }

      public static void Move(double distance)
      {
         Show();
         var radians = (Angle-90.0) * System.Math.PI / 180;
         var x1 = X;
         var y1 = Y;
         var x2 = x1 + distance * System.Math.Cos(radians);
         var y2 = y1 + distance * System.Math.Sin(radians);
         X = x2;
         Y = y2;
         if (_isPenDown)
         {
            _graphics.DrawLine(x1, y1, x2, y2);
         }
         _shapes.Move("Turtle", x2 - 8.0, y2 - 8.0);
      }

      public static void MoveTo(double x, double y)
      {
         Show();
         X = x;
         Y = y;
         _shapes.Move("Turtle", x - 8.0, y - 8.0);
      }

      public static void Turn(double angle)
      {
         Show();
         Angle += angle%360.0;
         _shapes.Rotate("Turtle", Angle);
      }

      public static void TurnLeft()
      {
         Show();
         Turn(-90);
      }

      public static void TurnRight()
      {
         Show();
         Turn(90);
      }

      public static void PenUp()
      {
         Show();
         _isPenDown = false;
      }

      public static void PenDown()
      {
         Show();
         _isPenDown = true;
      }

      public static void Show()
      {
         if (!_isVisible)
         {
            _isVisible = true;
            _shapes.ShowShape("Turtle");
         }
      }

      public static void Hide()
      {
         _isVisible = false;
         _shapes.HideShape("Turtle");
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Turtle.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\_Library.cs
namespace FunBasic.Library
{
   using System.Collections.Generic;
   using System.Linq;
   using System.Threading;
   using System.Reflection;
   using System;

   public static class _Library
   {
      public static void Initialize(         
         IConsole console,
         ISurface surface,
         IStyle style,
         IDrawings drawing,
         IShapes shapes,
         IImages images,
         IControls controls,
         ISounds sounds,         
         IKeyboard keyboard,
         IMouse mouse,
         ITimer timer,
         IFlickr flickr,
         ISpeech speech,
         CancellationToken token)
      {
         TextWindow.Init(console);
         Desktop.Init(surface);                  
         GraphicsWindow.Init(style, surface, drawing, keyboard, mouse);
         Shapes.Init(shapes);
         ImageList.Init(images);
         Turtle.Init(surface, drawing, shapes);
         Controls.Init(controls);
         Sound.Init(sounds);                      
         Timer.Init(timer);
         Stack.Init();
         Flickr.Init(flickr);
         Speech.Init(speech);
         Program.Init(token);
         D2D.Init(keyboard);
      }

      public static IDictionary<string, Tuple<string,string>[]> GetMemberLookup()
      {
         var ass = typeof(_Library).GetTypeInfo().Assembly;
         var types = ass.DefinedTypes;
         var lookup = new Dictionary<string, Tuple<string,string>[]>();
         foreach(var ti in types.Where(t => !t.IsInterface && !t.Name.StartsWith("_") && !t.Name.StartsWith("<")))
         {
            var ty = ass.GetType("FunBasic.Library." + ti.Name);
            var ms =
               ty.GetRuntimeMethods()
                 .Where(m => 
                        m.IsStatic && m.IsPublic && 
                        !(m.Name.StartsWith("get_") || 
                          m.Name.StartsWith("set_") || 
                          m.Name.StartsWith("add_") || 
                          m.Name.StartsWith("remove_")))
                 .Select(m => Tuple.Create(m.Name, "Method "+m.Name + "(" + String.Join(", ",m.GetParameters().Select(pi => pi.Name)) +")"));
            
            var ps =
               ty.GetRuntimeProperties().Where(p => !p.Name.StartsWith("_")).Select(p => Tuple.Create(p.Name, "Property " + p.Name));
            var es =
               ty.GetRuntimeEvents().Select(e => Tuple.Create(e.Name, "Event "+e.Name));            

            lookup.Add(ty.Name, ms.Concat(ps).Concat(es).OrderBy(x=>x).ToArray());            
         }
         return lookup;
      }
   }
}
// @@@ END_INCLUDE: C:\temp\GitHub\funbasic\FunBasic.Library\_Library.cs
// ############################################################################
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to bottom in order to work properly    
// ############################################################################
// ############################################################################
namespace Include
{
    static partial class MetaData
    {
        public const string RootPath        = @"C:\temp\GitHub\funbasic\FunBasic.Interactive\..\FunBasic.Library";
        public const string IncludeDate     = @"2015-11-29T23:32:12";

        public const string Include_0       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IFlickr.cs";
        public const string Include_1       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISpeech.cs";
        public const string Include_2       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Clock.cs";
        public const string Include_3       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Controls.cs";
        public const string Include_4       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Desktop.cs";
        public const string Include_5       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Flickr.cs";
        public const string Include_6       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\GraphicsWindow.cs";
        public const string Include_7       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IConsole.cs";
        public const string Include_8       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IControls.cs";
        public const string Include_9       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IDrawings.cs";
        public const string Include_10       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IImages.cs";
        public const string Include_11       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IKeyboard.cs";
        public const string Include_12       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\ImageList.cs";
        public const string Include_13       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IMouse.cs";
        public const string Include_14       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IShapes.cs";
        public const string Include_15       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISounds.cs";
        public const string Include_16       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\IStyle.cs";
        public const string Include_17       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ISurface.cs";
        public const string Include_18       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Interfaces\ITimer.cs";
        public const string Include_19       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Math.cs";
        public const string Include_20       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Mouse.cs";
        public const string Include_21       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Program.cs";
        public const string Include_22       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Shapes.cs";
        public const string Include_23       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Sound.cs";
        public const string Include_24       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Speech.cs";
        public const string Include_25       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Stack.cs";
        public const string Include_26       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Text.cs";
        public const string Include_27       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\TextWindow.cs";
        public const string Include_28       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Timer.cs";
        public const string Include_29       = @"C:\temp\GitHub\funbasic\FunBasic.Library\Modules\Turtle.cs";
        public const string Include_30       = @"C:\temp\GitHub\funbasic\FunBasic.Library\_Library.cs";
    }
}
// ############################################################################





