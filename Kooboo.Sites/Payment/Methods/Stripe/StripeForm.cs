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
<script engine='kscript'>
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
</script>
<div k-content='res.html'></div>";

        public string Name => "StripeForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Stripe", Context);

        public string Icon => "https://s3.us-east-2.amazonaws.com/upload-icon/uploads/icons/png/63467571551952112-64.png";

        public string IconType => "img";

        public List<string> supportedCurrency => new List<string> {
            "USD", "EUR", "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN",
            "BAM", "BBD", "BDT", "BGN", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BWP", "BZD", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CVE",
            "CZK", "DJF", "DKK", "DOP", "DZD", "EGP", "ETB", "FJD", "FKP", "GBP", "GEL", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG",
            "HUF", "IDR", "ILS", "INR", "ISK", "JMD", "JPY", "KES", "KGS", "KHR", "KMF", "KRW", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "MAD",
            "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRO", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "PAB",
            "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SEK", "SGD", "SHP", "SLL", "SOS", "SRD",
            "STD", "SZL", "THB", "TJS", "TOP", "TRY", "TTD", "TWD", "TZS", "UAH", "UGX", "UYU", "UZS", "VND", "VUV", "WST", "XAF", "XCD", "XOF", "XPF",
            "YER", "ZAR", "ZMW"
        };

        public RenderContext Context { get; set; }

        [Description(Description)]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            request.Additional.TryGetValue("quantity", out var quantity);
            request.Additional.TryGetValue("paymentMethodType", out var paymentMethodType);
            string cancelUrl = string.IsNullOrEmpty(request.CancelUrl) ? Setting.CancelUrl : request.CancelUrl;
            string successUrl = string.IsNullOrEmpty(request.ReturnUrl) ? Setting.SuccessUrl : request.ReturnUrl;

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

            if (paymentMethodType is object[])
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
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = lineItems,
                PaymentMethodTypes = paymentMethodTypesList,
                ClientReferenceId = request.Id.ToString()
            };

            var sessionId = StripeUtility.CreateSession(options, Setting.Secretkey).Result;
            var response = new HiddenFormResponse
            {
                paymemtMethodReferenceId = request.Id.ToString()
            };
            response.html = GenerateHtml(Setting.Publishablekey, sessionId);
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
            if (stripeEvent.Type == lib.Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                result.RequestId = new Guid(session.ClientReferenceId);
            }
            return result;
        }

        private string GenerateHtml(string publishableKey, string sessionId)
        {
            var html = string.Format(@"
<script src = ""https://js.stripe.com/v3/"" ></script>
<script>
  var stripe = Stripe('{0}');
  stripe.redirectToCheckout({{
    sessionId: '{1}'
}}).then(function (result) {{
alert(JSON.stringify(result))
    console.log(result.error.message)
}});
</script>", publishableKey, sessionId);
            return html;
        }

        private PaymentStatus ConvertStatus(string status)
        {
            switch (status)
            {
                case lib.Events.CheckoutSessionCompleted:
                    return PaymentStatus.Paid;
                case lib.Events.PaymentIntentPaymentFailed:
                    return PaymentStatus.Rejected;
                case lib.Events.PaymentIntentCanceled:
                    return PaymentStatus.Cancelled;
                case lib.Events.PaymentIntentCreated:
                    return PaymentStatus.Pending;
                default:
                    return PaymentStatus.NotAvailable;
            }
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotSupportedException("Stripe dose not implement API to check status");
        }
    }
}
