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
        private static void Main()
        {
            //AsymmetricEncryption.GenerateRegisterFile("yazhi.pang", "autox");
            //CreateProject();
            TestWorkflow();
            //Console.WriteLine(AsymmetricEncryption.Hmacmd5("autox:b3842073-5a7a-4782-abbc-e7234e09f8ac", "5f9fef27854ca50a3c132ce331cb6034"));
            Console.Read();
        }

        private static void TestWorkflow()
        {
            AutoClient auto  = new AutoClient();
            WorkflowInstance workflowInstance = new WorkflowInstance("7fdcbd7a-b30e-4c36-aa46-58ba74b02401", null);
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