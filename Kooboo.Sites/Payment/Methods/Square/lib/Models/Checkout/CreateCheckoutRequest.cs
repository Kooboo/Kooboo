using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CreateCheckoutRequest
    {
        /// <summary>
        /// A unique string that identifies this checkout among others
        /// you've created. It can be any valid string but must be unique for every
        /// order sent to Square Checkout for a given location ID.
        /// The idempotency key is used to avoid processing the same order more than
        /// once. If you're unsure whether a particular checkout was created
        /// successfully, you can reattempt it with the same idempotency key and all the
        /// same other parameters without worrying about creating duplicates.
        /// We recommend using a random number/string generator native to the language
        /// you are working in to generate strings for your idempotency keys.
        /// See [Idempotency keys](#idempotencykeys) for more information.
        /// </summary>
        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }

        /// <summary>
        /// Getter for order
        /// </summary>
        [JsonProperty("order")]
        public CreateOrderRequest Order { get; set; }

        /// <summary>
        /// The URL to redirect to after checkout is completed with `checkoutId`,
        /// Square's `orderId`, `transactionId`, and `referenceId` appended as URL
        /// parameters. For example, if the provided redirect_url is
        /// `http://www.example.com/order-complete`, a successful transaction redirects
        /// the customer to:
        /// `http://www.example.com/order-complete?checkoutId=xxxxxx&orderId=xxxxxx&referenceId=xxxxxx&transactionId=xxxxxx`
        /// If you do not provide a redirect URL, Square Checkout will display an order
        /// confirmation page on your behalf; however Square strongly recommends that
        /// you provide a redirect URL so you can verify the transaction results and
        /// finalize the order through your existing/normal confirmation workflow.
        /// Default: none; only exists if explicitly set.
        /// </summary>
        [JsonProperty("redirect_url")]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// An optional note to associate with the checkout object.
        /// This value cannot exceed 60 characters.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; }
    }
}
