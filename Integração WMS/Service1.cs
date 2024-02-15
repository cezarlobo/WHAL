using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Integração.WMS
{
    partial class Service1 : ServiceBase
    {
        private RoboService RoboService;

        public Service1()
        {
            InitializeComponent();
            this.RoboService = new RoboService();
        }

        protected override void OnStart(string[] args)
        {
            this.RoboService.Run();
        }

        public void StartDebug() => this.OnStart((string[])null);

        protected override void OnStop()
        {
            
        }
    }
}
