using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class CreateTransferRequest
    {
        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, Link> Links { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public Money Amount { get; set; }

        public Dictionary<string, string> Metadata { get; set; }

        //public string CorrelationId { get; set; }
    }
}