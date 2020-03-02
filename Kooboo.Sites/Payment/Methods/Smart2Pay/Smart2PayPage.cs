using System;
using System.Collections.Generic;
using System.ComponentModel;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Smart2Pay.Lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Smart2Pay
{
    public class Smart2PayPage : IPaymentMethod<Smart2PaySetting>
    {
        public string Name => "Smart2Pay";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Smart2Pay", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-smart2pay.svg";

        public string IconType => "img";

        public List<string> supportedCurrency => new List<string>();

        public RenderContext Context { get; set; }

        public Smart2PaySetting Setting { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 150; // $1.50
    charge.currency='USD';
    charge.name = 'green tea order'; 
    charge.description = 'The best tea from Xiamen';  
    var res = k.payment.smart2pay.charge(charge);  
    charge.email = 'customer@email.test'; 
    k.response.redirect(res.redirectUrl);
    // var smart2payReferenceId = res.paymemtMethodReferenceId;
</script>")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            //var callbackUrl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);
            request.Additional.TryGetValue("email", out var email);
            var req = new Smart2PayRequest
            {
                Payment = new Smart2PayPaymentBasic
                {
                    MerchantTransactionId = request.Id.ToString(),
                    Amount = (long)request.TotalAmount,
                    Currency = request.Currency,
                    Description = request.Description,
                    ReturnUrl = Setting.ReturnUrl,
                    Customer = new Smart2PayPaymentBasic.CustomerInfo
                    {
                        Email = (string)email
                    }
                }
            };

            var resp = new Smart2PayApi(Setting).CreatePayment(req);

            return new RedirectResponse(resp.Payment.RedirectUrl, request.Id)
            {
                paymemtMethodReferenceId = resp.Payment.Id.ToString()
            };
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var result = new PaymentStatusResponse { HasResult = true };
            try
            {
                var paymentId = request.ReferenceId;
                var statusResponse = new Smart2PayApi(Setting).CheckStatus(long.Parse(paymentId));

                result.Status = ConvertStatus(statusResponse.Payment.Status.Info);
            }
            catch (AggregateException ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex.InnerException);
            }

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var response = new PaymentCallback
            {
                RawData = context.Request.Body,
                CallbackResponse = new Callback.CallbackResponse
                {
                    StatusCode = 204
                }
            };

            try
            {
                var body = context.Request.Body;
                var notification = JsonHelper.Deserialize<Smart2PayNotification>(body);
                if (notification.Payment.MerchantTransactionId == null ||
                    !Guid.TryParse(notification.Payment.MerchantTransactionId, out var requestId) ||
                    PaymentManager.GetRequest(requestId, context) == null)
                {
                    return response;
                }

                response.Status = ConvertStatus(notification.Payment.Status.Info);
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return response;
        }

        // https://docs.smart2pay.com/category/status-codes/
        private PaymentStatus ConvertStatus(string status)
        {
            switch (status)
            {
                case "Success":
                case "Captured":
                case "PartiallyCaptured":
                    return PaymentStatus.Paid;
                case "Cancelled":
                case "Reversed":
                case "Chargedback":
                    return PaymentStatus.Cancelled;
                case "Failed":
                    return PaymentStatus.Rejected;
                case "Authorized":
                    return PaymentStatus.Authorized;
                case "Open":
                case "Processing":
                case "CaptureRequested":
                case "Exception":
                case "CancelRequested":
                case "Disputed":
                    return PaymentStatus.Pending;
                case "Expired":
                default:
                    return PaymentStatus.NotAvailable;
            }
        }
    }
}
