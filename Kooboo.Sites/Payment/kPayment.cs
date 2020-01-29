using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Payment
{
     
    public class kPay : IkScript
    {

        [KIgnore]
        public string Name => "Payment";

        [KIgnore]
        public RenderContext context { get; set; }

        [KIgnore]
        public KPaymentMethodWrapper this[string key]
        {
            get
            {
                return Get(key); 
            } 
        }

       public KPaymentMethodWrapper Get(string PaymentMothod)
        {
            var paymentmethod = PaymentManager.GetMethod(PaymentMothod, this.context);
            if (paymentmethod != null)
            {
                KPaymentMethodWrapper method = new KPaymentMethodWrapper(paymentmethod, this.context); 
                return method;
            }
            return null;
        }

        [KExtension]
        static KeyValuePair<string, Type>[] _ = PaymentContainer.PaymentMethods.Select(s => new KeyValuePair<string, Type>(s.Name, s.GetType())).ToArray();  

    }
}