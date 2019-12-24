using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Payment
{
    public class kPay : IkScript
    {
        public string Name => "Payment";

        public RenderContext context { get; set; }

        [Ignore]
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

        [Ignore, Extension]
        public static KeyValuePair<string, Type>[] PaymentMethods => PaymentContainer.PaymentMethods.Select(s => new KeyValuePair<string, Type>(s.Name, s.GetType())).ToArray();
    }
}