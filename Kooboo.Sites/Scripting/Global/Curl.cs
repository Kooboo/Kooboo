//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global
{
    // TODO: should try to use async/await.
    public class Curl
    {
        public string get(string url)
        {
            return _get(url).Result;
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

            if (obj is IDictionary dict)
            {
                foreach (var item in dict.Keys)
                {
                    var value = dict[item];
                    result.Add(item.ToString(), value != null ? value.ToString() : string.Empty);
                }
            }
            else if (obj is System.Dynamic.ExpandoObject)
            {
                IDictionary<String, Object> value = obj as IDictionary<String, Object>;
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        result.Add(item.Key, item.Value != null ? item.Value.ToString() : string.Empty);
                    }
                }
            }
            else if (obj is IDictionary<string, object> value)
            {
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        result.Add(item.Key, item.Value != null ? item.Value.ToString() : string.Empty);
                    }
                }
            }

            return result;
        }

        private static async Task<string> _Post(string url, string json, string userName = null, string password = null)
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
                    Content = content,
                };
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    var bytes = Encoding.UTF8.GetBytes($"{userName}:{password}");
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

        private static async Task<string> _get(string url, Dictionary<string, string> query = null, string userName = null, string password = null)
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
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    var bytes = Encoding.UTF8.GetBytes($"{userName}:{password}");
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

        public string postform(string url, string data, string userName, string password)
        {
            return _PostForm(url, data, userName, password).Result;
        }

        public string postform(string url, object data, string userName, string password)
        {
            string json = JsonHelper.Serialize(data);
            return _PostForm(url, json, userName, password).Result;
        }

        private async Task<string> _PostForm(string url, string json, string userName = null, string password = null)
        {
            string result;
            try
            {
                HttpClient client = HttpClientHelper.Client;
                FormUrlEncodedContent content = new FormUrlEncodedContent(JsonHelper.Deserialize<IDictionary<string, string>>(json));
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = content
                };
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes($"{userName}:{password}");
                    httpRequestMessage.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Basic " + Convert.ToBase64String(bytes));
                }
                byte[] array = await (await client.SendAsync(httpRequestMessage)).Content.ReadAsByteArrayAsync();
                result = Encoding.UTF8.GetString(array, 0, array.Length);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}