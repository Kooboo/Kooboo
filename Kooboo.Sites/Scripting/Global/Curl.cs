//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    // TODO: should try to use async/await. 
    public class Curl
    {
        public string get(string url)
        {
            return _get(url);
        }

        public string Get(string url, string username, string password)
        {
            return _get(url, null, username, password);
        }

        public string post(string url, string data)
        {
            return _Post(url, data);
        }

        public string post(string url, string data, string userName, string password)
        {
            return _Post(url, data, userName, password);
        }

        public string postData(string url, object data, string userName, string password)
        {
            string poststring = Kooboo.Lib.Helper.JsonHelper.Serialize(data);

            return _Post(url, poststring, userName, password);
        }


        private Dictionary<string, string> getvalues(object obj)
        {

            Dictionary<string, string> result = new Dictionary<string, string>();

            if (obj is IDictionary)
            {
                var dict = obj as IDictionary;
                foreach (var item in dict.Keys)
                {
                    var value = dict[item];
                    if (value != null)
                    {
                        result.Add(item.ToString(), value.ToString());
                    }
                    else
                    {
                        result.Add(item.ToString(), string.Empty);
                    }
                }

            }

            else if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = obj as IDictionary<String, Object>;
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        if (item.Value != null)
                        {
                            result.Add(item.Key, item.Value.ToString());
                        }
                        else
                        {
                            result.Add(item.Key, string.Empty);
                        }
                    }
                }

            }
            else if (obj is IDictionary<string, object>)
            {
                IDictionary<string, object> value = obj as IDictionary<string, object>;
                if (value != null)
                {
                    foreach (var item in value)
                    { 
                        if (item.Value != null)
                        {
                            result.Add(item.Key, item.Value.ToString());
                        }
                        else
                        {
                            result.Add(item.Key, string.Empty);
                        }
                    }
                }
            }

            return result; 
        }


        private static string _Post(string url, string json, string UserName = null, string Password = null)
        {
            try
            {
                var postData = Encoding.UTF8.GetBytes(json);
                var client = new WebClient();
                client.Proxy = null;

                client.Headers.Add("user-agent", DefaultUserAgent);
                client.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                }

                SetSslValidate(url);

                var responseData = client.UploadData(url, "POST", postData);

                return Encoding.UTF8.GetString(responseData);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private static void SetSslValidate(string url)
        {
            if (url.ToLower().StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //make self signed cert ,so not validate cert in client
            return true;
        }

        private static string _get(string url, Dictionary<string, string> query = null, string UserName = null, string Password = null)
        {

            try
            {
                if (query != null)
                {
                    url = UrlHelper.AppendQueryString(url, query);
                }
                var client = new WebClient();
                client.Headers.Add("user-agent", DefaultUserAgent);

                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(bytes));
                }
                client.Proxy = null;
                client.Encoding = Encoding.UTF8;
                SetSslValidate(url);
                var uri = new Uri(url);
                return client.DownloadString(url);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 4.0.30319)";
    }
}
