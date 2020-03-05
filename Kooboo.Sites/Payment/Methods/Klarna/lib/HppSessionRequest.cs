using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class HppSessionRequest
    {
        [JsonProperty("payment_session_url")]
        public string PaymentSessionUrl { get; set; }

        [JsonProperty("merchant_urls")]
        public MerchantUrls MerchantUrls { get; set; }

        /// <summary>
        /// if need to request with place order, pass in an instance of Option.
        /// </summary>
        [JsonProperty("options")]
        public Option Options { get; set; }

        public class Option
        {
            [JsonProperty("place_order_mode")]
            public string PlaceOrderMode => "PLACE_ORDER";
        }
    }
}