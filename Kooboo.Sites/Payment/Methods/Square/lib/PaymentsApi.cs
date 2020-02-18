using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Payment.Methods.Square.lib
{
    public class PaymentsApi
    {
        public static void Pay(string nonce, string accessToken)
        {
            string uuid = Guid.NewGuid().ToString();
            Money amount = new Money.Builder()
    .Amount(500L)
    .Currency("USD")
    .Build();

            string note = "From Square Sample Csharp App";

            var body = new Models.CreatePaymentRequest(nonce, uuid, amount, null, null, null, null, null, null, null, null, null, null, null, null, note, null);

            var _body = JsonSerialize(body);

            var _queryUrl = "https://connect.squareupsandbox.com/v2/payments";

            var response = DoHttpRequest(_queryUrl, _body, accessToken);
        }

        public static string DoHttpRequest(string url, string data, string accessToken, bool autoRedirect = true, Action<HttpWebResponse> callback = null)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            //httpWebRequest.Headers.Add("user-agent", "Square-DotNet-SDK/4.1.0");
            //httpWebRequest.Headers.Add("accept", "application/json");
            //httpWebRequest.Headers.Add("content-type", "application/json; charset=utf-8");
            httpWebRequest.Headers.Add("Square-Version", "2020-01-22");
            httpWebRequest.AllowAutoRedirect = autoRedirect;

            httpWebRequest.Timeout = 30000;

            byte[] postBytes = Encoding.UTF8.GetBytes(data ?? string.Empty);
            httpWebRequest.ContentLength = postBytes.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();
            }

            var httpWebResponse = httpWebRequest.GetResponse();
            if (callback != null)
            {
                callback(httpWebResponse as HttpWebResponse);
            }

            using (var responseStream = httpWebResponse.GetResponseStream())
            {
                using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    var result = streamReader.ReadToEnd();

                    return result;
                }
            }
        }

        public static string JsonSerialize(object obj, JsonConverter converter = null)
        {
            if (null == obj)
                return null;

            var settings = new JsonSerializerSettings();

            if (converter == null)
                settings.Converters.Add(new IsoDateTimeConverter());
            else
                settings.Converters.Add(converter);

            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }
    }
}
