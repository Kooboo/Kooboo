using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class KlarnaApi
    {
        public static KpSessionResponse CreateKpSession(string endpoint, KpSessionRequest request, string userName,
            string password)
        {
            var response = Post(
                    $"https://{endpoint}/payments/v1/sessions",
                    JsonHelper.Serialize(request),
                    userName,
                    password)
                .Result;

            return JsonHelper.Deserialize<KpSessionResponse>(response);
        }

        public static HppSessionResponse CreateHppSession(string endpoint, string kpSessionId, string userName,
            string password, MerchantUrls merchantUrls)
        {
            var request = new HppSessionRequest
            {
                PaymentSessionUrl = $"https://{endpoint}/payments/v1/sessions/{kpSessionId}",
                MerchantUrls = merchantUrls,
            };
            var response = Post(
                    $"https://{endpoint}/hpp/v1/sessions",
                    JsonHelper.Serialize(request),
                    userName,
                    password)
                .Result;

            return JsonHelper.Deserialize<HppSessionResponse>(response);
        }

        public static CheckStatusResponse CheckStatus(string endpoint, string hppSessionId)
        {
            var response = Get($"https://{endpoint}/hpp/v1/sessions/{hppSessionId}")
                .Result;

            return JsonHelper.Deserialize<CheckStatusResponse>(response);
        }

        public static async Task<string> Post(string url, string jsonBody, string userName = null,
            string password = null)
        {
            using (var client = new HttpClient())
            {
                if (userName != null && password != null)
                {
                    var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", b64);
                }

                var resp = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                return await ReadResponse(resp);
            }
        }

        public static async Task<string> Get(string url, string userName = null, string password = null)
        {
            using (var client = new HttpClient())
            {
                if (userName != null && password != null)
                {
                    var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", b64);
                }

                var resp = await client.GetAsync(url);
                return await ReadResponse(resp);
            }
        }

        private static async Task<string> ReadResponse(HttpResponseMessage resp)
        {
            var response = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return response;
            }

            var error = JsonHelper.Deserialize<ErrorResponse>(response);
            throw new HttpRequestException(string.Join(";", error.ErrorMessages));
        }
    }
}