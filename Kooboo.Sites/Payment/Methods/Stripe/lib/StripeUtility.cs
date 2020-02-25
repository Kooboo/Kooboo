using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    public static class StripeUtility
    {
        public static string SessionDataToContentString(SessionCreateOptions options)
        {
            var contentList = new List<string>();
            contentList.Add("success_url=" + HttpUtility.UrlEncode((string)options.SuccessUrl));
            contentList.Add("cancel_url=" + HttpUtility.UrlEncode((string)options.CancelUrl));
            
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
    }
}
