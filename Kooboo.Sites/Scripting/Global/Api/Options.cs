using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Api
{
    public class Options
    {
        public Options(IDictionary<string, object> options, RenderContext renderContext)
        {
            if (options.TryGetValue("metas", out var metas))
            {
                (metas as IDictionary<string, object>[]).Select(s => new FieldMeta(s, renderContext));
            }
        }

        public FieldMeta[] Metas { get; set; }
    }
}
