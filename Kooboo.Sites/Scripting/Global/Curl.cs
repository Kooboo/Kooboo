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
using Kooboo.Data.Attributes;

namespace KScript
{

    public class Curl
    {
        [KIgnore]
        private RenderContext context { get; set; }

        public Curl(RenderContext context)
        {
            this.context = context;
        }

        #region get

        [Description(@"Get data string from the url
var webcontent = k.url.get(""http://www.kooboo.com""); ")]
        public string get(string url) => _get(url).Result;

        [Description(@"Get data string from remote url using HTTP Bearer authentication
var webcontent = k.url.get(""http://www.kooboo.com"", ""token"");")]
        public string get(string url, string token) => _get(url, null, BuildBearerHeader(token)).Result;

        [Description(@"Get data string from remote url using HTTP Basic authentication
var webcontent = k.url.get(""http://www.kooboo.com"", ""username"", ""password"");")]
        public string get(string url, string username, string password) => _get(url, null, BuildBasicHeader(username, password)).Result;

        [Description(@"Get data string from the url
var webcontent = k.url.get(""http://www.kooboo.com"",{'Authentication','Bearer xxx'}); ")]
        public string get(string url, IDictionary<string, object> headers) => _get(url, null, headers).Result;

        #endregion

        #region getJson

        [Description(@"Get data string from remote url and deserialize the string as a JSON object.
var webcontent = k.url.getJson(""http://www.kooboo.com""); ")]
        public object GetJson(string url)
        {
            string result = get(url);
            Jint.Native.Json.JsonParser praser = new Jint.Native.Json.JsonParser(new Jint.Engine());
            return praser.Parse(result);
        }

        [Description(@"Get data string from remote url using HTTP Basic authentication, and deserialize the string as a JSON object
var webcontent = k.url.getJson(""http://www.kooboo.com"",""xxx"");")]
        public object GetJson(string url, string token)
        {
            string result = get(url, token);
            Jint.Native.Json.JsonParser praser = new Jint.Native.Json.JsonParser(new Jint.Engine());
            return praser.Parse(result);
        }

        [Description(@"Get data string from remote url using HTTP Basic authentication, and deserialize the string as a JSON object
var webcontent = k.url.getJson(""http://www.kooboo.com"",""admin"",""abc123"");")]
        public object GetJson(string url, string username, string password)
        {
            string result = get(url, username, password);
            Jint.Native.Json.JsonParser praser = new Jint.Native.Json.JsonParser(new Jint.Engine());
            return praser.Parse(result);
        }

        [Description(@"Get data string from remote url and deserialize the string as a JSON object
var webcontent = k.url.getJson(""http://www.kooboo.com"",{'Authentication','Bearer xxxx'}); ")]
        public object GetJson(string url, IDictionary<string, object> headers)
        {
            string result = get(url, headers);
            Jint.Native.Json.JsonParser praser = new Jint.Native.Json.JsonParser(new Jint.Engine());
            return praser.Parse(result);
        }

        #endregion

        #region post

        [Description(@"Post data to remote url
var data = ""name=myname&field=value""; 
k.url.post(""http://www.kooboo.com/fakereceiver"", data); ")]
        public string post(string url, string data) => _Post(url, data).Result;

        [Description(@"Post data to remote url using HTTP Basic authentication
var data = ""name=myname&field=value""; 
k.url.post(""http://www.kooboo.com/fakereceiver"", data,'xxx');
")]
        public string post(string url, string data, string token) => _Post(url, data, BuildBearerHeader(token)).Result;

        [Description(@"Post data to remote url using HTTP Basic authentication
var data = ""name=myname&field=value""; 
k.url.post(""http://www.kooboo.com/fakereceiver"", data,'admin','abc123');
")]
        public string post(string url, string data, string userName, string password) => _Post(url, data, BuildBasicHeader(userName, password)).Result;



        #endregion

        #region postData

        [Description(@"Post data to remote url
var data = {
    name:'abc',
    age:23
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data); ")]
        public string postData(string url, object data)
        {
            string poststring = Kooboo.Lib.Helper.JsonHelper.Serialize(data);
            return post(url, poststring);
        }

        [Description(@"Post data to remote url using HTTP Bearer authentication
var data = {
    name:'abc',
    age:23
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'xxx'); ")]
        public string postData(string url, object data, string token)
        {
            string poststring = Kooboo.Lib.Helper.JsonHelper.Serialize(data);
            return post(url, poststring, token);
        }

        [Description(@"Post data as a Json string to remote url using HTTP Basic authentication
var data = {
    name:'abc',
    age:23
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'admin','abc123'); 
")]
        public string postData(string url, object data, string userName, string password)
        {
            string poststring = Kooboo.Lib.Helper.JsonHelper.Serialize(data);
            return post(url, poststring, userName, password);
        }

        #endregion

        #region postForm

        [Description(@"Post form data to remote url
var data = '{""name"":""abc"", ""age"":23}'
k.url.postData(""http://www.kooboo.com/fakereceiver"", data); ")]
        public string postform(string url, string data)
        {
            return _PostForm(url, data).Result;
        }

        [Description(@"Post form data to remote url
var data = {
    name:'abc',
    age:123
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data); ")]
        public string postform(string url, object data)
        {
            string json = JsonHelper.Serialize(data);
            return _PostForm(url, json).Result;
        }

        [Description(@"Post form data to remote url
var data = '{""name"":""abc"", ""age"":23}'
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'xxx'); ")]
        public string postform(string url, string data, string token)
        {
            return _PostForm(url, data, BuildBearerHeader(token)).Result;
        }

        [Description(@"Post form data to remote url
var data = {
    name:'abc',
    age:123
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'xxx'); ")]
        public string postform(string url, object data, string token)
        {
            string json = JsonHelper.Serialize(data);
            return _PostForm(url, json, BuildBearerHeader(token)).Result;
        }

        [Description(@"Post form data to remote url
var data = '{""name"":""abc"", ""age"":23}'
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'admin','abc123'); ")]
        public string postform(string url, string data, string userName, string password)
        {
            return _PostForm(url, data, BuildBasicHeader(userName, password)).Result;
        }

        [Description(@"Post form data to remote url
var data = {
    name:'abc',
    age:123
}
k.url.postData(""http://www.kooboo.com/fakereceiver"", data,'admin','abc123'); ")]
        public string postform(string url, object data, string userName, string password)
        {
            string json = JsonHelper.Serialize(data);
            return _PostForm(url, json, BuildBasicHeader(userName, password)).Result;
        }

        #endregion

        #region downloadZip

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

        #endregion

        #region helper
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

        private static async Task<string> _get(string url, Dictionary<string, string> query = null, IDictionary<string, object> headers = null)
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

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        requestMessage.Headers.Add(item.Key, item.Value?.ToString());
                    }
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

        private static async Task<string> _Post(string url, string json, IDictionary<string, object> headers = null)
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

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        requestMessage.Headers.Add(item.Key, item.Value?.ToString());
                    }
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

        private async Task<string> _PostForm(string url, string json, IDictionary<string, object> headers = null)
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

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        httpRequestMessage.Headers.Add(item.Key, item.Value?.ToString());
                    }
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

        private static Dictionary<string, object> BuildBearerHeader(string token)
        {
            var header = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(token))
            {
                header.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {token}");
            }

            return header;
        }

        private static Dictionary<string, object> BuildBasicHeader(string username, string password)
        {
            var header = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", username, password));
                header.Add(HttpRequestHeader.Authorization.ToString(), "Basic " + Convert.ToBase64String(bytes));
            }

            return header;
        }

        #endregion
    }
}
