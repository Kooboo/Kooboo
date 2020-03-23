using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class GetWebhookSubscriptionsResponse : BaseGetResponse<WebhookSubscription>
    {
        [JsonProperty(PropertyName = "_embedded")]
        public new WebhookSubscriptionsEmbed Embedded { get; set; }
    }

    public class WebhookSubscriptionsEmbed : Embed<WebhookSubscription>
    {
        [JsonProperty(PropertyName = "webhook-subscriptions")]
        public List<WebhookSubscription> WebhookSubscriptions { get; set; }

        public override List<WebhookSubscription> Results() => WebhookSubscriptions;
    }
}
