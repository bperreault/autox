// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (TestScreenActivity), "TestScreen.bmp")]
    [Designer(typeof (TestScreenDesigner))]
    public sealed class TestScreenActivity : AutomationActivity, IPassData
    {
        private string _name;
        private CompletionCallback _onChildComplete;
        private bool _result;
        private string _steps = "<Steps />";
        private string _userData = "";

        public TestScreenActivity()
        {
            //ErrorLevel = OnError.Continue;
            //OwnDataFirst = true;
        }

        [Browsable(false)]
        public string GUID { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                DisplayName = "Screen: " + _name;
                NotifyPropertyChanged("Steps");
            }
        }


        public string Description { get; set; }


//        [DisplayName("On Error")]
//        public OnError ErrorLevel { get; set; }

        [DisplayName("Test Steps")]
        [Editor(typeof (StepsEditor), typeof (DialogPropertyValueEditor))]
        public string Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }
/*****
        [DisplayName("User Data")]
        [Editor(typeof (UserDataEditor), typeof (DialogPropertyValueEditor))]
        public string UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                NotifyPropertyChanged("UserData");
            }
        }
*****/
        #region IPassData Members

        public void PassData(string instanceId, string outerData)
        {
            //InstanceId = instanceId;
            //UserData = Utilities.PassData(outerData, UserData, OwnDataFirst);
        }

        public override bool GetResult()
        {
            return _result;
        }

        #endregion

        protected override void Execute(NativeActivityContext context)
        {
            //InternalExecute(context, null);
        }
/*******
        private void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        {
            if (_onChildComplete == null)
            {
                _onChildComplete = InternalExecute;
            }
            Log.Info("in TestScreenActivity internalexecute");
            var steps = GetSteps();
            Host.SetCommand(steps);
            var rElement = Host.GetResult(GUID);
            //TODO Log should be done at the Host side, we use this result to get some variables to use in the workflow
            Log.Info(rElement.ToString());
            SetResult(rElement);
        }

        private XElement GetSteps()
        {
            //set command to instance, then get the result
            var data = Utilities.GetActualUserData(UserData, Host);
            //Utilities.PrintDictionary(data);
            //update the Steps into the format we want
            var steps = XElement.Parse("<AutoX.Steps />");
            steps.SetAttributeValue(Constants.ON_ERROR, ErrorLevel.ToString());
            steps.SetAttributeValue(Constants.INSTANCE_ID, InstanceId);
            steps.SetAttributeValue(Constants._ID, GUID);
            foreach (XElement descendant in XElement.Parse(_steps).Descendants(Constants.STEP))
            {
                var enable = descendant.GetAttributeValue(Constants.ENABLE);
                if (string.IsNullOrEmpty(enable))
                {
                    enable = "True";
                }
                if (!enable.ToLower().Equals("true"))
                    continue;
                var action = descendant.GetAttributeValue(Constants.ACTION);
                
                if (string.IsNullOrEmpty(action))
                {
                    Log.Error("Action is empty, please check!");
                    continue;
                }
                
                var step = XElement.Parse("<Step />");

                step.SetAttributeValue(Constants.ACTION, action);
                var dataref = descendant.GetAttributeValue(Constants.DATA);
                if (string.IsNullOrEmpty(dataref))
                {
                    var defaultData = descendant.GetAttributeValue(Constants.DEFAULT_DATA);
                    if (!string.IsNullOrEmpty(defaultData))
                        step.SetAttributeValue(Constants.DATA, defaultData);
                }
                else
                {
                    step.SetAttributeValue(Constants.DATA, data.ContainsKey(dataref) ? data[dataref] : "");
                }
                var stepId = descendant.GetAttributeValue(Constants._ID);
                if (string.IsNullOrEmpty(stepId))
                {
                    Log.Error("Step id is empty.");
                }
                else
                {
                    step.SetAttributeValue("StepId",stepId);
                }
                var uiid = descendant.GetAttributeValue(Constants.UI_ID);
                var uiObject = descendant.GetAttributeValue(Constants.UI_OBJECT);
                if (!string.IsNullOrEmpty(uiObject))
                {
                    step.SetAttributeValue(Constants.UI_OBJECT, uiObject);
                }
                //TODO we have NOT handle the parent here, add it later; for now, it can work.
                if (string.IsNullOrEmpty(uiid)) continue;
                step.SetAttributeValue(Constants.UI_ID, uiid);
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
        ********/
    }
}