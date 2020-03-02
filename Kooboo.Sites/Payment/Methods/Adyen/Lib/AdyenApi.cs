using System.Net.Http;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenApi
    {
        private readonly ApiClient _client;
        private readonly string _checkoutEndpoint;

        public AdyenApi(AdyenSetting setting)
        {
            _client = ApiClient.Create().SetDefaultHeader("x-API-Key", setting.XApiKey);
            _checkoutEndpoint = setting.CheckoutEndpoint;
        }

        public AdyenPaymentResponse CreatePayment(AdyenPaymentRequest paymentRequest)
        {
            var response = _client.PostJsonAsync($"{_checkoutEndpoint}/v51/paymentLinks", paymentRequest).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.ReadAs<AdyenPaymentResponse>();
            }

            var error = response.ReadAs<ErrorResponse>();
            throw new HttpRequestException($"Error Code: {error.ErrorCode}. {error.Message}");
        }
    }
}
