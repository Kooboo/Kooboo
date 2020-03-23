using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class WebhooksEvent
    {
        public string ApiVersion { get; set; }

        public string Id { get; set; }

        public string Created { get; set; }

        public string MerchantId { get; set; }

        public string Type { get; set; }

        public Payment Payment { get; set; }
    }
}
