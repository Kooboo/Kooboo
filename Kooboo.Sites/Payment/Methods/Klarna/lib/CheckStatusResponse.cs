using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class CheckStatusResponse
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public KlarnaStatus Status { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}