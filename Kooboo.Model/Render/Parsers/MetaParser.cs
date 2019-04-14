using System;

using Kooboo.Dom;

namespace Kooboo.Model.Render.Parsers
{
    public class MetaParser : IVirtualElementParser
    {
        public string Name => "meta";

        public int Priority => ParserPriority.High;

        public void Parse(Element el, TagParseContext context, Action visitChildren)
        {
        }
    }
}
