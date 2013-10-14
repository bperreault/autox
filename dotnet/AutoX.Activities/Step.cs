#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities
{
    public class Step
    {
        public string _id { get; set; }
        public string UIObject { get; set; }
        public string XPath { get; set; }
        public string UIId { get; set; }
        public string Action { get; set; }
        public ArrayList PossibleAction { get; set; }
        public ArrayList PossibleData { get; set; }
        public string Data { get; set; }
        public string DefaultData { get; set; }
        public bool Enable { get; set; }

        public XElement ToXElement()
        {
            var element = new XElement(Constants.STEP);
            element.SetAttributeValue(Constants._ID, _id);
            element.SetAttributeValue(Constants.UI_OBJECT, UIObject);
            element.SetAttributeValue(Constants.XPATH, XPath);
            element.SetAttributeValue(Constants.UI_ID, UIId);
            element.SetAttributeValue(Constants.ACTION, Action);
            element.SetAttributeValue(Constants.DATA, Data);
            element.SetAttributeValue(Constants.DEFAULT_DATA, DefaultData);
            element.SetAttributeValue(Constants.ENABLE, Enable);
            return element;
        }
    }
}