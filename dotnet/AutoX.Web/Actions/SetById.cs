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
    public class SetById : NativeActivity
    {
        protected override void Execute(NativeActivityContext context)
        {
            var commandStr = Utils.GetContextValue(context, "command");

            try
            {
                var content = XElement.Parse(commandStr);
                var node = XElement.Parse(content.FirstNode.ToString());
                var iDataObject = node.GetDataObjectFromXElement();
                var parentId = node.GetAttributeValue("ParentId");
                var guid = node.GetAttributeValue("GUID");
                if (parentId != null && parentId.Equals("Deleted"))
                {
                    if (guid.StartsWith("00"))
                    {
                        Utils.SetFailedReturnMessage(context,
                                                     "All item with Id start with '0' cannot be deleted.\nWe assume it is an important root item.");
                        return;
                    }
                    DBManager.GetInstance().DeleteOneDataFromDB(guid);
                }

                else
                {
                    DBManager.GetInstance().AddOrUpdateOneDataToDB(guid, parentId, iDataObject);
                }


                Utils.SetSuccessReturnMessage(context);
                return;
            }
            catch (Exception ex)
            {
                Log.Error("we receive invalid command string:\n" + commandStr + "\n" + ex.Message);
                Utils.SetFailedReturnMessage(context, "SetById failed.\n" + ex.Message);
                return;
            }
            //Utils.SetFailedReturnMessage(context, "Unknown reason, please check SetById.cs");
        }
    }
}