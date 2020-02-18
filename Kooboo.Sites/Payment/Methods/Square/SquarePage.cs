using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquarePage : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquarePage";

        public string DisplayName => throw new NotImplementedException();

        public string Icon => throw new NotImplementedException();

        public string IconType => throw new NotImplementedException();

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("USD");
                return list;
            }
        }

        public RenderContext Context { get; set; }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            if (this.Setting == null)
            {
                return null;
            }

            Money amount = new Money(500L, "USD");

            var result = PaymentsApi.Pay(request.SquareResponseNonce, amount, Setting.AccessToken, Setting.PaymentURL);

            return res;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
