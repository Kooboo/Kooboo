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
        // e.g. for PayPal, all currencies supported by PayPal are also supported by Mollie. So the supported currency have to be all.
        // https://docs.mollie.com/payments/multicurrency
        public List<string> supportedCurrency => new List<string>();

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
                    Value = request.TotalAmount.ToString("#.00")
                },
                Description = request.Description,
                RedirectUrl = Setting.RedirectUrl,
                WebhookUrl = callbackUrl
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

            var status = ConvertStatus(statusResponse.Status);

            return new PaymentCallback
            {
                Status = status,
                RawData = context.Request.Body,
                CallbackResponse = new Callback.CallbackResponse
                {
                    StatusCode = 204
                }
            };
        }

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
