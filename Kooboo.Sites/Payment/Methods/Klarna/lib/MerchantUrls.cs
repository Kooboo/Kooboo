using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    // https://developers.klarna.com/documentation/hpp/api/create-session/#merchants-urls
    public class MerchantUrls
    {
        /// <summary>
        /// Consumer will get redirected there after a successful authorization of payment for both KP and KCO. When using KP as Payment Provider, a place holder will be required to get the KP Authorization Token to place the order.
        /// </summary>
        [JsonProperty("success")]
        public string Success { get; set; }

        /// <summary>
        /// Consumer will get redirected there when clicking on the cancellation button. See back button versus cancel button chapter.
        /// </summary>
        [JsonProperty("cancel")]
        public string Cancel { get; set; }

        /// <summary>
        /// Consumer will get redirected there when clicking on the back button. See back button versus cancel button chapter.
        /// </summary>
        [JsonProperty("back")]
        public string Back { get; set; }

        /// <summary>
        /// Consumer will get redirected there when payment is refused by Klarna. If an error occurs and no <c>error</c> URL was given, then the consumer will also get redirect to this URL.
        /// </summary>
        [JsonProperty("failure")]
        public string Failure { get; set; }

        /// <summary>
        /// Consumer will get redirected there when an error occurred in the flow. If this parameter is not set and a <c>failure</c> URL is present, the Consumer will get redirected there.
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }

        /// <summary>
        /// Url that will be used for callbacks. https://developers.klarna.com/documentation/hpp/api/status-callbacks
        /// </summary>
        [JsonProperty("status_update")]
        public string StatusUpdate { get; set; }
    }
}