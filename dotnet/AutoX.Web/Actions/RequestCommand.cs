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
    public class RequestCommand : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            var commandStr = Utils.GetContextValue(context, "command");
            Log.Debug(commandStr);
            try
            {
                var content = XElement.Parse(commandStr);
                var xAttribute = content.Attribute("ComputerName");
                if (xAttribute != null)
                {
                    var computerName = xAttribute.Value;
                    var retMessage = ComputersManager.GetInstance().GetComputer(computerName).GetCommand();
                    Utils.SetReturnMessage(context, retMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("we receive invalid command string or ComputerName is NULL:\n" +
                                                 commandStr + "\n" + ex.Message);
            }
            Utils.SetIdleReturnMessage(context);
        }
    }
}