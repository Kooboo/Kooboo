using System.Linq;
using System.Net;
using System.Text;
using Kooboo.Lib.Helper;
using Newtonsoft.Json.Linq;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailHttpHelper
    {
        public static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 4.0.30319)";

        public static T Get<T>(string url, Dictionary<string, string> query, Dictionary<string, string> headers)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }

            Data.Log.Instance.Email.Write("--get--" + url);

            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                AddHeader(client.Headers, headers);

                client.Proxy = null;
                client.Encoding = Encoding.UTF8;

                var backstring = client.DownloadString(url);

                Data.Log.Instance.Email.Write(backstring);

                return ProcessApiResponse<T>(backstring);
            }
        }

        public static T Post<T>(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            var postString = String.Join("&", parameters.Select(it => String.Concat(it.Key, "=", Uri.EscapeDataString(it.Value))));
            var postData = Encoding.UTF8.GetBytes(postString);

            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                AddHeader(client.Headers, headers);

                var responseData = client.UploadData(url, "POST", postData);

                Kooboo.Data.Log.Instance.Email.Write("--post-- " + url + " -- " + postString);

                var strResult = Encoding.UTF8.GetString(responseData);

                Kooboo.Data.Log.Instance.Email.Write(strResult);

                return ProcessApiResponse<T>(strResult);
            }

        }

        public static T Post<T>(string url, Dictionary<string, string> Headers, byte[] postBytes, string contentType)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", contentType ?? "application/octet-stream");

                AddHeader(client.Headers, Headers);

                var responseData = client.UploadData(url, "POST", postBytes);

                string result = Encoding.UTF8.GetString(responseData);

                Kooboo.Data.Log.Instance.Email.Write("--post--" + url);
                Kooboo.Data.Log.Instance.Email.Write(result);

                return ProcessApiResponse<T>(result);

            }
        }


        public static byte[] PostBytes(string url, byte[] data, Dictionary<string, string> headers)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                AddHeader(client.Headers, headers);
                return client.UploadData(url, "POST", data);
            }
        }

        private static void AddHeader(WebHeaderCollection webHeaderCollection, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webHeaderCollection.Add(header.Key, header.Value);
                }
            }
        }

        private static T ProcessApiResponse<T>(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                return default(T);
            }

            var jobject = Lib.Helper.JsonHelper.DeserializeJObject(response);

            if (jobject != null)
            {
                var successStr = Lib.Helper.JsonHelper.GetString(jobject, "success");

                var modelstring = Lib.Helper.JsonHelper.GetString(jobject, "Model");


                if (string.IsNullOrWhiteSpace(modelstring) && typeof(T) == typeof(bool))
                {
                    modelstring = successStr;
                }

                if (!string.IsNullOrEmpty(modelstring))
                {
                    var type = typeof(T);

                    if (!type.IsValueType && type != typeof(string))
                    {
                        return Lib.Helper.JsonHelper.Deserialize<T>(modelstring);
                    }
                    else
                    {
                        return (T)Lib.Reflection.TypeHelper.ChangeType(modelstring, typeof(T));
                    }
                }

                var messages = GetErrorResponse(jobject);
                if (!string.IsNullOrEmpty(messages))
                {
                    throw new Exception(messages.ToString());
                }

            }

            return default(T);
        }



        private static string GetErrorResponse(JObject jobject)
        {
            if (jobject == null)
            {
                return null;
            }
            var token = jobject["messages"];
            if (token == null)
            {

                foreach (var item in jobject.Properties().ToList())
                {
                    if (item.Name.ToLower() == "messages")
                    {
                        token = item.Value;
                    }
                }
            }

            var error = string.Empty;

            if (token != null)
            {
                var array = (JArray)token;
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        error += item?.ToString() + System.Environment.NewLine;
                    }
                }
            }
            return error.Trim();

        }
    }
}
