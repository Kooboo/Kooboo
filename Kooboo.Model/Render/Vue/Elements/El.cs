using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Model.Render.Vue
{
    public partial class El
    {
        public string Name { get; set; }

        public List<object> Items { get; } = new List<object>();
    }

    partial class El
    {
        public class Renderer : IVueRenderer
        {
            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                var item = items.FirstOrDefault() as El;
                if (item == null)
                    throw new ArgumentException("Contains at least one El", nameof(items));

                builder.DirectRender(b => b.Append($"el: '{item.Name}'"));
            }
        }
    }
}
