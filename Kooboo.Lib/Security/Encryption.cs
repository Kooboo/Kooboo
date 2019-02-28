//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Xml;

namespace Kooboo.Lib.Security
{ 
    public static class Encryption
    {
        public static string Encrypt(string input, string key)
        {
            byte[] inputArray =  Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = GetKeyBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7; 
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static byte[] GetKeyBytes(string key)
        {
            byte[] result = new byte[24];
            if (!string.IsNullOrEmpty(key))
            {
                var bytes = Encoding.UTF8.GetBytes(key);
                if (bytes.Length >24)
                {
                    System.Buffer.BlockCopy(bytes, 0, result, 0, 24); 
                }
                else
                {
                    System.Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
                }
            }

            return result; 
            
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = GetKeyBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Encoding.UTF8.GetString(resultArray);
        }
    }

    public class RSAEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey,string content)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RSAEncrypt(publickey, content);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey,string content)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RSADecrypt(privatekey, content);
            
        }

        /// <summary>
        /// Generates 2 XML files (public and private key) 
        /// </summary> 
        /// <param name="privateKeyPath">RSA private key file path</param> 
        /// <param name="publicKeyPath">RSA private key file path</param> /
        // <param name="size">secure size must be above 512</param> 
        public static void GenerateRsa(string privateKeyPath, string publicKeyPath, int size)
        {
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GenerateRsa(privateKeyPath, publicKeyPath, size);
        }

        public static RsaKeys GenerateKeys(int size=512)
        {
           return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GenerateKeys(size);
        }
    }

    public class RsaKeys
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
