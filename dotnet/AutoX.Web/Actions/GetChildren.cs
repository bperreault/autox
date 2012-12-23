// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Activities;
using System.Collections.Generic;
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
            string commandStr = Utils.GetContextValue(context, "command");

            try
            {
                XElement content = XElement.Parse(commandStr);
                string guid = content.GetAttributeValue("GUID");
                List<IDataObject> list = DBManager.GetInstance().FindDataFromDB(guid);

                XElement rElement = XElement.Parse("<Children />");
                foreach (IDataObject dataObject in list)
                {
                    XElement element = dataObject.GetXElementFromDataObject();

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
                Logger.GetInstance().Log().Debug("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
            }
            Utils.SetFailedReturnMessage(context, "Unknown reason, please check GetById.cs");
        }
    }
}