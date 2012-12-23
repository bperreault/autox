// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Net;
using System.ServiceModel;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.Comm.AutoXService;

#endregion

namespace AutoX.Comm
{
    public sealed class Communication : IDisposable, IHost
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
            string ret = GetInstance().GetById(id);
            object obj = XElement.Parse(ret).GetObjectFromXElement();
            return (IDataObject) obj;
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
            string ret = Client.Command(xmlFormatCommand);
            //Client.Command(xmlFormatCommand);
            return ret;
        }

        private static XElement GetCommandXElement(string name)
        {
            var command = new XElement("Command");
            command.SetAttributeValue("Action", name);
            return command;
        }

        public string Register()
        {
            XElement xCommand = GetCommandXElement("Register");
            Computer computer = Computer.GetLocalHost();

            xCommand.SetAttributeValue("ComputerName", computer.ComputerName);
            xCommand.SetAttributeValue("IPAddress", computer.IPAddress);
            xCommand.SetAttributeValue("Version", computer.Version);

            return Command(xCommand.ToString());
        }


        public string RequestCommand()
        {
            XElement xCommand = GetCommandXElement("RequestCommand");
            string computerName = Dns.GetHostName();
            xCommand.SetAttributeValue("ComputerName", computerName);
            return Command(xCommand.ToString());
        }

        public string SetResult(XElement result)
        {
            XElement xCommand = GetCommandXElement("SetResult");
            xCommand.Add(result);
            return Command(xCommand.ToString());
        }

        public string GetComputersInfo()
        {
            XElement xCommand = GetCommandXElement("GetComputersInfo");
            return Command(xCommand.ToString());
        }

        public string GetInstancesInfo()
        {
            XElement xCommand = GetCommandXElement("GetInstancesInfo");
            return Command(xCommand.ToString());
        }

        //public string GetProjectsRoot()
        //{
        //    XElement xCommand = GetCommandXElement("GetProjectsRoot");
        //    return Command(xCommand.ToString());
        //}

        public string SetInstanceInfo(XElement instance)
        {
            XElement xCommand = GetCommandXElement("SetInstanceInfo");
            xCommand.Add(instance);
            return Command(xCommand.ToString());
        }

        public string StartInstance(string guid)
        {
            XElement xCommand = GetCommandXElement("StartInstance");
            xCommand.SetAttributeValue("GUID", guid);
            return Command(xCommand.ToString());
        }

        //public string GetInstanceLog(string GUID)
        //{
        //    XElement xCommand = GetCommandXElement("GetInstanceLog");
        //    xCommand.SetAttributeValue("GUID", GUID);
        //    return Command(xCommand.ToString());
        //}

        public string StopInstance(string guid)
        {
            XElement xCommand = GetCommandXElement("StopInstance");
            xCommand.SetAttributeValue("GUID", guid);
            return Command(xCommand.ToString());
        }

        public string DeleteInstance(string guid)
        {
            XElement xCommand = GetCommandXElement("DeleteInstance");
            xCommand.SetAttributeValue("GUID", guid);
            return Command(xCommand.ToString());
        }

        public string GetById(string guid)
        {
            XElement xCommand = GetCommandXElement("GetById");
            xCommand.SetAttributeValue("GUID", guid);
            return Command(xCommand.ToString());
        }

        public string SetById(XElement content)
        {
            XElement xCommand = GetCommandXElement("SetById");
            xCommand.Add(content);
            return Command(xCommand.ToString());
        }

        public string GetChildren(string guid)
        {
            XElement xCommand = GetCommandXElement("GetChildren");
            xCommand.SetAttributeValue("GUID", guid);
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