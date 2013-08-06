using System;
using System.Net;
using System.Web;
using AutoX.Basic;

namespace AutoX.WindowsService
{
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
            Log.Debug("Virtual Path:"+_virtualDir+" Physical Path:"+_physicalDir);
            HttpRuntime.ProcessRequest(workerRequest);
        }
    }
}
