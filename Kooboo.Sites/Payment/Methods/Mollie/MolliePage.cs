using System;
using System.Collections.Generic;
using System.ComponentModel;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Mollie.Lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Mollie
{
    public class MolliePage : IPaymentMethod<MollieSetting>
    {
        public string Name => "Mollie";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Mollie", Context);

        // https://www.mollie.com/en/resources
        public string Icon => "/_Admin/View/Market/Images/payment-mollie.svg";

        public string IconType => "img";

        // Mollie supports multiple payment method suppliers.
        // e.g. for PayPal, all currencies supported by PayPal are also supported by Mollie.
        // https://docs.mollie.com/payments/multicurrency
        public List<string> supportedCurrency => new List<string> { "AED", "AUD", "BGN", "BRL", "CAD", "CHF", "CZK", "DKK", "EUR", "GBP", "HKD", "HRK", "HUF", "ILS", "ISK", "JPY", "MXN", "MYR", "NOK", "NZD", "PHP", "PLN", "RON", "RUB", "SEK", "SGD", "THB", "TWD", "USD", "ZAR", };

        public RenderContext Context { get; set; }

        public MollieSetting Setting { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 1.50;
    charge.currency='USD';
    charge.name = 'green tea order'; 
    charge.description = 'The best tea from Xiamen';  
    var res = k.payment.mollie.charge(charge);  
    k.response.redirect(res.redirectUrl);
    // var molliePaymentId = res.paymemtMethodReferenceId;
</script>")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var callbackUrl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);
            var req = new MolliePaymentRequest
            {
                Amount = new Amount
                {
                    Currency = request.Currency,
                    Value = CurrencyDecimalPlaceConverter.ToDecimalPlaceString(request.Currency, request.TotalAmount)
                },
                Description = request.Description,
                RedirectUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Setting.RedirectUrl,
                WebhookUrl = callbackUrl,
                Metadata = request.Id.ToString()
            };

            var resp = new MollieApi(Setting.ApiToken).CreatePayment(req);

            return new RedirectResponse(resp.Links.Checkout.Href, request.Id)
            {
                paymemtMethodReferenceId = resp.Id
            };
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var result = new PaymentStatusResponse { HasResult = true };
            try
            {
                var paymentId = request.ReferenceId;
                var statusResponse = new MollieApi(Setting.ApiToken).CheckStatus(paymentId);

                result.Status = ConvertStatus(statusResponse.Status);
            }
            catch (AggregateException ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex.InnerException);
            }

            return result;
        }

        // https://docs.mollie.com/guides/webhooks#example
        public PaymentCallback Notify(RenderContext context)
        {
            // mollie callback only inputs an id, we need to check status using check status api manually.
            var molliePaymentId = context.Request.Get("id");
            if (string.IsNullOrWhiteSpace(molliePaymentId))
            {
                return null;
            }

            var statusResponse = new MollieApi(Setting.ApiToken).CheckStatus(molliePaymentId);
            if (!Guid.TryParse(statusResponse.Metadata, out var requestId) ||
                PaymentManager.GetRequest(requestId, context) == null)
            {
                return null;
            }

            var status = ConvertStatus(statusResponse.Status);

            return new PaymentCallback
            {
                RequestId = requestId,
                Status = status,
                RawData = context.Request.Body,
                CallbackResponse = new Callback.CallbackResponse
                {
                    StatusCode = 204
                }
            };
        }

        // https://docs.mollie.com/payments/status-changes
        private PaymentStatus ConvertStatus(MollieStatus mollieStatus)
        {
            switch (mollieStatus)
            {
                case MollieStatus.paid:
                    return PaymentStatus.Paid;
                case MollieStatus.canceled:
                    return PaymentStatus.Cancelled;
                case MollieStatus.failed:
                    return PaymentStatus.Rejected;
                case MollieStatus.authorized:
                case MollieStatus.open:
                case MollieStatus.pending:
                    return PaymentStatus.Pending;
                case MollieStatus.expired:
                default:
                    return PaymentStatus.NotAvailable;
            }
        }
    }
}
