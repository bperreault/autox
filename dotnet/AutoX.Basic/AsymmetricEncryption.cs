#region

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Microsoft.Win32;

#endregion

namespace AutoX.Basic
{
    public static class AsymmetricEncryption
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;

        public static bool GenerateRegisterFile()
        {
            var userName = Configuration.Settings("UserName");
            var collectionName = Configuration.Settings("ProjectName", "Project_" + userName.Replace(".", ""));
            return !string.IsNullOrEmpty(userName) && GenerateRegisterFile(userName, collectionName);
        }

        public static bool GenerateRegisterFile(string userName, string collectionName)
        {
            const int keySize = 2048;
            string publicAndPrivateKey;
            string publicKey;
            if (!string.IsNullOrEmpty(Configuration.Settings("PublicKey")))
                return false;
            GenerateKeys(keySize, out publicKey, out publicAndPrivateKey);

            var productid = GetProductId();

            var text = userName + productid;
            var encrypted = EncryptText(text, keySize, publicKey);

            //send encrypted data to service
            var forSave = new XElement("Register");
            forSave.SetAttributeValue("ProjectName", collectionName);
            var root = new XElement("Root");

            var rootId = Guid.NewGuid().ToString();
            var projectId = Guid.NewGuid().ToString();
            var uiId = Guid.NewGuid().ToString();
            var dataId = Guid.NewGuid().ToString();
            var translationId = Guid.NewGuid().ToString();
            var resultId = Guid.NewGuid().ToString();

            root.SetAttributeValue(Constants._ID, rootId);
            root.SetAttributeValue("Project", projectId);
            root.SetAttributeValue("UI", uiId);
            root.SetAttributeValue(Constants.DATA, dataId);
            root.SetAttributeValue("Translation", translationId);
            root.SetAttributeValue(Constants.RESULT, resultId);
            root.SetAttributeValue("PublicKey", publicKey);
            root.SetAttributeValue("PublicAndPrivateKey", publicAndPrivateKey);
            root.SetAttributeValue("Secret", encrypted);
            root.SetAttributeValue("UserName", userName);
            forSave.Add(root);

            //            File.WriteAllText(userName + ".pem", "UserName:\n" + userName + "\nPublic Key:\n" + publicKey + "\nPublic and Private Key:\n" +
            //                            publicAndPrivateKey + "\nSecrect:\n" + encrypted + "\nFor Test:\n" + productid);
            forSave.Add(
                XElement.Parse("<Project _type='Folder' Name='Project' _id='" + projectId + "' _parentId='" + rootId +
                               "' />"));
            forSave.Add(
                XElement.Parse("<Data  _type='Folder' Name='Data' _id='" + dataId + "' _parentId='" + rootId + "'  />"));
            forSave.Add(XElement.Parse("<UI  _type='Folder' Name='UI' _id='" + uiId + "' _parentId='" + rootId + "'  />"));
            forSave.Add(
                XElement.Parse("<Translation  Name='Translation' _id='" + translationId + "' _parentId='" + rootId +
                               "'  />"));
            forSave.Add(
                XElement.Parse("<Result  _type='Folder' Name='Result' _id='" + resultId + "' _parentId='" + rootId +
                               "'  />"));
            File.WriteAllText(userName + ".pem", forSave.ToString());
            Configuration.Set("UserName", userName);
            Configuration.Set("PublicKey", publicKey);
            Configuration.Set("ProjectName", collectionName);
            Configuration.SaveSettings();
            return true;
        }

        public static string Hmacmd5(string key, string sessionId)
        {
            var md5 = new HMACMD5(Encoding.UTF8.GetBytes(key));
            return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sessionId))).Replace("-", "").ToLower();
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
            var data = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(text)));
            var decrypted = Sign(data, keySize, publicAndPrivateKeyXml);
            return Convert.ToBase64String(decrypted);

            //return Encoding.UTF8.GetString(decrypted);
        }

        public static byte[] Sign(byte[] data, int keySize, string publicAndPrivateKeyXml)
        {
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml))
                throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

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
            var data = Convert.FromBase64String(signature);
            if (data == null || data.Length == 0) throw new ArgumentException("Data are empty", "signature");

            // int maxLength = GetMaxDataLength(keySize);
            // if (data.Length > maxLength) throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            //if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);

                SHA1 sha1 = new SHA1Managed();
                var buffer = sha1.ComputeHash(sha1.ComputeHash(Encoding.UTF8.GetBytes(origin)));
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
            var maxLength = GetMaxDataLength(keySize);
            if (data.Length > maxLength)
                throw new ArgumentException(String.Format("Maximum data length is {0}", maxLength), "data");
            if (!IsKeySizeValid(keySize)) throw new ArgumentException("Key size is not valid", "keySize");
            if (String.IsNullOrEmpty(publicKeyXml)) throw new ArgumentException("Key is null or empty", "publicKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicKeyXml);
                return provider.Encrypt(data, OptimalAsymmetricEncryptionPadding);
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
            if (String.IsNullOrEmpty(publicAndPrivateKeyXml))
                throw new ArgumentException("Key is null or empty", "publicAndPrivateKeyXml");

            using (var provider = new RSACryptoServiceProvider(keySize))
            {
                provider.FromXmlString(publicAndPrivateKeyXml);

                return provider.Decrypt(data, OptimalAsymmetricEncryptionPadding);
            }
        }

        public static int GetMaxDataLength(int keySize)
        {
            return ((keySize - 384)/8) + 37;
        }

        public static bool IsKeySizeValid(int keySize)
        {
            return keySize >= 384 &&
                   keySize <= 16384 &&
                   keySize%8 == 0;
        }

        public static string GetProductId()
        {
            using (var openSubKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                if (openSubKey != null)
                {
                    var digitId = openSubKey.GetValue("DigitalProductId") as byte[];
                    if (digitId == null) return null;
                    var productid = Convert.ToBase64String(digitId);
                    return productid;
                }
            }
            return null;
        }
    }
}