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
    public class SetResult : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            var commandStr = Utils.GetContextValue(context, "command");

            try
            {
                //put result to instance, the instance will trigger the workflow things
                var content = XElement.Parse(commandStr);
                var node = XElement.Parse(content.FirstNode.ToString());
                var instanceId = node.GetAttributeValue("InstanceId");
                InstanceManager.GetInstance().GetTestInstance(instanceId).SetResult(node);

                Utils.SetSuccessReturnMessage(context);
                return;
            }
            catch (Exception ex)
            {
                Log.Debug("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
                Utils.SetFailedReturnMessage(context, "SetResult failed.\n" + ex.Message);
                return;
            }
            //Utils.SetFailedReturnMessage(context, "Unknown reason, please check SetResult.cs");
        }
    }
}