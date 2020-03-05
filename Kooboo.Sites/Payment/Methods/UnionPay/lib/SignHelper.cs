////using Kooboo.Sites.Payment.Methods.Alipay.lib;
////using System;
////using System.Collections.Generic;
////using System.Text;

////namespace Kooboo.Sites.Payment.Methods.UnionPay.lib
////{
////    public class SignHelper
////    {
////        const string VERSION_1_0_0 = "1.0.0";
////        const string VERSION_5_0_0 = "5.0.0";

////        /// <summary>
////        /// 使用配置文件配置的证书/密钥签名
////        /// </summary>
////        /// <param name="reqData"></param>
////        /// <param name="encoding"></param>
////        /// <returns></returns>
////        public static void Sign(Dictionary<string, string> reqData, Encoding encoding)
////        {
////            if (!reqData.ContainsKey("version"))
////            {
////                throw new UnionPayException("version cannot by null.");
////            }
////            string version = reqData["version"];

////            string signMethod = null;
////            if (reqData.ContainsKey("signMethod"))
////            {
////                signMethod = reqData["signMethod"];
////            }
////            else if (!VERSION_1_0_0.Equals(version))
////            {
////                throw new UnionPayException("signMethod cannot be null.");
////            }

////            if ("01".Equals(signMethod) || VERSION_1_0_0.Equals(version))
////            {
////                SignByCertInfo(reqData, SDKConfig.SignCertPath, SDKConfig.SignCertPwd, encoding);
////            }
////            else if ("11".Equals(signMethod) || "12".Equals(signMethod))
////            {
////                SignBySecureKey(reqData, SDKConfig.SecureKey, encoding);
////            }
////            else
////            {
////                log.Error("Error signMethod [" + signMethod + "], " + "version [" + version + "] in Sign. ");
////            }
////        }

////        /// <summary>
////        /// 证书方式签名（多证书时使用），指定证书路径。
////        /// </summary>
////        /// <param name="reqData"></param>
////        /// <param name="encoding">编码</param>
////        /// <param name="certPath">证书路径</param>
////        /// <param name="certPwd">证书密码</param>
////        /// <returns></returns>
////        public static void SignByCertInfo(Dictionary<string, string> reqData, string certPath, string certPwd, Encoding encoding)
////        {
////            if (!reqData.ContainsKey("version"))
////            {
////                throw new UnionPayException("version cannot by null.");
////            }
////            string version = reqData["version"];

////            string signMethod = null;
////            if (reqData.ContainsKey("signMethod"))
////            {
////                signMethod = reqData["signMethod"];
////            }
////            else if (!VERSION_1_0_0.Equals(version))
////            {
////                throw new UnionPayException("signMethod cannot be null.");
////            }

////            if ("01".Equals(signMethod) || VERSION_1_0_0.Equals(version))
////            {
////                reqData["certId"] = CertUtil.GetSignCertId(certPath, certPwd);

////                //将Dictionary信息转换成key1=value1&key2=value2的形式
////                string stringData = SDKUtil.CreateLinkString(reqData, true, false, encoding);
////                //log.Info("待签名排序串：[" + stringData + "]");

////                if (VERSION_5_0_0.Equals(version) || VERSION_1_0_0.Equals(version))
////                {
////                    byte[] signDigest = SecurityUtil.Sha1(stringData, encoding);

////                    string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);
////                    //log.Info("sha1结果：[" + stringSignDigest + "]");

////                    byte[] byteSign = SecurityUtil.SignSha1WithRsa(CertUtil.GetSignKeyFromPfx(certPath, certPwd), encoding.GetBytes(stringSignDigest));

////                    string stringSign = Convert.ToBase64String(byteSign);
////                    //log.Info("5.0.0报文sha1RSA签名结果：[" + stringSign + "]");

////                    //设置签名域值
////                    reqData["signature"] = stringSign;
////                }
////                else
////                {
////                    byte[] signDigest = SecurityUtil.Sha256(stringData, encoding);

////                    string stringSignDigest = SDKUtil.ByteArray2HexString(signDigest);
////                    //log.Info("sha256结果：[" + stringSignDigest + "]");

////                    byte[] byteSign = SecurityUtil.SignSha256WithRsa(CertUtil.GetSignKeyFromPfx(certPath, certPwd), encoding.GetBytes(stringSignDigest));

////                    string stringSign = Convert.ToBase64String(byteSign);
////                    //log.Info("5.1.0报文sha256RSA签名结果：[" + stringSign + "]");

////                    //设置签名域值
////                    reqData["signature"] = stringSign;

////                }
////            }
////            else
////            {
////                throw new UnionPayException("Error signMethod [" + signMethod + "] in SignByCertInfo. ");
////            }
////        }
////    }
////}
