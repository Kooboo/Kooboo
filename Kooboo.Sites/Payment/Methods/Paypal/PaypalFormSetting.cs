using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class PaypalFormSetting : IPaymentSetting, ISettingDescription
    {
        public string PaypalUrl
        {
            get
            {
                if (UseSandBox)
                {
                    return "https://www.sandbox.paypal.com/cgi-bin/webscr";
                }
                return "https://www.paypal.com/cgi-bin/webscr";

            }
        }

        public string IPNUrl
        {
            get
            {
                if (UseSandBox)
                {
                    return "https://www.sandbox.paypal.com/cgi-bin/webscr";
                }
                return "https://ipnpb.paypal.com/cgi-bin/webscr";
            }
        }

        public string EmailAddress { get; set; }

        public bool UseSandBox { get; set; }

        public string ReturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public string LogoImage { get; set; }

        public string Name => "PaypalFormPayment";

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
