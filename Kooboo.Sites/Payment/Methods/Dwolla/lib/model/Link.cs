using System;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    public class Link
    {
        public Uri Href { get; set; }
        public string Type { get; set; }

        [JsonProperty(PropertyName = "resource-type")]
        public string ResourceType { get; set; }
    }
}