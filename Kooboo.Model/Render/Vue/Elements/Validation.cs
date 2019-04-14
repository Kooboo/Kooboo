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

        public List<Validation> Validations { get; set; } 

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
                        Render(b, item);
                        //b.AppendLine($"{ParserHelper.ToJsName(item.Name)}: [").Indent();
                        //int i = 0;
                        //foreach (var rule in item.Rules)
                        //{
                        //    if (i > 0)
                        //    {
                        //        b.AppendLine(",");
                        //    }
                        //    b.Append(rule.GetRule());
                        //    i++;
                        //}

                        //b.AppendLine().Unindent().Append("]");
                    });
                }
            }
            private void Render(InnerJsBuilder b,Validation item)
            {
                if (item.Rules==null||item.Rules.Count == 0)
                {
                    b.AppendLine($"{ParserHelper.ToJsName(item.Name)}:{{").Indent();
                    foreach(var valiation in item.Validations)
                    {
                        Render(b, valiation);
                        b.Append(",");
                    }
                    b.AppendLine().Unindent().Append("}");
                }
                else
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
                }
                
            }
        }
    }
}
