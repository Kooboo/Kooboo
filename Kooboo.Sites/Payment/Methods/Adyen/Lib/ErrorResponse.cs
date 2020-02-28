using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class ErrorResponse
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("errorType")]
        public string ErrorType { get; set; }
    }
}
