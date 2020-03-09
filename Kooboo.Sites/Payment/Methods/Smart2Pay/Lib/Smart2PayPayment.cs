using System;
using Kooboo.Lib.Helper;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Smart2Pay.Lib
{
    public class Smart2PayPayment : Smart2PayPaymentBasic
    {
        [JsonProperty("ID")]
        public long? Id { get; set; }

        [JsonConverter(typeof(FormatedDateTimeConverter), "yyyyMMddHHmmss")]
        public DateTime? Created { get; set; }

        [JsonConverter(typeof(FormatedDateTimeConverter), "yyyyMMddHHmmss")]
        public DateTime? NotificationDateTime { get; set; }

        [JsonProperty("OriginatorTransactionID")]
        public string OriginatorTransactionId { get; set; }

        public long? CapturedAmount { get; set; }

        [JsonProperty("SiteID")]
        public int? SiteId { get; set; }

        public Smart2PayStatus Status { get; set; }

        public object Capture { get; set; }

        [JsonProperty("RedirectURL")]
        public string RedirectUrl { get; set; }

        public class Smart2PayStatus
        {
            [JsonProperty("ID")]
            public int? Id { get; set; }

            public string Info { get; set; }

            public Reason[] Reasons { get; set; }
        }

        public class Reason
        {
            public string Code { get; set; }

            public string Info { get; set; }
        }
    }
}
