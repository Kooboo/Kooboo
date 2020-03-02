using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class ClientTokenRequest
    {
        [JsonProperty("client-token")]
        public ClientToken CientToken { get; set; }
    }

    public class ClientToken
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
