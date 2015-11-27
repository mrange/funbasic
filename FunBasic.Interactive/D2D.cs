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

    public static void Move (int id, double x, double y)
    {
      Scene.Move (id, x, y);
    }

    public static int Rectangle (object fillBrush, object strokeBrush, double strokeWidth, double x, double y, double w, double h)
    {
      return Scene.Rectangle (fillBrush, strokeBrush, strokeWidth, x, y, w, h);
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
