using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Dwolla.lib
{
    /// <summary>
    ///     Implemented by any model returned by DwollaClient
    /// </summary>
    public interface IDwollaResponse
    {
    }

    public abstract class BaseResponse : IDwollaResponse
    {
        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, Link> Links { get; set; }
    }
}