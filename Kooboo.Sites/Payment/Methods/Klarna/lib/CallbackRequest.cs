using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class CallbackRequest
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("session")]
        public CallbackDetail Session { get; set; }

        public class CallbackDetail
        {
            [JsonProperty("session_id")]
            public string SessionId { get; set; }

            [JsonProperty("status")]
            [JsonConverter(typeof(StringEnumConverter))]
            public KlarnaStatus Status { get; set; }

            [JsonProperty("authorization_token")]
            public string AuthorizationToken { get; set; }

            [JsonProperty("order_id")]
            public string OrderId { get; set; }

            [JsonProperty("klarna_reference")]
            public string KlarnaReference { get; set; }

            [JsonProperty("updated_at")]
            public DateTime UpdatedAt { get; set; }

            [JsonProperty("expires_at")]
            public DateTime ExpiresAt { get; set; }
        }
    }
}