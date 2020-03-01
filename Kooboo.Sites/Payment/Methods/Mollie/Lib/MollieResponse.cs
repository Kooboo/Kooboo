using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    // https://docs.mollie.com/reference/v2/payments-api/get-payment
    public class MollieResponse
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("isCancelable")]
        public bool IsCancelable { get; set; }

        [JsonProperty("authorizedAt")]
        public DateTime AuthorizedAt { get; set; }

        [JsonProperty("canceledAt")]
        public DateTime CanceledAt { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("failedAt")]
        public DateTime FailedAt { get; set; }

        [JsonProperty("paidAt")]
        public DateTime PaidAt { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("status")]
        public MollieStatus Status { get; set; }

        [JsonProperty("amountRefunded")]
        public Amount AmountRefunded { get; set; }

        [JsonProperty("amountRemaining")]
        public Amount AmountRemaining { get; set; }

        [JsonProperty("amountCaptured")]
        public Amount AmountCaptured { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("profileId")]
        public string ProfileId { get; set; }

        [JsonProperty("sequenceType")]
        public string SequenceType { get; set; }

        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        [JsonProperty("settlementAmount")]
        public Amount SettlementAmount { get; set; }

        [JsonProperty("details")]
        public CardDetails Details { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }

        public class CardDetails
        {
            [JsonProperty("cardNumber")]
            public string CardNumber { get; set; }

            [JsonProperty("cardHolder")]
            public string CardHolder { get; set; }

            [JsonProperty("cardAudience")]
            public string CardAudience { get; set; }

            [JsonProperty("cardLabel")]
            public string CardLabel { get; set; }

            [JsonProperty("cardCountryCode")]
            public string CardCountryCode { get; set; }

            [JsonProperty("cardSecurity")]
            public string CardSecurity { get; set; }

            [JsonProperty("feeRegion")]
            public string FeeRegion { get; set; }
        }
    }
}
