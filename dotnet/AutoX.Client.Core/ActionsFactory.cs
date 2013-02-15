// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using System;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Client.Core
{
    internal class ActionsFactory
    {
        const string LINK = "Link";
        public static XElement Execute(XElement steps, Browser browser, Config config)
        {
            var ret = new XElement(Constants.RESULT);
		ret.SetAttributeValue(Constants.RESULT,"Success");
            ret.SetAttributeValue(Constants._ID,Guid.NewGuid().ToString());
            var instanceId = steps.GetAttributeValue(Constants.INSTANCE_ID);
            var runtimeId = steps.GetAttributeValue(Constants.RUNTIME_ID);
            var onError = steps.GetAttributeValue(Constants.ON_ERROR);
            string link = null;
            if (!string.IsNullOrEmpty(onError))
                ret.SetAttributeValue(Constants.ON_ERROR, onError);
            if (!string.IsNullOrEmpty(instanceId))
                ret.SetAttributeValue(Constants.INSTANCE_ID, instanceId);
            if (!string.IsNullOrEmpty(runtimeId))
                ret.SetAttributeValue(Constants.RUNTIME_ID, runtimeId);
            var setEnv = steps.Element(Constants.SET_ENV);
            if (setEnv != null)
            {
                foreach (var env in setEnv.Attributes())
                {
                    config.Set(env.Name.ToString(), env.Value);
                }
                return ret;
            }
            browser.DismissUnexpectedAlert();
            var query = from o in steps.Elements(Constants.STEP)
                        select o;
            foreach (var step in query)
            {
                var xAttribute = step.Attribute(Constants.ACTION);
                var xId = step.GetAttributeValue(Constants._ID);
                if (xAttribute != null)
                {
                    if(!HandleOneStep(browser, config,  ref ret, instanceId, link, step, xAttribute))
			break;
                }
            }
            return ret;
        }

	//return bool to show whether we need to continue the next steps
        private static bool HandleOneStep(Browser browser, Config config, ref XElement ret, string instanceId, string link, XElement step, XAttribute xAttribute)
        {
            var action = Configuration.Settings(xAttribute.Value, xAttribute.Value);
            var xData = step.Attribute(Constants.DATA);
            string data = null;
            if (xData != null)
                data = xData.Value;
            XElement uiObj = null;
            if (step.HasElements)
            {
                uiObj = step.Elements().First();
            }
            var startTime = DateTime.Now;
            var result = CallAction(action, data, uiObj, browser, config);
            var endTime = DateTime.Now;
            var currentLink = browser.GetResultLink();
            if (!string.IsNullOrEmpty(currentLink))
            {
                if (!currentLink.Equals(link))
                {
                    link = currentLink;
                    result.SetAttributeValue(LINK, link);
                }
            }
            result.SetAttributeValue("StartTime", startTime);
            result.SetAttributeValue("EndTime", endTime);
            result.SetAttributeValue("Duration", string.Format("{0:0.000}", (endTime.Ticks - startTime.Ticks) / 10000000.00));
            result.SetAttributeValue(Constants.INSTANCE_ID, instanceId);
            CopyAttribute(result, step, Constants.UI_ID);
            CopyAttribute(result, step, Constants.UI_OBJECT);

            //result.SetAttributeValue(Constants._ID,xId);
            ret.Add(result);
	if (!string.IsNullOrEmpty(link))
                ret.SetAttributeValue(LINK, link);
	var stepResult = result.GetAttributeValue(Constants.RESULT);
	var onError = ret.GetAttributeValue(Constants.ON_ERROR);
	if(!stepResult.Equals("Success")){
		if(onError.Equals("AlwaysReturnTrue")) return true;
		ret.SetAttribute(Constants.RESULT,"Failed");
		if(onError.Equals("StopCurrentScript")||onError.Equals("Terminate"))
			return false;
	}
	return true;
        }

        private static void CopyAttribute(XElement ret, XElement step, string attribteName)
        {
            if (!string.IsNullOrEmpty(step.GetAttributeValue(attribteName)))
                ret.SetAttributeValue(attribteName, step.GetAttributeValue(attribteName));
        }

        private static XElement CallAction(string action, string data, XElement uiObj, Browser browser, Config config)
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
