using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Payment.Methods.Mollie.Lib
{
    // https://docs.mollie.com/reference/v2/payments-api/create-payment
    public class MolliePaymentRequest
    {
        [JsonProperty("amount")]
        public Amount Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// callback url
        /// </summary>
        [JsonProperty("webhookUrl")]
        public string WebhookUrl { get; set; }

        /// <summary>
        /// after payment succedd, redirect browser
        /// </summary>
        [JsonProperty("redirectUrl")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// The ID of the Customer for whom the payment is being created.
        /// This is used for <see cref="PaymentSequenceType.first"/> and <see cref="PaymentSequenceType.recurring"/>.
        /// </summary>
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("sequenceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PaymentSequenceType SequenceType { get; set; }

        /// <summary>
        /// Provide any data you like, for example a string or a JSON object. We will save the data alongside the payment.
        /// Whenever you fetch the payment with our API, we’ll also include the metadata. You can use up to approximately 1kB.
        /// </summary>
        [JsonProperty("metadata")]
        public string Metadata { get; set; }

        public enum PaymentSequenceType
        {
            /// <summary>
            /// Default, a regular non-recurring payment 
            /// </summary>
            oneoff,

            /// <summary>
            /// allowing the customer to agree to automatic recurring charges taking place on their account in the future
            /// </summary>
            first,

            /// <summary>
            /// the customer’s card is charged automatically
            /// </summary>
            recurring
        }
    }
}
