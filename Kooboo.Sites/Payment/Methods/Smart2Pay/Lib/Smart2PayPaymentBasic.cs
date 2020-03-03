using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Smart2Pay.Lib
{
    public class Smart2PayPaymentBasic
    {
        [JsonProperty("MerchantTransactionID")]
        public string MerchantTransactionId { get; set; }

        public long? Amount { get; set; }

        public string Currency { get; set; }

        [JsonProperty("MethodID", NullValueHandling = NullValueHandling.Ignore)]
        public int? MethodId { get; set; }

        [JsonProperty("ReturnURL")]
        public string ReturnUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TokenLifetime { get; set; }

        public string Description { get; set; }

        public CustomerInfo Customer { get; set; }

        public class CustomerInfo
        {
            public string Email { get; set; }
        }
    }
}
