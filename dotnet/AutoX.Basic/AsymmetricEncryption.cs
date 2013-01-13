using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace AutoX.Basic
{
    public static class AsymmetricEncryption
    {
        private static bool _optimalAsymmetricEncryptionPadding = false;

        public static bool GenerateRegisterFile()
        {
            string userName = Configuration.Settings("UserName");
            if (string.IsNullOrEmpty(userName))
                return false;
            return GenerateRegisterFile(userName);
        }
        public static bool GenerateRegisterFile(string userName)
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;
            if (!string.IsNullOrEmpty(Configuration.Settings("PublicKey")))
                return false;
            GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);
            
            var productid = GetProductId();
            
            string text = userName + productid;
            string encrypted = EncryptText(text, keySize, publicKey);

            //send encrypted data to service
            File.WriteAllText(userName + ".pem", "UserName:\n" + userName + "\nPublic Key:\n" + publicKey + "\nPublic and Private Key:\n" +
                            publicAndPrivateKey + "\nSecrect:\n" + encrypted+"\nFor Test:\n"+productid);
            
            Configuration.Set("UserName", userName);
            Configuration.Set("PublicKey", publicKey);
            Configuration.SaveSettings();
            return true;
        }

        public static void GenerateKeys(int keySize, out string publicKey, out string publicAndPrivateKey)
        {
            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                publicKey = provider.ToXmlString(false);
                publicAndPrivateKey = provider.ToXmlString(true);
            }
        }

        public static string Sign(string text, int keySize, string publicAndPrivateKeyXml)
        {
            
            SHA1 sha1 = new SHA1Managed();
            byte[] data = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(text)));
            var decrypted = Sign(data , keySize, publicAndPrivateKeyXml);
            return Convert.ToBase64String(decrypted);
            //return Encoding.UTF8.GetString(decrypted);
            
        }

        public static byte[] Sign(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                
                //Create a signature for HashValue and assign it to 
                //SignedHashValue.
                return provider.SignData(data, new SHA1CryptoServiceProvider());
//                return RSAFormatter.CreateSignature(data);
//
//                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }


        public static bool VerifySign(string signature, string origin, int keySize, string publicKeyXml)
        {
            byte[] data = Convert.FromBase64String(signature);
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
           // int maxLength = GetMaxDataLength(keySize);
           // if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            //if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                
                SHA1 sha1 = new SHA1Managed();
            byte[] buffer = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(origin)));
                return provider.VerifyData(buffer, new SHA1CryptoServiceProvider(), data);
                //return RSADeformatter.VerifySignature(Encoding.UTF8.GetBytes(origin), data);
                //return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static string EncryptText(string text, int keySize, string publicKeyXml)
        {
            var encrypted = Encrypt(Encoding.UTF8.GetBytes(text), keySize, publicKeyXml);
            return Convert.ToBase64String(encrypted);
        }

        public static byte[] Encrypt(byte[] data, int keySize, string publicKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            int maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        public static string DecryptText(string text, int keySize, string publicAndPrivateKeyXml)
        {
            var decrypted = Decrypt(Convert.FromBase64String(text), keySize, publicAndPrivateKeyXml);
            return Encoding.UTF8.GetString(decrypted);
        }

        public static byte[] Decrypt(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml)) throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);
                
                return provider.Decrypt(data, _optimalAsymmetricEncryptionPadding);
            }
        }

        
        public static int GetMaxDataLength(int keySize)
        {
            if (_optimalAsymmetricEncryptionPadding)
            {
                return ((keySize - 384) / 8) + 7;
            }
            return ((keySize - 384) / 8) + 37;
        }

        public static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 &&
                    keySize <= 16384 &&
                    keySize % 8 == 0;
        }

        public static string GetProductId()
        {
            string productid = Convert.ToBase64String(
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")
                        .GetValue("DigitalProductId") as byte[]);
            return productid;
        }
    }
}
