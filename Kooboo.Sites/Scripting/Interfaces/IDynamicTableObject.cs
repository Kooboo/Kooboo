using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Scripting.Helper;
using System.Collections.Generic;

namespace KScript
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public interface IDynamicTableObject : IDynamic
    {
        [Kooboo.Data.Attributes.KIgnore]
        object this[string key] { get; set; }

        [Kooboo.Data.Attributes.KIgnore]
        IDictionary<string, object> obj { get; set; }
    }
}