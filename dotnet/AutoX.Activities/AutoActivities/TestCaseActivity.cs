// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (TestCaseDesigner), "TestCase.bmp")]
    [Designer(typeof (TestCaseDesigner))]
    public sealed class TestCaseActivity : AutomationActivity, IPassData
    {
        private readonly Variable<int> _currentIndex;
        private string _name;
        private CompletionCallback _onChildComplete;
        private bool _result;
        private string _userData = "";

        public TestCaseActivity()
        {
            children = new Collection<Activity>();
            _currentIndex = new Variable<int>();
            ErrorLevel = OnError.Continue;
            OwnDataFirst = true;
        }


        [Browsable(false)]
        public string GUID { get; set; }

        public new string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                DisplayName = "Case: " + _name;
            }
        }

        public string Description { get; set; }

        [DisplayName("On Error")]
        public OnError ErrorLevel { get; set; }

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

        [Browsable(false)]
        public Collection<Activity> children { get; set; }

        #region IPassData Members

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
            UserData = Utilities.PassData(outerData, UserData, OwnDataFirst);
        }

        public override bool GetResult()
        {
            return _result;
        }

        #endregion

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //call base.CacheMetadata to add the Activities and Variables to this activity's metadata
            base.CacheMetadata(metadata);
            //add the private implementation variable: currentIndex 
            metadata.AddImplementationVariable(_currentIndex);
        }

        protected override void Execute(NativeActivityContext context)
        {
            InternalExecute(context, null);
        }

        private void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        {
            //grab the index of the current Activity
            var currentActivityIndex = _currentIndex.Get(context);
            if (currentActivityIndex == children.Count)
            {
                //if the currentActivityIndex is equal to the count of MySequence's Activities
                //MySequence is complete
                return;
            }

            if (_onChildComplete == null)
            {
                //on completion of the current child, have the runtime call back on this method
                _onChildComplete = InternalExecute;
            }

            //grab the next Activity in MySequence.Activities and schedule it
            var nextChild = children[currentActivityIndex];
            ((IPassData) nextChild).PassData(InstanceId, UserData);
            if (nextChild is AutomationActivity)
            {
                ((AutomationActivity)nextChild).SetHost(Host);
                ((AutomationActivity)nextChild).SetParentResultId(ParentResultId);
            }
            context.ScheduleActivity(nextChild, _onChildComplete);
            //Get result here, it is sync or async????
            _result = _result && ((IPassData) nextChild).GetResult();
            if (!_result)
            {
                if (ErrorLevel == OnError.AlwaysReturnTrue)
                    _result = true;
                if (ErrorLevel == OnError.Terminate)
                {
                    //TODO terminate the instance (send a status to instance)
                }
                if (ErrorLevel == OnError.Continue)
                {
                    //do nothing, just continue
                }
                if (ErrorLevel == OnError.JustShowWarning)
                {
                    //do nothing, log warning
                }
                if (ErrorLevel == OnError.StopCurrentScript)
                {
                    //log error, then return
                }
            }


            //increment the currentIndex
            _currentIndex.Set(context, ++currentActivityIndex);
        }
    }
}