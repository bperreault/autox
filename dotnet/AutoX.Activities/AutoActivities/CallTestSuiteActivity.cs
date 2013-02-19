// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.XamlIntegration;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (CallTestSuiteActivityDesigner), "TestSuite.bmp")]
    [Designer(typeof (CallTestSuiteActivityDesigner))]
    public sealed class CallTestSuiteActivity : AutomationActivity, IPassData
    {
        
        private string _testSuiteName;

        [DisplayName("Test Suite Name")]
        public string TestSuiteName
        {
            get { return _testSuiteName; }
            set
            {
                _testSuiteName = value;
                DisplayName = "Call Test Suite: " + _testSuiteName;
            }
        }


        [Browsable(false)]
        public string TestSuiteId { get; set; }

        [DisplayName("Description")]
        public string TestSuiteDescription { get; set; }

        #region IPassData Members

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
        }


        #endregion

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.

        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

		    SetVariablesBeforeRunning(context);
            //TODO implement it!!!
            //invoke a test suite here
            Log.Debug("in CallTestSuite, before Executing Test Suite: " + TestSuiteName);
            var screen = Host.GetDataObject(TestSuiteId);
            if (screen == null) return;
            var activity = ActivityXamlServices.Load(new StringReader(screen.GetAttributeValue(Constants.CONTENT)));
            if (activity is AutomationActivity)
            {
                ((AutomationActivity)activity).SetHost(Host);
                ((AutomationActivity)activity).SetParentResultId(ParentResultId);
                ((AutomationActivity)activity).InstanceId = InstanceId;
            }
            WorkflowInvoker.Invoke(activity);
            //TODO calculate result here
            _result = ((IPassData) activity).GetResult();
        }
    }
}
