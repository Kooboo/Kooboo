using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout
{
    public class CheckOrderRequest
    {
        [JsonProperty(PropertyName = "order_ids")]
        public List<string> OrderIDs { get; set; }
    }
}
