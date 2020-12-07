using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Braintree
{
    public class BraintreeSetting : IPaymentSetting,ISettingDescription
    {
        public string Name => "Braintree";

        public string MerchantId { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public string SucceedRedirectURL { get; set; }

        public string FailureRedirectURL { get; set; }

        public bool UseSandBox { get; set; }

        public string ServerUrl
        {
            get
            {
                if (UseSandBox)
                {
                    return "https://api.sandbox.braintreegateway.com";
                }
                return "https://api.braintreegateway.com";

            }
        }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
