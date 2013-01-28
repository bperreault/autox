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
    internal class ActionsFactory
    {
        public static XElement Execute(XElement steps,Browser browser,Config config)
        {
            var ret = new XElement("Result");
            var instanceId = steps.GetAttributeValue("InstanceId");
            var runtimeId = steps.GetAttributeValue("RunTimeId");
            var onError = steps.GetAttributeValue("OnError");
            if(!string.IsNullOrEmpty(onError))
                ret.SetAttributeValue("OnError",onError);
            if (!string.IsNullOrEmpty(instanceId))
                ret.SetAttributeValue("InstanceId", instanceId);
            if (!string.IsNullOrEmpty(runtimeId))
                ret.SetAttributeValue("RunTimeId", runtimeId);
            var query = from o in steps.Elements("Step")
                        select o;
            foreach (var step in query)
            {
                var xAttribute = step.Attribute("Action");
                var xId = step.GetAttributeValue("_id");
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
                    var startTime = DateTime.Now;
                    var result = CallAction(action, data, uiObj,browser,config);
                    var endTime = DateTime.Now;
                    result.SetAttributeValue("StartTime", startTime);
                    result.SetAttributeValue("EndTime", endTime);
                    result.SetAttributeValue("Duration", string.Format("{0:0.000}",(endTime.Ticks-startTime.Ticks)/10000000.00));
                    result.SetAttributeValue("InstanceId", instanceId);
                    CopyAttribute(result, step, "UIId");
                    CopyAttribute(result, step, "UIObject");
                    //result.SetAttributeValue("_id",xId);
                    ret.Add(result);
                }
            }

            return ret;
        }

        private static void CopyAttribute(XElement ret, XElement step, string attribteName)
        {
            if (!string.IsNullOrEmpty(step.GetAttributeValue(attribteName)))
                ret.SetAttributeValue(attribteName, step.GetAttributeValue(attribteName));
        }

        private static XElement CallAction(string action, string data, XElement uiObj,Browser browser,Config config)
        {
            var act = Type.GetType(action);
            if (act == null)
                return
                    XElement.Parse("<StepResult Action='" + action +
                                   "' Result='Error' Reason='Client does not support this action' />");
            dynamic actDyn = Activator.CreateInstance(act) as AbstractAction;
            actDyn.Browser = browser;
            actDyn.Config = config;

            return actDyn.Do(data, uiObj);
        }
    }
}