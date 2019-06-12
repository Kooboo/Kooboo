//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Kooboo.Lib.Helper
{
    public class HttpHelper
    {
        static HttpHelper()
        {
            //ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            ////turn on tls12 and tls11,default is ssl3 and tls
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11; 
            SetCustomSslChecker(); 
        }

        public static bool HasSetCustomSSL { get; set; }

        public static void SetCustomSslChecker()
        {
            if (!HasSetCustomSSL)
            { 
                ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
                HasSetCustomSSL = true;
            }
        }
         
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //make self signed cert ,so not validate cert in client
            return true;
        }

        public static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 4.0.30319)";

        public static T ProcessApiResponse<T>(string response)
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

        public static T Post<T>(string url, Dictionary<string, string> parameters, string UserName = null, string Password = null)
        {
            try
            {
                var postString = String.Join("&", parameters.Select(it => String.Concat(it.Key, "=", Uri.EscapeDataString(it.Value))));
                var postData = Encoding.UTF8.GetBytes(postString);
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                    {
                        var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                        client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                    }

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
      
        public static byte[] ConvertKooboo(string url, byte[] data, Dictionary<string, string> headers, string UserName = null, string Password = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    foreach (var item in headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                    if (!string.IsNullOrEmpty(UserName))
                    {
                        var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                        client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                    }

                    return client.UploadData(url, "POST", data);
                }
            }
            catch (Exception ex)
            {
                //TODO: log exception
            }
            return null;
        }
          
        public static T Post<T>(string url, string json)
        {
            try
            {
                json = System.Net.WebUtility.UrlEncode(json);  ///????? What is this????
                var postData = Encoding.UTF8.GetBytes(json);
                using (var client = new WebClient())
                {
                    client.Proxy = null; 
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                     
                    var responseData = client.UploadData(url, "POST",  postData);

                    return ProcessApiResponse<T>(Encoding.UTF8.GetString(responseData));
                }
            }
            catch (Exception)
            {
                //TODO: log exception
            }
            return default(T);
        }

        public static T Get<T>(string url, Dictionary<string, string> query = null, string UserName = null, string Password = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                }
                client.Proxy = null;
                client.Encoding = Encoding.UTF8;

                var backstring = client.DownloadString(url);

                return ProcessApiResponse<T>(backstring);
            }
        }
         
        public static T TryGet<T>(string url, Dictionary<string, string> query = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);

                    client.Proxy = null;
                    client.Encoding = Encoding.UTF8;

                    var backstring = client.DownloadString(url);

                    return ProcessApiResponse<T>(backstring);
                }
            }
            catch (Exception)
            {
                 
            }
            return default(T);  
        }

        public static async Task<T> GetAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> query = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                return default(T);
            }
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                }

                if (query != null)
                {
                    url = UrlHelper.AppendQueryString(url, query);
                }

                client.Proxy = null;
                client.Encoding = Encoding.UTF8;

                var backstring = await client.DownloadStringTaskAsync(new Uri(url));
                var result = ProcessApiResponse<T>(backstring);
                return result;
            }
        }

        public static bool PostData(string url, Dictionary<string, string> Headers, byte[] PostBytes, string UserName = null, string Password = null)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                if (Headers == null)
                {
                    Headers = new Dictionary<string, string>();
                }
                var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
            }
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "multipart/form-data");
                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                }

                bool success = false;
                try
                {
                    var responseData = client.UploadData(url, "POST", PostBytes);

                    var ok = ProcessApiResponse<bool>(Encoding.UTF8.GetString(responseData));

                    success = ok;
                }
                catch (Exception ex)
                {
                    success = false;
                }
                return success;
            }
        }
         
    } 
}