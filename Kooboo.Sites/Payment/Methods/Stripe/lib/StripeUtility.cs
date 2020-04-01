using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    public static class StripeUtility
    {
        public static string SessionDataToContentString(SessionCreateOptions options)
        {
            var contentList = new List<string>();
            contentList.Add("success_url=" + HttpUtility.UrlEncode((string)options.SuccessUrl));
            contentList.Add("cancel_url=" + HttpUtility.UrlEncode((string)options.CancelUrl));
            contentList.Add("client_reference_id=" + HttpUtility.UrlEncode((string)options.ClientReferenceId));
            var index = 0;
            foreach(var option in options.LineItems)
            {
                var lineItemStr = "line_items[{0}][{1}]={2}";
                var indexStr = index++.ToString();
                contentList.Add(string.Format(lineItemStr, indexStr, "name", option.Name));
                contentList.Add(string.Format(lineItemStr, indexStr, "description", option.Description));
                contentList.Add(string.Format(lineItemStr, indexStr, "amount", option.Amount));
                contentList.Add(string.Format(lineItemStr, indexStr, "currency", option.Currency));
                contentList.Add(string.Format(lineItemStr, indexStr, "quantity", option.Quantity));
            }
            
            index = 0;
            foreach(var type in options.PaymentMethodTypes)
            {
                var typeStr = "payment_method_types[{0}]={1}";
                var indexStr = index++.ToString();
                contentList.Add(string.Format(typeStr, indexStr, type));
            }
            
            var content = string.Join("&", contentList);
            return content;
        }

        public static async Task<string> CreateSession(SessionCreateOptions options, string Secretkey)
        { 
            var client = ApiClient.Create();
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + Secretkey }
            };
            var result = await client.PostAsync("https://api.stripe.com/v1/checkout/sessions", SessionDataToContentString(options), "application/x-www-form-urlencoded", headers);
            var response =  result.Content;
            JObject json = JObject.Parse(response);
            return json.Value<string>("id"); 
        }
    }
}
