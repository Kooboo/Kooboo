using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

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
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] bytes;
            rsa.FromXmlString(publickey); 
            bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false); 
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey,string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(); 
            byte[] bytes;
            rsa.FromXmlString(privatekey);
            bytes = rsa.Decrypt(Convert.FromBase64String(content), false); 
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Generates 2 XML files (public and private key) 
        /// </summary> 
        /// <param name="privateKeyPath">RSA private key file path</param> 
        /// <param name="publicKeyPath">RSA private key file path</param> /
        // <param name="size">secure size must be above 512</param> 
        public static void GenerateRsa(string privateKeyPath, string publicKeyPath, int size)
        {
            //stream to save the keys
            FileStream fs = null;
            StreamWriter sw = null;

            //create RSA provider
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
      
            try
            {
                //save private key
                fs = new FileStream(privateKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(rsa.ToXmlString(true));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }

            try
            {
                //save public key
                fs = new FileStream(publicKeyPath, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.Write(rsa.ToXmlString(false));
                sw.Flush();
            }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
            rsa.Clear();
        }

        public static RsaKeys GenerateKeys(int size=512)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
            RsaKeys keys = new RsaKeys(); 
            keys.PublicKey =  rsa.ToXmlString(false);
            keys.PrivateKey = rsa.ToXmlString(true); 
            return keys;
        }
    }

    public class RsaKeys
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
