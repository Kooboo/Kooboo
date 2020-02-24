using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe
{
    public class StripeFormSetting : IPaymentSetting
    {
        private const string name = "StripPayment";

        public string Name => name;
     }
}
