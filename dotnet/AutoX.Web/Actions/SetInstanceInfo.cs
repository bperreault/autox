// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Activities;
using System.Xml.Linq;

#endregion

namespace AutoX.Web.Actions
{
    public class SetInstanceInfo : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            string commandStr = Utils.GetContextValue(context, "command");

            try
            {
                XElement content = XElement.Parse(commandStr);
                XElement first = content.Element("AutoX.Basic.Model.Instance");

                if (InstanceManager.GetInstance().UpdateInstance(first))
                {
                    Utils.SetSuccessReturnMessage(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                Utils.SetFailedReturnMessage(context, ex.Message);
            }
            //Utils.SetFailedReturnMessage(context, "Not sure why...");
        }
    }
}