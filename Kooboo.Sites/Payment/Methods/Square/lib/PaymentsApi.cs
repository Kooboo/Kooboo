using System;
using System.IO;
using System.Net;
using System.Text;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Methods.Square.lib
{
    public class PaymentsApi
    {
        // https://developer.squareup.com/reference/square/payments-api/create-payment
        public static string CreatPayment(string nonce, Money amount, SquareSetting setting)
        {
            var queryUrl = setting.BaseURL + "/v2/payments";

            var uuid = Guid.NewGuid().ToString();

            var body = new Models.PaymentRequest { SourceId = nonce, IdempotencyKey = uuid, AmountMoney = amount };

            return ApiClient.Create("Bearer", setting.AccessToken)
                .PostAsync(queryUrl, JsonSerialize(body)).Result.Content;
        }

        // https://developer.squareup.com/docs/checkout-api-overview
        public static string CheckoutCreatOrder(CreateCheckoutRequest request, SquareSetting setting)
        {
            var queryUrl = setting.BaseURL + "/v2/locations/" + setting.LocationId + "/checkouts";

            return ApiClient.Create("Bearer", setting.AccessToken)
                .PostAsync(queryUrl, JsonSerialize(request)).Result.Content;
        }

        // https://developer.squareup.com/reference/square/orders-api/batch-retrieve-orders#type-orderstate
        public static string CheckOrder(CheckOrderRequest orderRequest, SquareSetting setting)
        {
            var queryUrl = setting.BaseURL + "/v2/locations/" + setting.LocationId + "/orders/batch-retrieve";

            return ApiClient.Create("Bearer", setting.AccessToken)
                .PostAsync(queryUrl, JsonSerialize(orderRequest)).Result.Content;
        }

        public static string JsonSerialize(object obj, JsonConverter converter = null)
        {
            if (null == obj)
            {
                return null;
            }

            var settings = new JsonSerializerSettings();

            if (converter == null)
            {
                settings.Converters.Add(new IsoDateTimeConverter());
            }
            else
            {
                settings.Converters.Add(converter);
            }

            return JsonConvert.SerializeObject(obj, Formatting.None, settings);
        }
    }
}
