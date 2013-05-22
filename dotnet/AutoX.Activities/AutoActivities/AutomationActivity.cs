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
            ResultId = ResultId == null ? Guid.NewGuid().ToString() : ResultId;
        }
        public string Name { get; set; }
        Collection<Variable> variables = new Collection<Variable>();
        [Browsable(false)]
        public Collection<Variable> Variables
        {
            get
            {
                return this.variables;
            }
        }

        protected Dictionary<string, string> _upperVariables = new Dictionary<string, string>();
        public void SetVariables(Dictionary<string, string> vars)
        {
            foreach (var key in vars.Keys)
            {
                var value = vars[key];
                if (_upperVariables.ContainsKey(key))
                {
                    if (!OwnDataFirst)
                        _upperVariables[key] = value;
                }
                else
                {
                    _upperVariables.Add(key, value);
                }
            }
        }

        protected void SetVariablesBeforeRunning(NativeActivityContext context)
        {
            foreach (var key in _upperVariables.Keys)
            {
                var value = _upperVariables[key];
                if (ContainsVariableByContext(context, key))
                {
                    if (!OwnDataFirst)
                        SetVariableValueByContext(context, key, value);
                }
                else
                {
                    SetVariableValueByContext(context, key, value);
                }
            }
        }
        protected bool ContainsVariableByContext(NativeActivityContext context, string key)
        {
            var input = context.DataContext.GetProperties()[key];
            return input != null;
        }
        protected string GetVariableValueByContext(NativeActivityContext context, string key)
        {
            var input = context.DataContext.GetProperties()[key];

            if (input == null) return null;
            return input.GetValue(context.DataContext).ToString();
        }
        protected bool SetVariableValueByContext(NativeActivityContext context, string key, string value)
        {

            var input = context.DataContext.GetProperties()[key];
            if (input == null) return false;
            input.SetValue(context.DataContext, value);
            return true;
        }
        protected void SetResult(XElement result)
        {
            result.SetAttributeValue(Constants.PARENT_ID, ParentResultId);
            result.SetAttributeValue(Constants._ID, ResultId);
            result.SetAttributeValue(Constants.INSTANCE_ID, InstanceId);
            result.SetAttributeValue(Constants._TYPE, Constants.RESULT);
            result.SetAttributeValue(Constants.NAME, DisplayName + " " + DateTime.Now.ToUniversalTime());
            result.SetAttributeValue(SCRIPT_ID, Id);
            var ret = result.GetAttributeValue(Constants.RESULT);
            if (!string.IsNullOrEmpty(ret))
            {
                result.SetAttributeValue("Original", ret);
                result.SetAttributeValue("Final", ret);
                _result = ret.Equals("Success")&&_result;
            }
            else
                _result = false;
            //result.SetAttributeValue(Constants.UI_OBJECT, UIObject);
            DBFactory.GetData().Save(result);
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


        protected bool _result = true;
        /// <summary>
        ///   you must call this method after workflowinvoker.invoke
        /// </summary>
        /// <returns> </returns>
        public bool GetResult() { return _result; }

        protected void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
