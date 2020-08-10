//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KScript
{
    public class Security
    {
        [Description(@"Compute a 32 length text string value
var input = ""myvalue"";  
    var md5value = k.security.md5(input); ")]
        public string md5(string input)
        {
            Guid id = Kooboo.Lib.Security.Hash.ComputeHashGuid(input);
            return id.ToString("N");
        }

        [Description(@"Compute a 40 length text string value
  var input = ""myvalue"";  
     var shavalue = k.security.sha1(input); ")]
        public string sha1(string input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();

            var hash = cryptoTransformSHA1.ComputeHash(bytes);

            return HexStringFromBytes(hash);
        }

        private string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        [Description(@"Two-way encryption
var input = ""myvalue""; 
     var key = ""hashkey"";  
     var encrptyValue = k.security.encrypt(input, key);   
     var decryptValue = k.security.decrypt(encrptyValue, key);")]
        public string encrypt(string input, string key)
        {
            return EncryptHelper.EncryptString(input, key);
        }

        [Description(@"Two-way encryption
var input = ""myvalue""; 
     var key = ""hashkey"";  
     var encrptyValue = k.security.encrypt(input, key);   
     var decryptValue = k.security.decrypt(encrptyValue, key);")]
        public string decrypt(string input, string key)
        {
            return EncryptHelper.DecryptString(input, key);
        }

        [Description("Convert to base64 format string")]
        public string ToBase64(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input); 
            return Convert.ToBase64String(bytes);
        }

        [Description("Convert from base64 string")]
        public string FromBase64(string base64string)
        {
            var bytes = Convert.FromBase64String(base64string);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
         
        [Description("Generate a new Guid")]
        public string NewGuid()
        {
           var id =  System.Guid.NewGuid();
            return id.ToString().Replace("-", ""); 
        }

        [Description("Generate a new Guid, encrypt to short length")]
        public string ShortGuid()
        {
            return Kooboo.Lib.Security.ShortGuid.GetNewShortId(); 
        }

    }


    public static class EncryptHelper
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        //Encrypt
        public static string EncryptString(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }
        //Decrypt
        public static string DecryptString(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }

}