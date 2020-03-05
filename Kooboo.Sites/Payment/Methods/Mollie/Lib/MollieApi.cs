using System.Net.Http;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    public class MollieApi
    {
        private readonly ApiClient _client;

        public MollieApi(string apiToken)
        {
            _client = ApiClient.Create("Bearer", apiToken);
        }

        public MollieResponse CreatePayment(MolliePaymentRequest molliePaymentRequest)
        {
            var response = _client.PostJsonAsync("https://api.mollie.com/v2/payments", molliePaymentRequest).Result;
            return DeserializeResponse<MollieResponse>(response);
        }

        public MollieResponse CheckStatus(string paymentId)
        {
            var body = _client.GetAsync($"https://api.mollie.com/v2/payments/{paymentId}").Result;
            return DeserializeResponse<MollieResponse>(body);
        }

        private static T DeserializeResponse<T>(ApiClient.ApiResponse resp) where T : class
        {
            if (resp.IsSuccessStatusCode)
            {
                return resp.ReadAs<T>();
            }

            var error = resp.ReadAs<ErrorResponse>();
            throw new HttpRequestException($"Error Status: {error.Status}; filed: {error.Field}; detail: {error.Detail}; link: {error.Links.Documentation.Href}.");
        }
    }
}