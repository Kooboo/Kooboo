using System;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Sites.SMS.ChinaMobile.Helpers
{
    public class MD5Helper
    {
        /// <summary>
        /// Encrypt string to MD5 string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string Encrpty(string inputString)
        {
            using (MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(inputString);
                byte[] md5Hash = md5CryptoServiceProvider.ComputeHash(inputBytes);

                return BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
            };
        }
    }
}
