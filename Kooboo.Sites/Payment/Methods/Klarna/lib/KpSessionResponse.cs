using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Lib.Helper;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class KpSessionResponse
    {
        /// <summary>
        /// default is kpSessionId, if request with place order, this is a kcoOrderId
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("client_token")]
        public string ClientToken { get; set; }

        [JsonProperty("payment_method_categories")]
        public PaymentMethodCategory[] PaymentMethodCategories { get; set; }

        public class PaymentMethodCategory
        {
            [JsonProperty("identifier")]
            public string Identifier { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("asset_urls")]
            public AssetUrls AssetUrls { get; set; }
        }

        public class AssetUrls
        {
            [JsonProperty("descriptive")]
            public string Descriptive { get; set; }

            [JsonProperty("standard")]
            public string Standard { get; set; }
        }
    }

}
