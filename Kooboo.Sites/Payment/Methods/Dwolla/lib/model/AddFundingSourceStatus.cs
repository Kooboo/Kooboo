using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class AddFundingSourceStatus
    {
        [JsonProperty(PropertyName = "funding-source")]
        public Link FundingSource { get; set; }

        [JsonProperty(PropertyName = "verify-micro-deposits")]
        public Link VerifyMicroDeposits { get; set; }
    }
}
