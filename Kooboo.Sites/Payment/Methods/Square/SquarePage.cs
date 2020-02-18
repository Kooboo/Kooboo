using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Square.lib;
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

            PaymentsApi.Pay(request.SquareResponseNonce, Setting.AccessToken);

            return res;
        }

        public void GetPage()
        {

        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
