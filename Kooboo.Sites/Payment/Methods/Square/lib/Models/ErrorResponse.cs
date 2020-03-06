using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models
{
    public class ErrorResponse
    {
        [JsonProperty("errors")]
        public List<ErrorDetail> ErrorDetails { get; set; }
    }

    public class ErrorDetail
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }
    }
}
