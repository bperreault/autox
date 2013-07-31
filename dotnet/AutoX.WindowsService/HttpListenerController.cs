#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
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
        private bool _listening = false;
        private string _virtualDir;
        private string _physicalDir;
        private string[] _prefixes;
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
            _pump = new Thread(new ThreadStart(Pump));
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
                EventLog myLog = new EventLog();
                myLog.Source = "HttpListenerController";
                if (null != ex.InnerException)
                    myLog.WriteEntry(ex.InnerException.ToString(), EventLogEntryType.Error);
                else
                    myLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
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

            foreach (string prefix in prefixes)
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
            HttpListenerContext ctx = _listener.GetContext();
            HttpListenerWorkerRequest workerRequest =
                new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
            HttpRuntime.ProcessRequest(workerRequest);
        }
    }


}