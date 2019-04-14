using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Dom;

namespace Kooboo.Model.Render.Parsers
{
    public class RefParser : IVirtualElementParser
    {
        public string Name => "ref";

        public int Priority => ParserPriority.High;

        public void Parse(Element el, TagParseContext context, Action visitChildren)
        {
            var viewUrl = el.getAttribute(context.Options.GetAttributeName(Name));
            if (!ViewName.TryParse(viewUrl, out ViewName viewName))
                throw new ViewParseException($"Invalid view name in kv-ref {viewUrl}");

            var html = context.ViewContext.ViewProvider.GetView(viewName.Name);

            var parser = new ViewParser(context.Options);
            var js = parser.RenderSubView(html, viewName.Parameters);

            context.Js.Component(viewName.Name, js);
        }

        class ViewName
        {
            public string Name { get; set; }

            public string[] Parameters { get; set; }

            public override string ToString()
            {
                if (Parameters == null || !Parameters.Any())
                    return Name;

                return Name + "?" + String.Join("&", Parameters.Select(o => $"{o}={{{o}}}"));
            }

            public static bool TryParse(string str, out ViewName result)
            {
                result = null;

                if (String.IsNullOrEmpty(str))
                    return false;

                var qindex = str.IndexOf('?');
                if (qindex < 0 || qindex == str.Length - 1)
                {
                    result = new ViewName { Name = str };
                    return true;
                }

                result = new ViewName { Name = str.Substring(0, qindex) };

                var query = str.Substring(qindex + 1);
                var spl = query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                result.Parameters = spl
                    .Select(o => o.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())
                    .Where(o => o != null)
                    .ToArray();

                return true;
            }
        }
    }
}
