//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.ComponentModel;
using Kooboo.Data.Context;

namespace KScript
{

    public class Curl
    {
        private RenderContext context { get; set; }
        public Curl(RenderContext context)
        {
            this.context = context;
        }

        [Description(@"Get data string from the url
var webcontent = k.url.get(""http://www.kooboo.com""); ")]
        public string get(string url)
        {
            return _get(url).Result;
        }

        [Description(@"Get data string from remote url using HTTP Basic authentication
var webcontent = k.url.get(""http://www.kooboo.com"", ""username"", ""password"");")]
        public string Get(string url, string username, string password)
        {
            return _get(url, null, username, password).Result;
        }

        [Description(@"Get data string from remote url using HTTP Basic authentication, and deserialize the string as a JSON object")]
        public object GetJson(string url, string username, string password)
        {
            string result = _get(url, null, username, password).Result;
            return Kooboo.Lib.Helper.JsonHelper.Deserialize(result);
        }

        [Description(@"Get data string from remote url and deserialize the string as a JSON object")]
        public object GetJson(string url)
        {
            string result = _get(url).Result;
            return Kooboo.Lib.Helper.JsonHelper.Deserialize(result);
        }


        [Description(@"Post data to remote url
var data = ""name=myname&field=value""; 
      k.url.post(""http://www.kooboo.com/fakereceiver"", data); ")]
        public string post(string url, string data)
        {
            return _Post(url, data).Result;
        }

        [Description(@"Post data to remote url using HTTP Basic authentication")]
        public string post(string url, string data, string userName, string password)
        {
            return _Post(url, data, userName, password).Result;
        }
        [Description(@"Post data as a Json string to remote url using HTTP Basic authentication")]
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
                    Content = content,
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
        [Description(@"Download zip package by url.")]
        public void DownloadZip(string url)
        {
            HttpClient client = HttpClientHelper.Client;
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };

            var response = client.SendAsync(requestMessage).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            context.Response.Headers.Add("Content-Disposition", response.Content.Headers.ContentDisposition.ToString());
            context.Response.ContentType = "application/zip";
            context.Response.Body = bytes;
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

        public string postform(string url, string data, string userName, string password)
        {
            return _PostForm(url, data, userName, password).Result;
        }

        public string postform(string url, object data, string userName, string password)
        {
            string json = JsonHelper.Serialize(data);
            return _PostForm(url, json, userName, password).Result;
        }

        private async Task<string> _PostForm(string url, string json, string UserName = null, string Password = null)
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
                if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}:{1}", UserName, Password));
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
