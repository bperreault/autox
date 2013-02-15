// Hapa Project, CC
// Created @2012 09 18 15:26
// Last Updated  by Huang, Jien @2012 09 18 15:26

#region

using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.DB;
using System.Collections.ObjectModel;

#endregion

namespace AutoX.Activities.AutoActivities
{
    public abstract class AutomationActivity : NativeActivity, INotifyPropertyChanged
    {
        const string SCRIPT_ID = "ScriptId";
        protected IHost Host = null;

        [Browsable(false)]
        public string InstanceId { get; set; }

        protected string ResultId;
        protected string ParentResultId;
        public void SetParentResultId(string parentResultId)
        {
            if (string.IsNullOrEmpty(parentResultId))
            {
                Log.Error("Pass a null parent result id!");
                return;
            }
            ParentResultId = parentResultId;
            ResultId = Guid.NewGuid().ToString();
        }
        public string Name { get; set; }
        Collection<Variable> variables = new Collection<Variable>();
        public Collection<Variable> Variables
        {
            get
            {
                return this.variables;
            }
        }

        protected void SetResult(XElement result)
        {
            result.SetAttributeValue(Constants.PARENT_ID,ParentResultId);
            result.SetAttributeValue(Constants._ID,ResultId);
            result.SetAttributeValue(Constants.INSTANCE_ID,InstanceId);
            result.SetAttributeValue(Constants._TYPE,Constants.RESULT);
            result.SetAttributeValue(Constants.NAME,DisplayName);
            result.SetAttributeValue(SCRIPT_ID, Id);
            //result.SetAttributeValue(Constants.UI_OBJECT, UIObject);
            Data.Save(result);
        }
        private bool _enabled = true;
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool Enabled { get { return _enabled; } set { _enabled = value; NotifyPropertyChanged("Enabled"); } }

        [DisplayName("Local Data First")]
        public bool OwnDataFirst { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void SetHost(IHost host)
        {
            Host = host;
        }



        /// <summary>
        ///   you must call this method after workflowinvoker.invoke
        /// </summary>
        /// <returns> </returns>
        public abstract bool GetResult();

        protected void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}