using Newtonsoft.Json;
using System.Collections.Generic;
namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class CreateHostedCheckoutResponse
    {
        [JsonProperty(PropertyName = "RETURNMAC")]
        public string RETURNMAC { get; set; } = null;

        public string HostedCheckoutId { get; set; } = null;

        public IList<string> InvalidTokens { get; set; } = null;

        public string MerchantReference { get; set; } = null;

        public string PartialRedirectUrl { get; set; } = null;
    }
}
