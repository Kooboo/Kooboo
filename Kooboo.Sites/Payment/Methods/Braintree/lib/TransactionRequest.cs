using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class TransactionRequest
    {
        [JsonProperty("transaction")]
        public TransactionRequestChildren Transaction { get; set; }
    }

    public class TransactionRequestChildren
    {
        [JsonProperty("order-id")]
        public string OrderId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("options")]
        public TransactionOptionsRequest Options { get; set; }

        [JsonProperty("payment-method-nonce")]
        public string PaymentMethodNonce { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class TransactionOptionsRequest
    {
        [JsonProperty("submit-for-settlement")]
        public bool? SubmitForSettlement { get; set; }
    }
}
