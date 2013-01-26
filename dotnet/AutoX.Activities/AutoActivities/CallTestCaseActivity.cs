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

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (CallTestCaseDesigner), "TestCase.bmp")]
    [Designer(typeof (CallTestCaseDesigner))]
    public sealed class CallTestCaseActivity : AutomationActivity
    {
        private bool _result;
        private string _testCaseName;
        private string _userData = "";


        public CallTestCaseActivity()
        {
            OwnDataFirst = true;
        }

        [DisplayName("Test Case Name")]
        public string TestCaseName
        {
            get { return _testCaseName; }
            set
            {
                _testCaseName = value;
                DisplayName = "Call Test Case: " + _testCaseName;
            }
        }

        [Browsable(false)]
        public string TestCaseId { get; set; }

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


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
            UserData = Utilities.PassData(outerData, UserData, OwnDataFirst);
        }

        public override bool GetResult()
        {
            return _result;
        }

        protected override void Execute(NativeActivityContext nativeActivityContext)
        {
            //TODO implement it!!!
            //invoke a test suite here
/*
            Logger.GetInstance().Log().Debug("in CallTestCase, before Executing Test Case: " + TestCaseName);
            var screen = DBManager.GetInstance().FindOneDataFromDB(TestCaseId) as Script;
            if (screen == null) return;
            var activity = ActivityXamlServices.Load(new StringReader(screen.Content));
            ((IPassData) activity).PassData(InstanceId, UserData);
            WorkflowInvoker.Invoke(activity);
            result = ((IPassData) activity).GetResult();
*/
            var screen = Host.GetDataObject(TestCaseId);
            if (screen == null) return;
            var activity = ActivityXamlServices.Load(new StringReader(screen.GetAttributeValue("Content")));
            if (activity is AutomationActivity)
            {
                ((AutomationActivity)activity).SetHost(Host);
                ((AutomationActivity)activity).SetParentResultId(ParentResultId);
            }
            WorkflowInvoker.Invoke(activity);
            _result = ((IPassData)activity).GetResult();
            
            
        }
    }
}