using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Model.ValidationRules;

namespace Kooboo.Model.Render.Vue
{
    public partial class Validation
    {
        public string Name { get; set; }

        public List<ValidationRule> Rules { get; set; }
    }

    partial class Validation
    {
        public class Renderer : IVueRenderer
        {
            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                foreach (Validation item in items)
                {
                    builder.Validations(b =>
                    {
                        b.AppendLine($"{ParserHelper.ToJsName(item.Name)}: [").Indent();
                        int i = 0;
                        foreach (var rule in item.Rules)
                        {
                            if (i > 0)
                            {
                                b.AppendLine(",");
                            }
                            b.Append(rule.GetRule());
                            i++;
                        }

                        b.AppendLine().Unindent().Append("]");
                    });
                }
            }
        }
    }
}
