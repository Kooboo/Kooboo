using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Payment
{
    public class ApiClient
    {
        public AuthenticationHeaderValue AuthenticationHeaderValue { get; private set; }

        #region Create

        private ApiClient() { }

        public static ApiClient Create()
        {
            return new ApiClient();
        }

        public static ApiClient Create(AuthenticationHeaderValue authentication)
        {
            return new ApiClient
            {
                AuthenticationHeaderValue = authentication
            };
        }

        public static ApiClient CreateWithBasicAuth(string userName, string password)
        {
            var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
            return Create(new AuthenticationHeaderValue("Basic", b64));
        }

        public static ApiClient Create(string authSchema, string authParameter)
        {
            return Create(new AuthenticationHeaderValue(authSchema, authParameter));
        }

        #endregion

        public async Task<ApiResponse> PostJsonAsync(
            string url,
            object body,
            string contentType = "application/json",
            Dictionary<string, string> headers = null,
            bool withAuth = true)
        {
            var content = body == null ? null : JsonHelper.Serialize(body);
            return await PostAsync(url, content, contentType, headers, withAuth);
        }

        public async Task<ApiResponse> PostAsync(
            string url,
            string body,
            string contentType = "application/json",
            Dictionary<string, string> headers = null,
            bool withAuth = true)
        {
            var content = string.IsNullOrWhiteSpace(body) ? null : new StringContent(body, Encoding.UTF8, contentType);
            return await SendAsync(HttpMethod.Post, url, content, headers, withAuth);
        }

        public async Task<ApiResponse> GetAsync(
            string url,
            Dictionary<string, string> headers = null,
            bool withAuth = true)
        {
            return await SendAsync(HttpMethod.Get, url, null, headers, withAuth);
        }

        /// <summary>
        /// Send custom api request
        /// </summary>
        public async Task<ApiResponse> SendAsync(
            HttpMethod method,
            string url,
            HttpContent content = null,
            Dictionary<string, string> headers = null,
            bool withAuth = true)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, url)
                {
                    Content = content
                };

                if (withAuth)
                {
                    request.Headers.Authorization = AuthenticationHeaderValue;
                }

                if (headers?.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var resp = await client.SendAsync(request);
                return await ReadResponse(resp);
            }
        }

        private static async Task<ApiResponse> ReadResponse(HttpResponseMessage resp)
        {
            var response = await resp.Content.ReadAsStringAsync();
            return new ApiResponse
            {
                StatusCode = resp.StatusCode,
                Content = response
            };
        }

        public class ApiResponse
        {
            public HttpStatusCode StatusCode { get; set; }

            public bool IsSuccessStatusCode =>
                (StatusCode >= HttpStatusCode.OK) && (StatusCode <= (HttpStatusCode)299);

            public string Content { get; set; }

            public ApiResponse EnsureSuccessStatusCode()
            {
                if (!IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Http request failed with status code: {(int)StatusCode}");
                }

                return this;
            }

            public T ReadAs<T>() where T : class
            {
                return JsonHelper.Deserialize<T>(Content);
            }
        }
    }
}
