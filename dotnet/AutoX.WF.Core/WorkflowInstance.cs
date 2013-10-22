#region

using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using AutoX.Activities.AutoActivities;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.DB;

#endregion

namespace AutoX.WF.Core
{
    public class WorkflowInstance : IHost, IDataObject
    {
        private const string FINISHED_STATUSES = "Completed|Aborted|Canceled|Faulted";
        private readonly StatusTracker _statusTracker = new StatusTracker();
        private readonly WorkflowApplication _workflowApplication;
        private volatile XElement _command;
        private string _instanceId;
        //private List<IObserver> _observers = new List<IObserver>();
        private volatile XElement _result;
        private Dictionary<string, string> _variables = new Dictionary<string, string>();

        public WorkflowInstance(string instanceId, string workflowId, Dictionary<string, string> upperLevelVariables)
        {
            InstanceId = instanceId;
            Status = "Invalid";
            ScriptGUID = workflowId;
            Variables = Configuration.Clone().GetList();
            if (upperLevelVariables != null)
                foreach (KeyValuePair<string, string> upperLevelVariable in upperLevelVariables)
                {
                    if (Variables.ContainsKey(upperLevelVariable.Key))
                        Variables[upperLevelVariable.Key] = upperLevelVariable.Value;
                    else
                        Variables.Add(upperLevelVariable.Key, upperLevelVariable.Value);
                }
            var rootId = Variables["Root"];
            ClientId = Variables.ContainsKey("ClientId") ? Variables["ClientId"] : null;
            var versionName = Variables.ContainsKey("AUTVersion") ? Variables["AUTVersion"] : "TestVersion";
            var buildName = Variables.ContainsKey("AUTBuild") ? Variables["AUTBuild"] : "TestBuild";
            if (!string.IsNullOrEmpty(rootId))
            {
                var xRoot = DBFactory.GetData().GetChildren(rootId);

                var resultId = xRoot.GetSubElement(Constants.NAME, Constants.RESULT).GetAttributeValue(Constants._ID);

                if (!string.IsNullOrEmpty(resultId))
                {
                    var versionId = FindOrCreateSubResultFolder(versionName, resultId);
                    var buildId = FindOrCreateSubResultFolder(buildName, versionId);
                    var script = GetDataObject(workflowId);
                    _result = null;
                    _command = null;
                    if (script != null)
                        _workflowApplication = CreateActivity(script.GetAttributeValue(Constants.CONTENT), buildId,
                            Variables);
                    if (_workflowApplication != null)
                    {
                        //very important! it make the workflow run in Synchronized way.
                        //_workflowApplication.SynchronizationContext = new SynchronizationContext();
                        
                        Status = "Ready";
                    }
                }
            }
        }

        public string ClientId { get; set; }

        public string TestName { get; set; }

        public string SuiteName { get; set; }

        public string Status { get; set; }

        public string ClientName { get; set; }

        public string Language { get; set; }

        public string ScriptGUID { get; set; }

        public string DefaultURL { get; set; }

        public Dictionary<string, string> Variables
        {
            get { return _variables; }
            set { _variables = value; }
        }

        public string InstanceId
        {
            get { return _instanceId; }
            set { _instanceId = value; }
        }

        public string ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public string _parentId { get; set; }

        public string _id
        {
            get { return _instanceId; }
            set { _instanceId = value; }
        }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public XElement GetDataObject(string id)
        {
            return DBFactory.GetData().Read(id);
        }

        public Config GetConfig()
        {
            var _config = Configuration.Clone();
            foreach (var variable in Variables)
            {
                _config.Set(variable.Key, variable.Value);
            }
            _config.Set("DefaultURL", DefaultURL);

            return _config;
        }

        public void SetCommand(XElement steps)
        {
            Log.Debug("Set Command:\n" + steps);
            steps.SetAttributeValue(Constants.INSTANCE_ID, InstanceId);
            if (!string.IsNullOrEmpty(ClientId))
            {
                ClientInstancesManager.GetInstance().GetComputer(ClientId).SetCommand(steps.ToString());
            }
            lock (this)
            {
                _command = steps;
            }
            
        }

        public XElement GetResult()
        {
            var count = 0;
            while (_result == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 3000)
                    return null;
            }
            var resultString = _result.ToString();
            lock (this)
            {
                _result = null;
            }
            
            return XElement.Parse(resultString);
        }

        public void Stop()
        {
            if (_workflowApplication != null)
            {
                ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                _workflowApplication.Cancel();
            }
        }

        public XElement ToXElement()
        {
            var xInstance = this.GetXElementFromObject();
            foreach (KeyValuePair<string, string> pair in Variables)
            {
                if (pair.Key.Contains(":")) continue;
                xInstance.SetAttributeValue(pair.Key, pair.Value);
            }
            xInstance.SetAttributeValue("Status", Status);
            return xInstance;
        }

