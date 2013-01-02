// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections.ObjectModel;
using System.Xml.Linq;
using OpenQA.Selenium;

#endregion

namespace AutoX.Client.Core
{
    public abstract class AbstractAction : IAction
    {
        public ReadOnlyCollection<IWebElement> UIObject { get; set; }
        public string Data { get; set; }

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
            UIObject = Browser.GetInstance().GetWebElement(uiObj);
        }


        public abstract XElement Act();
    }
}