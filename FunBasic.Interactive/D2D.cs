using FunBasic.DirectX.Scene;
using System;

namespace FunBasic.Library
{
  public static partial class D2D
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

    public static string LastKey
    {
      get
      {
        return keyboard.LastKey;
      }
    }
  }
}
