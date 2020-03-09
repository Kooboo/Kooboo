using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class ClientTokenResponse
    {
        [JsonProperty("clientToken")]
        public Token CientToken { get; set; }
    }

    public class Token
    {
        [JsonProperty("value")]
        public string value { get; set; }
    }
}
