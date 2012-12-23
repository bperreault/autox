#region hapa

// // Hapa Project, CC
// // Created @2012 08 03 3:35 PM
// // Last Updated  by Huang, Jien @2012 08 06 12:19 AM

#endregion

#region

using System.Collections.Generic;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Database
{
    public class InstanceManager
    {
        private static InstanceManager _instance;
        private readonly Dictionary<string, TestInstance> _instanceList = new Dictionary<string, TestInstance>();


        private InstanceManager()
        {
        }

        public static InstanceManager GetInstance()
        {
            return _instance ?? (_instance = new InstanceManager());
        }

        public bool UpdateInstance(XElement instanceInfo)
        {
            var name = instanceInfo.GetAttributeValue("TestName");
            var scriptGuid = instanceInfo.GetAttributeValue("ScriptGUID");
            var computer = instanceInfo.GetAttributeValue("ClientName");
            var guid = instanceInfo.GetAttributeValue("GUID");
            var status = instanceInfo.GetAttributeValue("Status");
            var language = instanceInfo.GetAttributeValue("Language");
            var suiteName = instanceInfo.GetAttributeValue("SuiteName");
            if (_instanceList.ContainsKey(guid))
            {
                var instance = _instanceList[guid];
                instance.ClientName = computer;
                instance.Status = status;
                instance.TestName = name;
                return !instance.Status.Equals("Invalid");
            }
            else
            {
                var instance = new TestInstance(guid, scriptGuid, name, computer, suiteName, language);
                _instanceList.Add(guid, instance);
                return !instance.Status.Equals("Invalid");
            }
        }

        public TestInstance GetTestInstance(string guid)
        {
            return _instanceList.ContainsKey(guid) ? _instanceList[guid] : null;
        }

        public string GetInstances()
        {
            var list = new XElement("Instances");
            foreach (var ti in _instanceList.Values)
            {
                list.Add(ti.GetXElementFromObject());
            }
            return list.ToString();
        }

        public void RemoveTestInstance(string guid)
        {
            if (!_instanceList.ContainsKey(guid)) return;
            _instanceList[guid].Remove();
            _instanceList.Remove(guid);
        }
    }
}