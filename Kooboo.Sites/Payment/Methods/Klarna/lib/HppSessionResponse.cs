using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Klarna.lib
{
    public class HppSessionResponse
    {
        /// <summary>
        /// Generated identifier for the Session, HPP Session ID.
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// Use this URL to redirect directly the Consumer’s browser to the Payment Page or the HPP Session you just created. This URL is public and doesn’t need any credential. 
        /// </summary>
        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Use this URL to read the HPP Session. This endpoint requires Merchant Credentials to accept requests.
        /// </summary>
        [JsonProperty("session_url")]
        public string SessionUrl { get; set; }

        /// <summary>
        /// Use this URL to display a QR Code to your Consumer, so that they will access the Payment Page or the HPP Session you just created by scanning it with their phone.
        /// </summary>
        [JsonProperty("qr_code_url")]
        public string QrCodeUrl { get; set; }

        /// <summary>
        /// Use this URL to distribute to your Consumer the HPP Session you just created by Email or SMS. Learn more in the distribution of the HPP Session chapter(https://developers.klarna.com/documentation/hpp/guides/distribute-session). This endpoint requires Merchant Credentials to accept requests.
        /// </summary>
        [JsonProperty("distribution_url")]
        public string DistributionUrl { get; set; }

        /// <summary>
        /// Date when this session will not be able for the consumer to pay anymore.
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("distribution_module")]
        public DistributionModuleClass DistributionModule { get; set; }

        public class DistributionModuleClass
        {
            [JsonProperty("token")]
            public string Token { get; set; }
         
            [JsonProperty("standalone_url")]
            public string StandaloneUrl { get; set; }
            
            [JsonProperty("generation_url")]
            public string GenerationUrl { get; set; }
        }
    }
}