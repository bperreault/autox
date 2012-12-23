// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Xml.Linq;

#endregion

namespace AutoX.Activities
{
    public class Step
    {
        public string UIObject { get; set; }
        public string UIId { get; set; }
        public string Action { get; set; }
        public ArrayList PossibleAction { get; set; }
        public ArrayList PossibleData { get; set; }
        public string Data { get; set; }
        public string DefaultData { get; set; }
        public bool Enable { get; set; }

        public XElement ToXElement()
        {
            var element = new XElement("Step");
            element.SetAttributeValue("UIObject", UIObject);
            element.SetAttributeValue("UIId", UIId);
            element.SetAttributeValue("Action", Action);
            element.SetAttributeValue("Data", Data);
            element.SetAttributeValue("DefaultData", DefaultData);
            element.SetAttributeValue("Enable", Enable);
            return element;
        }
    }
}