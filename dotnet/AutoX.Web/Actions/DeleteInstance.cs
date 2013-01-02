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
    public class DeleteInstance : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            var commandStr = Utils.GetContextValue(context, "command");

            try
            {
                var content = XElement.Parse(commandStr);

                var instanceId = content.GetAttributeValue("GUID");
                InstanceManager.GetInstance().RemoveTestInstance(instanceId);
            }
            catch (Exception ex)
            {
                Log.Debug("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
            }
            Utils.SetSuccessReturnMessage(context);
        }
    }
}