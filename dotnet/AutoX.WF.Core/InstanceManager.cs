#region

using System.Collections.Generic;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

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
            var _1StNode = ((XElement) instanceInfo.FirstNode);
            //var name = instanceInfo.GetAttributeValue("TestName");
            var scriptGuid = _1StNode.GetAttributeValue("ScriptGUID");
            var defaultURL = _1StNode.GetAttributeValue("DefaultURL");
            //var clientId = _1StNode.GetAttributeValue("ClientId");
            var guid = _1StNode.GetAttributeValue(Constants._ID);
            //var status = instanceInfo.GetAttributeValue("Status");
            //var language = instanceInfo.GetAttributeValue("Language");
            //var suiteName = instanceInfo.GetAttributeValue("SuiteName");
            if (_instanceList.ContainsKey(guid))
            {
                var instance = _instanceList[guid];
                instance.Variables = ((XElement) instanceInfo.FirstNode).GetAttributeList();
                return instance.Status == null || !instance.Status.Equals("Invalid");
            }
            else
            {
                var instance = new WorkflowInstance(guid, scriptGuid,
                    ((XElement) instanceInfo.FirstNode).GetAttributeList());
                //new WorkflowInstance(guid, scriptGuid, name, computer, suiteName, language);
                _instanceList.Add(guid, instance);
                return instance.Status == null || !instance.Status.Equals("Invalid");
            }
        }

        public WorkflowInstance GetTestInstance(string guid)
        {
            return _instanceList.ContainsKey(guid) ? _instanceList[guid] : null;
        }

        public XElement GetInstances()
        {
            var list = new XElement("Instances");
            foreach (var ti in _instanceList.Values)
            {
                list.Add(ti.ToXElement());
            }
            return list;
        }

        public void RemoveTestInstance(string guid)
        {
            if (!_instanceList.ContainsKey(guid)) return;
            _instanceList[guid].Stop();
            _instanceList.Remove(guid);
        }

        internal XElement SetResult(XElement action)
        {
            var guid = ((XElement) action.FirstNode).GetAttributeValue(Constants.INSTANCE_ID);
            if (guid == null)
                return XElement.Parse("<Result Result='Success' />");
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            _instanceList[guid].SetResult(((XElement) action.FirstNode));
            return XElement.Parse("<Result Result='Success' />");
        }

        internal XElement StartInstance(XElement action)
        {
            var guid = action.GetAttributeValue(Constants._ID);
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            return _instanceList[guid].Start();
        }

        internal XElement StopInstance(XElement action)
        {
            var guid = action.GetAttributeValue(Constants._ID);
            if (!_instanceList.ContainsKey(guid)) return XElement.Parse("<Result Result='Error' />");
            _instanceList[guid].Stop();
            return XElement.Parse("<Result Result='Success' />");
        }

        internal XElement DeleteInstance(XElement action)
        {
            var guid = action.GetAttributeValue(Constants._ID);
            RemoveTestInstance(guid);
            return XElement.Parse("<Result Result='Success' />");
        }
    }
}