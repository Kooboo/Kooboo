using System;
using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class Sha1Hasher
    {
        public Sha1Hasher()
        {
        }

        public string HmacHash(string key, string message)
        {
            var hmac = new HMACSHA1(Sha1Bytes(key));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        public byte[] Sha1Bytes(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            return SHA1.Create().ComputeHash(data);
        }
    }
}