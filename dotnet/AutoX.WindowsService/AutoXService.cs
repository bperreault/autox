using System;
using System.IO;
using System.ServiceProcess;
using AutoX.Basic;

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
            var prefixString = Configuration.Settings("Prefixes",
                @"http://localhost:8081/AutoX.Web/;http://127.0.0.1:8081/AutoX.Web/;http://*:8081/AutoX.Web/");
            var prefixes = prefixString.Split(';');
            var path = AppDomain.CurrentDomain.BaseDirectory;
            _controller = new HttpListenerController(prefixes,
                Configuration.Settings("VirtualPath", "/AutoX.Web"),
                path+Configuration.Settings("PhysicalPath", ""));
            _controller.Start();

        }

        protected override void OnStop()
        {
            _controller.Stop();
        }
    }
}
