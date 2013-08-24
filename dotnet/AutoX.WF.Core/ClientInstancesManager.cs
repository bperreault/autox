#region

// Hapa Project, CC
// Created @2012 08 29 08:33
// Last Updated  by Huang, Jien @2012 08 29 08:33

#region

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.WF.Core
{
    public class ClientInstancesManager : IDisposable
    {
        private static ClientInstancesManager _instance;
        private readonly Dictionary<string, ClientInstance> _computerList = new Dictionary<string, ClientInstance>();
        private readonly Task _task;

        private ClientInstancesManager()
        {
            // check the computers every 5 minutes, if it is not accessed for 1 hour, then remove it. we guess it is dead
            var cancell = new CancellationTokenSource();
            var token = cancell.Token;
            _task = new Task(() =>
            {
                while (true)
                {
                    var cancelled = token.WaitHandle.WaitOne(5*60*1000);
                    var now = DateTime.Now;
                    foreach (var nameOfComputer in _computerList.Keys)
                    {
                        var computer = _computerList[nameOfComputer];
                        var lostMessage = now - computer.Updated;
                        if (lostMessage.Hours >= 1)
                            _computerList.Remove(nameOfComputer);
                    }
                    if (cancelled)
                    {
                        throw new OperationCanceledException(token);
                    }
                }
            }, token);
            _task.Start();
        }

        #region IDisposable Members

        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (_task != null)
            {
                _task.Dispose();
            }
        }

        #endregion

        public void Register(XElement xElement)
        {
            var xAction = xElement.Attribute(Constants.ACTION);
            if (xAction != null) xAction.Remove();
            var computer = new ClientInstance(xElement);

            if (_computerList.ContainsKey(computer._id))
            {
                _computerList.Remove(computer._id);
            }
            _computerList.Add(computer._id, computer);
        }

        public static ClientInstancesManager GetInstance()
        {
            return _instance ?? (_instance = new ClientInstancesManager());
        }

        public string GetAReadyClientInstance()
        {
            return (from instance in _computerList where instance.Value.Status.Equals("Ready") select instance.Key).FirstOrDefault();
        }

        public ClientInstance GetComputer(string idOfComputer)
        {
            return
                (from id in _computerList.Keys where id.Contains(idOfComputer) select _computerList[id]).
                    FirstOrDefault();
        }

        public override string ToString()
        {
            var cl = ToXElement();
            return cl.ToString(SaveOptions.None);
        }

        public XElement ToXElement()
        {
            var cl = new XElement("ComputerList");
            cl.SetAttributeValue("Number", _computerList.Count);
            foreach (ClientInstance computer in _computerList.Values)
            {
                cl.Add(computer.Element());
            }
            return cl;
        }
    }
}