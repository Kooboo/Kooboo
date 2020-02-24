using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.qualpay.lib
{
    public class ApiClient
    {
        public static string Post(string url, string body,string securityKey)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", securityKey);
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
            var httpContent = new StringContent(body, Encoding.Default, "application/json");
            var apiResult = httpClient.PostAsync(url, httpContent);
            apiResult.Wait();
            apiResult.Result.EnsureSuccessStatusCode();
            var res = apiResult.Result.Content.ReadAsStringAsync();
            res.Wait();
            return res.Result;
        }
    }
}
