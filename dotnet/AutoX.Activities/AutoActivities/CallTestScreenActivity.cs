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

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (CallTestScreenDesigner), "TestScreen.bmp")]
    [Designer(typeof (CallTestScreenDesigner))]
    public sealed class CallTestScreenActivity : AutomationActivity, IPassData
    {
        // Define an activity input argument of type string
        private bool _result;
        private string _testScreenName;
        private string _userData = "";

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


        [Browsable(false)]
        public string TestSreenId { get; set; }

        [DisplayName("User Data")]
        [Editor(typeof (UserDataEditor), typeof (DialogPropertyValueEditor))]
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
            
        }
    }
}