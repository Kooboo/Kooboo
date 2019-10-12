using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Web.Payment
{
    public class kPay : IkScript
    {
        public string Name => "Payment";

        public RenderContext context { get; set; }

        public KPaymentMethod this[string key]
        {
            get
            {
                var paymentmethod = Kooboo.Web.Payment.PaymentManager.GetMethod(key);
                if (paymentmethod != null)
                {
                    KPaymentMethod method = new KPaymentMethod();
                    method.Context = this.context;
                    method.PaymentMethod = paymentmethod;
                    return method;
                }
                return null;
            }
            set { }
        }
    }
}