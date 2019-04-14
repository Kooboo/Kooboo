using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render.Vue
{
    partial class InnerJsBuilder
    {
        public static Dictionary<string, Action<InnerJsBuilder, List<Action<InnerJsBuilder>>>> Renderers = new Dictionary<string, Action<InnerJsBuilder, List<Action<InnerJsBuilder>>>>
        {
            { VueKeywords.Methods, RenderMethods },
            { VueKeywords.Created, RenderCreated },
            { VueKeywords.Data, RenderData },
            { VueKeywords.Components, RenderComponents },
            { VueKeywords.Validations, RenderValidations },
        };

        public static void RenderMethods(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            RenderProperty(builder, VueKeywords.Methods, innerRender);
        }

        public static void RenderCreated(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            RenderFunction(builder, VueKeywords.Created, innerRender);
        }

        public static void RenderData(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            if (builder.ViewType == ViewType.Root)
            {
                builder.AppendLine("data: {").Indent();
            }
            else
            {
                builder.AppendLine("data: function() {").Indent()
                    .AppendLine("return {").Indent();
            }

            RenderProperties(builder, innerRender);

            if (builder.ViewType == ViewType.Root)
            {
                builder.AppendLine()
                    .Unindent().Append("}");
            }
            else
            {
                builder.AppendLine()
                    .Unindent().AppendLine("}")
                    .Unindent().Append("}");
            }
        }

        public static void RenderComponents(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            RenderProperty(builder, VueKeywords.Components, innerRender);
        }

        public static void RenderValidations(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            RenderProperty(builder, VueKeywords.Validations, innerRender);
        }

        private static void RenderProperty(InnerJsBuilder builder, string name, List<Action<InnerJsBuilder>> innerRender)
        {
            builder.AppendLine($"{name}: {{").Indent();

            RenderProperties(builder, innerRender);

            builder.AppendLine().Unindent().Append("}");
        }

        private static void RenderFunction(InnerJsBuilder builder, string name, List<Action<InnerJsBuilder>> innerRender)
        {
            builder.AppendLine($"{name}: function() {{").Indent();

            foreach (var each in innerRender)
            {
                each(builder);
            }

            builder.AppendLine().Unindent().Append("}");
        }

        private static void RenderProperties(InnerJsBuilder builder, List<Action<InnerJsBuilder>> innerRender)
        {
            int i = 0;
            foreach (var each in innerRender)
            {
                if (i > 0)
                {
                    builder.AppendLine(",");
                }
                each(builder);
                i++;
            }
        }
    }

    public static class InnerJsBuilderExtensions
    {
        public static InnerJsBuilder Created(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(VueKeywords.Created, render);
        }

        public static InnerJsBuilder Methods(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(VueKeywords.Methods, render);
        }

        public static InnerJsBuilder Components(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(VueKeywords.Components, render);
        }

        public static InnerJsBuilder Data(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(VueKeywords.Data, render);
        }

        public static InnerJsBuilder Validations(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(VueKeywords.Validations, render);
        }

        public static InnerJsBuilder DirectRender(this InnerJsBuilder builder, Action<InnerJsBuilder> render)
        {
            return builder.AddItem(InnerJsBuilder.DirectRenderPrefix + Guid.NewGuid().ToString("n"), render);
        }
    }
}
