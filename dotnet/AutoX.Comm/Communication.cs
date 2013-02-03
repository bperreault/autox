// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.Comm.AutoXService;
using System;
using System.Net;
using System.ServiceModel;
using System.Xml.Linq;

#endregion

namespace AutoX.Comm
{
    public sealed class Communication : IDisposable
    {
        private static Communication _instance;
        private static readonly ServiceSoapClient Client = new ServiceSoapClient();

        public Communication()
        {
            _instance = this;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion

        #region IHost Members

        public IDataObject GetDataObject(string id)
        {
            var ret = GetInstance().GetById(id);
            var obj = XElement.Parse(ret).GetObjectFromXElement();
            return (IDataObject)obj;
        }

        public void SetCommand(string instanceId, XElement steps)
        {
            throw new Exception("should not called here, at least no plan for this version");
        }

        public XElement GetResult(string instanceId, string guid)
        {
            throw new Exception("should not called here, at least no plan for this version");
        }

        #endregion

        public static Communication GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Communication();
            }
            if (Client.State != CommunicationState.Opened)
                Client.Open();
            return _instance;
        }

        public void SetAddress(string address)
        {
            Client.Close();
            Client.Endpoint.Address = new EndpointAddress((address));
            Client.Open();
        }

        public string Command(string xmlFormatCommand)
        {
            var ret = Client.Command(xmlFormatCommand);

            //Client.Command(xmlFormatCommand);
            return ret;
        }

        private static XElement GetCommandXElement(string name)
        {
            var command = new XElement("Command");
            command.SetAttributeValue(Constants.ACTION, name);
            return command;
        }

        public string Register(Config config)
        {
            var xCommand = GetCommandXElement("Register");
            var computer = Computer.GetLocalHost();
            xCommand.SetAttributeValue(Constants._ID, config.Get(Constants._ID));
            xCommand.SetAttributeValue("ComputerName", computer.ComputerName);
            xCommand.SetAttributeValue("IPAddress", computer.IPAddress);
            xCommand.SetAttributeValue("Version", computer.Version);
            foreach (var key in config.GetList().Keys)
            {
                xCommand.SetAttributeValue(key, config.GetList()[key]);
            }
            return Command(xCommand.ToString());
        }

        public string RequestCommand(string clientId)
        {
            var xCommand = GetCommandXElement("RequestCommand");
            var computerName = Dns.GetHostName();
            xCommand.SetAttributeValue("ComputerName", computerName);
            xCommand.SetAttributeValue(Constants._ID, clientId);
            return Command(xCommand.ToString());
        }

        public string SetResult(string clientId, XElement result)
        {
            var xCommand = GetCommandXElement("SetResult");
            xCommand.SetAttributeValue("ClientId", clientId);
            xCommand.Add(result);
            return Command(xCommand.ToString());
        }

        public string GetComputersInfo()
        {
            var xCommand = GetCommandXElement("GetComputersInfo");
            return Command(xCommand.ToString());
        }

        public string GetInstancesInfo()
        {
            var xCommand = GetCommandXElement("GetInstancesInfo");
            return Command(xCommand.ToString());
        }

        //public string GetProjectsRoot()
        //{
        //    XElement xCommand = GetCommandXElement("GetProjectsRoot");
        //    return Command(xCommand.ToString());
        //}

        public string SetInstanceInfo(XElement instance)
        {
            var xCommand = GetCommandXElement("SetInstanceInfo");
            xCommand.Add(instance);
            return Command(xCommand.ToString());
        }

        public string StartInstance(string guid)
        {
            var xCommand = GetCommandXElement("StartInstance");
            xCommand.SetAttributeValue(Constants._ID, guid);
            return Command(xCommand.ToString());
        }

        //public string GetInstanceLog(string _id)
        //{
        //    XElement xCommand = GetCommandXElement("GetInstanceLog");
        //    xCommand.SetAttributeValue(Constants._ID, _id);
        //    return Command(xCommand.ToString());
        //}

        public string StopInstance(string guid)
        {
            var xCommand = GetCommandXElement("StopInstance");
            xCommand.SetAttributeValue(Constants._ID, guid);
            return Command(xCommand.ToString());
        }

        public string DeleteInstance(string guid)
        {
            var xCommand = GetCommandXElement("DeleteInstance");
            xCommand.SetAttributeValue(Constants._ID, guid);
            return Command(xCommand.ToString());
        }

        public string GetById(string guid)
        {
            var xCommand = GetCommandXElement("GetById");
            xCommand.SetAttributeValue(Constants._ID, guid);
            return Command(xCommand.ToString());
        }

        public string SetById(XElement content)
        {
            var xCommand = GetCommandXElement("SetById");
            xCommand.Add(content);
            return Command(xCommand.ToString());
        }

        public string GetChildren(string guid)
        {
            var xCommand = GetCommandXElement("GetChildren");
            xCommand.SetAttributeValue(Constants._ID, guid);
            return Command(xCommand.ToString());
        }

        //public string GetLogRoot()
        //{
        //    XElement xCommand = GetCommandXElement("GetLogRoot");
        //    return Command(xCommand.ToString());
        //}

        public void Close()
        {
            if (Client != null)
                Client.Close();
        }
    }
}