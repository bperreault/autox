using AutoX.Basic;
using AutoX.Basic.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoX.WF.Core
{
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
            xAttribute = info.Attribute(Constants._ID);
            if (xAttribute != null) _id = xAttribute.Value;
            xAttribute = info.Attribute("Version");
            if (xAttribute != null) Version = xAttribute.Value;
            Created = DateTime.Now;
            Updated = DateTime.Now;
            _element.SetAttributeValue("Created", Created.ToString());
            _element.SetAttributeValue("Updated", Updated.ToString());
        }

        public string Name { set; get; }

        public string IPAddress { set; get; } //computer's role

        public string Version { get; set; }

        public DateTime Updated { set; get; }

        public string _id { get; set; }

        public DateTime Created { get; set; }

        public string _parentId { get; set; }

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
                _element.SetAttributeValue("Updated", Updated.ToString());
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
            return XElement.Parse(retCommand);
        }
    }
}
