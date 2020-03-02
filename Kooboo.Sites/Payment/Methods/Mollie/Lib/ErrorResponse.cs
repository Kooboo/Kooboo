using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    public  class ErrorResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }
}
