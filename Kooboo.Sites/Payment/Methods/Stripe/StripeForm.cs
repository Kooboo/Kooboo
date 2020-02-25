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

        public string Name => "StripeForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("Stripe", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-wechat.jpg";

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


        [Description(@"Pay by stripe.")]
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
            return new HiddenFormResponse { 
                paymemtMethodReferenceId = sessionId
            };
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
