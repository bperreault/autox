// Hapa Project, CC
// Created @2012 08 29 08:33
// Last Updated  by Huang, Jien @2012 08 29 08:33

#region

using AutoX.Basic.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

#endregion

namespace AutoX.WF.Core
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
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
                        foreach (string nameOfComputer in _computerList.Keys)
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
            
            var xAction = xElement.Attribute("Action");
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

        public ClientInstance GetComputer(string nameOfComputer)
        {
            return
                (from name in _computerList.Keys where name.Contains(nameOfComputer) select _computerList[name]).
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

    public class ClientInstance : IDataObject
    {
        private readonly ArrayList _commandList = new ArrayList();
        private readonly XElement _element;

        public ClientInstance(XElement info)
        {
            _element = info;
            _element.Name = "ClientInstance";
            var xAttribute = info.Attribute("ComputerName");
            if (xAttribute != null) Name = xAttribute.Value;
            xAttribute = info.Attribute("IPAddress");
            if (xAttribute != null) IPAddress = xAttribute.Value;
            xAttribute = info.Attribute("_id");
            if (xAttribute != null) _id = xAttribute.Value;
            xAttribute = info.Attribute("Version");
            if (xAttribute != null) Version = xAttribute.Value;
            Created = DateTime.Now;
            Updated = DateTime.Now;
            _element.SetAttributeValue("Created",Created.ToString());
            _element.SetAttributeValue("Updated", Updated.ToString());
        }
//TODO add some other properties about sauce, browser, etc
        public string Name { set; get; }
        public string IPAddress { set; get; } //computer's role
        public string Version { get; set; }
        public DateTime Updated { set; get; }
        public string _id { get; set; }
        public DateTime Created { get; set; }

        public XElement Element()
        {
            return _element;
        }

        public override string ToString()
        {
            return _element.ToString(SaveOptions.None);
        }

        public void SetCommand(string command)
        {
            lock (_commandList)
            {
                if (string.IsNullOrEmpty(command)) return;
                //Monitor.Wait(CommandList);                  
                _commandList.Add(command);
                Updated = DateTime.Now;
                _element.SetAttributeValue("Updated",Updated.ToString());
                Monitor.Pulse(_commandList);
            }
        }

        public XElement GetCommand()
        {
            //Register last access time, for further clear use
            string retCommand;
            lock (_commandList)
            {
                //Monitor.Wait(CommandList);
                if (_commandList.Count == 0)
                {
                    //no command for this computer now, send back a Wait
                    retCommand = @"<Steps> <Step Data='17' Action='Wait' /> </Steps>";
                }
                else
                {
                    retCommand = _commandList[0].ToString();
                    _commandList.RemoveAt(0);
                }
                Updated = DateTime.Now;
                _element.SetAttributeValue("Updated", Updated.ToString());
                Monitor.Pulse(_commandList);
            }
            return XElement.Parse( retCommand);
        }

        
    }
}