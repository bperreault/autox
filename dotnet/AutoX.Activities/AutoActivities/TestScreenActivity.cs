#region

// Hapa Project, CC
// Created @2012 09 18 14:34
// Last Updated  by Huang, Jien @2012 09 18 14:34

#region

using System.Activities;
using System.Activities.Presentation.PropertyEditing;
using System.ComponentModel;
using System.Drawing;

#endregion

#endregion

namespace AutoX.Activities.AutoActivities
{
    [ToolboxBitmap(typeof (TestScreenActivity), "TestScreen")]
    [Designer(typeof (TestScreenDesigner))]
    public sealed class TestScreenActivity : AutomationActivity
    {
        private string _name;
        private string _steps = "<Steps />";

        public TestScreenActivity()
        {
            Enabled = true;
        }

        [Browsable(false)]
        public string GUID { get; set; }

        public new string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                DisplayName = "Screen: " + _name;
                NotifyPropertyChanged("Steps");
            }
        }
        
        [DisplayName(@"Test Steps")]
        [Editor(typeof (StepsEditor), typeof (DialogPropertyValueEditor))]
        public string Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }

        protected override void Execute(NativeActivityContext context)
        {
            //It should not be called.
        }
    }
}