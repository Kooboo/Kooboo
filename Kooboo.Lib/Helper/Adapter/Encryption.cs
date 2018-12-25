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
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] bytes;
#if NETSTANDARD
            FromXmlString(rsa, publickey);
#else
            rsa.FromXmlString(publickey); 
#endif
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
#if NETSTANDARD
            FromXmlString(rsa,privatekey);
#else
            rsa.FromXmlString(privatekey);
#endif
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
#if NETSTANDARD
                sw.Write(ToXmlString(rsa, true));
#else
                sw.Write(rsa.ToXmlString(true));
#endif
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
#if NETSTANDARD
                sw.Write(ToXmlString(rsa,false));
#else
                sw.Write(rsa.ToXmlString(false));
#endif
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
#if NETSTANDARD
            keys.PublicKey = ToXmlString(rsa,false);
            keys.PrivateKey = ToXmlString(rsa, true);
#else
            keys.PublicKey =  rsa.ToXmlString(false);
            keys.PrivateKey = rsa.ToXmlString(true); 
#endif
            return keys;
        }

#if NETSTANDARD
        private static void FromXmlString(RSACryptoServiceProvider rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Exponent": parameters.Exponent = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "P": parameters.P = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "Q": parameters.Q = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DP": parameters.DP = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "DQ": parameters.DQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "InverseQ": parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                        case "D": parameters.D = (string.IsNullOrEmpty(node.InnerText) ? null : Convert.FromBase64String(node.InnerText)); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        private static String ToXmlString(RSACryptoServiceProvider rsa, bool includePrivateParameters)
        {

            // we extend appropriately for private components
            RSAParameters rsaParams = rsa.ExportParameters(includePrivateParameters);
            StringBuilder sb = new StringBuilder();
            sb.Append("<RSAKeyValue>");
            // Add the modulus
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            // Add the exponent
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");
            if (includePrivateParameters)
            {
                // Add the private components
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }
            sb.Append("</RSAKeyValue>");
            return (sb.ToString());
        }
#endif
    }

    public class RsaKeys
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
