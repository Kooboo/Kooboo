using System.Security.Cryptography;
using System.Text;

namespace Kooboo.Lib.Helper;

public static class SignHelper
{
    public static string HexEncode(byte[] raw)
    {
        if (raw == null)
        {
            return string.Empty;
        }

        StringBuilder stringBuilder = new StringBuilder(raw.Length * 2);
        for (int i = 0; i < raw.Length; i++)
        {
            stringBuilder.Append(raw[i].ToString("x2"));
        }

        return stringBuilder.ToString();
    }

    public static byte[] HmacSHA256SignByBytes(string stringToSign, byte[] secret)
    {
        using KeyedHashAlgorithm keyedHashAlgorithm = CryptoConfig.CreateFromName("HMACSHA256") as KeyedHashAlgorithm;
        keyedHashAlgorithm.Key = secret;
        return keyedHashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToSign.ToCharArray()));
    }
}
