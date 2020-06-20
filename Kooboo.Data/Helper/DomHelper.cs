//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Helper
{
public static    class DomHelper
    {
          
        private static string _clearBodyStyle (string html)
        {
            var doc = Kooboo.Dom.DomParser.CreateDom(html);
            var iterator = doc.createNodeIterator(doc.body, enumWhatToShow.ELEMENT, null);

            var nextnode = iterator.nextNode();

            StringBuilder sb = new StringBuilder();

            int totallen = html.Length; 

            int currentindex = 0; 

            while (nextnode != null)
            {
                if (nextnode.nodeType != enumNodeType.ELEMENT)
                {
                    nextnode = iterator.nextNode(); 
                    continue; 
                }

                var el = nextnode as Element; 

                if (el.tagName == "img")
                {
                    nextnode = iterator.nextNode();
                    continue; 
                }

                if (el.hasAttribute("style") || el.hasAttribute("class"))
                { 
                    int len = nextnode.location.openTokenStartIndex - currentindex;
                     
                        var element = nextnode as Element;
                        bool IsSelfClosed = IsSelfCloseTag(el.tagName);
                  
                        if (len > 0)
                        {
                          sb.Append(doc.HtmlSource.Substring(currentindex, len));
                        }

                    el.removeAttribute("style");
                    el.removeAttribute("class"); 

                    sb.Append(ReSerializeOpenTag(el));  // if selfclosed, already closed. 
                     
                    if (IsSelfClosed)
                    {
                        currentindex = nextnode.location.endTokenEndIndex + 1;
                        nextnode = iterator.NextSibling(nextnode);
                    }
                    else
                    {
                        currentindex = nextnode.location.openTokenEndIndex + 1;
                        nextnode = iterator.nextNode();
                    } 
                }
                else
                {
                    nextnode = iterator.nextNode();
                }

            }

            if (currentindex < totallen - 1)
            {
               sb.Append(doc.HtmlSource.Substring(currentindex, totallen - currentindex));
            }  
            return sb.ToString();

        }

        public static string CleanBodyStyle(string html)
        {
            var result = _clearBodyStyle(html);

            if (result == null)
            {
                return null;
            } 
            result = RemoveBodyTag(result); 
            return result;
        }

        public static string RemoveBodyTag(string result)
        {
            int bodybegin = result.IndexOf("<body>", StringComparison.OrdinalIgnoreCase);
            int bodyend = result.IndexOf("</body>", StringComparison.OrdinalIgnoreCase);


            if (bodybegin > -1 && bodyend > bodybegin + 5)
            {
                result = result.Substring(bodybegin + 6, bodyend - bodybegin - 6);
            }

            return result;
        }

        public static bool IsSelfCloseTag(string tagName)
        {
            string lower = tagName.ToLower();

            if (lower == "img" || lower == "input" || lower == "link" || lower == "br" || lower == "area" || lower == "base" || lower == "col" || lower == "command" || lower == "embed" || lower == "hr" || lower == "keygen" || lower == "meta" || lower == "param" || lower == "source" || lower == "track" || lower == "wbr")
            {
                return true;
            }
            return false;
        }

        public static string GetElementDisplayName(Element element)
        {
            string name = element.tagName;

            if (!string.IsNullOrEmpty(element.id))
            {
                name = name + " id=" + element.id;
            }

            if (!string.IsNullOrEmpty(element.className))
            {
                name = name + " class=" + element.className;
            }

            return name;
        }

        /// <summary>
        /// after chanage of element attributes, reserialize the element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string ReSerializeElement(Element element)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            }

            if (IsSelfCloseTag(element.tagName))
            {
                ehtml += " />";
            }
            else
            {
                ehtml += ">";
                ehtml += element.InnerHtml;
                ehtml += "</" + element.tagName + ">";
            }

            return ehtml;
        }

        public static string GetHalfOpenTag(Element element)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            } 
            return ehtml;  
        }


        public static string ReSerializeOpenTag(Element element)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            }

            if (IsSelfCloseTag(element.tagName))
            {
                ehtml += " />";
            }
            else
            {
                ehtml += ">";
            }

            return ehtml;
        }

        public static string GenerateOpenTag(Dictionary<string, string> attributes, string tagName)
        {
            string ehtml = string.Empty;
            ehtml = "<" + tagName;
            foreach (var item in attributes)
            {
                ehtml += " " + item.Key;
                if (!string.IsNullOrEmpty(item.Value))
                {
                    ehtml += "=\"" + item.Value + "\"";
                }
            }
            ehtml += ">";
            return ehtml;
        }

        /// <summary>
        /// get the open tag part of this element without reserialize the element. 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string GetOpenTag(Element element)
        {
            if (element.location.openTokenStartIndex >= element.location.endTokenEndIndex)
            {
                return string.Empty;
            }
            else
            {
                return element.ownerDocument.HtmlSource.Substring(element.location.openTokenStartIndex, element.location.openTokenEndIndex - element.location.openTokenStartIndex + 1);
            }

        }

        public static bool HasHeadTag(Dom.Document doc)
        {
            var head = doc.head;

            return !IsFakeElement(head);

        }

        /// <summary>
        /// Is Fake and generated element, like Head.. without the <head> tag. 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool IsFakeElement(Element el)
        {
            if (el.location.openTokenStartIndex >= el.location.endTokenEndIndex)
            {
                return true;
            }

            if (el.location.endTokenEndIndex <= 0 || el.location.endTokenStartIndex <= 0 || el.location.openTokenEndIndex <= 0)
            {
                return true;
            }

            string outhtml = el.OuterHtml;
            if (outhtml.Length < 7)
            {
                return true;
            }

            if (el.location.endTokenEndIndex > 1)
            {
                return false;
            }

            return true;
        }

        public static string ReSerializeElement(Element element, string innerHtml)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            }

            if (IsSelfCloseTag(element.tagName))
            {
                ehtml += " />";
            }
            else
            {
                ehtml += ">";
                ehtml += innerHtml;
                ehtml += "</" + element.tagName + ">";
            }
            return ehtml;
        }

    }
}
