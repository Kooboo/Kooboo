using System.Net.Http;

namespace Kooboo.Sites.Payment.Methods.Smart2Pay.Lib
{
    public class Smart2PayApi
    {
        private readonly ApiClient _client;
        private readonly string _endpoint;

        public Smart2PayApi(Smart2PaySetting setting)
        {
            _client = ApiClient.CreateWithBasicAuth(setting.RestApiSiteId, setting.RestApiApiKey);
            _endpoint = setting.Endpoint;
        }

        public Smart2PayResponse CreatePayment(Smart2PayRequest smart2PayPaymentRequest)
        {
            var response = _client.PostJsonAsync($"{_endpoint}/v1/payments", smart2PayPaymentRequest).Result;
            return DeserializeResponse(response);
        }

        public Smart2PayResponse CheckStatus(long smart2PayPaymentId)
        {
            var response = _client.GetAsync($"{_endpoint}/v1/payments/{smart2PayPaymentId}").Result;
            return DeserializeResponse(response);
        }

        private static Smart2PayResponse DeserializeResponse(ApiClient.ApiResponse resp)
        {
            var response = resp.ReadAs<Smart2PayResponse>();
            if (resp.IsSuccessStatusCode)
            {
                return response;
            }

            var error = response.Payment.Status.Reasons?[0];
            throw new HttpRequestException($"Error Code: {error?.Code}; Info:{error?.Info}.");
        }
    }
}