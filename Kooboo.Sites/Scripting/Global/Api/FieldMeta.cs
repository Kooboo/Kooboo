using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Api
{
    public class FieldMeta
    {
        private readonly object _name;
        private readonly object _required;
        private readonly object _type;

        public FieldMeta(IDictionary<string, object> dictionary, RenderContext renderContext)
        {
            dictionary.TryGetValue("name", out _name);
            dictionary.TryGetValue("required", out _required);
            dictionary.TryGetValue("type", out _type);
        }

        public string Name => _name as string;
        public bool Required => (bool)_required;
        public string Type => _type as string;
    }
}
