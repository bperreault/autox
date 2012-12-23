// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Activities;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX.Web.Actions
{
    public class StartInstance : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            string commandStr = Utils.GetContextValue(context, "command");

            try
            {
                XElement content = XElement.Parse(commandStr);
                string guid = content.GetAttributeValue("GUID");
                InstanceManager.GetInstance().GetTestInstance(guid).Start();
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Log().Debug("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
                Utils.SetFailedReturnMessage(context, "Exception:\n" + ex.Message);
                return;
            }
            Utils.SetSuccessReturnMessage(context);
        }
    }
}