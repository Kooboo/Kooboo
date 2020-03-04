using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.Payment.Methods.qualpay.lib
{
    public class DataHelper
    {
        public static string GetValue(string name, string body)
        {
            if (body != null)
            {
                var converted = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);

                string[] keys = name.Split('.');
                if (!converted.ContainsKey(keys[0]))
                    return "";

                if (keys.Length == 1)
                {
                    return converted[keys[0]].ToString();
                }

                if (converted[keys[0]] is JArray)
                {
                    var dataAr = (JArray)converted[keys[0]];

                    if (dataAr.Count < 1)
                    {
                        return null;
                    }
                }
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(converted[keys[0]].ToString());
                if (!data.ContainsKey(keys[1].TrimEnd()))
                    return "";
                if (data[keys[1]] is JArray)
                {
                    var array = (JArray)data[keys[1]];
                    var tokens = array.Select(it => it.SelectToken(keys[2]).Value<string>()).ToArray();
                    return string.Join(",", tokens);

                }
                else if (data[keys[1]] is JObject)
                {
                    var obj = (JObject)data[keys[1]];
                    return obj[keys[2]].ToString();
                }
                else
                {
                    return data[keys[1]].ToString();
                }

            }
            return "";
        }

        public static string GeneratePurchaseId(Guid requestId)
        {
            byte[] bytes = requestId.ToByteArray();
            return Convert.ToBase64String(bytes);
        }

        public static Guid GenerateRequestId(string purchasetId)
        {
            byte[] bytes = Convert.FromBase64String(purchasetId);
            return new Guid(bytes);
        }

        public static string CreateToken(string message, string secret)
        {

            secret = secret ?? "";
            var encoding = new System.Text.UTF8Encoding();

            byte[] keyByte = encoding.GetBytes(secret);

            byte[] messageBytes = encoding.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}
