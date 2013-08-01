#region

// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (TestSuiteDesigner), "TestSuite.bmp")]
    [Designer(typeof (TestSuiteDesigner))]
    public sealed class TestSuiteActivity : AutomationActivity, IPassData
    {
        private readonly Variable<int> _currentIndex;
        private string _name;
        private CompletionCallback _onChildComplete;

        public TestSuiteActivity()
        {
            children = new Collection<Activity>();
            _currentIndex = new Variable<int>();
            ErrorLevel = OnError.Continue;
        }


        [Browsable(false)]
        public string GUID { get; set; }

        public new string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                DisplayName = "Suite: " + _name;
            }
        }

        public string Description { get; set; }

        [DisplayName(@"On Error")]
        [DefaultValue(OnError.Continue)]
        public OnError ErrorLevel { get; set; }

        [Browsable(false)]
        public Collection<Activity> children { get; set; }

        #region IPassData Members

        public void PassData(string instanceId, string outerData)
        {
            InstanceId = instanceId;
        }

        #endregion

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            //call base.CacheMetadata to add the Activities and Variables to this activity's metadata
            base.CacheMetadata(metadata);
            //add the private implementation variable: currentIndex 
            metadata.AddImplementationVariable(_currentIndex);
            //foreach (var child in children)
            //{
            //    metadata.AddImplementationChild(child);
            //}
            //TODO validation

            //if (true)
            //{
            //    //Add a validation error with a custom message
            //    metadata.AddValidationError("error message");
            //}
        }


        protected override void Execute(NativeActivityContext context)
        {
            //set env environment variables
            var steps =
                XElement.Parse("<AutoX.Steps  OnError=\"" + ErrorLevel + "\" InstanceId=\"" + InstanceId + "\"/>");
            var set_env = XElement.Parse("<Step />");
            set_env.SetAttributeValue(Constants.ACTION, Constants.SET_ENV);
            foreach (PropertyDescriptor _var in context.DataContext.GetProperties())
            {
                set_env.SetAttributeValue(_var.Name, _var.GetValue(context.DataContext));
            }
            steps.Add(set_env);
            Host.SetCommand(steps);
            Host.GetResult();
            //add a result level here
            var result = new XElement(Constants.RESULT);
            SetResult(result);
            SetVariablesBeforeRunning(context);
            InternalExecute(context, null);
                        
        }

        private void InternalExecute(NativeActivityContext context, ActivityInstance instance)
        {
            Log.Info("In Test Suite InternalExecute!");
            //grab the index of the current Activity
            var currentActivityIndex = _currentIndex.Get(context);
            if (currentActivityIndex == children.Count)
            {
                //if the currentActivityIndex is equal to the count of MySequence's Activities
                //Suite is complete, then we close the browser here
                var steps =
                    XElement.Parse("<AutoX.Steps  OnError=\"AlwaysReturnTrue\" InstanceId=\"" + InstanceId + "\"/>");
                var close = XElement.Parse("<Step Action=\"Close\" />");
                steps.Add(close);
                Host.SetCommand(steps);
                Host.GetResult();
                return;
            }

            if (_onChildComplete == null)
            {
                //on completion of the current child, have the runtime call back on this method

                _onChildComplete = InternalExecute;
            }

            //grab the next Activity in MySequence.Activities and schedule it
            var nextChild = children[currentActivityIndex];
            var childEnabled = false;
            if (nextChild is AutomationActivity)
            {
                ((AutomationActivity) nextChild).SetHost(Host);
                ((AutomationActivity) nextChild).InstanceId = InstanceId;
                ((AutomationActivity) nextChild).SetParentResultId(ResultId);
                childEnabled = ((AutomationActivity) nextChild).Enabled;
            }
            //TODO if enabled, run it, may need to use while???
            context.ScheduleActivity(nextChild, _onChildComplete);
            //Get result here, it is sync or async????
            _result = _result && ((IPassData) nextChild).GetResult();
            //TODO set variables value ((AutomationActivity)nextChild).Name to _result
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
                    Log.Warn("Warning:\n" + nextChild.DisplayName + " Error happened, but we ignore it");
                    _result = true;
                }
                if (ErrorLevel == OnError.StopCurrentScript)
                {
                    Log.Error("Error:\n" + nextChild.DisplayName + " Error happened, stop current script.");
                    return;
                }
            }
            //increment the currentIndex
            _currentIndex.Set(context, ++currentActivityIndex);
        }
    }
}