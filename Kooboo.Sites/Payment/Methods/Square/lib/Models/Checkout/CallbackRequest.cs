using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CallbackRequest
    {
        [JsonProperty("merchant_id")]
        public string MerchantID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("event_id")]
        public string EventID { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("object")]
        public Object Object { get; set; }
    }

    public class Object
    {
        [JsonProperty("payment")]
        public Payment Payment { get; set; }
    }

    public class Payment
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("location_id")]
        public string LocationID { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("total_money")]
        public Money TotalMoney { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }
    }
}
