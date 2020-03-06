using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Adyen.Lib
{
    public class AdyenNotification
    {
        [JsonProperty("live")]
        public bool Live { get; set; }

        [JsonProperty("notificationItems")]
        public NotificationItem[] NotificationItems { get; set; }

        public class NotificationItem
        {
            [JsonProperty("NotificationRequestItem")]
            public NotificationRequestItem NotificationRequestItem { get; set; }
        }

        public class NotificationRequestItem
        {
            [JsonProperty("additionalData")]
            public Dictionary<string, object> AdditionalData { get; set; }

            [JsonProperty("eventCode")]
            public string EventCode { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("eventDate")]
            public DateTime EventDate { get; set; }

            [JsonProperty("merchantAccountCode")]
            public string MerchantAccountCode { get; set; }

            [JsonProperty("merchantReference")]
            public string MerchantReference { get; set; }

            [JsonProperty("paymentMethod")]
            public string PaymentMethod { get; set; }

            [JsonProperty("pspReference")]
            public string PspReference { get; set; }

            [JsonProperty("originalReference")]
            public string OriginalReference { get; set; }

            [JsonProperty("reason")]
            public string Reason { get; set; }

            [JsonProperty("amount")]
            public AdyenAmount Amount { get; set; }
        }
    }
}
