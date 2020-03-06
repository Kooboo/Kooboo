using Kooboo.Sites.Payment.Methods.Alipay.lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.UnionPay.lib
{
    public class SignHelper
    {
        const string VERSION_1_0_0 = "1.0.0";
        const string VERSION_5_0_0 = "5.0.0";

        const string SignCertPwd = "000000"; // to be remove to setting 
        const string SignCertPath = "d:/certs/acp_test_sign.pfx"; // to be remove to setting

        /// <summary>
        /// 使用配置文件配置的证书/密钥签名
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static void Sign(Dictionary<string, string> reqData, Encoding encoding)
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
                SignByCertInfo(reqData, SignCertPath, SignCertPwd, encoding);
            }
        }

        /// <summary>
        /// 证书方式签名（多证书时使用），指定证书路径。
        /// </summary>
        /// <param name="reqData"></param>
        /// <param name="encoding">编码</param>
        /// <param name="certPath">证书路径</param>
        /// <param name="certPwd">证书密码</param>
        /// <returns></returns>
        public static void SignByCertInfo(Dictionary<string, string> reqData, string certPath, string certPwd, Encoding encoding)
        {
            string version = reqData["version"];
            string signMethod = reqData["signMethod"];

            //  以下这个方式可以得到 certId
            var x5092 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath, certPwd);
            var ar = x5092.GetSerialNumber();
            reqData["certId"] = new System.Numerics.BigInteger(ar).ToString();

            //将Dictionary信息转换成key1=value1&key2=value2的形式
            string stringData = SDKUtil.CreateLinkString(reqData, true, false, encoding);
            byte[] signDigest = System.Security.Cryptography.SHA256.Create().ComputeHash(encoding.GetBytes(stringData));
            string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);

            //byte[] byteSign = SecurityUtil.SignSha256WithRsa(CertUtil.GetSignKeyFromPfx(certPath, certPwd), encoding.GetBytes(stringSignDigest));

            //string stringSign = Convert.ToBase64String(byteSign);

            //设置签名域值
            //reqData["signature"] = stringSign;
        }
    }
}
