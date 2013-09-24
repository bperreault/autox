#region

using System.Collections.Generic;
using System.Xml.Linq;
using AutoX.Basic;
using System.Threading.Tasks;
using System;
using System.Threading;

#endregion

namespace AutoX.WF.Core
{
    public class InstanceManager
    {
        private static InstanceManager _instance;
        private readonly Dictionary<string, WorkflowInstance> _instanceList = new Dictionary<string, WorkflowInstance>();
        private readonly List<WorkflowInstance> _waitingList = new List<WorkflowInstance>();
        private readonly Task _task;

        private InstanceManager()
        {
            
            _task = new Task(() =>
            {
                while (true)
                {
                    lock(_waitingList){
                        int toRemove = -1;
                    foreach (var instance in _waitingList)
                    {
                        if(string.IsNullOrEmpty(instance.ClientId)){
                            var clientId = ClientInstancesManager.GetInstance().GetAReadyClientInstance();
                            if(string.IsNullOrEmpty(clientId))
                                continue;
                            else
                                instance.ClientId = clientId;
                        }
                        var clientStatus = ClientInstancesManager.GetInstance().GetComputer(instance.ClientId).Status;
                        if (clientStatus.Equals("Running"))
                            continue;
                        else{
                            toRemove = _waitingList.IndexOf(instance);
                            break;
                        }
                    }
                        if(toRemove>-1){
                            _waitingList[toRemove].RealStart();
                            _waitingList.RemoveAt(toRemove);
                        }
                    }
                    Thread.Sleep(500);
                }
            });
            _task.Start();
        }

        public static InstanceManager GetInstance()
        {
            return _instance ?? (_instance = new InstanceManager());
        }

        public XElement UpdateInstance(XElement instanceInfo)
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
            
            WorkflowInstance instance;
            if (_instanceList.ContainsKey(guid))
            {
                //instance already existed
                instance = _instanceList[guid];                        
            }
            else
            {
                //instance not existed, create one
                instance = new WorkflowInstance(guid, scriptGuid,
                    ((XElement) instanceInfo.FirstNode).GetAttributeList());
                //new WorkflowInstance(guid, scriptGuid, name, computer, suiteName, language);
                _instanceList.Add(guid, instance);               
            }
            
            if(instance==null)
                return XElement.Parse("<Result Result='Error' Reason='Instance is null' />");

            instance.Variables = ((XElement)instanceInfo.FirstNode).GetAttributeList();
            var status = instance.Status;
            var instanceId = instance._id;
            if(string.IsNullOrEmpty(status) || !status.Equals("Invalid"))
                return XElement.Parse("<Result Result='Success' InstanceId ='"+instanceId+"' />");
            else
                return XElement.Parse("<Result Result='Error' Reason='" + status + "' InstanceId ='" + instanceId + "' />");
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
    
        internal void AddToWaitingList(WorkflowInstance workflowInstance)
        {
 	        lock(_waitingList){
                _waitingList.Add(workflowInstance);
                Monitor.Pulse(_waitingList);
            }
        }

    }
}