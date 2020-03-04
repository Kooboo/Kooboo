using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Payment.Methods.qualpay.lib;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json.Linq;

namespace Kooboo.Sites.Payment.Methods.qualpay
{
    public class QualpayForm : IPaymentMethod<QualpayFormSetting>
    {
        public const string CheckoutSuccessEvent = "checkout_payment_success";
        public const string TransactionUpdatedEvent = "transaction_status_updated";

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
    var url = resForm.redirectUrl;
    k.response.redirect(url);
    </script>
    <div class='jumbotron'>
    </div>")]
        [KDefineType(Return = typeof(RedirectResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("amt_tran", request.TotalAmount.ToString("0.00"));
            var currency = GetCurrencyCode(request.Currency);
            dic.Add("tran_currency", currency);
            dic.Add("purchase_id", DataHelper.GeneratePurchaseId(request.Id));
            var result = QualpayAPI.CheckOutUrl(dic, Setting);
            if (result == null)
                return null;
            var redirectUrl = result["checkout_link"];
            RedirectResponse res = new RedirectResponse(redirectUrl, request.Id);

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

        public PaymentCallback NofityUrl(RenderContext context)
        {
            var result = new PaymentCallback();
            var headers = context.Request.Headers["x-qualpay-webhook-signature"];
            if (Validate(Setting.WebHookKey, headers, context.Request.Body))
            {
                if (this.Setting == null)
                {
                    return null;
                }

                var eventType = DataHelper.GetValue("event", context.Request.Body);
                var purchaseId = DataHelper.GetValue("data.purchase_id", context.Request.Body);
                var transationId = DataHelper.GetValue("data.pg_id", context.Request.Body);
                if (string.IsNullOrEmpty(purchaseId))
                    return null;

                var requestId = DataHelper.GenerateRequestId(purchaseId);
                result.RequestId = requestId;
                if (string.Equals(eventType, CheckoutSuccessEvent))
                {
                    var request = PaymentManager.GetRequest(requestId, context);
                    request.ReferenceId = transationId;
                    PaymentManager.UpdateRequest(request, context);
                    result.Status = PaymentStatus.Pending;
                }
                else
                {
                    result.Status = PaymentStatus.NotAvailable;
                }

                if (string.Equals(eventType, TransactionUpdatedEvent))
                {
                    string code = DataHelper.GetValue("tran_status", context.Request.Body);
                    result.Status = ConvertStatus(code);
                }

                return result;
            }

            return null;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var resp = new PaymentStatusResponse();

            var code = QualpayAPI.GetTransaction(request.ReferenceId, Setting);

            if (code != null)
            {
                resp.Status = ConvertStatus(code);
            }
            else
            {
                resp.Status = PaymentStatus.NotAvailable;
            }

            return resp;
        }

        private PaymentStatus ConvertStatus(string code)
        {
            var status = PaymentStatus.Pending;
            switch (code.ToUpper())
            {
                case "S":
                    status = PaymentStatus.Paid;
                    break;
                case "R":
                    status = PaymentStatus.Rejected;
                    break;
                case "C ":
                    status = PaymentStatus.Pending;
                    break;
                case "V":
                    status = PaymentStatus.NotAvailable;
                    break;
                case "K":
                    status = PaymentStatus.Cancelled;
                    break;
            }

            return status;
        }

        private bool Validate(string secret, string header, string postData)
        {
            bool isValid = false;
            if (secret != null && !string.IsNullOrEmpty(secret) && header != null || !string.IsNullOrEmpty(header))
            {
                string[] signatureArr = header.Split(',');
                var token = DataHelper.CreateToken(postData, secret);
                foreach (var item in signatureArr)
                {
                    if (string.Equals(token, item))
                    {
                        isValid = true;
                        break;
                    }
                }

            }
            return isValid;
        }
    }
}
