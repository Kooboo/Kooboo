using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Stripe.lib;
using Kooboo.Sites.Payment.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.Payment.Methods.Stripe
{
    public class StripeForm : IPaymentMethod<StripeFormSetting>
    {
        public StripeFormSetting Setting { get; set; }

        private const string Description = @"Pay by stripe. Example:
<script engine=""kscript"">
  var charge = {
    successUrl: 'https://example.com/success',
    cancelUrl: 'https://example.com/cancel',
    name: 'T-shirt',
    description: 'Comfortable cotton t-shirt',
    totalAmount: 1500,
    currency: 'eur',
    quantity: 2,
    paymentMethodType: ['card', 'ideal']
  };
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
                var list = new List<string> 
                {
                    "USD",
                    "EUR"
                };
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

            var lineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions {
                    Name = request.Name,
                    Description = request.Description,
                    Amount = long.Parse(request.TotalAmount.ToString()),
                    Currency = request.Currency,
                    Quantity = long.Parse(quantity.ToString())
                }
            };

            var paymentMethodTypesList = new List<string>();
            
            if(paymentMethodType is object[])
            {
                var paymentMethodTypeArray = Array.ConvertAll((object[])paymentMethodType, x => x.ToString());
                paymentMethodTypesList.AddRange(paymentMethodTypeArray);
            }
            else if (paymentMethodType is object)
            {
                paymentMethodTypesList.Add((string)paymentMethodType);
            }
            
            var options = new SessionCreateOptions
            {
                SuccessUrl = (string)successUrl,
                CancelUrl = (string)cancelUrl,
                LineItems = lineItems,
                PaymentMethodTypes = paymentMethodTypesList
            };

            var sessionId = StripeUtility.CreateSession(options, Setting.Secretkey).Result;
            var response = new HiddenFormResponse
            {
                paymemtMethodReferenceId = sessionId
            };
            response.setFieldValues("publishableKey", Setting.Publishablekey);
            return response;
        }
 
        public PaymentCallback Notify(RenderContext context)
        {
            var body = context.Request.Body;
            var signi = context.Request.Headers.Get("Stripe-Signature");

            var stripeEvent = EventUtility.ConstructEvent(body, signi, Setting.WebhookSigningSecret);
            var result = new PaymentCallback
            {
                Status = ConvertStatus(stripeEvent.Type),
                RawData = body,
                CallbackResponse = new Callback.CallbackResponse { StatusCode = 204 },
            };
            return result;
        }

        private PaymentStatus ConvertStatus(string status)
        {
            switch (status)
            {
                case "payment_intent.succeeded":
                    return PaymentStatus.Paid;
                case "payment_intent.payment_failed":
                    return PaymentStatus.Rejected;
                case "payment_intent.canceled":
                    return PaymentStatus.Cancelled;
                case "payment_intent.created":
                    return PaymentStatus.Pending;
                default:
                    return PaymentStatus.NotAvailable;
            }
        } 
 
        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();
            return result;
        }
    }
}
