using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    public class Amount
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}