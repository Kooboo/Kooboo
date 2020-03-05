using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    public class Links
    {
        [JsonProperty("self")]
        public Link Self { get; set; }

        [JsonProperty("checkout")]
        public Link Checkout { get; set; }

        [JsonProperty("changePaymentState")]
        public Link ChangePaymentState { get; set; }

        [JsonProperty("refunds")]
        public Link Refunds { get; set; }

        [JsonProperty("order")]
        public Link Order { get; set; }

        [JsonProperty("documentation")]
        public Link Documentation { get; set; }
        
        public class Link
        {
            [JsonProperty("href")]
            public string Href { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}