        public static Instance FromXElement(XElement element)
        {
            return element.GetObjectFromXElement() as Instance;
        }

        //This method only be used to create build, version folder
        private static string FindOrCreateSubResultFolder(string versionName, string resultId)
        {
            var results = DBFactory.GetData().GetChildren(resultId);
            string versionId = null;
            if (results != null)
            {
                foreach (XElement result in results.Descendants())
                {
                    if (versionName.Equals(result.GetAttributeValue(Constants.NAME)))
                    {
                        versionId = result.GetAttributeValue(Constants._ID);
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(versionId))
            {
                versionId = Guid.NewGuid().ToString();
                DBFactory.GetData()
                    .Save(
                        XElement.Parse("<Result Name='" + versionName + "' _id='" + versionId + "' _parentId='" +
                                       resultId + "' _type='Folder' Created='" +
                                       DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT) + "' Updated='" +
                                       DateTime.UtcNow.ToString(Constants.DATE_TIME_FORMAT) + "' />"));
            }
            return versionId;
        }
        public string Source { get; set; }
        public StatusTracker Tracker {get { return _statusTracker; }}

        private WorkflowApplication CreateActivity(string workflow, string resultParentId,
            Dictionary<string, string> upperLevelVariables)
        {
            Source = workflow;
            var activity = ActivityXamlServices.Load(new StringReader(workflow)) as AutomationActivity;
            if (activity != null)
            {
                activity.SetHost(this);
                activity.InstanceId = InstanceId;
                activity.SetParentResultId(resultParentId);
                activity.SetVariables(upperLevelVariables);
                var workflowApplication = GetWorkflowApplication(activity);
                workflowApplication.Extensions.Add(_statusTracker);
                
                return workflowApplication;
                //_workflowApplication.Run();
            }
            return null;
        }

        public XElement Start()
        {
            if (_workflowApplication != null)
                if (!IsFinished())
                {
                    //while (String.IsNullOrEmpty(ClientId))
                    //{
                    //    Thread.Sleep(23);
                    //    ClientId = ClientInstancesManager.GetInstance().GetAReadyClientInstance();
                    //}
                    //while (true)
                    //{
                    //    var clientStatus = ClientInstancesManager.GetInstance().GetComputer(ClientId).Status;
                    //    if (!clientStatus.Equals("Running"))
                    //        break;
                    //    else
                    //        Thread.Sleep(17000);
                    //}
                    //RealStart();
                    InstanceManager.GetInstance().AddToWaitingList(this);
                    return XElement.Parse("<Result Result='Success' Reason='In the waiting List' />");
                }
            return XElement.Parse("<Result Result='Error' Reason='script error or it is already finished.' />");
        }

        public void RealStart()
        {
            ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Running";
            Variables["ClientName"] = ClientInstancesManager.GetInstance().GetComputer(ClientId).Name;
            Variables["ClientId"] = ClientId;
            Status = "Running";
            _workflowApplication.Run();
        }

        public void SetResult(XElement result)
        {
            lock (this)
            {
                _result = result;
            }
            
        }

        public XElement GetCommand()
        {
            var count = 0;
            while (_command == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 300)
                    return null;
            }
            var commandString = _command.ToString();
            lock (this)
            {
                _command = null;
            }
            
            return XElement.Parse(commandString);
        }

        public bool IsFinished()
        {
            return Status == null || (FINISHED_STATUSES.Contains(Status));
        }

        public WorkflowApplication GetWorkflowApplication(AutomationActivity activity)
        {
            var workflowApplication = new WorkflowApplication(activity)
            {
                Completed = delegate(WorkflowApplicationCompletedEventArgs e)
                {
                    switch (e.CompletionState)
                    {
                        case ActivityInstanceState.Faulted:
                            Log.Error("workflow [" + activity.Id + "] " + activity.DisplayName +
                                      " stopped! Error Message:\n" + e.TerminationException.GetType().FullName + "\n" +
                                      e.TerminationException.Message + "\n" + e.TerminationException.StackTrace);
                            Status = "Terminated";
                            ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                            break;

                        case ActivityInstanceState.Canceled:
                            Log.Error("workflow [" + activity.Id + "] " + activity.DisplayName + " Cancel.");
                            Status = "Canceled";
                            ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                            break;

                        default:
                            Log.Info("workflow [" + activity.Id + "] " + activity.DisplayName + " Completed.");
                            Status = "Completed";
                            ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                            break;
                    }
                },
                Aborted = delegate(WorkflowApplicationAbortedEventArgs e)
                {
                    Log.Error(" aborted! Error Message:\n" + e.Reason.GetType().FullName + "\n" + e.Reason.Message);
                    Status = "Aborted";
                    ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                }
            };
            return workflowApplication;
        }
    }


}