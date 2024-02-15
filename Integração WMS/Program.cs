// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Program
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe

using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;


namespace Integração.WMS
{
  internal static class Program
  {
    private static void Main()
    {
      if (Debugger.IsAttached)
      {
        new Service1().StartDebug();
        Thread.Sleep(-1);
      }
      else
        ServiceBase.Run(new ServiceBase[1]
        {
          (ServiceBase) new Service1()
        });
    }
  }
}
