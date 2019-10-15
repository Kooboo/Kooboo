using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Web.Payment
{
    public class kPay : IkScript
    {
        public string Name => "Payment";

        public RenderContext Context { get; set; }

        public KPaymentMethod this[string key]
        {
            get
            {
                var paymentmethod = Kooboo.Web.Payment.PaymentManager.GetMethod(key);
                if (paymentmethod != null)
                {
                    KPaymentMethod method = new KPaymentMethod { Context = this.Context, PaymentMethod = paymentmethod };
                    return method;
                }
                return null;
            }
            set { }
        }
    }
}