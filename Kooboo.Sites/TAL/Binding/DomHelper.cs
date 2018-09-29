//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Dom;

namespace Kooboo.TAL.Binding
{
    public static class DomHelper
    {
        private static readonly HashSet<string> SelfClosingTags = new HashSet<string>
        {
            "img", "hr", "br"
        };

        public static string ToHtml(this Node node, bool indent = false, int level = 0)
        {
            if (node.nodeType == enumNodeType.COMMENT)
            {
                var comment = String.Format("<!--{0}-->", ((Comment) node).data);
                if (!indent || level == 0)
                {
                    return comment;
                }

                return new String(' ', 4 * level) + comment;
            }
            
            if (node.nodeType == enumNodeType.ELEMENT)
            {
                var element = (Element)node;

                var html = new StringBuilder();

                if (indent && level > 0)
                {
                    html.Append(' ', 4*level);
                }

                html.Append("<").Append(element.tagName);

                if (element.attributes.Count > 0)
                {
                    foreach (var attr in element.attributes)
                    {
                        if (attr.name.StartsWith("__"))
                        {
                            continue;
                        }

                        html.Append(" ").Append(attr.name).AppendFormat("=\"{0}\"", attr.value);
                    }
                }

                if (SelfClosingTags.Contains(element.tagName))
                {
                    html.Append(" />");
                    return html.ToString();
                }

                html.Append(">");

                foreach (var childNode in element.childNodes.item)
                {
                    if (indent && childNode.nodeType == enumNodeType.ELEMENT)
                    {
                        html.AppendLine();
                    }

                    html.Append(childNode.ToHtml(indent, level + 1));
                }

                if (element.childNodes.item.Count > 0 &&
                    element.childNodes.item[element.childNodes.item.Count - 1].nodeType == enumNodeType.ELEMENT)
                {
                    html.AppendLine();
                    html.Append(' ', level*4);
                }

                html.Append("</").Append(element.tagName).Append(">");

                return html.ToString();
            }

            return node.textContent;
        }
    }
}
