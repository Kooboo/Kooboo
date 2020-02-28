using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenAmount
    {
        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
