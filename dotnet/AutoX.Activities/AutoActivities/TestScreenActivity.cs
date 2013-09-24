#region

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
            //add validation to this activity:every enabled steps must have action
            var stepsX = XElement.Parse(_steps);
            foreach(var step in stepsX.Descendants("Step"))
            {
                var enabled = step.GetAttributeValue("Enable");
                if (string.IsNullOrEmpty(enabled))
                    continue;
                var action = step.GetAttributeValue("Action");
                if (string.IsNullOrEmpty(action))
                    return "Enabled step must has an action";
            }
            return null;
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