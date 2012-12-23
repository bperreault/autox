// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Activities;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace AutoX.Web.Actions
{
    public static class Utils
    {
        public static string GetContextValue(NativeActivityContext context, string name)
        {
            WorkflowDataContext dc = context.DataContext;
            return (from object p in dc.GetProperties()
                    where ((PropertyDescriptor) p).Name.Equals(name)
                    select ((PropertyDescriptor) p).GetValue(dc)
                    into value where value != null select value.ToString()).FirstOrDefault();
        }

        public static bool SetContextValue(NativeActivityContext context, string name, string value)
        {
            WorkflowDataContext dc = context.DataContext;
            foreach (object p in dc.GetProperties())
            {
                if (!((PropertyDescriptor) p).Name.Equals(name)) continue;
                ((PropertyDescriptor) p).SetValue(dc, value);
                return true;
            }
            return false;
        }

        public static void SetReturnMessage(NativeActivityContext context, string message)
        {
            SetContextValue(context, "returnMessage", message);
        }

        public static void SetReturnMessage(NativeActivityContext context, XElement r)
        {
            SetContextValue(context, "returnMessage", r.ToString());
        }

        //RULE: feed back raw message
        public static void SetIdleReturnMessage(NativeActivityContext context)
        {
            XElement r = XElement.Parse(@"<Steps> <Step Data='17' Action='AutoX.Client.Wait' /> </Steps>");
            SetContextValue(context, "returnMessage", r.ToString());
        }

        public static void SetSuccessReturnMessage(NativeActivityContext context)
        {
            SetContextValue(context, "returnMessage", "<Result Result='Success' />");
        }

        public static void SetFailedReturnMessage(NativeActivityContext context, string reason)
        {
            SetContextValue(context, "returnMessage", "<Result Result='Failed' Reason='" + reason + "' />");
        }

        public static void SetEmptyReturnMessage(NativeActivityContext context)
        {
            XElement r = XElement.Parse(@"<Steps> <Step Data='1' Action='AutoX.Client.Wait' /> </Steps>");
            SetContextValue(context, "returnMessage", r.ToString());
        }
    }
}