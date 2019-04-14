using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Dom;

namespace Kooboo.Model.Render.OutputHtml
{
    public class OutputHtml
    {
        private static HashSet<string> SelfClosingTags = new HashSet<string>
        {
            "area",
            "base",
            "br",
            "col",
            "embed",
            "hr",
            "img",
            "input",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr",
        };

        private static Dictionary<enumNodeType, Action<TextWriter, Node>> OutputDelegates = new Dictionary<enumNodeType, Action<TextWriter, Node>>
        {
            { enumNodeType.COMMENT, OutputComment },
            { enumNodeType.TEXT, OutputText },
            { enumNodeType.ELEMENT, OutputElement }
        };

        private IEnumerable<Node> _nodes;

        public OutputHtml(IEnumerable<Node> nodes)
        {
            _nodes = nodes;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                foreach (var node in _nodes)
                {
                    OutputNode(writer, node);
                }
            }

            return builder.ToString();
        }

        private static void OutputNode(TextWriter writer, Node node)
        {
            if (!OutputDelegates.TryGetValue(node.nodeType, out Action<TextWriter, Node> output))
                throw new Exception($"Encounter unsupported node type {node.nodeType}");

            output(writer, node);
        }

        private static void OutputComment(TextWriter writer, Node node)
        {
            var comment = node as Comment;
            writer.Write("<!--");
            writer.Write(comment.data);
            writer.Write("-->");
        }

        private static void OutputText(TextWriter writer, Node node)
        {
            var text = node as Text;
            writer.Write(text.data);
        }

        private static void OutputElement(TextWriter writer, Node node)
        {
            var el = node as Element;

            // Open tag
            writer.Write(Symbols.LessThan);
            writer.Write(el.tagName);

            foreach (var attr in el.attributes)
            {
                writer.Write(" ");
                OutputAttribute(writer, attr);
            }

            if (el.childNodes.item.Count == 0 && SelfClosingTags.Contains(el.localName))
            {
                writer.Write("/>");
                return;
            }
            else
            {
                writer.Write(Symbols.GreaterThan);
            }

            // Children
            var text = el.firstChild() as Text;
            if (text != null && text.data.Length > 0 && text.data[0] == Symbols.LineFeed)
            {
                writer.Write(Symbols.LineFeed);
            }

            foreach (var child in el.childNodes.item)
            {
                OutputNode(writer, child);
            }

            // Close tag
            writer.Write("</");
            writer.Write(el.tagName);
            writer.Write(">");
        }

        private static void OutputAttribute(TextWriter writer, Attr attr)
        {
            writer.Write(attr.name);
            writer.Write(Symbols.Equality);
            writer.Write(Symbols.DoubleQuote);

            foreach (var c in attr.value)
            {
                switch (c)
                {
                    case Symbols.Ampersand: writer.Write("&amp;"); break;
                    case Symbols.NoBreakSpace: writer.Write("&nbsp;"); break;
                    case Symbols.DoubleQuote: writer.Write("&quot;"); break;
                    default: writer.Write(c); break;
                }
            }

            writer.Write(Symbols.DoubleQuote);
        }
    }
}
