using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kooboo.Model.Render.Vue
{
    public partial class LoadData
    {
        /// <summary>
        /// The model name which loaded data is assign to
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// The api url load from
        /// </summary>
        public string Url { get; set; }
    }

    partial class LoadData
    {
        public const string Keyword_ApiGet = "api.get";
        public const string Keyword_ParameterBind = "$parameterBinder().bind";

        public class RootViewRenderer : IVueRenderer
        {
            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                var notNullItems = items.Cast<LoadData>().Where(o => !String.IsNullOrEmpty(o.ModelName)).ToArray();

                builder.Created(b => RenderApiGets(b, notNullItems, options));
            }
        }

        public class SubViewRenderer : IVueRenderer
        {
            public const string Keyword_Show = "show";

            private Regex ParameterRegex = new Regex("\\{[^\\}]+\\}");

            public void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options)
            {
                builder.Methods(b =>
                {
                    b.AppendLine($"{Keyword_Show}: function({VueKeywords.Props}) {{").Indent();

                    b.Append($"this.{VueKeywords.Props} = {VueKeywords.Props}");

                    var notNullItems = items.Cast<LoadData>().Where(o => !String.IsNullOrEmpty(o.ModelName)).ToArray();
                    if (notNullItems.Any())
                    {
                        b.AppendLine();
                        RenderApiGets(b, notNullItems, options);
                    }

                    b.AppendLine().Unindent().Append("}");
                });
            }
        }

        public static void RenderApiGets(InnerJsBuilder builder, IEnumerable<LoadData> items, VueJsBuilderOptions options)
        {
            builder.AppendLine("const vm = this");
            builder.AppendLine("var url = ''").AppendLine();
            int i = 0;
            foreach (var item in items)
            {
                if (String.IsNullOrEmpty(item.ModelName))
                    continue;

                if (i > 0)
                {
                    builder.AppendLine().AppendLine();
                }

                builder.AppendLine($"url = vm.{Keyword_ParameterBind}('{item.Url}')");
                builder.Append($"{Keyword_ApiGet}(url).then(function(d) {{ vm.{item.ModelName} = d }})");

                i++;
            }
        }
    }
}
