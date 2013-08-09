#region

// Hapa Project, CC
// Created @2012 09 18 15:26
// Last Updated  by Huang, Jien @2012 09 18 15:26

#region

using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.DB;

#endregion

#endregion

namespace AutoX.Activities.AutoActivities
{
    public abstract class AutomationActivity : NativeActivity, INotifyPropertyChanged
    {
        private const string SCRIPT_ID = "ScriptId";
        private readonly Collection<Variable> _variables = new Collection<Variable>();
        protected IHost Host = null;

        protected string ParentResultId;
        protected string ResultId;
        private bool _enabled = true;
        protected bool _runningResult = true;
        
        protected Dictionary<string, string> _upperVariables = new Dictionary<string, string>();
        protected XElement Result;

        [Browsable(false)]
        public string InstanceId { get; set; }

        public string Name { get; set; }

        [Browsable(false)]
        public Collection<Variable> Variables
        {
            get { return _variables; }
        }

        protected string _author;
        [DisplayName(@"Authors")]
        public string Authors
        {
            get { return _author; }
            set
            {
                if (string.IsNullOrEmpty(_author)) _author = value;
                else if(!_author.Contains(value)) _author = _author +","+ value ;
            }
        }

        [DisplayName(@"Enabled")]
        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                NotifyPropertyChanged("Enabled");
            }
        }

        [DisplayName(@"Local Data First")]
        public bool OwnDataFirst { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void SetParentResultId(string parentResultId)
        {
            if (string.IsNullOrEmpty(parentResultId))
            {
                Log.Error("Pass a null parent result id!");
                return;
            }
            ParentResultId = parentResultId;
            ResultId = ResultId ?? Guid.NewGuid().ToString();
        }

        public void SetVariables(Dictionary<string, string> vars)
        {
            foreach (string key in vars.Keys)
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
            foreach (string key in _upperVariables.Keys)
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
            var value = input.GetValue(context.DataContext);
            return value != null ? value.ToString() : null;
        }

        protected bool SetVariableValueByContext(NativeActivityContext context, string key, string value)
        {
            var input = context.DataContext.GetProperties()[key];
            if (input == null) return false;
            input.SetValue(context.DataContext, value);
            return true;
        }

        protected void SetResult()
        {
            
            Result.SetAttributeValue(Constants.PARENT_ID, ParentResultId);
            Result.SetAttributeValue(Constants._ID, ResultId);
            Result.SetAttributeValue(Constants.INSTANCE_ID, InstanceId);
            Result.SetAttributeValue(Constants._TYPE, Constants.RESULT);
            Result.SetAttributeValue(Constants.NAME, DisplayName + " " + DateTime.Now.ToUniversalTime());
            Result.SetAttributeValue(SCRIPT_ID, Id);
            //var ret = _result.GetAttributeValue(Constants.RESULT);
            //if (!string.IsNullOrEmpty(ret))
            //{
            //    _result.SetAttributeValue("Original", ret);
            //    _result.SetAttributeValue("Final", ret);
            //    _runningResult = ret.Equals(Constants.SUCCESS) && _runningResult;
            //}
            //else
            //    _runningResult = false;
            //result.SetAttributeValue(Constants.UI_OBJECT, UIObject);
            DBFactory.GetData().Save(Result);
        }

        protected void SetFinalResult()
        {
            var ret = Constants.SUCCESS;
            if (!_runningResult)
                ret = Constants.ERROR;
            Result.SetAttributeValue("Original", ret);
            Result.SetAttributeValue("Final", ret);
            DBFactory.GetData().Save(Result);

        }

        public void SetHost(IHost host)
        {
            Host = host;
        }


        /// <summary>
        ///   you must call this method after workflowinvoker.invoke
        /// </summary>
        /// <returns> </returns>
        public bool GetResult()
        {
            return _runningResult;
        }

        protected void NotifyPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}