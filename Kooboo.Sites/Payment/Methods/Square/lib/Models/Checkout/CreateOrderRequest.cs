using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CreateOrderRequest
    {
        /// <summary>
        /// A value you specify that uniquely identifies this
        /// order among orders you've created.
        /// If you're unsure whether a particular order was created successfully,
        /// you can reattempt it with the same idempotency key without
        /// worrying about creating duplicate orders.
        /// See [Idempotency](https://developer.squareup.com/docs/basics/api101/idempotency) for more information.
        /// </summary>
        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }

        /// <summary>
        /// __Deprecated__: Please set the reference_id on the nested [order](#type-order) field
        /// instead.
        /// An optional ID you can associate with the order for your own
        /// purposes (such as to associate the order with an entity ID in your
        /// own database).
        /// This value cannot exceed 40 characters.
        /// </summary>
        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// __Deprecated__: Please set the line_items on the nested [order](#type-order) field
        /// instead.
        /// The line items to associate with this order.
        /// Each line item represents a different product to include in a purchase.
        /// </summary>
        [JsonProperty("line_items")]
        public IList<CreateOrderRequestLineItem> LineItems { get; set; }
    }
}
