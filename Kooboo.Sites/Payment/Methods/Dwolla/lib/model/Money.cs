using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{

    public class Money
    {
        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}
