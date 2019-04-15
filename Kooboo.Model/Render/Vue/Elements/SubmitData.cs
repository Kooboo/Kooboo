using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Model.Render.Vue
{
    public partial class SubmitData
    {
        public string Url { get; set; }

        public string ModelName { get; set; }
    }

    partial class SubmitData
    {
        public const string Keyword_Submit = "submit";
        public const string Keyword_ApiPost = "api.post";
        public const string Keyword_Validate = "api.post";

        public class Renderer : IVueRenderer
        {
            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                foreach (SubmitData item in items)
                {
                    builder.Methods(b =>
                    {
                        b.AppendLine($"{Keyword_Submit}_{item.ModelName}: function(e) {{").Indent();

                        b.AppendLine($"const vm = this");
                        b.AppendLine($"this.$v.{item.ModelName}.$touch()");
                        b.AppendLine($"var valid=!this.$v.{item.ModelName}.$invalid");
                        b.AppendLine("//need encapsulate,then use this code");
                        b.AppendLine($"//e.target.validate(function(valid) {{").Indent();
                        b.AppendLine($"if (!valid) return");
                        b.AppendLine($"const url = {LoadData.Keyword_ParameterBind}('{item.Url}', vm)");
                        b.AppendLine($"{Keyword_ApiPost}(url, this.{item.ModelName})");
                        b.Unindent().AppendLine($"//}})");

                        b.Unindent().Append("}");
                    });
                }
            }
        }
    }
}
