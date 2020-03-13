using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Braintree.lib;
using Kooboo.Sites.Payment.Methods.Ogone.lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Ogone
{
    public class OgoneForm : IPaymentMethod<OgoneSetting>
    {
        public OgoneSetting Setting { get; set; }

        public string Name => "OgoneForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("ogone", Context);

        public string IconType => "img";

        public string Icon => "";

        public List<string> supportedCurrency => new List<string>();

        public RenderContext Context { get; set; }

        [Description(@"
<script engine='kscript'>
var charge = {};
charge.total = 1500; 
charge.currency='USD';
charge.Country='US';
charge.UserIp = 'kooboo',
charge.name = 'Tea from Xiamen'; 
var resForm = k.payment.ogoneForm.charge(charge);  
var url = resForm.redirectUrl;
k.response.redirect(url);
</script> ")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            RedirectResponse res = null;
            if (Setting == null)
                return res;

            var ogoneApi = new OgoneApi(Setting);

            var checkoutRequest = new CreateHostedCheckoutRequest
            {
                Order = new Order
                {
                    AmountOfMoney = new AmountOfMoney
                    {
                        Amount = (request.TotalAmount * 100).ToString("0"),
                        CurrencyCode = request.Currency
                    },
                    Customer = new Customer
                    {
                        BillingAddress = new Address
                        {
                            CountryCode = request.Country
                        },
                        MerchantCustomerId = Name
                    }
                }
            };
            var result = ogoneApi.Hostedcheckouts(checkoutRequest);
            if (result != null)
                res = new RedirectResponse(string.Format("{0}.{1}", Setting.BaseUrl(), result.PartialRedirectUrl), request.Id);
            return res;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public PaymentCallback Notify(RenderContext context)
        {
            throw new NotImplementedException();
        }
    }
}