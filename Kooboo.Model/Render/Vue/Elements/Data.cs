using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Model.Render.Vue
{
    public partial class Data
    {
        public string Name { get; set; }

        public string Json { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Data;
            if (other == null)
                return false;

            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name == null ? 0 : Name.GetHashCode();
        }
    }

    partial class Data
    {
        public class Renderer : IVueRenderer
        {
            private static Dictionary<string, Action<InnerJsBuilder>> SpecialData = new Dictionary<string, Action<InnerJsBuilder>>
            {
                { LogicKeywords.PropsData, b => b.Append($"{VueKeywords.Props}: null") }
            };

            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                foreach (var item in items.Cast<Data>().Distinct().OrderBy(o => o.Name))
                {
                    if (SpecialData.TryGetValue(item.Name, out Action<InnerJsBuilder> specialData))
                    {
                        builder.Data(specialData);
                    }
                    else
                    {
                        builder.Data(b => b.Append($"{item.Name}: {item.Json}"));
                    }
                }
            }
        }
    }
}
