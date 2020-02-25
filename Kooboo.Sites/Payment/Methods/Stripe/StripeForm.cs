using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Stripe.lib;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Stripe
{
    public class StripeForm : IPaymentMethod<StripeFormSetting>
    {
        public StripeFormSetting Setting { get; set; }

        private const string Description = @"Pay by stripe. Example:
<script engine=""kscript"">
  var charge = {};
  charge.successUrl = 'https://example.com/success'
  charge.cancelUrl = 'https://example.com/cancel'
  charge.name = 'T-shirt'
  charge.description = 'Comfortable cotton t-shirt'
  charge.totalAmount = 1500
  charge.currency = 'USD'
  charge.quantity = 2
  charge.paymentMethodType = 'card'
  var res = k.payment.stripeForm.charge(charge);
  var publishableKey = res.fieldValues.get(""publishableKey"");
</script>
<div k-content=""res.html""></div>
<div id = ""sessionId"" style=""display:none;"" k-content=""res.paymemtMethodReferenceId""></div>
<div id = ""publishableKey"" style=""display:none;"" k-content=""publishableKey""></div>
<script src = ""https://js.stripe.com/v3/"" ></script>
<script>
  var stripe = Stripe(document.getElementById('publishableKey').innerText);
  stripe.redirectToCheckout({
    sessionId: document.getElementById('sessionId').innerText
}).then(function (result) {
    console.log(result.error.message)
});
</script>";

        public string Name => "StripeForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Stripe", Context);

        public string Icon => "https://s3.us-east-2.amazonaws.com/upload-icon/uploads/icons/png/63467571551952112-64.png";

        public string IconType => "img";

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


        [Description(Description)]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            request.Additional.TryGetValue("quantity", out var quantity);
            request.Additional.TryGetValue("cancelUrl", out var cancelUrl);
            request.Additional.TryGetValue("successUrl", out var successUrl);
            request.Additional.TryGetValue("paymentMethodType", out var paymentMethodType);

            var lineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions {
                        Name = request.Name,
                        Description = request.Description,
                        Amount = long.Parse(request.TotalAmount.ToString()),
                        Currency = request.Currency,
                        Quantity = long.Parse(quantity.ToString())
                    }
                };

            var options = new SessionCreateOptions
            {
                SuccessUrl = (string)cancelUrl,
                CancelUrl = (string)successUrl,
                PaymentMethodTypes = new List<string> {
                    (string)paymentMethodType
                },
                LineItems = lineItems
            };

            var sessionId = CreateSession(options).Result;
            var response = new HiddenFormResponse
            {
                paymemtMethodReferenceId = sessionId
            };
            response.setFieldValues("publishableKey", Setting.Publishablekey);
            return response;
        }

        private async Task<string> CreateSession(SessionCreateOptions options)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Setting.Secretkey);
                
                var result = await httpClient.PostAsync("https://api.stripe.com/v1/checkout/sessions", new StringContent(StripeUtility.SessionDataToContentString(options), Encoding.UTF8, "application/x-www-form-urlencoded"));
                var response = await result.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(response);
                return json.Value<string>("id");
            }
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();
            return result;
        }
    }
}
