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
                Log.Debug("After Configure -> Virtual Path:" + _virtualDir + " Physical Path:" + _physicalDir);
                _listener.Start();
                Log.Debug("Listener Started ...");
                while (_listening)
                    _listener.ProcessRequest();
            }
            catch (Exception ex)
            {
                Log.Debug(null != ex.InnerException ? ex.InnerException.ToString() : ex.ToString());
            }
        }
    }

    

}