#region

using System;
using System.Reflection;
using AutoX.Basic;
using AutoX.Basic.Model;

#endregion

namespace AutoX.Test
{
    internal class Program
    {
        private static void Main(string[] args)
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
            Console.Read();
        }
    }
}