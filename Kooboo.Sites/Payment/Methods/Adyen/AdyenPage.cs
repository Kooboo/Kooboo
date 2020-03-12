using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Adyen.Lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Adyen
{
    public class AdyenPage : IPaymentMethod<AdyenSetting>
    {
        private static readonly Regex InvalidDateTimeFormat = new Regex(@"(\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}) (\d{2}:\d{2})", RegexOptions.Compiled);

        public string Name => "Adyen";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Adyen", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-adyen.svg";

        public string IconType => "img";

        // https://docs.adyen.com/development-resources/currency-codes#page-introduction
        public List<string> supportedCurrency => new List<string> { "AED", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BMD", "BND", "BOB", "BRL", "BSD", "BWP", "BYN", "BZD", "CAD", "CHF", "CLP", "CNY", "COP", "CRC", "CSD", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD", "EEK", "EGP", "ETB", "EUR", "FJD", "FKP", "GBP", "GEL", "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR", "ILS", "INR", "IQD", "ISK", "JMD", "JOD", "JPY", "KES", "KGS", "KHR", "KMF", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LTL", "LVL", "LYD", "MAD", "MDL", "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SBD", "SCR", "SEK", "SGD", "SHP", "SKK", "SLL", "SOS", "SRD", "STN", "SVC", "SZL", "THB", "TND", "TOP", "TRY", "TTD", "TWD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VEF", "VND", "VUV", "WST", "XAF", "XCD", "XOF", "XPF", "YER", "ZAR", "ZMW" };

        public RenderContext Context { get; set; }

        public AdyenSetting Setting { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 1.50;
    charge.currency='USD';
    charge.country='US';
    charge.name = 'green tea order'; 
    charge.description = 'The best tea from Xiamen';  
    var res = k.payment.adyen.charge(charge);  
    k.response.redirect(res.redirectUrl);
    // var adyenReferenceId = res.paymemtMethodReferenceId;
</script>")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var referenceId = request.Id.ToString();
            var req = new AdyenPaymentRequest
            {
                Amount = new AdyenAmount
                {
                    Currency = request.Currency,
                    Value = AdyenAmount.FormatAmountToMinorUnits(request.Currency, request.TotalAmount)
                },
                CountryCode = request.Country,
                MerchantAccount = Setting.MerchantAccount,
                Reference = referenceId,
                Description = request.Description,
                ReturnUrl = !string.IsNullOrWhiteSpace(request.ReturnUrl) ? request.ReturnUrl : Setting.ReturnUrl
            };

            var resp = new AdyenApi(Setting).CreatePayment(req);

            return new RedirectResponse(resp.Url, request.Id)
            {
                paymemtMethodReferenceId = referenceId
            };
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotSupportedException("Adyen does not support check status api, please setup notification webhooks. https://docs.adyen.com/development-resources/notifications#set-up-notifications-in-your-customer-area");
        }

        // Adyen requires response body "[accepted]" for notifications. https://docs.adyen.com/development-resources/notifications#accept-notifications
        public PaymentCallback Notify(RenderContext context)
        {
            //var callbackUrl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);
            var body = context.Request.Body;
            var paymentCallback = new PaymentCallback
            {
                RawData = body,
                CallbackResponse = new Callback.CallbackResponse
                {
                    StatusCode = 200,
                    Content = "[accepted]",
                    ContentType = "text/plain"
                }
            };

            try
            {
                if (IsNotifyAuthorized(context))
                {
                    var notification = JsonHelper.Deserialize<AdyenNotification>(FixDateTimeFormat(body));
                    // JSON and HTTP POST notifications always contain a single NotificationRequestItem object.
                    var notifyItem = notification.NotificationItems[0].NotificationRequestItem;

                    // check signiture
                    if (!string.IsNullOrWhiteSpace(Setting.HmacKey))
                    {
                        if (!new AdyenSignatureValidator(Setting.HmacKey).Validate(notifyItem))
                        {
                            return paymentCallback;
                        }
                    }

                    if (Guid.TryParse(notifyItem.MerchantReference, out var guid))
                    {
                        paymentCallback.RequestId = guid;
                    }

                    paymentCallback.Status = ConvertStatus(notifyItem.EventCode, notifyItem.Success);
                }
                else
                {
                    paymentCallback.CallbackResponse.StatusCode = 401;
                    paymentCallback.CallbackResponse.Content = "Unauthorized.";
                }
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return paymentCallback;
        }

        private bool IsNotifyAuthorized(RenderContext context)
        {
            var authHeader = context.Request.Headers.Get("Authorization");
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return false;
            }

            var headerValue = authHeader.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (headerValue.Length != 2 || !headerValue[0].Equals("Basic", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            try
            {
                var userNamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue[1]))
                    .Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                return userNamePassword.Length == 2
                       && userNamePassword[0] == Setting.NotifyUrlUserName
                       && userNamePassword[1] == Setting.NotifyUrlPassword;
            }
            catch
            {
                return false;
            }
        }

        // https://docs.adyen.com/development-resources/notifications/understand-notifications?tab=%23codeBlocksQ04C_Json#event-codes
        private PaymentStatus ConvertStatus(string eventCode, bool eventSuccess)
        {
            if (!eventSuccess)
            {
                if (eventCode == "CAPTURE" || eventCode == "ORDER_CLOSED" || eventCode == "AUTHORISATION")
                {
                    return PaymentStatus.Rejected;
                }

                return PaymentStatus.NotAvailable;
            }

            switch (eventCode)
            {
                case "CAPTURE":
                case "ORDER_CLOSED":
                    return PaymentStatus.Paid;
                case "CANCELLATION":
                    return PaymentStatus.Cancelled;
                case "CAPTURE_FAILED":
                    return PaymentStatus.Rejected;
                case "PENDING":
                case "AUTHORISATION":
                    return PaymentStatus.Pending;
                default:
                    return PaymentStatus.NotAvailable;
            }
        }

        private string FixDateTimeFormat(string body)
        {
            // adyen test api returns date time with format: "2020-02-14T00:38:29 01:00" which is invalid.
            // it should be "2020-02-14T00:38:29+01:00".
            // not sure if live environment has the same problem, the format mentioned in the doc is correct.
            return InvalidDateTimeFormat.Replace(body, match => $"{match.Groups[1]}+{match.Groups[2]}");
        }
    }
}
