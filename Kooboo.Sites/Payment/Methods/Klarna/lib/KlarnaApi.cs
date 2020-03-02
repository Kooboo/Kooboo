using System.Net.Http;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class KlarnaApi
    {
        private readonly string _endpoint;
        private readonly ApiClient _client;

        public KlarnaApi(KlarnaHppSetting klarnaHppSetting, string country)
        {
            _endpoint = klarnaHppSetting.GetEndpoint(country);
            _client = ApiClient.CreateWithBasicAuth(klarnaHppSetting.UserName, klarnaHppSetting.Password, Encoding.UTF8);
        }

        public KpSessionResponse CreateKpSession(KpSessionRequest request)
        {
            var response = _client.PostJsonAsync($"https://{_endpoint}/payments/v1/sessions", request).Result;

            return DeserializeResponse<KpSessionResponse>(response);
        }

        public HppSessionResponse CreateHppSession(string kpSessionId, MerchantUrls merchantUrls)
        {
            var request = new HppSessionRequest
            {
                PaymentSessionUrl = $"https://{_endpoint}/payments/v1/sessions/{kpSessionId}",
                MerchantUrls = merchantUrls,
            };
            var response = _client.PostJsonAsync($"https://{_endpoint}/hpp/v1/sessions", request).Result;

            return DeserializeResponse<HppSessionResponse>(response);
        }

        public CheckStatusResponse CheckStatus(string hppSessionId)
        {
            var response = _client.GetAsync($"https://{_endpoint}/hpp/v1/sessions/{hppSessionId}").Result;

            return DeserializeResponse<CheckStatusResponse>(response);
        }

        private T DeserializeResponse<T>(ApiClient.ApiResponse resp) where T : class
        {
            if (resp.IsSuccessStatusCode)
            {
                return resp.ReadAs<T>();
            }

            var error = resp.ReadAs<ErrorResponse>();
            throw new HttpRequestException(string.Join(";", error.ErrorMessages));
        }
    }
}