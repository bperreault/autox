using AutoX.Basic.Model;
using AutoX.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoX.WF.Core
{
    public class InstanceManager
    {
        private static InstanceManager _instance;
        private readonly Dictionary<string, WorkflowInstance> _instanceList = new Dictionary<string, WorkflowInstance>();

        private InstanceManager()
        {
        }

        public static InstanceManager GetInstance()
        {
            return _instance ?? (_instance = new InstanceManager());
        }

        public bool UpdateInstance(XElement instanceInfo)
        {
            //var name = instanceInfo.GetAttributeValue("TestName");
            //var scriptGuid = instanceInfo.GetAttributeValue("ScriptGUID");
            //var computer = instanceInfo.GetAttributeValue("ClientName");
            var guid = ((XElement)instanceInfo.FirstNode).GetAttributeValue(Constants._ID);
            //var status = instanceInfo.GetAttributeValue("Status");
            //var language = instanceInfo.GetAttributeValue("Language");
            //var suiteName = instanceInfo.GetAttributeValue("SuiteName");
            if (_instanceList.ContainsKey(guid))
            {
                var instance = _instanceList[guid];
                instance.Variables = ((XElement)instanceInfo.FirstNode).GetAttributeList();
                return instance.Status == null || !instance.Status.Equals("Invalid");
            }
            else
            {
                var instance = new WorkflowInstance(guid, ((XElement)instanceInfo.FirstNode).GetAttributeList());//new WorkflowInstance(guid, scriptGuid, name, computer, suiteName, language);
                _instanceList.Add(guid, instance);
                return instance.Status==null||!instance.Status.Equals("Invalid");
            }
        }

        public WorkflowInstance GetTestInstance(string guid)
        {
            return _instanceList.ContainsKey(guid) ? _instanceList[guid] : null;
        }

        public string GetInstances()
        {
            var list = new XElement("Instances");
            foreach (WorkflowInstance ti in _instanceList.Values)
            {
                list.Add(((IDataObject)ti).GetXElementFromObject());
            }
            return list.ToString();
        }

        public void RemoveTestInstance(string guid)
        {
            if (!_instanceList.ContainsKey(guid)) return;
            _instanceList[guid].Stop();
            _instanceList.Remove(guid);
        }

        internal XElement SetResult(XElement action)
        {
            var guid = ((XElement)action.FirstNode).GetAttributeValue(Constants._ID);
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            _instanceList[guid].SetResult(action);
            return XElement.Parse("<Result Result='Error' />");
        }

        internal XElement StartInstance(XElement action)
        {
            var guid = ((XElement)action.FirstNode).GetAttributeValue(Constants._ID);
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            return _instanceList[guid].Start();
        }

        internal XElement StopInstance(XElement action)
        {
            var guid = ((XElement)action.FirstNode).GetAttributeValue(Constants._ID);
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            _instanceList[guid].Stop();
            return XElement.Parse("<Result Result='Error' />");
        }
    }
}
