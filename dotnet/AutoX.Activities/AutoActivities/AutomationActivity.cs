// Hapa Project, CC
// Created @2012 09 18 15:26
// Last Updated  by Huang, Jien @2012 09 18 15:26

#region

using System.Activities;
using System.ComponentModel;
using AutoX.Basic;

#endregion

namespace AutoX.Activities.AutoActivities
{
    public abstract class AutomationActivity : NativeActivity, INotifyPropertyChanged
    {
        protected IHost Host = null;

        [Browsable(false)]
        public string InstanceId { get; set; }

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