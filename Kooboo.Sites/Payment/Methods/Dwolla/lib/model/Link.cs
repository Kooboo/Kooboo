using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class Link
    {
        [JsonProperty(PropertyName = "href")]
        public Uri Href { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "resource-type")]
        public string ResourceType { get; set; }
    }
}