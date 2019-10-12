using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

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
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                AddHeader(client.Headers, headers);

                client.Proxy = null;
                client.Encoding = Encoding.UTF8;

                var backstring = client.DownloadString(url);

                return ProcessApiResponse<T>(backstring);
            }
        }

        public static T Post<T>(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers)
        {
            try
            {
                var postString = String.Join("&", parameters.Select(it => String.Concat(it.Key, "=", Uri.EscapeDataString(it.Value))));
                var postData = Encoding.UTF8.GetBytes(postString);
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    AddHeader(client.Headers, headers);

                    var responseData = client.UploadData(url, "POST", postData);
                    var strResult = Encoding.UTF8.GetString(responseData);
                    return ProcessApiResponse<T>(strResult);
                }
            }
            catch (Exception ex)
            {
                //TODO: log exception
            }
            return default(T);
        }

        public static T Post<T>(string url, Dictionary<string, string> Headers, byte[] postBytes)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "multipart/form-data");

                AddHeader(client.Headers, Headers);

                try
                {
                    var responseData = client.UploadData(url, "POST", postBytes);

                    return ProcessApiResponse<T>(Encoding.UTF8.GetString(responseData));
                }
                catch (Exception ex)
                {
                }
                return default(T);
            }
        }

        public static byte[] PostBytes(string url, byte[] data, Dictionary<string, string> headers)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    AddHeader(client.Headers, headers);
                    return client.UploadData(url, "POST", data);
                }
            }
            catch (Exception ex)
            {
                //TODO: log exception
            }
            return null;
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

                    if (type.IsClass && type != typeof(string))
                    {
                        return Lib.Helper.JsonHelper.Deserialize<T>(modelstring);
                    }
                    else
                    {
                        return (T)Lib.Reflection.TypeHelper.ChangeType(modelstring, typeof(T));
                    }
                }
            }

            return default(T);
        }
    }
}