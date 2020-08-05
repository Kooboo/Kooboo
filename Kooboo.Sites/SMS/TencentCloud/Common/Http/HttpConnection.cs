/*
 * Copyright (c) 2018 THL A29 Limited, a Tencent company. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TencentCloud.Common.Http
{
    public class HttpConnection
    {
        private HttpClient client;

        public HttpConnection(string baseUrl, int timeout)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeout);
        }

        private static string AppendQuery(StringBuilder builder, Dictionary<string, string> param)
        {
            foreach (KeyValuePair<string, string> kvp in param)
            {
                builder.Append($"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}&");
            }
            return builder.ToString().TrimEnd('&');
        }

        public async Task<HttpResponseMessage> GetRequest(string url, Dictionary<string, string> param)
        {
            var urlBuilder = new StringBuilder($"{url}?");
            string fullUrl = AppendQuery(urlBuilder, param);
            var response = await client.GetAsync(fullUrl);
            return response;
        }

        public async Task<HttpResponseMessage> GetRequest(string path, string queryString, Dictionary<string, string> headers)
        {
            var urlBuilder = new StringBuilder($"{path}?{queryString}");
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(urlBuilder.ToString());

            request.Headers.Add("Authorization", $"TC3-HMAC-SHA256 {headers["Authorization"].Substring("TC3-HMAC-SHA256".Length)}");
            headers.Remove("Authorization");
            headers.Remove("Content-Type");
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }

            var response = await client.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> PostRequest(string path, string payload, Dictionary<string, string> headers)
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(path);

            request.Headers.Add("Authorization", $"TC3-HMAC-SHA256 {headers["Authorization"].Substring("TC3-HMAC-SHA256".Length)}");
            headers.Remove("Authorization");
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }

            request.Content = new StringContent(payload, Encoding.UTF8, headers["Content-Type"]);
            headers.Remove("Content-Type");

            var response = await client.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> PostRequest(string url, Dictionary<string, string> param)
        {
            var bodysb = new StringBuilder();
            string body = AppendQuery(bodysb, param);

            var response = await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
            return response;
        }
    }
}
