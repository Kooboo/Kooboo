using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenPaymentRequest
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("amount")]
        public AdyenAmount Amount { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("merchantAccount")]
        public string MerchantAccount { get; set; }

        [JsonProperty("returnUrl")]
        public string ReturnUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
