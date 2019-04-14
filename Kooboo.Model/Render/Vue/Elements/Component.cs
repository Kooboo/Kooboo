using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Model.ValidationRules;

namespace Kooboo.Model.Render.Vue
{
    public partial class Component
    {
        public string Name { get; set; }

        public string Js { get; set; }
    }

    partial class Component
    {
        public class Renderer : IVueRenderer
        {
            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                foreach (Component item in items)
                {
                    builder
                        .Block($"const {item.Name} = {item.Js}")
                        .Components(b => b.Append(item.Name));
                }
            }
        }
    }
}
