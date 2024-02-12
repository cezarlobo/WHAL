using System.ServiceProcess;
using IntegracoesVTEX;

namespace IntegracoesVETX
{
	internal static class Program
	{
		private static void Main()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
#if DEBUG // debugando como DEBUG
                var service = new Scheduler();
                service.StartDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#endif // debugando como Release
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Scheduler()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
