using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Client.Core;

namespace AutoX.Comm
{
    public class ClientInstance
    {
        
        private volatile bool _registered;
        private readonly Config _config = Configuration.Clone();
        public string _id { get { return _config.Get("_id"); } }
        public Config Config
        {
            get
            {
                
                return _config;
            }
        }

        Task _task;
        readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        
        public void Start()
        {
            _task = Task.Factory.StartNew(DoWhile,_tokenSource.Token);
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
                if(command.Attribute("_id")!=null)
                    SendResult(result);
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
            var result = ActionsFactory.Execute(steps);
            return result;
        }

        public string SendResult(XElement stepResult)
        {
            return Communication.GetInstance().SetResult(Config.Get("_id"),stepResult);
        }

        
    }
}
