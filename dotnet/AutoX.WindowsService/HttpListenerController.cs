#region Using directives
using System;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Threading;
using System.Diagnostics;
using AutoX.Basic;
#endregion

namespace AutoX.WindowsService
{
    public class HttpListenerController
    {
        private Thread _pump;
        private bool _listening;
        private readonly string _virtualDir;
        private readonly string _physicalDir;
        private readonly string[] _prefixes;
        private HttpListenerWrapper _listener;

        public HttpListenerController(string[] prefixes, string vdir, string pdir)
        {
            _prefixes = prefixes;
            _virtualDir = vdir;
            _physicalDir = pdir;
        }

        public void Start()
        {
            _listening = true;
            _pump = new Thread(Pump);
            _pump.Start();
        }

        public void Stop()
        {
            _listening = false;

            Process.GetCurrentProcess().Kill();
            _pump.Abort();
            _pump.Join();
        }

        private void Pump()
        {
            try
            {
                Log.Debug("Before CreateApplicationHost");
                
                _listener = (HttpListenerWrapper)ApplicationHost.CreateApplicationHost(
                    typeof(HttpListenerWrapper), _virtualDir, _physicalDir);
                Log.Debug("Create CreateApplicationHost OK");
                _listener.Configure(_prefixes, _virtualDir, _physicalDir);
                _listener.Start();

                while (_listening)
                    _listener.ProcessRequest();
            }
            catch (Exception ex)
            {
                var myLog = new EventLog {Source = "HttpListenerController"};
                myLog.WriteEntry(null != ex.InnerException ? ex.InnerException.ToString() : ex.ToString(),
                    EventLogEntryType.Error);
            }
        }
    }

    public class HttpListenerWrapper : MarshalByRefObject
    {
        private HttpListener _listener;
        private string _virtualDir;
        private string _physicalDir;

        public void Configure(string[] prefixes, string vdir, string pdir)
        {
            _virtualDir = vdir;
            _physicalDir = pdir;
            _listener = new HttpListener();

            foreach (var prefix in prefixes)
                _listener.Prefixes.Add(prefix);
        }
        public void Start()
        {
            _listener.Start();
        }
        public void Stop()
        {
            _listener.Stop();
        }
        public void ProcessRequest()
        {
            var ctx = _listener.GetContext();
            var workerRequest =
                new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
            HttpRuntime.ProcessRequest(workerRequest);
        }
    }


}