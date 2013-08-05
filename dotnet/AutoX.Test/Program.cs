#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Client.Core;
using AutoX.DB;
using AutoX.WF.Core;

#endregion

namespace AutoX.Test
{
    internal class Program
    {
        private static readonly Dictionary<string, string> parameters = new Dictionary<string, string>();

        private static int Main(string[] args)
        {
            //Dictionary<string,string> parameters = new Dictionary<string,string>();

            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    parameters.Add(args[i], args[i + 1]);
                }
            }
            //if (parameters.ContainsKey("-C"))
            //{
            //    CleanProject();
            //}
            //if (parameters.ContainsKey("-R"))
            //{
            //    RemoveResults();
            //}
            if (parameters.ContainsKey("-G"))
            {
                //create a project for a new user
            }
            if (parameters.ContainsKey("-A"))
            {
                //add user to project
            }
            if (parameters.ContainsKey("-S"))
            {
                //run a test suite
                if (parameters.ContainsKey("-i"))
                {
                }
                else
                {
                    PrintUsage();
                }
            }
            if (parameters.ContainsKey("-F"))
            {
                //run a set of tests
                if (parameters.ContainsKey("-i"))
                {
                }
                else
                {
                    PrintUsage();
                }
            }
            if (parameters.Count <= 0)
                PrintUsage();

            //AsymmetricEncryption.GenerateRegisterFile("yazhi.pang", "autox");
            //CreateProject();
            //TestWorkflow();
            //Console.WriteLine(AsymmetricEncryption.Hmacmd5("autox:b3842073-5a7a-4782-abbc-e7234e09f8ac", "5f9fef27854ca50a3c132ce331cb6034"));
            //Console.Read();
            return 0;
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"Usage:");
            Console.WriteLine(@"	-C	Clean The Current Project");
            Console.WriteLine(@"	-R	Remove all Results from Current Project");
            Console.WriteLine(@"	-G	Create a Project for a new User, require -f");
            Console.WriteLine(@"	-A	Add a User to Project, require -p -u");
            Console.WriteLine(@"	-S	Run One Script, require -i");
            Console.WriteLine(@"	-F	Run a Set of Tests, require -i");
            Console.WriteLine(@"	-u	User Name");
            Console.WriteLine(@"	-p	Project Name");
            Console.WriteLine(@"	-f	File Name");
            Console.WriteLine(@"	-i	Id");
            Console.WriteLine(@"	-v	Version");
            Console.WriteLine(@"	-b	Build");
        }

        //private static void RemoveResults()
        //{
        //    //find root element
        //    var xRoot = DBFactory.GetData().Read(Constants._TYPE, "Root");
        //    var resultId = xRoot.GetAttributeValue(Constants.RESULT);
        //    var results = DBFactory.GetData().GetChildren(resultId);
        //    foreach (var kids in results.Descendants())
        //    {
        //        DBFactory.GetData().Delete(kids.GetAttributeValue(Constants._ID));
        //    }
        //}

        //private static void CleanProject()
        //{
        //    //find root element
        //    var xRoot = DBFactory.GetData().Read(Constants._TYPE, "Root");
        //    var root = DBFactory.GetData().GetChildren(xRoot.GetAttributeValue(Constants._ID));
        //    foreach (var kid in root.Descendants())
        //    {
        //        var id = kid.GetAttributeValue(Constants._ID);
        //        if (id.Equals(xRoot.GetAttributeValue(Constants._ID)))
        //            continue;
        //        foreach (var grandKid in DBFactory.GetData().GetChildren(id).Descendants())
        //        {
        //            DBFactory.GetData().Delete(grandKid.GetAttributeValue(Constants._ID));
        //        }
        //    }
        //}

        private static void TestWorkflow()
        {
            var auto = new AutoClient();
            var workflowInstance = new WorkflowInstance(Guid.NewGuid().ToString(),
                "7fdcbd7a-b30e-4c36-aa46-58ba74b02401", Configuration.Clone().GetList());
            var xCommand = workflowInstance.GetCommand();

            Console.WriteLine(xCommand.ToString());
            var xResult = auto.Execute(xCommand);
            Console.WriteLine(xResult);
            workflowInstance.SetResult(xResult);
        }

        private static void CreateProject()
        {
            //AsymmetricEncryption.GenerateRegisterFile("yazhi.pang", "autox");
            var fileContent = File.ReadAllText("yazhi.pang.pem");
            var forSake = XElement.Parse(fileContent);
            var projectName = forSake.GetAttributeValue("ProjectName");
            if (MongoDBManager.GetInstance().IsProjectExisted(projectName))
            {
                Console.WriteLine("Project already existed, continue?(y/n):");
                if (!Console.ReadKey().KeyChar.Equals('y'))
                    return;
            }
            MongoDBManager.GetInstance().SetProject(projectName);
            var root = forSake.Element("Root");
            var publicAndPrivateKey = root.GetAttributeValue("PublicAndPrivateKey");
            var secret = root.GetAttributeValue("Secret");
            var decrypted = AsymmetricEncryption.DecryptText(secret, 2048, publicAndPrivateKey);
            var userName = root.GetAttributeValue("UserName");

            foreach (XElement descendant in forSake.Descendants())
            {
                DBFactory.GetData().Save(descendant);
            }
        }

        private static void ForTempSave(string userName)
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;
            var root = new XElement("Root");
            root.SetAttributeValue(Constants._ID, Guid.NewGuid().ToString());
            AsymmetricEncryption.GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);
            Console.WriteLine(@"public key:" + publicKey);
            Console.WriteLine(@"public & private key:" + publicAndPrivateKey);
            root.SetAttributeValue("PublicKey", publicKey);
            root.SetAttributeValue("PublicAndPrivateKey", publicAndPrivateKey);
            var productid = AsymmetricEncryption.GetProductId();
            root.SetAttributeValue("ProductId", productid);

            //string userName = "jien.huang";
            var text = userName + productid;
            var encrypted = AsymmetricEncryption.EncryptText(text, keySize, publicKey);

            Console.WriteLine(@"Encrypted: {0}", encrypted);

            //send encrypted data to service
            File.WriteAllText(userName + ".pem",
                @"UserName:
" + userName + @"
Public Key:" + publicKey + @"
Public and Private Key:
" +
                publicAndPrivateKey + @"Secrect:
" + encrypted + @"
For Test:
" + productid);
            var decrypted = AsymmetricEncryption.DecryptText(encrypted, keySize, publicAndPrivateKey);

            //service person do below
            Console.WriteLine(@"Decrypted: {0}", decrypted);

            //            Configuration.Set("UserName", userName);
            //            Configuration.Set("PublicKey", publicKey);
            //            Configuration.SaveSettings();
            DBFactory.GetData().Save(root);
            Console.WriteLine(DBFactory.GetData().Read("a02cf4ad-ba0c-4c69-9f7c-e7d73a8fecad"));
        }
    }
}