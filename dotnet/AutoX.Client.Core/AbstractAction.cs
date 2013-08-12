#region

using System.Collections.ObjectModel;
using System.Xml.Linq;
using AutoX.Basic;
using OpenQA.Selenium;

#endregion

namespace AutoX.Client.Core
{
    public abstract class AbstractAction : IAction
    {
        public ReadOnlyCollection<IWebElement> UIObject { get; set; }

        public string Data { get; set; }

        //public string _id { get; set; }

        public Browser Browser { set; get; }

        public Config Config { set; get; }

        #region IAction Members

        public XElement Do(string data, XElement uiObj)
        {
            Data = data;
            FindUIObject(uiObj);
            return Act();
        }

        #endregion

        public void FindUIObject(XElement uiObj)
        {
            if (uiObj == null)
                return;
            
            UIObject = Browser.GetWebElement(uiObj);
        }

        public abstract XElement Act();
    }
}