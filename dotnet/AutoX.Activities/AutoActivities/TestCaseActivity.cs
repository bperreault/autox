#region

// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (TestCaseDesigner), "TestCase.png")]
    [Designer(typeof (TestCaseDesigner))]
    public sealed class TestCaseActivity : AutomationActivity, IPassData
    {
        private readonly Variable<int> _currentIndex;
        private string _name;
        private CompletionCallback _onChildComplete;
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

        [DisplayName(@"On Error")]
        public OnError ErrorLevel { get; set; }

        [DisplayName(@"User Data")]
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
            //add a result level here
            Result = new XElement(Constants.RESULT);
            SetResult();
            SetVariablesBeforeRunning(context);
            InternalExecute(context, null);
            
        }

        private void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        {
            //grab the index of the current Activity
            var currentActivityIndex = _currentIndex.Get(context);
            if (currentActivityIndex > 0)
            {
                var lastChild = children[currentActivityIndex - 1];
                //Get result here, it is sync or async????
                _runningResult = _runningResult && ((IPassData)lastChild).GetResult();
                //TODO set variables value ((AutomationActivity)nextChild).Name to _runningResult
                if (!_runningResult)
                {
                    if (ErrorLevel == OnError.AlwaysReturnTrue)
                        _runningResult = true;
                    //if (ErrorLevel == OnError.Terminate)
                    //{
                    //    //TODO terminate the instance (send a status to instance)
                    //}
                    if (ErrorLevel == OnError.Continue)
                    {
                        //do nothing, just continue
                    }
                    if (ErrorLevel == OnError.JustShowWarning)
                    {
                        Log.Warn("Warning:\n" + lastChild.DisplayName + " Error happened, but we ignore it");
                        _runningResult = true;
                    }
                    if (ErrorLevel == OnError.StopCurrentScript)
                    {
                        Log.Error("Error:\n" + lastChild.DisplayName + " Error happened, stop current script.");
                        return;
                    }
                }
            }
            if (currentActivityIndex == children.Count)
            {
                //if the currentActivityIndex is equal to the count of MySequence's Activities
                //MySequence is complete
                SetFinalResult();
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
            var child = nextChild as AutomationActivity;
            if (child != null)
            {
                child.SetHost(Host);
                child.SetParentResultId(ResultId);
            }
            context.ScheduleActivity(nextChild, _onChildComplete);
            
            ////Get result here, it is sync or async????
            
            //_runningResult = _runningResult && ((IPassData)nextChild).GetResult();
            //if (!_runningResult)
            //{
            //    if (ErrorLevel == OnError.AlwaysReturnTrue)
            //        _runningResult = true;
            //    //if (ErrorLevel == OnError.Terminate)
            //    //{
            //    //    //TODO terminate the instance (send a status to instance)
            //    //}
            //    if (ErrorLevel == OnError.Continue)
            //    {
            //        //do nothing, just continue
            //    }
            //    if (ErrorLevel == OnError.JustShowWarning)
            //    {
            //        //do nothing, log warning
            //        Log.Warn("Warning:\n" + DisplayName + " Error happened, but we ignore it");
            //        _runningResult = true;
            //    }
            //    if (ErrorLevel == OnError.StopCurrentScript)
            //    {
            //        //log error, then return
            //    }
            //}


            //increment the currentIndex
            _currentIndex.Set(context, ++currentActivityIndex);
        }
    }
}