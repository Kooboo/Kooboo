using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class ErrorResponse
    {
        [JsonProperty("error_code")]
        public string ErrorCode { get; set; }

        [JsonProperty("error_messages")]
        public string[] ErrorMessages { get; set; }
    
        [JsonProperty("correlation_id")]
        public string CorrelationId { get; set; }
    }
}