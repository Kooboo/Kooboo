using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class AddFundingSourceResponse
    {
        [JsonProperty(PropertyName = "_links")]
        public AddFundingSourceStatus Links { get; set; }

        [JsonProperty(PropertyName = "money")]
        public Money Money { get; set; }

        [JsonProperty(PropertyName = "requestId")]
        public Guid RequestId { get; set; }
    }
}
