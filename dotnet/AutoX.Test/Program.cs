#region

using System;
using System.IO;
using AutoX.Basic;
using AutoX.DB;

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
            //TestGenerateKey();
            DBManager.GetInstance();
        }

        private static void TestGenerateKey()
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;

            AsymmetricEncryption.GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);
            Console.WriteLine("public key:" + publicKey);
            Console.WriteLine("public & private key:" + publicAndPrivateKey);
            var productid = AsymmetricEncryption.GetProductId();
            string userName = "jien.huang";
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
//            string signature = AsymmetricEncryption.Sign(text,keySize,publicAndPrivateKey);
//            Console.WriteLine("Signature:"+signature);
//            Console.WriteLine("Verify Signature:" + AsymmetricEncryption.VerifySign(signature, text, keySize, publicKey));
//            //create default root id for Project, Result, Data, Object

            //get user name

            //use productid as connection string password

            Configuration.Set("UserName", userName);
            Configuration.Set("PublicKey", publicKey);
            Configuration.SaveSettings();
        }
    }
}