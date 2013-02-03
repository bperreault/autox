using AutoX.Activities.AutoActivities;
using AutoX.Basic;
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
    public class WorkflowInstance : IHost, IObserable
    {
        private Dictionary<string, string> _variables = new Dictionary<string, string>();
        private volatile XElement _command;
        private volatile XElement _result;
        private List<IObserver> _observers = new List<IObserver>();
        private string _instanceId = Guid.NewGuid().ToString();

        public string InstanceId
        {
            get { return _instanceId; }
        }

        public string ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public WorkflowInstance(string workflowId, Dictionary<string, string> upperLevelVariables, string resultParentId)
        {
            if (upperLevelVariables != null)
                foreach (var upperLevelVariable in upperLevelVariables)
                {
                    _variables.Add(upperLevelVariable.Key, upperLevelVariable.Value);
                }
            XElement script = GetDataObject(workflowId);
            _result = null;
            _command = null;
            if (script != null)
                StartActivity(script.GetAttributeValue(Constants.CONTENT), resultParentId);
        }

        private readonly StatusTracker _statusTracker = new StatusTracker();
        private WorkflowApplication _workflowApplication;
        private string _parentId;

        public string Status { get; set; }

        private void StartActivity(string workflow, string resultParentId)
        {
            var activity = ActivityXamlServices.Load(new StringReader(workflow)) as AutomationActivity;
            if (activity != null)
            {
                activity.SetHost(this);
                activity.InstanceId = InstanceId;
                activity.SetParentResultId(resultParentId);
                _workflowApplication = GetWorkflowApplication(activity);
                _workflowApplication.Extensions.Add(_statusTracker);
                _workflowApplication.Run();
            }
        }

        public XElement GetDataObject(string id)
        {
            return Data.Read(id);
        }

        public void SetCommand(XElement steps)
        {
            _command = steps;
        }

        public XElement GetResult(string guid)
        {
            int count = 0;
            while (_result == null)
            {
                Thread.Sleep(1000);
                count++;
                if (count > 300)
                    return null;
            }
            string resultString = _result.ToString();
            _result = null;
            return XElement.Parse(resultString);
        }

        public void SetResult(XElement result)
        {
            _result = result;
            Notify(result);
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

        public void Register(IObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Notify(XElement change)
        {
            foreach (var observer in _observers)
            {
                observer.Update(change);
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

                            //Logger.GetInstance().Log().Error("workflow " +
                            //                                 scriptGuid +
                            //                                 " stopped! Error Message:\n"
                            //                                 +
                            //                                 e.TerminationException.
                            //                                     GetType().FullName +
                            //                                 "\n"
                            //                                 +
                            //                                 e.TerminationException.
                            //                                     Message);
                            Status = "Terminated";
                            break;

                        case ActivityInstanceState.Canceled:

                            //Logger.GetInstance().Log().Warn("workflow " + scriptGuid +
                            //                                " Cancel.");
                            Status = "Canceled";
                            break;

                        default:

                            //Logger.GetInstance().Log().Info("workflow " + scriptGuid +
                            //                                " Completed.");
                            Status = "Completed";
                            break;
                    }
                },
                Aborted = delegate
                {
                    //Logger.GetInstance().Log().Error("workflow " +
                    //                                 scriptGuid
                    //                                 + " aborted! Error Message:\n"
                    //                                 + e.Reason.GetType().FullName + "\n" +
                    //                                 e.Reason.Message);
                    Status = "Aborted";
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