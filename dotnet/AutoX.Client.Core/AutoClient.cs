#region

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Comm;

#endregion

namespace AutoX.Client.Core
{
    public class AutoClient : IDisposable
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Browser _browser;

        private volatile bool _registered;
        private Task _task;
        private bool _disposed; // to detect redundant calls

        public AutoClient(Config config)
        {
            Config = config;
        }

        public AutoClient()
        {
            Config = Configuration.Clone();
        }

        public Config Config { get; private set; }

        public Browser Browser
        {
            get { return _browser ?? (_browser = new Browser(Config)); }
            set { _browser = value; }
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SupressFinalize(this);
        }

        public XElement Execute(XElement steps)
        {
            return ActionsFactory.Execute(steps, Browser, Config);
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(DoWhile, _tokenSource.Token);
        }

        private void DoWhile()
        {
            while (true)
            {
                if (!_registered)
                    _registered = Register();
                if (!_registered)
                {
                    Thread.Sleep(17*1000);
                    continue;
                }
                var command = RequestCommand();
                if (command.Attribute(Constants.INSTANCE_ID) != null)
                {
                    Log.Debug(command.ToString());
                    SendResult(command);
                }
                else
                {
                    Thread.Sleep(6*1000);
                }
            }
        }

        public void Stop()
        {
            if (_task == null)
                return;
            _tokenSource.Cancel();
        }

        public XElement RequestCommand()
        {
            return RequestCommand(Config.Get(Constants._ID));
        }

        public bool Register()
        {
            Communication.GetInstance().Register(Config);
            //if there is a return, it will always be success
            return true;
        }

        public XElement RequestCommand(string clientId)
        {
            var ret = Communication.GetInstance().RequestCommand(clientId);

            var steps = XElement.Parse(ret);
            var result = Execute(steps);
            return result;
        }

        public string SendResult(XElement stepResult)
        {
            return Communication.GetInstance().SetResult(Config.Get(Constants._ID), stepResult);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_tokenSource != null)
                    {
                        _tokenSource.Dispose();
                    }
                    if (_task != null)
                    {
                        _task.Dispose();
                    }
                }

                _disposed = true;
            }
        }
    }
}