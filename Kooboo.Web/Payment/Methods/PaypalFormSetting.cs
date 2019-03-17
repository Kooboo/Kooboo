using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment.Methods
{
    public class PaypalFormPaymentSetting : IPaymentSetting
    {
        public string PaypalUrl => "https://www.paypal.com/cgi-bin/webscr";

        public string PaypalSandboxUrl => "https://www.sandbox.paypal.com/cgi-bin/webscr";
         
        public string IPNUrl => "https://ipnpb.paypal.com/cgi-bin/webscr"; 

        public string EmailAddress { get; set; }

        public bool UseSandBox { get; set; }

        public string RuturnUrl { get; set; }

        public string CancelUrl { get; set; }

        public string LogoImage { get; set; }
          
        public string Name =>"PaypalFormPayment";
    }
}
