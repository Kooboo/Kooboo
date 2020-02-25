using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.qualpay
{
    public class QualpayFormSetting : IPaymentSetting
    {
        public string Name => "QualpayFormPayment";

        public string ServerUrl { get; set; }

        public string SuccessUrl { get; set; }

        public string FailureUrl { get; set; }

        public string MerchantId { get; set; }

        public string CheckoutProfileId { get; set; }

        public string SecurityKey { get; set; }
    }
}
