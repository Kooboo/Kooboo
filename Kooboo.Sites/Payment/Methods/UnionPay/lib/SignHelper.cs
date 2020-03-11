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
        /// <summary>
        /// 使用配置文件配置的证书/密钥签名
        /// </summary>
        /// <param name="reqData">请求的数据源</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="certRawData">加密证书的数据源</param>
        /// <param name="certPassword">加密证书的密码</param>
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
        /// <param name="reqData">请求的数据源</param>
        /// <param name="certRawData">证书数据</param>
        /// <param name="certPwd">证书密码</param>
        /// <param name="encoding">编码方式</param>
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
        /// <param name="rspData">数据源</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="rootCertRawData">根证书的数据</param>
        /// <param name="middleCertRawData">中级证书数据</param>
        /// <returns></returns>
        public static bool Validate(Dictionary<string, string> rspData, Encoding encoding, byte[] rootCertRawData, byte[] middleCertRawData)
        {
            if (!ValidateBaseData(rspData))
            {
                return false;
            }

            byte[] signByte = Convert.FromBase64String(rspData["signature"]);
            rspData.Remove("signature");

            string stringData = SDKUtil.CreateLinkString(rspData, true, false, encoding);
            byte[] signDigest = System.Security.Cryptography.SHA256.Create().ComputeHash(encoding.GetBytes(stringData));
            string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);

            string signPubKeyCert = rspData["signPubKeyCert"];
            signPubKeyCert = signPubKeyCert.Replace("-----END CERTIFICATE-----", "").Replace("-----BEGIN CERTIFICATE-----", "");

            var signCert = new X509Certificate2(Convert.FromBase64String(signPubKeyCert));
            var rootCert = new X509Certificate2(rootCertRawData);
            var middleCert = new X509Certificate2(middleCertRawData);

            var chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(rootCert);
            chain.ChainPolicy.ExtraStore.Add(middleCert);

            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;

            chain.Build(signCert);
            if (chain.ChainElements.Count != chain.ChainPolicy.ExtraStore.Count + 1)
                return false;

            var rsa = signCert.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;

            return rsa.VerifyData(encoding.GetBytes(stringSignDigest), System.Security.Cryptography.SHA256.Create(), signByte);
        }

        public static bool ValidateBaseData(Dictionary<string, string> rspData)
        {
            var result = false;

            if (rspData.ContainsKey("version") && rspData.ContainsKey("signature") && rspData.ContainsKey("signMethod") && "01".Equals(rspData["signMethod"]))
            {
                return true;
            }

            return result;
        }
    }
}
