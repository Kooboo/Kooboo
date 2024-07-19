

namespace Kooboo.Lib.Security
{
    public class RSAEncryption
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Encrypt(string publickey, string content)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RSAEncrypt(publickey, content);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Decrypt(string privatekey, string content)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.RSADecrypt(privatekey, content);

        }

        /// <summary>
        /// Generates 2 XML files (public and private key) 
        /// </summary> 
        /// <param name="privateKeyPath">RSA private key file path</param> 
        /// <param name="publicKeyPath">RSA private key file path</param> /
        // <param name="size">secure size must be above 512</param> 
        private static void GenerateRsa(string privateKeyPath, string publicKeyPath, int size)
        {
            Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GenerateRsa(privateKeyPath, publicKeyPath, size);
        }
        public static RsaKeys GenerateKeys(int size)
        {
            if (size <= 0)
            {
                size = 512;
            }
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.Framework.GenerateKeys(size);
        }
    }

    public class RsaKeys
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
