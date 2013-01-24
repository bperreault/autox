using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoX.Basic;
using System.Xml.Linq;
using AutoX.Comm;

namespace AutoX.Client.Core
{
    public class AutoClient
    {
        public Config Config { get; private set; }
        private Browser _browser;

        public Browser Browser
        {
            get { return _browser ?? (_browser = new Browser(Config)); }
        }

        private volatile bool _registered;
        Task _task;
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();


        public AutoClient(Config config)
        {
            Config = config;
        }
        public AutoClient()
        {
            Config = Configuration.Clone();
        }

        public XElement Execute(XElement steps)
        {
            
            return ActionsFactory.Execute(steps, Browser, Config);
        }

        public void Start()
        {
            _task = Task.Factory.StartNew(DoWhile, _tokenSource.Token);
            //task.Start();
        }

        private void DoWhile()
        {
            while (true)
            {
                if (!_registered)
                    _registered = Register();
                if (!_registered)
                {
                    Thread.Sleep(17 * 1000);
                    continue;
                }
                var command = RequestCommand();

                //TODO notify the observer
                var result = Execute(command);
                if (command.Attribute("_id") != null)
                    SendResult(result);
                else
                {
                    Thread.Sleep(6 * 1000);
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
            return RequestCommand(Config.Get("_id"));
        }

        public bool Register()
        {
            var ret = Communication.GetInstance().Register(Config);
            Log.Debug(ret);

            //Communication.GetInstance().SetResult(Id, ActionsFactory.Execute(XElement.Parse(ret)));
            //TODO it may be false, just for now
            return true;
        }

        public XElement RequestCommand(string clientId)
        {
            var ret = Communication.GetInstance().RequestCommand(clientId);
            Log.Debug(ret);

            var steps = XElement.Parse(ret);
            var result = Execute(steps);
            return result;
        }

        public string SendResult(XElement stepResult)
        {
            return Communication.GetInstance().SetResult(Config.Get("_id"), stepResult);
        }
    }
}
