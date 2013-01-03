#region

using System;
using System.Reflection;
using System.ServiceModel.Description;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.WF.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.Web.Services;

#endregion

namespace AutoX.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //JsonTest();
            ServiceTest();
            Console.Read();
        }

        private static void ServiceTest()
        {
            Uri address = new System.Uri("http://localhost/TestService");
            ServiceHost serviceHost = new ServiceHost(typeof(AutoX.WF.Core.Service),address);
            //serviceHost.AddServiceEndpoint(typeof(IService), new BasicHttpBinding(), "Soap");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            serviceHost.Description.Behaviors.Add(smb);
            serviceHost.Open();

            // The service can now be accessed.
            Console.WriteLine("The service is ready.");
            Console.WriteLine("Press <ENTER> to terminate service.");
            Console.ReadLine();

            // Close the ServiceHost.
            serviceHost.Close();
        }

        private static void JsonTest()
        {
            var ass = Assembly.LoadFrom("AutoX.Basic.dll");
            Log.Debug("Test Begin:");
            var testFolder = new Folder
                {
                    GUID = "guid",
                    Created = DateTime.Now,
                    Name = "TestFolder",
                    Description = "Description",
                    Updated = DateTime.Now
                };
            var jstring = testFolder.JsonSerialize();
            Log.Debug(jstring);

            var jobject = DataObjectExt.JsonDeserialize(jstring, ass.GetType("AutoX.Basic.Model.Folder"));

            Log.Debug(jobject.ToString());
        }
    }
}