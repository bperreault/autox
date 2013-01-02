using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Comm;
using System.Threading;

namespace AutoX.Client.Core
{
    public class ClientInstance
    {
        private readonly string _clientId = Guid.NewGuid().ToString();
        public string Id { get { return _clientId; } }
        private volatile bool _registered;
        Task task = null;

        
        public void Start()
        {
            task = Task.Factory.StartNew(DoWhile);
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
                   Thread.Sleep(17*1000);
                    continue;
                }
                var command = RequestCommand();
                //TODO notify the observer
                var result = ActionsFactory.Execute(command);
                SendResult(result);
            }
            
        }

        public void Stop()
        {
            if (task == null)
                return;
            if (task.IsCompleted || task.IsCanceled)
                return;
            task.Dispose();
        }

        public XElement RequestCommand()
        {
            return RequestCommand(Id);
        }

        public bool Register()
        {
            var ret = Communication.GetInstance().Register(Id);
            Log.Debug(ret);
            
            Communication.GetInstance().SetResult(Id, ActionsFactory.Execute(XElement.Parse(ret)));
            //TODO it may be false, just for now
            return true;
        }

        public XElement RequestCommand(string clientId)
        {
            var ret = Communication.GetInstance().RequestCommand(clientId);
            Log.Debug(ret);

            var steps = XElement.Parse(ret);
            var result = ActionsFactory.Execute(steps);
            return result;
        }

        public string SendResult(XElement stepResult)
        {
            return Communication.GetInstance().SetResult(Id,stepResult);
        }

        
    }
}
