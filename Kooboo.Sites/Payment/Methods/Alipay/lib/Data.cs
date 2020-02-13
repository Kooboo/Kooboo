using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class AlipayData
    {
        private SortedDictionary<string, string> m_values;

        public AlipayData()
        {
        }

        public SortedDictionary<string, string> SortDictionary(AopDictionary txtParams)
        {
            m_values = new SortedDictionary<string, string>(txtParams);

            return m_values;
        }

        public string ToStringContent(IDictionary<string, string> dic)
        {
            // 第一步：把字典按Key的字母顺序排序
            var dem = dic.GetEnumerator();

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
            return AlipaySignature.RSACheckContent(signContent, sign, publicKeyPem, charset, "RSA");
        }

        public string ToJson(IDictionary<string,string> dic)
        {
            JsonSerializerSettings jsetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(dic, Formatting.None, jsetting);
        }
    }
}
