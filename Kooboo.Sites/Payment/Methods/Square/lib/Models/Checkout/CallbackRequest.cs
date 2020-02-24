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
        public DataObject Data { get; set; }
    }

    public class DataObject
    {
        [JsonProperty("object")]
        public Shift Object { get; set; }
    }

    public class Shift
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("location_id")]
        public string LocationID { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("wage")]
        public HourlyRateDetail WageDetail { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class HourlyRateDetail
    {
        [JsonProperty("hourly_rate")]
        public Money HourlyRate { get; set; }
    }
}
