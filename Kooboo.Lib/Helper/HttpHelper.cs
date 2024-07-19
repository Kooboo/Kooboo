//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;

namespace Kooboo.Lib.Helper
{
    public class HttpHelper
    {
        static HttpHelper()
        {
            //ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            ////turn on tls12 and tls11,default is ssl3 and tls
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls13;
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

        public class ProcessApiResponseException : Exception
        {
            public ProcessApiResponseException(string message) : base(message)
            {
            }
        }

        public static T ProcessApiResponse<T>(string response, bool throwError = false)
        {
            if (string.IsNullOrEmpty(response))
            {
                return default(T);
            }

            var jobject = Lib.Helper.JsonHelper.DeserializeJObject(response);

            if (jobject != null)
            {
                var successStr = Lib.Helper.JsonHelper.GetString(jobject, "success");

                if (successStr?.ToLower() == "false" && throwError)
                {
                    var msg = jobject.GetValueOrDefault("messages").FirstOrDefault();
                    if (msg != null)
                    {
                        var message = msg.ToString();
                        throw new ProcessApiResponseException(message);
                    }
                    else
                    {
                        return default(T);
                    }
                }

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
            }
            else
            {
                //for not json support. 
                var type = typeof(T);
                return DataTypeHelper.ConvertType<T>(response);
            }

            return default(T);
        }

        public static T Post<T>(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers, bool throwError = false)
        {
            try
            {
                var postString = parameters == null ? string.Empty : String.Join("&", parameters.Select(it => String.Concat(it.Key, "=", Uri.EscapeDataString(it.Value))));
                var postData = Encoding.UTF8.GetBytes(postString);
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.Headers.Add(item.Key, item.Value);
                        }
                    }


                    var responseData = client.UploadData(url, "POST", postData);
                    var strResult = Encoding.UTF8.GetString(responseData);
                    return ProcessApiResponse<T>(strResult, throwError);
                }
            }
            catch (ProcessApiResponseException)
            {
                throw;
            }
            catch (Exception)
            {
                //TODO: log exception
            }
            return default(T);
        }

        public static async Task<T> PostAsync<T>(string url, Dictionary<string, string> parameters, Dictionary<string, string> headers, bool throwError = false)
        {
            try
            {
                var postString = parameters == null ? string.Empty : String.Join("&", parameters.Select(it => String.Concat(it.Key, "=", Uri.EscapeDataString(it.Value))));
                var postData = Encoding.UTF8.GetBytes(postString);
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.Headers.Add(item.Key, item.Value);
                        }
                    }

                    var uri = new Uri(url);

                    var responseData = client.UploadData(uri, "POST", postData);
                    var strResult = Encoding.UTF8.GetString(responseData);
                    return ProcessApiResponse<T>(strResult, throwError);
                }
            }
            catch (ProcessApiResponseException)
            {
            }
            catch (Exception)
            {

            }
            return default(T);
        }


        public static T Post<T>(string url, string json, Dictionary<string, string> headers)
        {
            try
            {
                var postData = Encoding.UTF8.GetBytes(json);
                using (var client = new WebClient())
                {
                    client.Proxy = null;
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    client.Headers.Add("Content-Type", "application/json");

                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.Headers.Add(item.Key, item.Value);
                        }
                    }

                    var responseData = client.UploadData(url, "POST", postData);

                    return ProcessApiResponse<T>(Encoding.UTF8.GetString(responseData));
                }
            }
            catch (Exception)
            {
                //TODO: log exception
            }
            return default(T);
        }

        public static T Post<T>(string url, Dictionary<string, string> parameters, string UserName = null, string Password = null, bool throwError = false)
        {
            var headers = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
            }

            return Post<T>(url, parameters, headers, throwError);
        }

        public static T Post<T>(string url, Dictionary<string, string> Headers, byte[] postBytes, string UserName = null, string Password = null, bool throwError = false)
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
                client.Headers.Add("Content-Type", "application/octet-stream");
                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    var responseData = client.UploadData(url, "POST", postBytes);

                    return ProcessApiResponse<T>(Encoding.UTF8.GetString(responseData), throwError);
                }
                catch (Exception)
                {

                }
                return default(T);
            }
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
            catch (Exception)
            {
                //TODO: log exception
            }
            return null;
        }

        public static T Post<T>(string url, string json)
        {
            return Post<T>(url, json, new Dictionary<string, string>());
        }

        public static T Get<T>(string url, Dictionary<string, string> query = null, string UserName = null, string Password = null, bool throwError = false)
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

                return ProcessApiResponse<T>(backstring, throwError);
            }
        }

        public static T Get2<T>(string url, Dictionary<string, string> query = null, Dictionary<string, string> headers = null, bool throwError = false)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }
            HttpClient client = HttpClientHelper.Client;

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = client.Send(requestMessage);
            var backString = response.Content.ReadAsStringAsync().Result;
            return ProcessApiResponse<T>(backString, throwError);
        }


        public static string GetString2(string url, Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }
            HttpClient client = HttpClientHelper.Client;

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = client.Send(requestMessage);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

        public static async Task<string> GetString2Async(string url, Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }
            HttpClient client = HttpClientHelper.Client;

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };

            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = await client.SendAsync(requestMessage);
            return await response.Content.ReadAsStringAsync();
        }



        public static T Get2<T>(string url, int longTimeOut, Dictionary<string, string> query = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }


            HttpClient client = HttpClientHelper.CreateClientInstance(longTimeOut);

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };

            var response = client.Send(requestMessage);
            var backString = response.Content.ReadAsStringAsync().Result;
            return ProcessApiResponse<T>(backString);
        }


        public static async Task<T> Get2Async<T>(string url, Dictionary<string, string> query = null, Dictionary<string, string> headers = null)
        {
            if (query != null)
            {
                url = UrlHelper.AppendQueryString(url, query);
            }

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };


            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }

            var response = await HttpClientHelper.Client.SendAsync(requestMessage);
            var backString = await response.Content.ReadAsStringAsync();
            return ProcessApiResponse<T>(backString);
        }

        public static T Get3Times<T>(string url, Dictionary<string, string> query = null)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var result = Get2<T>(url, query);
                    return result;
                }
                catch (Exception)
                {

                }
                System.Threading.Thread.Sleep(3000);
            }

            return default(T);
        }

        /// <summary>
        /// Get and return string. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string GetString(string url, Dictionary<string, string> headers = null)
        {
            try
            {
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

                    //client.Proxy = null;
                    client.Encoding = Encoding.UTF8;

                    return client.DownloadString(url);

                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        /// <summary>
        /// Post and return string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        public static string PostString(string url, string json, Dictionary<string, string> headers = null)
        {
            try
            {
                // json = System.Net.WebUtility.UrlEncode(json);  ///????? What is this????
                //var postData = Encoding.UTF8.GetBytes(json);
                using (var client = new WebClient())
                {
                    //     client.Proxy = null;
                    //    client.Headers.Add("user-agent", DefaultUserAgent);
                    // client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    if (headers != null)
                    {
                        foreach (var item in headers)
                        {
                            client.Headers.Add(item.Key, item.Value);
                        }
                    }

                    return client.UploadString(url, json);

                }
            }
            catch (Exception)
            {
                //TODO: log exception
            }
            return null;
        }


        public static async Task<string> GetStringAsync(string url, Dictionary<string, string> query = null)
        {
            try
            {

                if (query != null)
                {
                    url = UrlHelper.AppendQueryString(url, query);
                }

                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", DefaultUserAgent);

                    client.Proxy = null;
                    client.Encoding = Encoding.UTF8;

                    return await client.DownloadStringTaskAsync(new Uri(url));
                }
            }
            catch (Exception)
            {

            }

            return null;
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

                    var backString = client.DownloadString(url);

                    return ProcessApiResponse<T>(backString);
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
            using (var client = new LocalWebClient())
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

                var backString = await client.DownloadStringTaskAsync(new Uri(url));
                var result = ProcessApiResponse<T>(backString);
                return result;
            }
        }


        public static async Task<T> TryGetAsync<T>(string url, Dictionary<string, string> headers = null, Dictionary<string, string> query = null)
        {

            try
            {
                return await GetAsync<T>(url, headers, query);
            }
            catch (Exception)
            {
                return default(T);
            }
        }


        public static bool PostData(string url, Dictionary<string, string> Headers, byte[] PostBytes, string UserName = null, string Password = null, string contentType = null)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                if (Headers == null)
                {
                    Headers = new Dictionary<string, string>();
                }
                else if (!Headers.ContainsKey("Authorization"))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                }

            }
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", contentType ?? "application/octet-stream");
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
                catch (Exception)
                {
                    success = false;
                }
                return success;
            }
        }



        /// <summary>
        /// Post to URl and return the byte array. 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Headers"></param>
        /// <param name="PostBytes"></param>
        /// <returns></returns>
        public static byte[] PostBinary(string url, Dictionary<string, string> Headers, byte[] PostBytes)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "application/octet-stream");
                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    return client.UploadData(url, "POST", PostBytes);

                }
                catch (Exception)
                {

                }
                return null;
            }
        }
        public Dictionary<string, string> RequestHeader(string url)
        {
            try
            {
                var res = HttpClientHelper.Client.GetAsync(url).Result;
                Dictionary<string, string> headers = new Dictionary<string, string>();

                foreach (var item in res.Headers)
                {
                    var joinValue = string.Join(',', item.Value);

                    headers.Add(item.Key, joinValue);
                }
                res.Dispose();
                return headers;

            }
            catch (Exception)
            {

            }

            return null;

        }

        public static T PostData<T>(string url, Dictionary<string, string> Headers, byte[] PostBytes, string contentType = null)
        {

            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", contentType ?? "application/octet-stream");
                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        client.Headers.Add(item.Key, item.Value);
                    }
                }

                try
                {
                    var responseData = client.UploadData(url, "POST", PostBytes);

                    return Kooboo.Lib.Helper.HttpHelper.ProcessApiResponse<T>(Encoding.UTF8.GetString(responseData));

                }
                catch (Exception)
                {

                }
                return default;
            }
        }

    }

    public class LocalWebClient : WebClient
    {

        /// <summary>
        /// Creates the request for this client and sets connection defaults.
        /// </summary>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;

            if (request != null)
            {
                request.ServicePoint.ConnectionLimit = 100;
            }

            return request;
        }

    }
}