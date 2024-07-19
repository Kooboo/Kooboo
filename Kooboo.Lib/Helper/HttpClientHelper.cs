//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Net;
using System.Net.Http;

namespace Kooboo.Lib.Helper
{
    public static class HttpClientHelper
    {

        public static HttpClient Client { get; private set; }

        private static CookieContainer _cookieContainer;

        static HttpClientHelper()
        {
            _cookieContainer = new CookieContainer();
            Client = CreateHttpClient(_cookieContainer);
        }

        public static void SetCookieContainer(CookieContainer cookieContainer, string url)
        {
            if (cookieContainer == null)
                return;
            Uri uri;
            if (cookieContainer != null && Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                var cookies = cookieContainer.GetCookies(uri);
                foreach (var cookieObj in cookies)
                {
                    var cookie = cookieObj as Cookie;
                    _cookieContainer.Add(uri, cookie);
                }
            }
        }
        public static HttpClient CreateHttpClient(CookieContainer cookieContainer)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            //ServicePointManager does not affect httpclient in dotnet core
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            handler.Proxy = null;
            handler.AllowAutoRedirect = true;
            handler.MaxConnectionsPerServer = 100;
            handler.UseCookies = false;

            HttpClient client = new HttpClient(handler);
            client.Timeout = new TimeSpan(0, 0, 45);

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            return client;
        }

        public static HttpClient CreateClientInstance()
        {
            return CreateClientInstance(45);
        }

        public static HttpClient CreateClientInstance(int TimeOutSecond)
        {
            HttpClientHandler handler = new HttpClientHandler();

            //ServicePointManager does not affect httpclient in dotnet core
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            handler.Proxy = null;
            handler.AllowAutoRedirect = true;

            HttpClient client = new HttpClient(handler);
            client.Timeout = new TimeSpan(0, 0, 45);

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            return client;
        }
    }
}
