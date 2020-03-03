using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class TransactionResponse
    {
        [JsonProperty("transaction")]
        public TransactionResponseChildren Transaction { get; set; }
    }

    public class TransactionResponseChildren
    {
        [JsonProperty("id")]
        public virtual string Id { get; protected set; }
        [JsonProperty("amount")]
        public virtual decimal? Amount { get; protected set; }
        [JsonProperty("status")]
        public string Status { get; protected set; }
        [JsonProperty("currencyIsoCode")]
        public virtual string CurrencyIsoCode { get; protected set; }
        [JsonProperty("orderId")]
        public virtual string OrderId { get; protected set; }
        [JsonProperty("taxAmount")]
        public virtual decimal? TaxAmount { get; protected set; }
        [JsonProperty("type")]
        public virtual string Type { get; protected set; }
        [JsonProperty("updatedAt")]
        public virtual DateTime? UpdatedAt { get; protected set; }
        [JsonProperty("createdAt")]
        public virtual DateTime? CreatedAt { get; protected set; }
    }
}
