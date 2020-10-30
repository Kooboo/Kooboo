using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class AlipayData
    {
        public const string RESPONSE_SUFFIX = "_response";
        public const string ALIPAY_QUERY = "alipay.trade.query";

        public AlipayData()
        {
        }

        public SortedDictionary<string, string> SortDictionary(AopDictionary txtParams)
        {
            var m_values = new SortedDictionary<string, string>(txtParams);

            return m_values;
        }

        public string ToStringContent(IDictionary<string, string> dic)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(dic);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            var query = new StringBuilder("");
            while (dem.MoveNext())
            {
                var key = dem.Current.Key;
                var value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    query.Append(key).Append("=").Append(value).Append("&");
            }
            var content = query.ToString().Substring(0, query.Length - 1);

            return content;
        }

        public string RSASign(IDictionary<string, string> parameters, string privateKeyPem, string charset,
            string signType)
        {
            var signContent = ToStringContent(parameters);

            return AlipaySignature.RSASignCharSet(signContent, privateKeyPem, charset, signType);
        }

        public bool RSACheckV1(IDictionary<string, string> parameters, string publicKeyPem, string charset)
        {
            var sign = parameters["sign"];

            parameters.Remove("sign");
            parameters.Remove("sign_type");
            var signContent = ToStringContent(parameters);
            return AlipaySignature.RSACheckContent(signContent, sign, publicKeyPem, charset, "RSA2");
        }

        public string ToJson(IDictionary<string, string> dic)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(dic, Formatting.None, jsetting);
        }

        public IDictionary<string, object> FromJson(string body, AlipayFormSetting setting)
        {
            var json = JsonConvert.DeserializeObject<IDictionary<string, object>>(body);

            try
            {
                string sign = json["sign"].ToString();
                var signSourceDate = GetSignSourceData(body);

                json = JsonConvert.DeserializeObject<IDictionary<string, object>>(signSourceDate);
                if (signSourceDate.Contains("sub_code"))
                {
                    throw new AliPayException(json["sub_code"].ToString());
                }

                if (!string.IsNullOrEmpty(sign) && !string.IsNullOrEmpty(signSourceDate))
                {
                    CheckResponseSign(sign, signSourceDate, setting);

                    if (json["code"].ToString() != "10000")
                    {
                        throw new AliPayException(json["msg"].ToString());
                    }
                }
                else
                {
                    throw new AliPayException("sign check fail: body or reponse content is Empty!");
                }
            }
            catch (AliPayException ex)
            {
                throw new AliPayException(ex.Message);
            }

            return json;
        }

        public static void CheckResponseSign(string sign, string signSourceDate, AlipayFormSetting setting)
        {
            if (string.IsNullOrEmpty(setting.PublicKey) || string.IsNullOrEmpty(setting.Charset))
            {
                throw new AliPayException("public key or charset is Empty!");
            }

            if (signSourceDate == null)
            {
                throw new AliPayException("sign check fail: sign is Empty!");
            }

            var rsaCheckContent = AlipaySignature.RSACheckContent(signSourceDate, sign, setting.PublicKey,
                setting.Charset, setting.SignType);

            if (!rsaCheckContent)
            {
                throw new AliPayException("sign check fail: check Sign and Data Fail!");
            }

        }

        public static string GetSignSourceData(string body)
        {
            string rootNode = ALIPAY_QUERY.Replace(".", "_") + RESPONSE_SUFFIX;
            int indexOfRootNode = body.IndexOf(rootNode);

            string result = null;
            if (indexOfRootNode > 0)
            {
                result = ParseSignSourceData(body, rootNode, indexOfRootNode);
            }

            return result;
        }

        private static string ParseSignSourceData(string body, string rootNode, int indexOfRootNode)
        {
            int signDataStartIndex = indexOfRootNode + rootNode.Length + 2;
            int indexOfSign = body.IndexOf("\"sign\"");
            if (indexOfSign < 0)
            {
                return null;
            }

            int signDataEndIndex = indexOfSign - 1;
            int length = signDataEndIndex - signDataStartIndex;

            return body.Substring(signDataStartIndex, length);
        }
    }
}
