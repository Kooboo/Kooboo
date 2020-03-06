﻿using Newtonsoft.Json;
using System;

namespace Kooboo.Sites.Payment.Methods.Stripe.lib
{
    //[JsonConverter(typeof(EventConverter))]
    public class Event //: StripeEntity<Event>, IHasId
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// String representing the object’s type. Objects of the same type share the same value.
        /// </summary>
        [JsonProperty("object")]
        public string Object { get; set; }

        /// <summary>
        /// For Connect events, the identifier of the account that emitted the event.
        /// </summary>
        [JsonProperty("account")]
        public string Account { get; set; }

        /// <summary>
        /// The Stripe API version used to render <c>data</c>.
        /// </summary>
        /// <remarks>
        /// Note: This property is populated only for events on or after October 31, 2014.
        /// </remarks>
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// Time at which the object was created.
        /// </summary>
        [JsonProperty("created")]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.UnixDateTimeConverter))]
        public DateTime Created { get; set; }

        /// <summary>
        /// Object containing data associated with the event.
        /// </summary>
        [JsonProperty("data")]
        public EventData Data { get; set; }

        /// <summary>
        /// Has the value <c>true</c> if the object exists in live mode or the value <c>false</c> if
        /// the object exists in test mode.
        /// </summary>
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        /// <summary>
        /// Number of webhooks that have yet to be successfully delivered (i.e., to return a 20x
        /// response) to the URLs you’ve specified.
        /// </summary>
        [JsonProperty("pending_webhooks")]
        public long PendingWebhooks { get; set; }

        /// <summary>
        /// Information on the API request that instigated the event.
        /// </summary>
        [JsonProperty("request")]
        public EventRequest Request { get; set; }

        /// <summary>
        /// Description of the event (e.g., <c>invoice.created</c> or <c>charge.refunded</c>).
        /// </summary>
        /// <seealso cref="Events"/>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class EventRequest //: StripeEntity<EventRequest>, IHasId
    {
        /// <summary>
        /// ID of the API request that caused the event. If null, the event was automatic (e.g.,
        /// Stripe’s automatic subscription handling). Request logs are available in the
        /// <a href="https://dashboard.stripe.com/logs">dashboard</a>, but currently not in the API.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The idempotency key transmitted during the request, if any.
        /// </summary>
        /// <remarks>
        /// Note: This property is populated only for events on or after May 23, 2017.
        /// </remarks>
        [JsonProperty("idempotency_key")]
        public string IdempotencyKey { get; set; }
    }
}
