using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Api;

namespace Kooboo.Model.Render
{
    public class ViewParseOptions
    {
        public static ViewParseOptions Instance { get; } = new ViewParseOptions()
            .UseParser<Parsers.RootParser>()
            .UseParser<Parsers.LoadParser>()
            .UseParser<Parsers.SubmitParser>()
            .UseParser<Parsers.RefParser>()
            .UseParser<Parsers.MetaParser>();

        public string AttributePrefix { get; set; } = "kv-";

        public ApiMeta.IApiMetaProvider ApiMetaProvider { get; set; }
        /// <summary>
        /// Map from attribute name to corresponding renderer.
        /// </summary>
        public Dictionary<string, IVirtualElementParser> ElementParsers { get; } = new Dictionary<string, IVirtualElementParser>();

        public ViewParseOptions UseParser<T>()
            where T : IVirtualElementParser
        {
            return UseParser(Activator.CreateInstance<T>());
        }

        public ViewParseOptions UseParser(IVirtualElementParser parser)
        {
            ElementParsers.Add(parser.Name, parser);
            return this;
        }

        public string GetVirtualElementName(string prefixedName)
        {
            return prefixedName.Substring(AttributePrefix.Length);
        }

        public string GetAttributeName(string unprefixName)
        {
            return AttributePrefix + unprefixName;
        }
    }
}
