using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    public class SessionLineItemOptions
    {
        /// <summary>
        /// The amount to be collected per unit of the line item.
        /// </summary>
        public long? Amount { get; set; }

        /// <summary>
        /// Three-letter ISO currency code, in lowercase. Must be a supported currency.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The description for the line item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of images representing this line item.
        /// </summary>
        public List<string> Images { get; set; }

        /// <summary>
        /// The name for the line item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Quantity of the line item being purchased.
        /// </summary>
        public long? Quantity { get; set; }
    }
}
