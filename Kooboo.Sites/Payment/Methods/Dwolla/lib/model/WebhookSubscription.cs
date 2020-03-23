using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class WebhookSubscription : BaseResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool Paused { get; set; }
        public DateTime Created { get; set; }
    }
}
