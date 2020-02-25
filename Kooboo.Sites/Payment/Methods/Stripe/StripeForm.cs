using Kooboo.Data.Context;
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


        [Description(@"Pay by stripe. Example:")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var id = CreateSession(request).Result;
            return new HiddenFormResponse { 
                paymemtMethodReferenceId = id
            };
        }

        private async Task<string> CreateSession(PaymentRequest request)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Setting.Secretkey);
                var contentList = new List<string>();
                request.Additional.TryGetValue("successUrl", out var successUrl);
                request.Additional.TryGetValue("cancelUrl", out var cancelUrl);
                contentList.Add("success_url=" + HttpUtility.UrlEncode((string)successUrl));
                contentList.Add("cancel_url=" + HttpUtility.UrlEncode((string)cancelUrl));
                contentList.Add("payment_method_types[0]=card");
                contentList.Add("line_items[0][name]=T-shirt");
                contentList.Add("line_items[0][description]=Comfortable cotton t - shirt");
                contentList.Add("line_items[0][amount]=1500");
                contentList.Add("line_items[0][currency]=usd");
                contentList.Add("line_items[0][quantity]=2");
                var body = string.Join("&", contentList);
                var result = await httpClient.PostAsync("https://api.stripe.com/v1/checkout/sessions", new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
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
