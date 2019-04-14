using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.Vue
{
    public class VueJsHelper
    {
        public static void Build(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
        {
            foreach (var group in items.GroupBy(o => o.GetType()))
            {
                if (!options.Renderers.TryGetValue(group.Key, out IVueRenderer renderer))
                    throw new NotSupportedException($"Type {group.Key.Name} is not supported by JS render");

                renderer.Render(builder, group, options);
            }
        }
    }
}
