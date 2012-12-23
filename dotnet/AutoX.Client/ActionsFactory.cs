// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.Client
{
    internal class ActionsFactory
    {
        internal static XElement Execute(XElement steps)
        {
            var ret = new XElement("Result");
            string instanceId = steps.GetAttributeValue("InstanceId");
            string runtimeId = steps.GetAttributeValue("RunTimeId");
            if (!string.IsNullOrEmpty(instanceId))
                ret.SetAttributeValue("InstanceId", instanceId);
            if (!string.IsNullOrEmpty(runtimeId))
                ret.SetAttributeValue("RunTimeId", runtimeId);
            IEnumerable<XElement> query = from o in steps.Elements("Step")
                                          select o;
            foreach (XElement step in query)
            {
                XAttribute xAttribute = step.Attribute("Action");
                if (xAttribute != null)
                {
                    string action = Configuration.Settings(xAttribute.Value, xAttribute.Value);
                    XAttribute xData = step.Attribute("Data");
                    string data = null;
                    if (xData != null)
                        data = xData.Value;
                    XElement uiObj = null;
                    if (step.HasElements)
                    {
                        uiObj = step.Elements().First();
                    }
                    XElement result = CallAction(action, data, uiObj);
                    ret.Add(result);
                }
            }

            return ret;
        }

        private static XElement CallAction(string action, string data, XElement uiObj)
        {
            Type act = Type.GetType(action);
            if (act == null)
                return
                    XElement.Parse("<StepResult Action='" + action +
                                   "' Result='Error' Reason='Client does not support this action' />");
            dynamic actDyn = Activator.CreateInstance(act);
            return actDyn.Do(data, uiObj);
        }
    }
}