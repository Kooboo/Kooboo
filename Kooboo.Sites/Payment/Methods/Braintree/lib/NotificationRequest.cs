using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class NotificationRequest
    {
        [JsonProperty("notification")]
        public DateTime Notification { get; set; }
    }

    public class Notification
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("kind")]
        public string kind { get; set; }
        [JsonProperty("subject")]
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        [JsonProperty("subscription")]
        public Subscription Subscription { get; set; }
    }

    public class Subscription
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("transactions")]
        public TransactionResponse[] Transactions { get; set; }
    }
}
