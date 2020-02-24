using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CreateCheckoutResponse
    {
        [JsonProperty(PropertyName = "checkout")]
        public CheckoutDetail Checkout { get; set; }

        public class CheckoutDetail
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "checkout_page_url")]
            public string CheckoutPageURL { get; set; }

            [JsonProperty(PropertyName = "order")]
            public OrderDetail Order { get; set; }
        }

        public class OrderDetail
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }
        }
    }
}
