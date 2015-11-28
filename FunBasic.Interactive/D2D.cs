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

    public static void WaitForRefresh ()
    {
      Scene.WaitForRefresh ();
    }

    public static void Background (string color)
    {
      Scene.Background (brushId, color);
    }
                                   
    public static void SolidBrush (int brushId, string color)
    {
      Scene.SolidBrush (brushId, color);
    }

    public static void Move (int visualId, double x, double y)
    {
      Scene.Move (visualId, x, y);
    }

    public static void Rectangle (int visualId, int fillBrush, int strokeBrush, double strokeWidth, double x, double y, double w, double h)
    {
      Scene.Rectangle (visualId, fillBrush, strokeBrush, strokeWidth, x, y, w, h);
    }

    public static void Ellipse (int visualId, int fillBrush, int strokeBrush, double strokeWidth, double x, double y, double w, double h)
    {
      Scene.Ellipse (visualId, fillBrush, strokeBrush, strokeWidth, x, y, w, h);
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
