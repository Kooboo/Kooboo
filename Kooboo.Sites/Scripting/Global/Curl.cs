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
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Kooboo.Sites.Scripting.Global
{
    // TODO: should try to use async/await. 
    public class Curl
    {
        public string get(string url)
        {
            return  _get(url).Result;
        }

        public string Get(string url, string username, string password)
        {
            return _get(url, null, username, password).Result;
        }

        public string post(string url, string data)
        {
            return _Post(url, data).Result;
        }

        public string post(string url, string data, string userName, string password)
        {
            return _Post(url, data, userName, password).Result;
        }

        public string postData(string url, object data, string userName, string password)
        {
            string poststring = Kooboo.Lib.Helper.JsonHelper.Serialize(data);

            return _Post(url, poststring, userName, password).Result;
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


        private static async Task<string> _Post(string url, string json, string UserName = null, string Password = null)
        {
            try
            {
                var client = HttpClientHelper.Client;

                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var requestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content=content,
                };
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    requestMessage.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Basic " + Convert.ToBase64String(bytes));
                }

                var response =await client.SendAsync(requestMessage);

                var byteArray = await response.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private static async Task<string> _get(string url, Dictionary<string, string> query = null, string UserName = null, string Password = null)
        {

            try
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
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", UserName, Password));
                    requestMessage.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Basic " + Convert.ToBase64String(bytes));
                }

                var response = await client.SendAsync(requestMessage);

                var byteArray = await response.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

    }
}
