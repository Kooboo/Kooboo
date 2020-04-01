using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Payment.Methods.Square.lib
{
    public class PaymentsApi
    {
        // https://developer.squareup.com/reference/square/payments-api/create-payment
        public static PaymentResponse CreatPayment(string nonce, Money amount, SquareSetting setting, string requestIdStr)
        {
            var queryUrl = setting.BaseURL + "/v2/payments";

            var uuid = Guid.NewGuid().ToString();

            var body = new Models.PaymentRequest { SourceId = nonce, IdempotencyKey = uuid, AmountMoney = amount, ReferenceId = requestIdStr };

            var response = ApiClient.Create("Bearer", setting.AccessToken)
                .PostJsonAsync(queryUrl, body).Result;

            return DeserializeResponse<PaymentResponse>(response);
        }

        // https://developer.squareup.com/docs/checkout-api-overview
        public static CreateCheckoutResponse CheckoutCreatOrder(CreateCheckoutRequest request, SquareSetting setting)
        {
            var queryUrl = setting.BaseURL + "/v2/locations/" + setting.LocationId + "/checkouts";

            var response = ApiClient.Create("Bearer", setting.AccessToken)
                   .PostJsonAsync(queryUrl, request).Result;

            return DeserializeResponse<CreateCheckoutResponse>(response);
        }

        // https://developer.squareup.com/reference/square/orders-api/batch-retrieve-orders#type-orderstate
        public static CheckOrderResponse CheckOrder(CheckOrderRequest orderRequest, SquareSetting setting)
        {
            var queryUrl = setting.BaseURL + "/v2/locations/" + setting.LocationId + "/orders/batch-retrieve";

            var response = ApiClient.Create("Bearer", setting.AccessToken)
                .PostJsonAsync(queryUrl, orderRequest).Result;

            return DeserializeResponse<CheckOrderResponse>(response);
        }

        private static T DeserializeResponse<T>(ApiClient.ApiResponse resp) where T : class
        {
            if (resp.IsSuccessStatusCode)
            {
                return resp.ReadAs<T>();
            }

            var error = resp.ReadAs<ErrorResponse>();
            throw new HttpRequestException($"Error Code: {error.ErrorDetails[0].Code}; filed: {error.ErrorDetails[0].Field}; detail: {error.ErrorDetails[0].Detail}.");
        }
    }
}
