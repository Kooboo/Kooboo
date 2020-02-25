using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    public class SessionCreateOptions
    {
        /// <summary>
        /// Specify whether Checkout should collect the customer’s billing address. If set to
        /// <c>required</c>, Checkout will always collect the customer’s billing address. If left
        /// blank or set to <c>auto</c> Checkout will only collect the billing address when
        /// necessary.
        /// </summary>
        public string BillingAddressCollection { get; set; }

        /// <summary>
        /// The URL the customer will be directed to if they decide to go back to your website.
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// A unique string to reference the Checkout Session. This can be a customer ID, a cart
        /// ID, or similar. It is included in the <c>checkout.session.completed</c> webhook and can
        /// be used to fulfill the purchase.
        /// </summary>
        public string ClientReferenceId { get; set; }

        /// <summary>
        /// The email address used to create the customer object. If you already know your
        /// customer’s email address, use this attribute to prefill it on Checkout.
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// ID of the customer this Checkout Session is for if one exists. May only be used with
        /// <c>LineItems</c>. Usage with <c>SubscriptionData</c> is not yet available.
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// A list of items your customer is purchasing.
        /// </summary>
        public List<SessionLineItemOptions> LineItems { get; set; }

        /// <summary>
        /// The IETF language tag of the locale Checkout is displayed in. If blank or <c>auto</c>,
        /// the browser’s locale is used.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Set of key-value pairs that you can attach to an object. This can be useful for storing
        /// additional information about the object in a structured format.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The mode of the Checkout Session which can be <c>payment</c>, <c>setup</c>, or
        /// <c>subscription</c>.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// The list of payment method types (e.g. card) that this Checkout Session is allowed to
        /// use.
        /// </summary>
        public List<string> PaymentMethodTypes { get; set; }

        /// <summary>
        /// The URL the customer will be directed to after a successful payment.
        /// </summary>
        public string SuccessUrl { get; set; }
    }
}
