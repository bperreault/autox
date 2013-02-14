using AutoX.Activities.AutoActivities;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.DB;
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace AutoX.WF.Core
{
    public class WorkflowInstance : IHost, IDataObject
    {
        private Dictionary<string, string> _variables = new Dictionary<string, string>();
        private volatile XElement _command;
        private volatile XElement _result;
        private List<IObserver> _observers = new List<IObserver>();
        private string _instanceId;
        private readonly StatusTracker _statusTracker = new StatusTracker();
        private WorkflowApplication _workflowApplication;
        public string _parentId { get; set; }

        public string ClientId { get; set; }

        public string TestName { get; set; }

        public string SuiteName { get; set; }

        public string Status { get; set; }

        public string ClientName { get; set; }

        public string Language { get; set; }

        public string ScriptGUID { get; set; }
        public string _id
        {
            get { return _instanceId; }
            set { _instanceId = value; }
        }

        public DateTime Created { get; set; }

        public DateTime Updated
        {
            get;
            set;
        }

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

        public XElement ToXElement()
        {
            var xInstance = this.GetXElementFromObject();
            foreach (var _var in Variables)
            {
                if (_var.Key.Contains(":")) continue;
                xInstance.SetAttributeValue(_var.Key, _var.Value);
            }
            xInstance.SetAttributeValue("Status", Status);
            return xInstance;
        }

        public static Instance FromXElement(XElement element)
        {
            return element.GetObjectFromXElement() as Instance;
        }

        public WorkflowInstance(string instanceId, string workflowId, Dictionary<string, string> upperLevelVariables)
        {
            InstanceId = instanceId;
            Status = "Invalid";
            ScriptGUID = workflowId;
            Variables = Configuration.Clone().GetList();
            if (upperLevelVariables != null)
                foreach (var upperLevelVariable in upperLevelVariables)
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
                var _1stLevelKid = Data.Read(rootId);
                var resultId = _1stLevelKid.GetAttributeValue("Result");

                if (!string.IsNullOrEmpty(resultId))
                {
                    string versionId = FindOrCreateSubResultFolder(versionName, resultId);
                    string buildId = FindOrCreateSubResultFolder(buildName, versionId);
                    XElement script = GetDataObject(workflowId);
                    _result = null;
                    _command = null;
                    if (script != null)
                        _workflowApplication = CreateActivity(script.GetAttributeValue(Constants.CONTENT), buildId);
                    if (_workflowApplication != null){
			//TODO set Variables to activity Variables
                        Status = "Ready";
			}
                }
            }
        }

        private static string FindOrCreateSubResultFolder(string versionName, string resultId)
        {
            var results = Data.GetChildren(resultId);
            string versionId = null;
            if (results != null)
            {
                foreach (var result in results.Descendants())
                {
                    if (versionName.Equals(result.GetAttributeValue("Name")))
                    {
                        versionId = result.GetAttributeValue("_id");
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(versionId))
            {
                versionId = Guid.NewGuid().ToString();
                Data.Save(XElement.Parse("<Result Name='" + versionName + "' _id='" + versionId + "' _parentId='" + resultId + "' _type='Folder' Created='" + DateTime.Now.ToString() + "' Updated='" + DateTime.Now.ToString() + "' />"));
            }
            return versionId;
        }

        private WorkflowApplication CreateActivity(string workflow, string resultParentId)
        {
            var activity = ActivityXamlServices.Load(new StringReader(workflow)) as AutomationActivity;
            if (activity != null)
            {
                activity.SetHost(this);
                activity.InstanceId = InstanceId;
                activity.SetParentResultId(resultParentId);
                WorkflowApplication workflowApplication = GetWorkflowApplication(activity);
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
                   
                    while (ClientId == null)
                    {
                        Thread.Sleep(23);
                        ClientId = ClientInstancesManager.GetInstance().GetAReadyClientInstance();
                    }
                    ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Running";
                    Status = "Running";
                    _workflowApplication.Run();
                    return XElement.Parse("<Result Result='Success' />");
                }
            return XElement.Parse("<Result Result='Error' />");
        }

        public XElement GetDataObject(string id)
        {
            return Data.Read(id);
        }

        public void SetCommand(XElement steps)
        {
            Log.Debug("Set Command:\n" + steps);
            if (!string.IsNullOrEmpty(ClientId))
            {
                ClientInstancesManager.GetInstance().GetComputer(ClientId).SetCommand(steps.ToString());
            }
            _command = steps;
        }

        public XElement GetResult()
        {
            int count = 0;
            while (_result == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 3000)
                    return null;
            }
            string resultString = _result.ToString();
            _result = null;
            return XElement.Parse(resultString);
        }

        public void SetResult(XElement result)
        {
            _result = result;
        }

        public XElement GetCommand()
        {
            int count = 0;
            while (_command == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 300)
                    return null;
            }
            string commandString = _command.ToString();
            _command = null;
            return XElement.Parse(commandString);
        }

        const string FINISHED_STATUSES = "Completed|Aborted|Canceled|Faulted";

        public bool IsFinished()
        {
            return Status == null || (FINISHED_STATUSES.Contains(Status));
        }

        public void Stop()
        {
            if (_workflowApplication != null)
            {
                ClientInstancesManager.GetInstance().GetComputer(ClientId).Status = "Ready";
                _workflowApplication.Cancel();
            }
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
                            Log.Error("workflow [" + activity.Id + "] " + activity.DisplayName + " stopped! Error Message:\n" + e.TerminationException.GetType().FullName + "\n" + e.TerminationException.Message);
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

    public class StatusTracker : TrackingParticipant
    {
        public string Status { get; private set; }
        TrackingProfile trackingProfile = new TrackingProfile();

        public StatusTracker()
        {

            trackingProfile.Queries.Add(new ActivityStateQuery
            {
                ActivityName = "*",
                States = { "*" },
                Variables = { "*" },
                Arguments = { "*" }
            });
            trackingProfile.Queries.Add(new WorkflowInstanceQuery
            {
                States = { "*" },
            });

            this.TrackingProfile = trackingProfile;
        }

        protected override void Track(TrackingRecord record, System.TimeSpan timeout)
        {
            if (record is WorkflowInstanceRecord)
            {
                Status = ((WorkflowInstanceRecord)record).State;
            }
        }
    }
}
