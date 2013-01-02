// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Linq;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.Client.Core
{
    public class ActionsFactory
    {
        public static XElement Execute(XElement steps)
        {
            var ret = new XElement("Result");
            var instanceId = steps.GetAttributeValue("InstanceId");
            var runtimeId = steps.GetAttributeValue("RunTimeId");
            if (!string.IsNullOrEmpty(instanceId))
                ret.SetAttributeValue("InstanceId", instanceId);
            if (!string.IsNullOrEmpty(runtimeId))
                ret.SetAttributeValue("RunTimeId", runtimeId);
            var query = from o in steps.Elements("Step")
                        select o;
            foreach (XElement step in query)
            {
                var xAttribute = step.Attribute("Action");
                if (xAttribute != null)
                {
                    var action = Configuration.Settings(xAttribute.Value, xAttribute.Value);
                    var xData = step.Attribute("Data");
                    string data = null;
                    if (xData != null)
                        data = xData.Value;
                    XElement uiObj = null;
                    if (step.HasElements)
                    {
                        uiObj = step.Elements().First();
                    }
                    var result = CallAction(action, data, uiObj);
                    ret.Add(result);
                }
            }

            return ret;
        }

        private static XElement CallAction(string action, string data, XElement uiObj)
        {
            var act = Type.GetType(action);
            if (act == null)
                return
                    XElement.Parse("<StepResult Action='" + action +
                                   "' Result='Error' Reason='Client does not support this action' />");
            dynamic actDyn = Activator.CreateInstance(act);
            return actDyn.Do(data, uiObj);
        }
    }
}