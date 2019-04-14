using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Dom;

namespace Kooboo.Model.Render
{ 
    public class ViewParser
    {
        public ViewParser(ViewParseOptions options)
        {
            Options = options;
        }

        public ViewParseOptions Options { get; }

        public void Parse(ViewParseContext context)
        {
            VisitNode(context.Dom, context);
        }

        private void VisitNode(Node node, ViewParseContext context)
        {
            if (!TryRenderNode(node, context, VisitChildern))
            {
                VisitChildern();
            }

            void VisitChildern()
            {
                foreach (var child in node.childNodes.item)
                {
                    VisitNode(child, context);
                }
            }
        }

        private bool TryRenderNode(Node node, ViewParseContext context, Action visitChildren)
        {
            var el = node as Element;
            if (el == null)
                return false;

            var potentialAttrs = el.attributes.Where(o => o.name.StartsWith(Options.AttributePrefix)).ToArray();
            var parsers = potentialAttrs
                .Select(o => Options.ElementParsers.TryGetValue(Options.GetVirtualElementName(o.name), out IVirtualElementParser parser) ? parser : null)
                .Where(o => o != null)
                .ToArray();

            if (!parsers.Any())
                return false;

            var tagContext = new TagParseContext(context, Options);

            foreach (var parser in parsers.OrderByDescending(o => o.Priority))
            {
                parser.Parse(el, tagContext, visitChildren);
            }

            // Remove kv- attributes
            foreach (var each in potentialAttrs)
            {
                el.removeAttribute(each.name);
            }

            return true;
        }
    }

    public static class ViewRendererExtensions
    {
        public static string RenderRootView(this ViewParser parser, string html, ModelRenderContext modelContext)
        {
            var context = new ViewParseContext
            {
                Dom = DomParser.CreateDom(html),
                Js = new Vue.RootViewJsBuilder(Vue.VueJsBuilderOptions.RootViewOptions),
                ViewProvider = new ViewProvider(modelContext)
            };

            parser.Parse(context);

            var result = new StringBuilder()
                .AppendLine(ParserHelper.ToHtml(context.Dom.childNodes.item))
                .AppendLine("<script>")
                .AppendLine(context.Js.Build())
                .AppendLine("</script>")
                .ToString();

            return result;
        }

        public static string RenderSubView(this ViewParser parser, string html, string[] parameters)
        {
            var js = new Vue.SubViewJsBuilder(Vue.VueJsBuilderOptions.SubViewOptions);
            var context = new ViewParseContext
            {
                Dom = DomParser.CreateDom(html),
                Js = js,
                ViewProvider = null,
                ViewType = ViewType.Sub,
                Parameters = parameters
            };

            parser.Parse(context);

            return js.BuildWithTemplate(ParserHelper.ToHtml(context.Dom.body.childNodes.item));
        }
    }
}
