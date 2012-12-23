// Hapa Project, CC
// Created @2012 08 29 08:34
// Last Updated  by Huang, Jien @2012 08 29 08:34

#region

using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using AutoX.Activities.AutoActivities;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.Database;

#endregion

namespace AutoX.Web
{
    public class TestInstance
    {
        private const string ExitStatus = "Completed;Aborted;Canceled;Terminated;Invalid";
        private readonly Dictionary<string, XElement> _results = new Dictionary<string, XElement>();
        private readonly WorkflowApplication _workflowApplication;
        private volatile string _status;
        private volatile string _currentStepGuid;
        public TestInstance(string guid, string scriptGuid, string name, string computer, string suiteName,
                            string language)
        {
            GUID = guid;
            //get workflow store GUID
            ScriptGUID = scriptGuid;
            SuiteName = suiteName;
            Language = language;
            Status = "Invalid";
            //get string content of workflow
            var script = DBManager.GetInstance().FindOneDataFromDB(scriptGuid) as Script;
            if (script == null) return;
            if (script.Content == null) return;
            //load workflow
            var activity = ActivityXamlServices.Load(new StringReader(script.Content)) as AutomationActivity;
            if (activity != null)
            {
                activity.InstanceId = GUID;
                activity.SetHost(InstanceManager.GetInstance());
                //var idleEvent = new AutoResetEvent(false);
                //TODO write log in workflow!!!
                
                _workflowApplication = new WorkflowApplication(activity)
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
            }

            //workflowApplication.Idle = delegate(WorkflowApplicationIdleEventArgs e)
            //                               {
            //                                   idleEvent.Set();
            //                               };
            //mainTask = new Task(() =>
            //                        {
            //                            while(!Status.Equals("Invalid"))
            //                            {
            //                                idleEvent.WaitOne();

            //                            }
            //                        });
            TestName = name;
            ClientName = computer;
            Status = "STOP";

            //create log entry here
            string logRootId = Configuration.Settings("ResultsRoot", "0020020000002");
            XElement xElement =
                XElement.Parse(
                    @"<Result Name='" + name + "' Type='Result' Description='Automation Result' GUID='" +
                    GUID + "' ParentId='" + logRootId + "' />");
            var iDataObject = xElement.GetDataObjectFromXElement() as Result;
            if (iDataObject == null) return;
            iDataObject.StopTime = DateTime.UtcNow;
            InsertResultToDB(guid,logRootId,iDataObject);
            //DBManager.GetInstance().AddOrUpdateOneDataToDB(guid, logRootId, iDataObject);
        }
        



        public string GUID { get; set; }
        public string ScriptGUID { get; set; }
        public string TestName { get; set; }
        public string ClientName { get; set; }
        public string SuiteName { get; set; }
        public string Language { get; set; }


        public string Status
        {
            get
            {
                lock (this)
                {
                    return _status;
                }
            }
            set
            {
                lock (this)
                {
                    _status = value;
                }
            }
        }

        public void Start()
        {
            //this is a start or resume
            Status = "Start";
            if (_results.Count == 0)
                _workflowApplication.Run();
            //start or resume workflow
        }

        public void Stop()
        {
            //infact this is a pause
            Status = "STOP";
            //TODO temp solution, can be use in most situation  
            _workflowApplication.Cancel();
            //suspend or pause workflow
        }

        public void SetResult(XElement result)
        {
            if (ExitStatus.Contains(Status))
                return;

            //TODO put it to result list, while workflow finished, workflow will write the log to DB
        }

        public void SetFinalResult(XElement result, string finalResult)
        {
            if (ExitStatus.Contains(Status))
                return;
            //TODO activity check the onerror part, give out the final result, write it to variable list etc
            //if it is Test case, write it to db
        }

        private void InsertResultToDB(string guid, string parentId, Result result )
        {
            DBManager.GetInstance().AddOrUpdateOneDataToDB(guid,parentId,result);
        }
        
        public void SetCommand(XElement steps)
        {
            if (ExitStatus.Contains(Status))
                return;
            while (true)
            {
                if (Status.Equals("Stop"))
                {
                    Thread.Sleep(1000*3);
                    continue;
                }

                if (Status.Equals("Start"))
                {
                    //we use runtime id to match results
                    string runtimeId = Guid.NewGuid().ToString();
                    steps.SetAttributeValue("RunTimeId", runtimeId);
                    _currentStepGuid = steps.GetAttributeValue("GUID");
                    _results.Add(runtimeId, null);
                    Computer computer = ComputersManager.GetInstance().GetComputer(ClientName);
                    computer.SetCommand(steps.ToString());
                }
                return;
            }
        }

        public XElement GetResult(string stepsId)
        {
            //try to get the result, if not found, keep waiting until timeout
            //while return timeout, activity will think this is a failure
            //if got the result, if status is stop, keep waiting until timeout or it turn to start
            //if it is invalid, return null

            //if return null, means fatal error
            if (ExitStatus.Contains(Status))
                return null;
            while (true)
            {
                //while not get the result, sleep a while, continue
                if (Status.Equals("Stop"))
                {
                    Thread.Sleep(1000*3);
                    continue;
                }
                if (!stepsId.Equals(_currentStepGuid))
                {
                    Logger.GetInstance().Log().Error("current GUID("+_currentStepGuid+") not equal the steps GUID("+stepsId+")");
                    return null;
                }
                    
                if (_results.ContainsKey(stepsId))
                {
                    if (_results[stepsId]!=null)
                        return _results[stepsId];
                }
                    
                Thread.Sleep(1000);
            }
        }

        public void Remove()
        {
            _workflowApplication.Cancel(new TimeSpan(0, 5, 0));
            Status = "Invalid";
        }

        //private void CallXaml(string workflowSource)
        //{
        //    var dyanamicActivity = GetActivityFromString(workflowSource);
        //    var workflowInvoker = new WorkflowInvoker(dyanamicActivity);

        //    WorkflowInvoker.Invoke(dyanamicActivity);
        //}

        //private void CallXaml(string workflowSource, object data)
        //{
        //    var dyanamicActivity = GetActivityFromString(workflowSource);
        //    if (dyanamicActivity == null)
        //    {
        //        Logger.GetInstance().Log().Error("loaded workflow source cannot generate a activity");
        //        return;
        //    }

        //    var prop = new DynamicActivityProperty
        //                   {
        //                       Name = "Data",
        //                       Type = typeof (InArgument<IDictionary<string, object>>),
        //                   };

        //    dyanamicActivity.Properties.Add(prop);

        //    WorkflowInvoker.Invoke(dyanamicActivity);
        //}

        //private static DynamicActivity GetActivityFromString(string workflowSource)
        //{
        //    var sr = new StringReader(workflowSource);

        //    var serviceImplementation = XamlServices.Load(sr);
        //    var service = serviceImplementation as WorkflowService;
        //    if (service != null)
        //    {
        //        var activity = service.Body;
        //        var dyanamicActivity = new DynamicActivity {Implementation = () => activity};
        //        return dyanamicActivity;
        //    }
        //    return null;
        //}
    }
}