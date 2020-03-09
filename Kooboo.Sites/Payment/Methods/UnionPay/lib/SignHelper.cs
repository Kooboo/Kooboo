using Kooboo.Sites.Payment.Methods.Alipay.lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.UnionPay.lib
{
    public class SignHelper
    {
        const string VERSION_5_0_0 = "5.0.0";

        /// <summary>
        /// 使用配置文件配置的证书/密钥签名
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static void Sign(Dictionary<string, string> reqData, Encoding encoding, byte[] certRawData, string certPassword)
        {
            if (!reqData.ContainsKey("version"))
            {
                throw new UnionPayException("version cannot by null.");
            }
            string version = reqData["version"];

            if (!reqData.ContainsKey("signMethod"))
            {
                throw new UnionPayException("signMethod cannot be null.");
            }

            string signMethod = reqData["signMethod"];

            if (signMethod == "01")
            {
                SignByCertInfo(reqData, certRawData, certPassword, encoding);
            }
        }

        /// <summary>
        /// 证书方式签名（多证书时使用），指定证书路径。
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="encoding">编码</param>
        /// <param name="certRawData">证书数据</param>
        /// <param name="certPwd">证书密码</param>
        /// <returns></returns>
        public static void SignByCertInfo(Dictionary<string, string> reqData, byte[] certRawData, string certPwd, Encoding encoding)
        {
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certRawData, certPwd, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);
            reqData["certId"] = new System.Numerics.BigInteger(cert.GetSerialNumber()).ToString();

            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = SDKUtil.CreateLinkString(reqData, true, false, encoding);
            byte[] signDigest = System.Security.Cryptography.SHA256.Create().ComputeHash(encoding.GetBytes(stringData));
            string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);

            var rsa = cert.PrivateKey as System.Security.Cryptography.RSACryptoServiceProvider;
            // Create a new RSACryptoServiceProvider
            var rsaClear = new System.Security.Cryptography.RSACryptoServiceProvider();

            // Export RSA parameters from 'rsa' and import them into 'rsaClear'
            rsaClear.ImportParameters(rsa.ExportParameters(true));
            byte[] byteSign = rsaClear.SignData(encoding.GetBytes(stringSignDigest), System.Security.Cryptography.SHA256.Create());

            string stringSign = Convert.ToBase64String(byteSign);

            //设置签名域值
            reqData["signature"] = stringSign;
        }
    }
}
