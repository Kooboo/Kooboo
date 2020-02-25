using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.Payment.Methods.qualpay.lib
{
    public class DataHelper
    {
        public static string GetValue(string name, string body)
        {
            if (body != null)
            {
                if (body is JArray)
                {
                    var array = (JArray)body;
                    foreach (var item in array)
                    {
                        if (item is JObject)
                        {
                            var itemobject = (JObject)item;
                            if (itemobject != null)
                            {
                                foreach (var itemproperty in itemobject.Properties())
                                {
                                    if (itemproperty.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(itemproperty.Value.ToString()))
                                    {
                                        return itemproperty.Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                else if (body is JObject)
                {
                    var data = (JObject)body;

                    if (data != null)
                    {
                        foreach (var item in data.Properties())
                        {
                            if (item.Value is JObject)
                            {
                                var node = (JObject)item.Value;
                                foreach (var nodeValue in node)
                                {
                                    if (item.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(item.Value.ToString()))
                                    {
                                        return item.Value.ToString();
                                    }
                                }
                            }
                            else if (item.Value is JArray)
                            {
                                var array = (JArray)item.Value;
                                foreach (var value in item.Value)
                                {
                                    if (value is JObject)
                                    {
                                        var itemobject = (JObject)value;
                                        if (itemobject != null)
                                        {
                                            foreach (var itemproperty in itemobject.Properties())
                                            {
                                                if (itemproperty.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(itemproperty.Value.ToString()))
                                                {
                                                    return itemproperty.Value.ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (item.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(item.Value.ToString()))
                            {
                                return item.Value.ToString();
                            }
                        }
                    }
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
