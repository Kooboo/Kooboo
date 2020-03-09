using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenPaymentResponse
    {
        [JsonProperty("amount")]
        public AdyenAmount Amount { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
