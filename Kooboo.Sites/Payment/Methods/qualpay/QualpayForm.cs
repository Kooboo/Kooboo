using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.qualpay.lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.qualpay
{
    public class QualpayForm : IPaymentMethod<QualpayFormSetting>
    {
        public string Name => "qualpayForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("qualpay", Context);

        public string Icon => "";//需要一张支付的图片

        // see:https://www.qualpay.com/developer/api/reference#country-codes
        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("CAD");
                list.Add("JPY");
                list.Add("GBP");
                list.Add("USD");
                list.Add("EUR");
                return list;
            }
        }

        public QualpayFormSetting Setting { get; set; }

        public string IconType => "img";

        public RenderContext Context { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 1.50; 
charge.currency='USD';
var resForm = k.payment.qualpayForm.charge(charge);
    var redirectUrl = resForm.redirectUrl;
</script>
 <div class='jumbotron'>
	 <iframe style = 'width:100%;height: 100%' frameborder='0' k-attributes='src redirectUrl'></iframe> 
</div>")]
        [KDefineType(Return = typeof(RedirectResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            if (this.Setting == null)
            {
                return null;
            }
            var dic = new Dictionary<string, object>();
            dic.Add("amt_tran", request.TotalAmount.ToString());
            var currency = GetCurrencyCode(request.Currency);
            dic.Add("tran_currency", currency);

            var result = QualpayAPI.CheckOutUrl(dic, Setting);
            var redirectUrl = result["checkout_link"];
            var requestId = new Guid(result["checkout_id"]);
            RedirectResponse res = new RedirectResponse(redirectUrl, requestId);

            return res;
        }

        private string GetCurrencyCode(string currency)
        {
            var currentCodes = new Dictionary<string, string>
            {
                {"JPY","392" },
                { "CAD","124"},
                { "GBP","826"},
                { "USD","840"},
                { "EUR","978"}
            };
            return currentCodes[currency];
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
