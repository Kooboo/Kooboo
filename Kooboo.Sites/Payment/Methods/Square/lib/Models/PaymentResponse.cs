using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models
{
    public class PaymentResponse
    {
        [JsonProperty(PropertyName = "payment")]
        public PaymentDetail Payment { get; set; }

        public class PaymentDetail
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }

            [JsonProperty(PropertyName = "created_at")]
            public string CreatedAt { get; set; }

            [JsonProperty(PropertyName = "updated_at")]
            public string UpdatedAt { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }

            [JsonProperty("amount_money")]
            public Models.Money AmountMoney { get; set; }

            [JsonProperty(PropertyName = "order_id")]
            public string OrderID { get; set; }
        }
    }
}
