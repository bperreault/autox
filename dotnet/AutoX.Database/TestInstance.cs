#region hapa

// // Hapa Project, CC
// // Created @2012 08 03 3:35 PM
// // Last Updated  by Huang, Jien @2012 08 06 12:19 AM

#endregion

#region

using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Activities;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Database
{
    public class TestInstance
    {
        private readonly WorkflowApplication _workflowApplication;
        private readonly Dictionary<string, XElement> results = new Dictionary<string, XElement>();
        private volatile string _status;

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
            var activity = ActivityXamlServices.Load(new StringReader(script.Content));
            //var idleEvent = new AutoResetEvent(false);
            _workflowApplication = new WorkflowApplication(activity);

            _workflowApplication.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
                                                 {
                                                     switch (e.CompletionState)
                                                     {
                                                         case ActivityInstanceState.Faulted:
                                                             Logger.GetInstance().Log().Error("workflow " +
                                                                                              scriptGuid +
                                                                                              " stopped! Error Message:\n"
                                                                                              +
                                                                                              e.TerminationException.
                                                                                                  GetType().FullName +
                                                                                              "\n"
                                                                                              +
                                                                                              e.TerminationException.
                                                                                                  Message);
                                                             Status = "Terminated";
                                                             break;
                                                         case ActivityInstanceState.Canceled:
                                                             Logger.GetInstance().Log().Warn("workflow " + scriptGuid +
                                                                                             " Cancel.");
                                                             Status = "Canceled";
                                                             break;
                                                         default:
                                                             Logger.GetInstance().Log().Info("workflow " + scriptGuid +
                                                                                             " Completed.");
                                                             Status = "Completed";
                                                             break;
                                                     }
                                                 };
            _workflowApplication.Aborted = delegate(WorkflowApplicationAbortedEventArgs e)
                                               {
                                                   Logger.GetInstance().Log().Error("workflow " +
                                                                                    scriptGuid
                                                                                    + " aborted! Error Message:\n"
                                                                                    + e.Reason.GetType().FullName + "\n" +
                                                                                    e.Reason.Message);
                                                   Status = "Aborted";
                                               };

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
            if (results.Count == 0)
                _workflowApplication.Run();
            //start or resume workflow
        }

        public void Stop()
        {
            //infact this is a pause
            Status = "STOP";

            //suspend or pause workflow
        }

        protected void SetResult(XElement result)
        {
            //TODO put it to result list
        }

        protected XElement GetResult(string bookmarkId)
        {
            //try to get the result, if not found, keep waiting until timeout
            //while return timeout, activity will think this is a failure
            //if got the result, if status is stop, keep waiting until timeout or it turn to start
            //if it is invalid, return null

            //if return null, means fatal error
            return null;
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