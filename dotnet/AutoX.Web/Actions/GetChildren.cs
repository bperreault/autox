// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Activities;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.Database;

#endregion

namespace AutoX.Web.Actions
{
    public class GetChildren : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            var commandStr = Utils.GetContextValue(context, "command");

            try
            {
                var content = XElement.Parse(commandStr);
                var guid = content.GetAttributeValue("GUID");
                var list = DBManager.GetInstance().FindDataFromDB(guid);

                var rElement = XElement.Parse("<Children />");
                foreach (IDataObject dataObject in list)
                {
                    var element = dataObject.GetXElementFromDataObject();

                    if (element != null)
                    {
                        element.SetAttributeValue("ParentId", guid);
                        element.DealExtra();
                        rElement.Add(element);
                    }
                }

                Utils.SetReturnMessage(context, rElement.ToString());
                return;
            }
            catch (Exception ex)
            {
                Log.Debug("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
            }
            Utils.SetFailedReturnMessage(context, "Unknown reason, please check GetById.cs");
        }
    }
}