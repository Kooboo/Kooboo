using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe
{
    public class StripeFormSetting : IPaymentSetting, ISettingDescription
    {
        private const string name = "StripePayment";

        public string Name => name;

        public string SuccessUrl { get; set; }

        public string CancelUrl { get; set; }

        public string Secretkey { get; set; }

        public string Publishablekey { get; set; }

        public string WebhookSigningSecret { get; set; }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
