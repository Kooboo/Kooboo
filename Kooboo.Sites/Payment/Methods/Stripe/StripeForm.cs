using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe
{
    public class StripeForm : IPaymentMethod<StripeFormSetting>
    {
        public StripeFormSetting Setting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => throw new NotImplementedException();

        public string DisplayName => throw new NotImplementedException();

        public string Icon => throw new NotImplementedException();

        public string IconType => throw new NotImplementedException();

        public List<string> supportedCurrency => throw new NotImplementedException();

        public RenderContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
