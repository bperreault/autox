#region

using System;
using System.IO;
using AutoX.Basic;
using AutoX.DB;
using System.Xml.Linq;
using AutoX.WF.Core;
using AutoX.Client.Core;

#endregion


namespace AutoX.Test
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Dictionary<string,string> params = new Dictionary<string,string>();
            for (int i = 0 ; i<args.Length; i++)
            {
                if(args[i].StartsWith("-")){
                   params.Add(args[i],args[i+1]);
                }
            }
                    if(params.ContainKey("-C")){
                        CleanProject();
                    }
                    if (params.ContainKey("-R"))
                    {
                        RemoveResults();
                    }
                    if (params.ContainKey("-G"))
                    {
                        //create a project for a new user
                    }
                    if (params.ContainKey("-A"))
                    {
                        //add user to project
                    }
                    if (params.ContainKey("-S"))
                    {
                        //run a test suite
                        if(params.ContainKey("-i")){

                        }
                        else{
                            PrintUsage();
                        }
                    }
                    if (params.ContainKey("-F"))
                    {
                        //run a set of tests
                        if(params.ContainKey("-i")){

                        }
                        else{
                            PrintUsage();
                        }
                    }
            if(params.Length <= 0)
                PrintUsage();    
            //AsymmetricEncryption.GenerateRegisterFile("yazhi.pang", "autox");
            //CreateProject();
            //TestWorkflow();
            //Console.WriteLine(AsymmetricEncryption.Hmacmd5("autox:b3842073-5a7a-4782-abbc-e7234e09f8ac", "5f9fef27854ca50a3c132ce331cb6034"));
            //Console.Read();
            return 0;
        }

        private static void PrintUsage(){
            Console.WriteLine("Usage:");
            Console.WriteLine("\t-C\tClean The Current Project");
            Console.WriteLine("\t-R\tRemove all Results from Current Project");
            Console.WriteLine("\t-G\tCreate a Project for a new User, require -f");
            Console.WriteLine("\t-A\tAdd a User to Project, require -p -u");
            Console.WriteLine("\t-S\tRun One Script, require -i");
            Console.WriteLine("\t-F\tRun a Set of Tests, require -i");
            Console.WriteLine("\t-u\tUser Name");
            Console.WriteLine("\t-p\tProject Name");
            Console.WriteLine("\t-f\tFile Name");
            Console.WriteLine("\t-i\tId");
            Console.WriteLine("\t-v\tVersion");
            Console.WriteLine("\t-b\tBuild");
        }

        private static void RemoveResults()
        {
            //find root element
            var xRoot = Data.Read("_type", "Root");
            var resultId = xRoot.GetAttributeValue("Result");
            var results = Data.GetChildren(resultId);
            foreach (var kids in results.Descendants())
            {
                Data.Delete(kids.GetAttributeValue("_id"));
            }
        }

        private static void CleanProject()
        {
            //find root element
            var xRoot = Data.Read("_type", "Root");
            var root = Data.GetChildren(xRoot.GetAttributeValue("_id"));
            foreach (var kid in root.Descendants())
            {
                var id = kid.GetAttributeValue("_id");
                if(id.Equals(xRoot.GetAttributeValue("_id")))
                    continue;
                foreach (var grandKid in Data.GetChildren(id).Descendants())
                {
                    Data.Delete(grandKid.GetAttributeValue("_id"));
                }
            }
        }

        private static void TestWorkflow()
        {
            AutoClient auto  = new AutoClient();
            WorkflowInstance workflowInstance = new WorkflowInstance("7fdcbd7a-b30e-4c36-aa46-58ba74b02401", null, "8afc65e2-fd1f-4cf0-8e20-337c40c27912");
            var xCommand = workflowInstance.GetCommand();
            
            Console.WriteLine(xCommand.ToString());
            var xResult = auto.Execute(xCommand);
            Console.WriteLine(xResult);
            workflowInstance.SetResult(xResult);
        }

        private static void CreateProject()
        {
            //AsymmetricEncryption.GenerateRegisterFile("yazhi.pang", "autox");
            string fileContent = File.ReadAllText("yazhi.pang.pem");
            XElement forSake = XElement.Parse(fileContent);
            string projectName = forSake.GetAttributeValue("ProjectName");
            if (DBManager.GetInstance().IsProjectExisted(projectName))
            {
                Console.WriteLine("Project already existed, continue?(y/n):");
                if(!Console.ReadKey().KeyChar.Equals('y'))
                    return;
            }
            DBManager.GetInstance().SetProject(projectName);
            var root = forSake.Element("Root");
            string publicAndPrivateKey = root.GetAttributeValue("PublicAndPrivateKey");
            string secret = root.GetAttributeValue("Secret");
            string decrypted = AsymmetricEncryption.DecryptText(secret, 2048, publicAndPrivateKey);
            string userName = root.GetAttributeValue("UserName");
            DBManager.GetInstance().AddUser(userName, decrypted);
            foreach (var descendant in forSake.Descendants())
            {
                Data.Save(descendant);
            }
        }

        private static void ForTempSave(string userName)
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;
            XElement root = new XElement("Root");
            root.SetAttributeValue("_id", Guid.NewGuid().ToString());
            AsymmetricEncryption.GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);
            Console.WriteLine("public key:" + publicKey);
            Console.WriteLine("public & private key:" + publicAndPrivateKey);
            root.SetAttributeValue("PublicKey", publicKey);
            root.SetAttributeValue("PublicAndPrivateKey", publicAndPrivateKey);
            var productid = AsymmetricEncryption.GetProductId();
            root.SetAttributeValue("ProductId", productid);
            //string userName = "jien.huang";
            string text = userName + productid;
            string encrypted = AsymmetricEncryption.EncryptText(text, keySize, publicKey);

            Console.WriteLine("Encrypted: {0}", encrypted);
            //send encrypted data to service
            File.WriteAllText(userName + ".pem",
                              "UserName:\n" + userName + "\nPublic Key:" + publicKey + "\nPublic and Private Key:\n" +
                              publicAndPrivateKey + "Secrect:\n" + encrypted + "\nFor Test:\n" + productid);
            string decrypted = AsymmetricEncryption.DecryptText(encrypted, keySize, publicAndPrivateKey);

            //service person do below
            Console.WriteLine("Decrypted: {0}", decrypted);

//            Configuration.Set("UserName", userName);
//            Configuration.Set("PublicKey", publicKey);
//            Configuration.SaveSettings();
            Data.Save(root);
            Console.WriteLine(Data.Read("a02cf4ad-ba0c-4c69-9f7c-e7d73a8fecad"));
        }
    }
}