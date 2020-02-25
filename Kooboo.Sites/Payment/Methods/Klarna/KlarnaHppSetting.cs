using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Klarna.lib;

namespace Kooboo.Sites.Payment.Methods.Klarna
{
    public class KlarnaHppSetting : IPaymentSetting
    {
        private const string name = "KlarnaHppPayment";
        public string Name => name;

        public bool UseSandBox { get; set; }

        public string EndpointEurope =>
            UseSandBox ? "api.playground.klarna.com" : "api.klarna.com";

        public string EndpointNorthAmerica =>
            UseSandBox ? "api-na.playground.klarna.com" : "api-na.klarna.com";

        public string EndpointOceania =>
            UseSandBox ? "api-oc.playground.klarna.com" : "api-oc.klarna.com";

        public string UserName { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Consumer will get redirected there after a successful authorization of payment for both KP and KCO. When using KP as Payment Provider, a place holder will be required to get the KP Authorization Token to place the order.
        /// </summary>
        public string Success { get; set; }

        /// <summary>
        /// Consumer will get redirected there when clicking on the cancellation button. See back button versus cancel button chapter.
        /// </summary>
        public string Cancel { get; set; }

        /// <summary>
        /// Consumer will get redirected there when clicking on the back button. See back button versus cancel button chapter.
        /// </summary>
        public string Back { get; set; }

        /// <summary>
        /// Consumer will get redirected there when payment is refused by Klarna. If an error occurs and no <c>error</c> URL was given, then the consumer will also get redirect to this URL.
        /// </summary>
        public string Failure { get; set; }

        /// <summary>
        /// Consumer will get redirected there when an error occurred in the flow. If this parameter is not set and a <c>failure</c> URL is present, the Consumer will get redirected there.
        /// </summary>
        public string Error { get; set; }

        // https://developers.klarna.com/documentation/hpp/api/create-session/#merchants-urls
        public MerchantUrls GetGetMerchantUrls(string callbackUrl, Guid requestId)
        {
            return new MerchantUrls
            {
                Back = Back,
                Cancel = Cancel,
                Error = Error,
                Failure = Failure,
                StatusUpdate = UrlHelper.AppendQueryString(callbackUrl, "secretToken", requestId.ToString()),
                Success = Success
            };
        }

        public string GetEndpoint(string country)
        {
            switch (country.Trim().ToUpper())
            {
                case "US":
                    return EndpointNorthAmerica;
                case "AU":
                    return EndpointOceania;
                case "GB":
                case "UK":
                case "CH":
                case "BE":
                case "DE":
                case "AT":
                case "NL":
                case "DK":
                case "NO":
                case "FI":
                case "SE":
                default:
                    return EndpointEurope;
            }
        }
    }
}
