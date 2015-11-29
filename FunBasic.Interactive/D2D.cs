using FunBasic.DirectX.Scene;
using System;

namespace FunBasic.Library
{
  public static class D2D
  {
    static IKeyboard keyboard;

    static Scene scene = null;

    static Scene Scene
    {
      get
      {
        if (scene == null)
        {
          scene = new Scene ();
        }

        return scene;
      }
    }

    static void DisposeIt (IDisposable d)
    {
      if (d != null)
      {
        d.Dispose ();
      }
    }

    public static void Init(IKeyboard kb)
    {
      keyboard = kb;
    }

    public static void Discard ()
    {
      if (scene != null)
      {
        DisposeIt (scene);
        scene = null;
      }
    }

    public static void Show ()
    {
      Scene.Show ();
    }

    public static void Close ()
    {
      Scene.Close ();
    }

    public static void Clear ()
    {
      Scene.Clear ();
    }

    public static void DownloadBitmap (int bitmapId, string url)
    {
      Scene.DownloadBitmap (bitmapId, url);
    }

    public static void WaitForRefresh ()
    {
      Scene.WaitForRefresh ();
    }

    public static void Background (string color)
    {
      Scene.Background (color);
    }
                                   
    public static void SolidBrush (int brushId, string color)
    {
      Scene.SolidBrush (brushId, color);
    }

    public static void TextFormat (int textFormatId, string fontFamily, double fontSize)
    {
      Scene.TextFormat (textFormatId, fontFamily, fontSize);
    }

    public static void Move (int visualId, double x, double y)
    {
      Scene.Move (visualId, x, y);
    }

    public static void Bitmap (int visualId, int bitmapId, double opacity, double left, double top, double width, double height)
    {
      Scene.Bitmap (visualId, bitmapId, opacity, left, top, width, height);
    }

    public static void Rectangle (int visualId, int fillBrushId, int strokeBrushId, double strokeWidth, double left, double top, double width, double height)
    {
      Scene.Rectangle (visualId, fillBrushId, strokeBrushId, strokeWidth, left, top, width, height);
    }

    public static void Ellipse (int visualId, int fillBrushId, int strokeBrushId, double strokeWidth, double centerX, double centerY, double radiusX, double radiusY)
    {
      Scene.Ellipse (visualId, fillBrushId, strokeBrushId, strokeWidth, centerX, centerY, radiusX, radiusY);
    }

    public static void Text (int visualId, int fillBrushId, int textFormatId, double left, double top, double width, double height, string text)
    {
      Scene.Text (visualId, fillBrushId, textFormatId, left, top, width, height, text);
    }

    public static string LastKey
    {
      get
      {
        return keyboard.LastKey;
      }
    }
  }
}
