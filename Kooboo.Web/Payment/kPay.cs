using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Payment
{
    public class kPay : IkScript
    {
        public string Name => "Payment";

        public RenderContext context { get; set; }

        [KIgnore]
        public KPaymentMethodMapper this[string key]
        {
            get
            {
                var PaymentMethodType = Kooboo.Web.Payment.PaymentManager.GetMethodType(key);
                if (PaymentMethodType != null)
                {
                    var sitedb = this.context.WebSite.SiteDb();

                    var paymentmethod = Activator.CreateInstance(PaymentMethodType) as IPaymentMethod;
                    paymentmethod.Context = this.context;

                    if (Lib.Reflection.TypeHelper.HasGenericInterface(PaymentMethodType, typeof(IPaymentMethod<>))) 
                    { 
                        
                        var settingtype = Lib.Reflection.TypeHelper.GetGenericType(PaymentMethodType);

                        if (settingtype != null)
                        {
                            var settingvalue = sitedb.CoreSetting.GetSiteSetting(settingtype) as IPaymentSetting;
                            //Setting
                            var setter = Lib.Reflection.TypeHelper.GetSetObjectValue("Setting", PaymentMethodType, settingtype);
                            setter(paymentmethod, settingvalue);
                        }

                    }
 

                    KPaymentMethodMapper method = new KPaymentMethodMapper();
                    method.Context = this.context;
                    method.PaymentMethod = paymentmethod;
                    return method;
                }
                return null;
            }
            set { }
        }

        [KExtension]
        static KeyValuePair<string, Type>[] _ = PaymentContainer.PaymentMethods.Select(s => new KeyValuePair<string, Type>(s.Name, s.GetType())).ToArray();



    }
}