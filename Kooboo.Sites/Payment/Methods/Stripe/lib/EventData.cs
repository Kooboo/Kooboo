﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    public class EventData
    {
        /// <summary>
        /// Object containing the API resource relevant to the event. For example, an
        /// <c>invoice.created</c> event will have a full invoice object (<see cref="Invoice"/>) as
        /// the value of the object key.
        /// </summary>
        [JsonProperty("object")]
        [JsonConverter(typeof(StripeObjectConverter))]
        public IHasObject Object { get; set; }

        /// <summary>
        /// Object containing the names of the attributes that have changed, and their previous
        /// values (sent along only with *.updated events).
        /// </summary>
        [JsonProperty("previous_attributes")]
        public dynamic PreviousAttributes { get; set; }

        /// <summary>
        /// This contains the same data as <see cref="EventData.Object"/>, but untyped. This is
        /// occasionally useful, e.g. when working with an API resource for which the Stripe.net
        /// library does not have a concrete type.
        /// </summary>
        [JsonIgnore]
        public dynamic RawObject { get; set; }
    }
}
