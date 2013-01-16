#region

using System;
using System.IO;
using AutoX.Basic;
using AutoX.DB;
using System.Xml.Linq;

#endregion


namespace AutoX.Test
{
    internal class Program
    {
        private static void Main()
        {
            InitProject();
            Console.Read();
        }

        private static void InitProject()
        {
            TestGenerateKey("yazhi.pang");
            
        }

        private static void TestGenerateKey(string userName)
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;
            XElement root = new XElement("Root");
            root.SetAttributeValue("_id", Guid.NewGuid().ToString());
            AsymmetricEncryption.GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);
            Console.WriteLine("public key:" + publicKey);
            Console.WriteLine("public & private key:" + publicAndPrivateKey);
            root.SetAttributeValue("PublicKey",publicKey);
            root.SetAttributeValue("PublicAndPrivateKey",publicAndPrivateKey);
            var productid = AsymmetricEncryption.GetProductId();
            root.SetAttributeValue("ProductId",productid);
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