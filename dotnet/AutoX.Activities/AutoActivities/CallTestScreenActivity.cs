// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Activities.XamlIntegration;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using AutoX.Basic;
using AutoX.Basic.Model;
using System.Xml.Linq;

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof(CallTestScreenDesigner), "TestScreen.bmp")]
    [Designer(typeof(CallTestScreenDesigner))]
    public sealed class CallTestScreenActivity : AutomationActivity, IPassData
    {
        // Define an activity input argument of type string
        private bool _result;
        private string _testScreenName;
        private string _userData = "";
        private CompletionCallback _onChildComplete;

        [DisplayName("Test Screen Name")]
        public string TestSreenName
        {
            get { return _testScreenName; }
            set
            {
                _testScreenName = value;
                DisplayName = "Call Test Screen: " + _testScreenName;
                NotifyPropertyChanged("TestScreenName");
                NotifyPropertyChanged("DisplayName");
            }
        }
        [DisplayName("On Error")]
        public OnError ErrorLevel { get; set; }
        [Browsable(false)]
        public string GUID { get; set; }
        [Browsable(false)]
        public string TestSreenId { get; set; }
        private string _steps = "<Steps />";
        [DisplayName("Test Steps")]
        [Editor(typeof(StepsEditor), typeof(DialogPropertyValueEditor))]
        public string Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }

        [DisplayName("User Data")]
        [Editor(typeof(UserDataEditor), typeof(DialogPropertyValueEditor))]
        public string UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                //NotifyPropertyChanged("UserData");
            }
        }

        #region IPassData Members

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
            UserData = Utilities.PassData(outerData, UserData, OwnDataFirst);
        }

        //        protected override void CacheMetadata(NativeActivityMetadata metadata)
        //        {
        //            base.CacheMetadata(metadata);
        //            metadata.AddImplementationVariable(result);
        //        }

        /// <summary>
        ///   you must call this method after workflowinvoker.invoke
        /// </summary>
        /// <returns> </returns>
        public override bool GetResult()
        {
            return _result;
        }

        #endregion

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            /******** old way: call test screen *****
            // Obtain the runtime value of the Text input argument

            //TODO implement it!!!
            //invoke a test suite here
            Log.Debug("in CallTestScreen, before Executing Test Sreen: " + TestSreenName);
            
            var screen = Host.GetDataObject(TestSreenId);
            if (screen == null) return;
            var activity = ActivityXamlServices.Load(new StringReader(screen.GetAttributeValue("Content"))) as TestScreenActivity;
            if (activity != null)
            {
                activity.PassData(InstanceId, UserData);
                activity.SetHost(Host);
                activity.SetParentResultId(ParentResultId);
                Utilities.GetWorkflowApplication(activity).Run();
            }
            ********* old way end here **********/
            InternalExecute(context, null);
        }

        private void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        {
            if (_onChildComplete == null)
            {
                _onChildComplete = InternalExecute;
            }
            Log.Info("in CallTestScreenActivity internalexecute");
            var steps = GetSteps();
            Host.SetCommand(steps);
            var rElement = Host.GetResult(GUID);
            //TODO Log should be done at the Host side, we use this result to get some variables to use in the workflow
            Log.Info(rElement.ToString());
            SetResult(rElement);
        }

        private XElement GetSteps()
        {
            //TODO: use screen id to get the latest steps, compare to the current one, 
            //force enable some steps(if original one enabled), 
            //delete some steps (if original one gone), 
            //add some steps, update some steps (new and mark enabled, also add the un-enabled items, they would not work anyway)
            //set command to instance, then get the result
            var data = Utilities.GetActualUserData(UserData, Host);
            //Utilities.PrintDictionary(data);
            //update the Steps into the format we want
            var steps = XElement.Parse("<AutoX.Steps />");
            steps.SetAttributeValue("OnError", ErrorLevel.ToString());
            steps.SetAttributeValue("InstanceId", InstanceId);
            steps.SetAttributeValue("_id", GUID);
            foreach (XElement descendant in XElement.Parse(_steps).Descendants("Step"))
            {
                var enable = descendant.GetAttributeValue("Enable");
                if (string.IsNullOrEmpty(enable))
                {
                    enable = "True";
                }
                if (!enable.ToLower().Equals("true"))
                    continue;
                var action = descendant.GetAttributeValue("Action");

                if (string.IsNullOrEmpty(action))
                {
                    Log.Error("Action is empty, please check!");
                    continue;
                }

                var step = XElement.Parse("<Step />");

                step.SetAttributeValue("Action", action);
                var dataref = descendant.GetAttributeValue("Data");
                if (string.IsNullOrEmpty(dataref))
                {
                    var defaultData = descendant.GetAttributeValue("DefaultData");
                    if (!string.IsNullOrEmpty(defaultData))
                        step.SetAttributeValue("Data", defaultData);
                }
                else
                {
                    step.SetAttributeValue("Data", data.ContainsKey(dataref) ? data[dataref] : "");
                }
                var stepId = descendant.GetAttributeValue("_id");
                if (string.IsNullOrEmpty(stepId))
                {
                    Log.Error("Step id is empty.");
                }
                else
                {
                    step.SetAttributeValue("StepId", stepId);
                }
                var uiid = descendant.GetAttributeValue("UIId");
                var uiObject = descendant.GetAttributeValue("UIObject");
                if (!string.IsNullOrEmpty(uiObject))
                {
                    step.SetAttributeValue("UIObject", uiObject);
                }
                //TODO we have NOT handle the parent here, add it later; for now, it can work.
                if (string.IsNullOrEmpty(uiid)) continue;
                step.SetAttributeValue("UIId", uiid);
                var uio = Host.GetDataObject(uiid);
                if (uio == null) continue;
                var xO = XElement.Parse("<UIObject />");
                var xpath = uio.GetAttributeValue("XPath");
                //TODO add name, id, css later!!!
                if (!string.IsNullOrEmpty(xpath))
                    xO.SetAttributeValue("XPath", xpath);
                step.Add(xO);
                steps.Add(step);
            }
            return steps;
        }
    }
}