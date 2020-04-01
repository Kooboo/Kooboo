using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone
{
    public class OgoneSetting : IPaymentSetting
    {
        public string Name => "OgonePay";

        public bool UseSandBox { get; set; }

        public string MerchantId { get; set; }

        public string ApiKeyId { get; set; }

        public string SecretApiKey { get; set; }

        public string ServerURL => UseSandBox ? "https://eu.sandbox.api-ingenico.com" : "https://world.api-ingenico.com";

        public string ReturnUrl { get; set; }

        public string SecretKey { get; set; }

        public string KeyId { get; set; }

        public string BaseUrl()
        {
            return "https://payment";
        }
    }
}
