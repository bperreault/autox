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

#endregion

namespace AutoX.Activities.AutoActivities
{
    public abstract class AutomationActivity : NativeActivity, INotifyPropertyChanged
    {
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

        protected void SetResult(XElement result)
        {
            result.SetAttributeValue("_parentId",ParentResultId);
            result.SetAttributeValue("_id",ResultId);
            result.SetAttributeValue("InstanceId",InstanceId);
            result.SetAttributeValue("_type","Result");
            result.SetAttributeValue("Name",GetType().Name);
            result.SetAttributeValue("ScriptId",Id);
            Data.Save(result);
        }

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