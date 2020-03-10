using Kooboo.Sites.Payment.Methods.Alipay.lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
            var cert = new X509Certificate2(certRawData, certPwd, X509KeyStorageFlags.Exportable);
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

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="rspData"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static bool Validate(Dictionary<string, string> rspData, Encoding encoding)
        {
            if (!rspData.ContainsKey("version"))
            {
                return false;
            }
            string version = rspData["version"];

            if (!rspData.ContainsKey("signature"))
            {
                return false;
            }
            string signature = rspData["signature"];

            if (!rspData.ContainsKey("signMethod"))
            {
                return false;
            }
            string signMethod = rspData["signMethod"];

            bool result = false;

            if ("01".Equals(signMethod))
            {
                byte[] signByte = Convert.FromBase64String(signature);
                rspData.Remove("signature");
                string stringData = SDKUtil.CreateLinkString(rspData, true, false, encoding);

                byte[] signDigest = System.Security.Cryptography.SHA256.Create().ComputeHash(encoding.GetBytes(stringData));
                string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);
                string signPubKeyCert = rspData["signPubKeyCert"];

                signPubKeyCert = signPubKeyCert.Replace("-----END CERTIFICATE-----", "").Replace("-----BEGIN CERTIFICATE-----", "");
                byte[] x509CertBytes = Convert.FromBase64String(signPubKeyCert);

                var roby = File.ReadAllBytes(@"D:\certs\acp_test_root.cer");
                var rootCert = new X509Certificate2(roby);

                var miby = File.ReadAllBytes(@"D:\certs\acp_test_middle.cer");
                var micert = new X509Certificate2(miby);

                var cert = new X509Certificate2(x509CertBytes);

                var chain = new X509Chain();
                chain.ChainPolicy.ExtraStore.Add(rootCert);
                chain.ChainPolicy.ExtraStore.Add(micert);
                
                // You can alter how the chain is built/validated.
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

                chain.Build(cert);

                if (chain.ChainElements.Count != chain.ChainPolicy.ExtraStore.Count + 1)
                    return false;

                var rsa = cert.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;

                result = rsa.VerifyData(encoding.GetBytes(stringSignDigest), System.Security.Cryptography.SHA256.Create(), signByte);
            }

            return result;
        }
    }
}
