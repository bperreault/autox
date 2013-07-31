using System.ServiceProcess;
using AutoX.Basic;
using System.Net;
using System.Threading;
using System.IO;
using System;

namespace AutoX.WindowsService
{
    public partial class AutoXService : ServiceBase
    {
        private HttpListenerController _controller;

        public AutoXService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string[] prefixes = new string[] {
                "http://localhost:8081/AutoX.Web/",
                "http://127.0.0.1:8081/AutoX.Web/",
            };
            _controller = new HttpListenerController(prefixes, "/AutoX.Web", @"C:\inetpub\wwwroot\AutoX.Web");
            _controller.Start();

        }

        protected override void OnStop()
        {
            _controller.Stop();
        }
    }
}
