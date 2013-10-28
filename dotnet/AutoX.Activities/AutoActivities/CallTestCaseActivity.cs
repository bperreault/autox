#region

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

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (CallTestCaseDesigner), "TestCase.png")]
    [Designer(typeof (CallTestCaseDesigner))]
    public sealed class CallTestCaseActivity : AutomationActivity, IPassData
    {
        private string _testCaseName;
        private string _userData = "";

        public CallTestCaseActivity()
        {
            OwnDataFirst = true;
        }

        [DisplayName(@"Test Case Name")]
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

        [DisplayName(@"User Data")]
        [Editor(typeof (UserDataEditor), typeof (DialogPropertyValueEditor))]
        public string UserData
        {
            get { return _userData; }
            set
            {
                _userData = value;
                NotifyPropertyChanged(@"UserData");
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //call base.CacheMetadata to add the Activities and Variables to this activity's metadata
            base.CacheMetadata(metadata);

            string errorMessage = AutomationActivityValidation();
            if (!string.IsNullOrEmpty(errorMessage))
                metadata.AddValidationError(errorMessage);

        }

        public override string AutomationActivityValidation()
        {
            //add validation to this activity: but we don't know what to verify for this activity now.
            
            return null;
        }
        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
            UserData = Utilities.PassData(outerData, UserData, OwnDataFirst);
        }

        protected override void Execute(NativeActivityContext nativeActivityContext)
        {
            SetVariablesBeforeRunning(nativeActivityContext);
            var screen = Host.GetDataObject(TestCaseId);
            if (screen == null) return;
            var activity = ActivityXamlServices.Load(new StringReader(screen.GetAttributeValue(Constants.CONTENT)));
            var automationActivity = activity as AutomationActivity;
            if (automationActivity != null)
            {
                automationActivity.SetHost(Host);
                automationActivity.SetParentResultId(ParentResultId);
            }
            WorkflowInvoker.Invoke(activity);
            RunningResult = ((IPassData) activity).GetResult();
        }
    }
}