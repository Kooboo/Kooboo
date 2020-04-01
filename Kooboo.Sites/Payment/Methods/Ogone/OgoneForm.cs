using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
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
charge.country='US';
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
            var additional = new Dictionary<string, object>(request.Additional, StringComparer.OrdinalIgnoreCase);

            var variant = GetValue(additional, "variant", null);
            var locale = GetValue(additional, "locale", null);
            var paymentIds = GetValue(additional, "paymentIds", null);
            if (Setting == null)
                return res;

            var ogoneApi = new OgoneApi(Setting);
            PaymentProductFiltersHostedCheckout PaymentProductFilters = null;
            if (paymentIds != null)
            {
                PaymentProductFilters = new PaymentProductFiltersHostedCheckout
                {
                    RestrictTo = new PaymentProductFilter
                    {
                        Products = paymentIds.Split(',').Select(Int32.Parse).ToList()
                    }
                };
            }

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

            if (Setting.ReturnUrl != null)
            {
                checkoutRequest.HostedCheckoutSpecificInput = new HostedCheckoutSpecificInput
                {
                    ReturnUrl = Setting.ReturnUrl
                };
            }

            var result = ogoneApi.Hostedcheckouts(checkoutRequest);
            if (result != null)
            {
                res = new RedirectResponse(string.Format("{0}.{1}", Setting.BaseUrl(), result.PartialRedirectUrl), request.Id);
                request.ReferenceId = result.HostedCheckoutId;
            }

            return res;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var ogoneApi = new OgoneApi(Setting);
            var resp = new PaymentStatusResponse();

            var result = ogoneApi.GetHostedcheckouts(request.ReferenceId);

            if (request != null)
            {
                if (string.Equals(result.Status, "PAYMENT_CREATED", StringComparison.OrdinalIgnoreCase))
                {
                    resp.Status = ConvertStatus(result.CreatedPaymentOutput?.Payment.StatusOutput.StatusCategory);
                }
                else
                {
                    resp.Status = PaymentStatus.NotAvailable;
                }

            }
            else
            {
                resp.Status = PaymentStatus.NotAvailable;
            }

            return resp;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var ogoneApi = new OgoneApi(Setting);
            var verifyCode = context.Request.Headers.Get("X-GCS-Webhooks-Endpoint-Verification");
            if (!string.IsNullOrEmpty(verifyCode))
            {
                return new PaymentCallback
                {
                    CallbackResponse = new Callback.CallbackResponse
                    {
                        Content = verifyCode,
                    }
                };
            }

            var webHook = ogoneApi.Unmarshal(context.Request.PostData, context.Request.Headers);
            var request = PaymentManager.GetRequestByReferece(webHook.Payment.HostedCheckoutSpecificOutput?.HostedCheckoutId?.ToString(), context);

            if (request != null)
            {
                var status = ConvertStatus(webHook.Payment.StatusOutput.StatusCategory);

                return new PaymentCallback
                {
                    RequestId = request.Id,
                    Status = status,
                    RawData = context.Request.Body,
                    CallbackResponse = new Callback.CallbackResponse
                    {
                        Content = verifyCode,
                    }
                };
            }

            return null;
        }

        private PaymentStatus ConvertStatus(string code)
        {
            var status = PaymentStatus.Pending;
            switch (code.ToUpper())
            {
                case "ACCOUNT_VERIFIED":
                    status = PaymentStatus.NotAvailable;
                    break;
                case "PENDING_MERCHANT":
                    status = PaymentStatus.Pending;
                    break;
                case "PENDING_CONNECT_OR_3RD_PARTY":
                    status = PaymentStatus.Pending;
                    break;
                case "COMPLETED":
                    status = PaymentStatus.Paid;
                    break;
                case "UNSUCCESSFUL":
                    status = PaymentStatus.NotAvailable;
                    break;
            }

            return status;
        }

        private string GetValue(Dictionary<string, object> additional, string key, string fallbackValue)
        {
            if (additional.TryGetValue(key, out var o) && o != null)
            {
                var value = o.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return fallbackValue;
        }
    }
}