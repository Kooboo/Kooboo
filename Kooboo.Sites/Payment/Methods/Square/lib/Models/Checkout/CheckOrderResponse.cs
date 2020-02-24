using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CheckOrderResponse
    {
        [JsonProperty(PropertyName = "orders")]
        public List<OrderDetail> Orders { get; set; }

        public class OrderDetail
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "state")]
            public string State { get; set; }
        }
    }
}
