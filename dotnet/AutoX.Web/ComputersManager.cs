// Hapa Project, CC
// Created @2012 08 29 08:33
// Last Updated  by Huang, Jien @2012 08 29 08:33

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

#endregion

namespace AutoX.Web
{
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ComputersManager : IDisposable
    {
        private static ComputersManager _instance;
        private readonly Dictionary<string, Computer> _computerList = new Dictionary<string, Computer>();
        private readonly Task _task;

        private ComputersManager()
        {
            // check the computers every 5 minutes, if it is not accessed for 1 hour, then remove it. we guess it is dead
            var cancell = new CancellationTokenSource();
            CancellationToken token = cancell.Token;
            _task = new Task(() =>
                                 {
                                     while (true)
                                     {
                                         bool cancelled = token.WaitHandle.WaitOne(5*60*1000);
                                         DateTime now = DateTime.Now;
                                         foreach (string nameOfComputer in _computerList.Keys)
                                         {
                                             Computer computer = _computerList[nameOfComputer];
                                             TimeSpan lostMessage = now - computer.LastAccess;
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

        public void Register(string xComputerInfo)
        {
            XElement xElement = XElement.Parse(xComputerInfo);
            XAttribute xAction = xElement.Attribute("Action");
            if (xAction != null) xAction.Remove();
            var computer = new Computer(xElement);

            if (_computerList.ContainsKey(computer.Name))
            {
                _computerList.Remove(computer.Name);
            }
            _computerList.Add(computer.Name, computer);
        }

        public static ComputersManager GetInstance()
        {
            return _instance ?? (_instance = new ComputersManager());
        }

        public Computer GetComputer(string nameOfComputer)
        {
            return
                (from name in _computerList.Keys where name.Contains(nameOfComputer) select _computerList[name]).
                    FirstOrDefault();
        }

        public override string ToString()
        {
            var cl = new XElement("ComputerList");
            cl.SetAttributeValue("Number", _computerList.Count);
            foreach (Computer computer in _computerList.Values)
            {
                cl.Add(computer.Element());
            }
            return cl.ToString(SaveOptions.None);
        }
    }

    public class Computer
    {
        private readonly ArrayList _commandList = new ArrayList();
        private readonly XElement _element;

        public Computer(XElement info)
        {
            _element = info;
            _element.Name = "Computer";
            XAttribute xAttribute = info.Attribute("ComputerName");
            if (xAttribute != null) Name = xAttribute.Value;
            xAttribute = info.Attribute("IPAddress");
            if (xAttribute != null) IPAddress = xAttribute.Value;
            xAttribute = info.Attribute("Version");
            if (xAttribute != null) Version = xAttribute.Value;
            xAttribute = info.Attribute("Name");
            if (xAttribute != null) xAttribute.Remove();

            LastAccess = DateTime.Now;
        }

        public string Name { set; get; }
        public string IPAddress { set; get; } //computer's role
        public string Version { get; set; }
        public DateTime LastAccess { set; get; }


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
                Monitor.Pulse(_commandList);
            }
        }

        public string GetCommand()
        {
            //Register last access time, for further clear use
            string retCommand;
            lock (_commandList)
            {
                //Monitor.Wait(CommandList);
                if (_commandList.Count == 0)
                {
                    //no command for this computer now, send back a Wait
                    retCommand = @"<Steps> <Step Data='17' Action='AutoX.Client.Wait' /> </Steps>";
                }
                else
                {
                    retCommand = _commandList[0].ToString();
                    _commandList.RemoveAt(0);
                }
                LastAccess = DateTime.Now;
                Monitor.Pulse(_commandList);
            }
            return retCommand;
        }
    }
}