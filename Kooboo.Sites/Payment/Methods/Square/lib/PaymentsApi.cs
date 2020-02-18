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
        public static string Pay(string nonce, Money amount, string accessToken, string paymentURL)
        {
            string uuid = Guid.NewGuid().ToString();

            var body = new Models.PaymentRequest(nonce, uuid, amount);

            return DoHttpPostRequest(paymentURL, JsonSerialize(body), accessToken);
        }

        public static string DoHttpPostRequest(string url, string data, string accessToken, bool autoRedirect = true, Action<HttpWebResponse> callback = null)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + accessToken);
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

            try
            {
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
            catch (Exception)
            {
                return "Payment failed";
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
