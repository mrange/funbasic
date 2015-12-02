namespace PowerShell.DirectX
{
  using FunBasic.DirectX.Scene;

  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Management.Automation;
  using System.Management.Automation.Runspaces;

  [RunInstaller(true)]
  public class DirectXSnapIn : CustomPSSnapIn
  {

    readonly Collection<CmdletConfigurationEntry> m_cmdLets;

    public DirectXSnapIn ()
    {
      m_cmdLets = AllCmdLets.CmdLets;

      m_cmdLets.Add (new CmdletConfigurationEntry ("Discard-D2DWindow", typeof(DiscardWindowCommand), ""));
    }

    public override string Description
    {
      get
      {
        return "DirectX SnapIn";
      }
    }

    public override string Name
    {
      get
      {
        return "DirectXSnapIn";
      }
    }

    public override string Vendor
    {
      get
      {
        return "VRONGE";
      }
    }

    public override Collection<CmdletConfigurationEntry> Cmdlets
    {
      get
      {
        return m_cmdLets;
      }
    }
  }

  sealed class DiscardWindowCommand : Cmdlet
  {

    protected override void ProcessRecord()
    {
      SceneManager.DiscardWindow ();
    }
  }

  static partial class SceneManager
  {
    static readonly object lockObject = new object ();
    static volatile Scene scene       = null;

    public static Scene Scene
    {
      get
      {
        if (scene == null)
        {
          lock (lockObject)
          {
            if (scene == null)
            {
              scene = new Scene ();
            }
          }
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

    public static void DiscardWindow ()
    {
      if (scene != null)
      {
        lock (lockObject)
        {
          if (scene != null)
          {
            DisposeIt (scene);
            scene = null;
          }
        }
      }
    }
  }
}
