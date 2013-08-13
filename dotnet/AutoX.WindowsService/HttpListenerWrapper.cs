using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
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
            var lease = (ILease) RemotingServices.GetLifetimeService(this);
            Debug.Assert(lease.CurrentState==LeaseState.Active);
            lease.Renew(TimeSpan.FromMinutes(30));
            var ctx = _listener.GetContext();
            var workerRequest =
                new HttpListenerWorkerRequest(ctx, _virtualDir, _physicalDir);
            //Log.Debug("Virtual Path:"+_virtualDir+" Physical Path:"+_physicalDir);
            HttpRuntime.ProcessRequest(workerRequest);
        }
    }
}
