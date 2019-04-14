using System;

using Kooboo.Dom;

namespace Kooboo.Model.Render.Parsers
{
    public class RootParser : IVirtualElementParser
    {
        public string Name => "el";

        public int Priority => ParserPriority.Highest;

        public void Parse(Element el, TagParseContext context, Action visitChildren)
        {
            var id = el.getAttribute("id");

            if (String.IsNullOrEmpty(id))
                throw new ViewParseException("Attribute \"id\" is required for root element.");

            context.Js.El($"#{id}");

            visitChildren?.Invoke();
        }
    }
}
