//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Service
{
    public static class DomService
    {

        public static string ApplyKoobooId(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }
            var doc = DomParser.CreateDom(html);
            var currentIndex = 0;
            var totallen = html.Length;

            var iterator = doc.createNodeIterator(doc.documentElement, enumWhatToShow.ELEMENT, null);

            var newHtml = new StringBuilder();

            var nextNode = iterator.nextNode();

            while (nextNode != null)
            {
                string koobooid = GetKoobooId(nextNode);

                var element = nextNode as Element;
                if (element != null && element.location.openTokenEndIndex > 0)
                {
                    int openTokenEndIndex = nextNode.location.openTokenEndIndex;
                    if (IsSelfCloseTag(element.tagName) && element.ownerDocument.HtmlSource[openTokenEndIndex - 1] == '/')
                    {
                        openTokenEndIndex = openTokenEndIndex - 1;
                    }

                    newHtml.Append(doc.HtmlSource.Substring(currentIndex, openTokenEndIndex - currentIndex));

                    newHtml.Append(" ").Append(Kooboo.Sites.SiteConstants.KoobooIdAttributeName).AppendFormat("=\"{0}\"", koobooid);

                    currentIndex = openTokenEndIndex; 

                }

                nextNode = iterator.nextNode();
            }

            if (currentIndex < totallen)
            {
                newHtml.Append(doc.HtmlSource.Substring(currentIndex));
            }

            return newHtml.ToString();

        }

        public static string GetKoobooId(Node node)
        {
            List<int> indexlist = new List<int>();

            indexlist.Add(node.siblingIndex);

            Node parentNode = node.parentNode;

            while (parentNode != null && parentNode.depth != 0)
            {
                indexlist.Add(parentNode.siblingIndex);

                parentNode = parentNode.parentNode;
            }

            indexlist.Reverse();

            return ConvertIntListToString(indexlist);

        }

        public static Node GetElementByKoobooId(Document doc, string KoobooId)
        {
            Node node = doc.documentElement;

            List<int> intlist = ConvertStringToIntList(KoobooId);

            foreach (int item in intlist)
            {
                if (item > node.childNodes.length - 1)
                {
                    return null;
                }
                node = node.childNodes.item[item];
            }

            return node;
        }

        public static List<int> ConvertStringToIntList(string IntListString)
        {
            List<int> intlist = new List<int>();

            string[] stringlist = IntListString.Split('-');

            foreach (var item in stringlist)
            {
                intlist.Add(Convert.ToInt32(item));
            }

            return intlist;
        }

        public static string ConvertIntListToString(List<int> IntList)
        {
            return string.Join("-", IntList);
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
            if (attributes !=null)
            {
                foreach (var item in attributes)
                {
                    ehtml += " " + item.Key;
                    if (!string.IsNullOrEmpty(item.Value))
                    {
                        ehtml += "=\"" + item.Value + "\"";
                    }
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
                if (element.location.openTokenEndIndex != -1 && element.location.openTokenStartIndex != -1)
                {
                    return element.ownerDocument.HtmlSource.Substring(element.location.openTokenStartIndex, element.location.openTokenEndIndex - element.location.openTokenStartIndex + 1);
                }
                else
                {
                    if (element.attributes ==null)
                    {
                        return GenerateOpenTag(null, element.tagName);
                    }
                    else
                    {
                        Dictionary<string, string> attr = new Dictionary<string, string>();
                        foreach (var item in element.attributes)
                        {
                            if (item != null && item.name != null)
                            {
                                attr.Add(item.name, item.value);
                            } 
                        } 
                        return GenerateOpenTag(attr, element.tagName); 
                    }
                   
                } 
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

        /// <summary>
        /// Parse the string into Dom Element. This method is only true when there is only one Element inside the Html String, the TAL element. 
        /// If there are more element, only the first element will be returned. 
        /// </summary>
        /// <param name="Html"></param>
        /// <returns></returns>
        public static Element ConvertToElement(string Html)
        {
            var doc = Dom.DomParser.CreateDom(Html);

            foreach (var item in doc.body.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    return item as Element;
                }
            }

            foreach (var item in doc.documentElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var el = item as Element;
                    if (el.tagName != "head" && el.tagName != "body")
                    {
                        return el;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Update a html source based on koobooid and new values. 
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Updates">Now use prefix kooboo_attribute to indicate that it is a change of attributes. </param>
        /// <returns></returns>
        public static string UpdateSource(string Source, Dictionary<string, string> Updates)
        {
            Document doc = Kooboo.Dom.DomParser.CreateDom(Source);

            return UpdateSource(doc, Updates);
        }

        public static string UpdateSource(Document doc, Dictionary<string, string> Updates)
        {
            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();

            foreach (var item in Updates)
            {
                string koobooid = CleanKoobooId(item.Key);
                if (!string.IsNullOrEmpty(koobooid))
                {
                    var element = GetElementByKoobooId(doc, koobooid);

                    if (element != null)
                    {
                        if (item.Value.StartsWith(SiteConstants.UpdateDomAttributePrefix))
                        {
                            /// this is to change the element attribute.  
                            Element currentitem = element as Element;
                            string AttributeJson = item.Value.Substring(SiteConstants.UpdateDomAttributePrefix.Length);

                            Dictionary<string, string> attrs = Lib.Helper.JsonHelper.Deserialize<Dictionary<string, string>>(AttributeJson);

                            foreach (var att in attrs)
                            {
                                // for safety reason. convert all duouble quote to single quote within the attribute value. 
                                string value = att.Value;
                                if (!string.IsNullOrEmpty(value))
                                {
                                    value = value.Replace("\"\"", "\"");
                                    value = value.Replace("\"", "'");
                                }
                                currentitem.setAttribute(att.Key, value);
                            }

                            SourceUpdate update = new SourceUpdate();
                            update.StartIndex = currentitem.location.openTokenStartIndex;
                            update.EndIndex = currentitem.location.openTokenEndIndex;
                            update.NewValue = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(currentitem);

                            sourceupdates.Add(update);

                        }
                        else
                        {
                            int start = element.location.openTokenEndIndex + 1;
                            int end = element.location.endTokenStartIndex - 1;
                            if (end < start)
                            {
                                end = -1;
                            }
                            sourceupdates.Add(new SourceUpdate()
                            {
                                StartIndex = start,
                                EndIndex = end,
                                NewValue = item.Value
                            });
                        }
                    }

                }
            }

            return UpdateSource(doc.HtmlSource, sourceupdates);
        }

        /// <summary>
        /// Update the source html based on the index location. 
        /// TODO: add  insertation, can be like endindex = -1; 
        /// </summary>
        /// <param name="htmlsource"></param>
        /// <param name="sourceupdates"></param>
        /// <returns></returns>
        public static string UpdateSource(string htmlsource, List<SourceUpdate> sourceupdates)
        {
            int currentindex = 0;
            int totallen = htmlsource.Length;
            StringBuilder sb = new StringBuilder();

            int laststart = -1;
            int lastend = -1;

            // update to real html source. 
            foreach (var item in sourceupdates.OrderBy(o => o.StartIndex))
            {
                if (item.StartIndex <= laststart || item.StartIndex < currentindex)
                {
                    // this is an insertation...
                    sb.Append(item.NewValue);
                    if (item.EndIndex > currentindex)
                    {
                        string currentString = htmlsource.Substring(currentindex, item.EndIndex - currentindex);
                        if (!string.IsNullOrEmpty(currentString))
                        {
                            sb.Append(currentString);
                        }
                        currentindex = item.EndIndex + 1;
                    }
                }
                else
                {
                    if (item.EndIndex > 0)
                    {
                        string currentString = htmlsource.Substring(currentindex, item.StartIndex - currentindex);
                        if (!string.IsNullOrEmpty(currentString))
                        {
                            sb.Append(currentString);
                        }
                        sb.Append(item.NewValue);

                        currentindex = item.EndIndex + 1;
                    }
                    else
                    {
                        // this should be treated as insertation. 
                        if (item.StartIndex > currentindex)
                        {
                            string currentString = htmlsource.Substring(currentindex, item.StartIndex - currentindex);
                            sb.Append(currentString);
                        }
                        sb.Append(item.NewValue);
                        currentindex = item.StartIndex;
                    }
                }

                if (laststart < item.StartIndex)
                { laststart = item.StartIndex; }

                if (lastend < item.EndIndex)
                {
                    lastend = item.EndIndex;
                }

            }

            if (currentindex < totallen - 1)
            {
                sb.Append(htmlsource.Substring(currentindex, totallen - currentindex));
            }

            return sb.ToString();
        }

        public static string UpdateInlineStyle(string HtmlSource, List<Kooboo.Sites.Models.InlineStyleChange> changes)
        {
            int currentindex = 0;
            int totallen = HtmlSource.Length;
            StringBuilder sb = new StringBuilder();
            Document doc = Kooboo.Dom.DomParser.CreateDom(HtmlSource);

            List<SourceUpdate> sourceupdates = new List<SourceUpdate>();

            foreach (var item in changes)
            {
                if (string.IsNullOrEmpty(item.KoobooId))
                { continue; }

                var node = GetElementByKoobooId(doc, item.KoobooId);
                if (node == null)
                { continue; }
                var element = node as Element;

                if (element == null)
                { continue; }

                var declarationtext = element.getAttribute("style");

                Kooboo.Dom.CSS.CSSDeclarationBlock declaration;
                if (string.IsNullOrEmpty(declarationtext))
                { declaration = new Dom.CSS.CSSDeclarationBlock(); }
                else
                {
                    declaration = Kooboo.Dom.CSS.CSSSerializer.deserializeDeclarationBlock(declarationtext);
                }

                foreach (var propertychange in item.PropertyValues)
                {
                    if (string.IsNullOrEmpty(propertychange.Value))
                    {
                        declaration.removeProperty(propertychange.Key);
                    }
                    else
                    {
                        declaration.setPropertyValue(propertychange.Key, propertychange.Value);
                    }
                }

                var newtext = declaration.GenerateCssText();

                if (string.IsNullOrEmpty(newtext))
                { element.removeAttribute("style"); }
                else
                { element.setAttribute("style", newtext); }


                string elementstring = ReSerializeOpenTag(element);

                SourceUpdate update = new SourceUpdate();
                update.StartIndex = element.location.openTokenStartIndex;
                update.EndIndex = element.location.openTokenEndIndex + 1;
                update.NewValue = Kooboo.Sites.Service.DomService.ReSerializeOpenTag(element);

                sourceupdates.Add(update);
            }

            // update to real html source. 
            foreach (var item in sourceupdates.OrderBy(o => o.StartIndex))
            {
                string currentString = doc.HtmlSource.Substring(currentindex, item.StartIndex - currentindex);
                if (!string.IsNullOrEmpty(currentString))
                {
                    sb.Append(currentString);
                }
                sb.Append(item.NewValue);

                currentindex = item.EndIndex;
            }

            if (currentindex < totallen - 1)
            {
                sb.Append(doc.HtmlSource.Substring(currentindex, totallen - currentindex));
            }

            return sb.ToString();
        }

        public static string CleanKoobooId(string originalKoobooId)
        {
            int commaindex = originalKoobooId.IndexOf(":");

            if (commaindex > -1)
            {
                return originalKoobooId.Substring(commaindex + 1);
            }
            else
            {
                return originalKoobooId;
            }
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

        /// <summary>
        /// Find the same element in the body container. 
        /// </summary>
        /// <param name="targetElement"></param>
        /// <param name="BodyContainer"></param>
        /// <returns></returns>
        public static Element getSameElement(Element targetElement, Element BodyContainer)
        {
            var parent = targetElement;
            while (parent.tagName != "body")
            {
                parent = parent.parentElement;
            }
            var element = _getSameElement(targetElement, parent, BodyContainer);
            return element;
        }

        public static Element getSameElement(Element targetElement, Document destinationDom)
        {
            return getSameElement(targetElement, destinationDom.body);
        }

        private static Element _getSameElement(Element target, Element targetParent, Element ContainerElement)
        {
            if (isSameElement(target, ContainerElement))
            {
                return ContainerElement;
            }

            if (ContainerElement.depth >= target.depth)
            {
                return null;
            }

            foreach (var item in ContainerElement.childNodes.item)
            {
                if (item is Element)
                {
                    foreach (var subParent in targetParent.childNodes.item)
                    {
                        if (subParent is Element)
                        {
                            Element subParentElement = subParent as Element;
                            Element itemElement = item as Element;

                            //TODO: check same element. 

                            Element returnelement = _getSameElement(target, subParent as Element, item as Element);
                            if (returnelement != null)
                            {
                                return returnelement;
                            }
                        }
                    }
                }

            }

            return null;
        }

        private static bool isSameElement(Element x, Element y)
        {
            if (x.depth != y.depth)
            {
                return false;
            }

            if (x.tagName != y.tagName)
            {
                return false;
            }

            /// check inner html. 
            if (x.InnerHtml != y.InnerHtml)
            {
                return false;
            }

            foreach (var item in x.attributes)
            {
                /// class can be like class = "active", we skip the class attribute for now...
                if (item.name != "class")
                {
                    var yitem = y.attributes.Find(o => o.name == item.name);
                    if (yitem == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (yitem.value != item.value)
                        {
                            return false;
                        }
                    }
                }
            }


            return true;

        }


        /// <summary>
        ///  Find the common parents of two elements. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Element FindParent(Element x, Element y)
        {
            if (x.depth == y.depth)
            {
                return _FindParent(x, y);
            }
            else
            {
                if (x.depth > y.depth)
                {
                    x = x.parentElement;
                    return FindParent(x, y);
                }
                else
                {
                    y = y.parentElement;
                    return FindParent(x, y);
                }
            }
        }

        private static Element _FindParent(Element x, Element y)
        {

            if (x.isEqualNode(y))
            {
                return x;
            }
            else
            {
                x = x.parentElement;
                y = y.parentElement;

                return _FindParent(x, y);
            }
        }

        public static Element FindParent(List<Element> elements)
        {
            Element parent = null;
            foreach (var item in elements)
            {
                if (parent == null)
                {
                    parent = item;
                }
                else
                {
                    parent = FindParent(parent, item);
                }
            }
            return parent;
        }
        /// <summary>
        /// Find the distince between two elements based on the node tree distince... 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetTreeDistance(Element x, Element y)
        {
            var parent = FindParent(x, y);

            if (parent == null)
            {
                return int.MaxValue;
            }

            List<Element> xchain = new List<Element>();
            List<Element> ychain = new List<Element>();

            var xparent = x.parentElement;

            while (xparent != null && xparent.depth != parent.depth)
            {
                xchain.Add(xparent);
                xparent = xparent.parentElement;
            }

            xchain.Reverse();

            var yparent = y.parentElement;

            while (yparent != null && yparent.depth != parent.depth)
            {
                ychain.Add(yparent);
                yparent = yparent.parentElement;
            }

            ychain.Reverse();

            int xcount = xchain.Count();
            int ycount = ychain.Count();
            int count = xcount;
            if (count < ycount)
            {
                count = ycount;
            }

            int xvalue = x.siblingIndex;
            int yvalue = y.siblingIndex;

            for (int i = 0; i < xcount; i++)
            {
                int addvalue = (xchain[i].siblingIndex + 1) * Convert.ToInt32(Math.Pow(10, (count - i)));
                xvalue = xvalue + addvalue;
            }

            for (int i = 0; i < ycount; i++)
            {
                int addvalue = (ychain[i].siblingIndex + 1) * Convert.ToInt32(Math.Pow(10, (count - i)));
                yvalue = yvalue + addvalue;
            }

            return Math.Abs(xvalue - yvalue);

        }

        /// <summary>
        /// Get the parent path of this element.
        /// Example: /body/div/div/p. p is the current element tag...
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static string getParentPath(Element element)
        {
            if (element == null)
            {
                return string.Empty;
            }
            var parent = element.parentElement;

            string path = "/" + element.tagName;

            while (parent != null && parent.tagName != "html")
            {
                path = "/" + parent.tagName + path;

                parent = parent.parentElement;
            }
            return path;
        }

        public static HTMLCollection GetElementsByTagName(List<Node> nodelist, string tagName)
        {

            HTMLCollection cols = new HTMLCollection();

            foreach (var item in nodelist)
            {
                if (item is Element)
                {
                    Element e = item as Element;

                    var col = e.getElementsByTagName(tagName).item;
                    if (col != null && col.Count > 0)
                    {
                        cols.item.AddRange(col);
                    }
                }
            }

            return cols;
        }


        /// <summary>
        /// test whether the sub element is contained with the parent or not.. 
        /// </summary>
        /// <param name="Parent"></param>
        /// <param name="Sub"></param>
        /// <returns></returns>
        public static bool ContainsOrEqualElement(Element Parent, Element Sub)
        {
            if (Sub.depth < Parent.depth)
            { return false; }

            var element = Sub;
            while (element.depth > Parent.depth)
            {
                element = element.parentElement;
                if (element == null)
                {
                    return false;
                }
            }
            return element.isEqualNode(Parent);
        }


        /// <summary>
        ///  get all node sitting between x & y. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static List<Node> GetNodesInBetween(Document doc, Element x, Element y)
        {
            List<Node> col = new List<Node>();

            if (ContainsOrEqualElement(x, y) || ContainsOrEqualElement(y, x))
            {
                return col;
            }

            if (x.location.openTokenStartIndex < y.location.openTokenStartIndex)
            {
                if (x.location.endTokenEndIndex > y.location.openTokenStartIndex)
                {
                    return col;
                }
                else
                {
                    _GetNodesInBetween(doc.body, col, x.location.endTokenEndIndex, y.location.openTokenStartIndex);
                    return col;
                }
            }
            else
            {
                if (y.location.endTokenEndIndex > x.location.openTokenStartIndex)
                { return col; }
                else
                {
                    _GetNodesInBetween(doc.body, col, y.location.endTokenEndIndex, x.location.openTokenStartIndex);
                    return col;
                }
            }

        }

        private static void _GetNodesInBetween(Node topElement, List<Node> collection, int startindex, int endindex)
        {
            if (topElement.location.openTokenStartIndex > startindex && topElement.location.endTokenEndIndex < endindex)
            {
                collection.Add(topElement);
            }
            else
            {
                foreach (var item in topElement.childNodes.item)
                {
                    _GetNodesInBetween(item, collection, startindex, endindex);
                }
            }
        }

        public static string ReplaceLink(Element linkElement, string href, string linktext)
        {
            linkElement.removeAttribute("href");
            linkElement.attributes.Add(new Attr() { name = "href", value = href });
            return ReSerializeElement(linkElement, linktext);
        }


        public static Element GetTitleElement(Dom.Document doc)
        {
            if (doc == null)
            {
                return null; 
            }

            var head = doc.head;
            foreach (var item in head.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var el = item as Element;
                    if (el.tagName.ToLower() == "title")
                    {
                        return el;
                    }
                }
            }

            return null;
        }

        public static List<SingleMeta> GetMetaValues(Dom.Document doc)
        {
            List<SingleMeta> metas = new List<SingleMeta>();

            var head = doc.head;
            foreach (var item in head.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var el = item as Element;
                    if (el.tagName.ToLower() == "meta")
                    {
                        SingleMeta meta = new SingleMeta();

                        meta.name = el.getAttribute("name");

                        meta.httpequiv = el.getAttribute("http-equiv");

                        meta.charset = el.getAttribute("charset");

                        meta.content = el.getAttribute("content");

                        metas.Add(meta);

                    }
                }
            }

            return metas;


        }

        public static string DetectKoobooId(Document dom, List<string> SubLinks)
        {
            var allinks = dom.getElementsByTagName("a").item;
            var GroupbyLinks = MenuService.SimpleGroupBy(allinks);
            bool match = true;
            bool onetimedismatch = false;
            foreach (var item in GroupbyLinks)
            {
                var allgrouplinks = item.Select(o => o.getAttribute("href"));

                match = false;
                foreach (var link in SubLinks)
                {
                    if (allgrouplinks.Any(o => IsSameUrl(o, link)))
                    {
                        match = true;
                    }
                    else
                    {
                        if (!onetimedismatch)
                        {
                            onetimedismatch = true;
                        }
                        else
                        {
                            match = false;
                            break;
                        }
                    }

                }

                if (match)
                {
                    var commoneparent = DomService.FindParent(item);
                    if (commoneparent != null)
                    {
                        return DomService.GetKoobooId(commoneparent);
                    }
                }
            }

            return null;
        }


        private static bool IsSameUrl(string x, string y)
        {
            if (Kooboo.Lib.Helper.StringHelper.IsSameValue(x, y))
            {
                return true;
            }
            x = x.Trim();
            y = y.Trim();
            x = x.Replace("\r", "");
            x = x.Replace("\n", "");

            y = y.Replace("\r", "");
            y = y.Replace("\n", "");

            if (Kooboo.Lib.Helper.StringHelper.IsSameValue(x, y))
            {
                return true;
            }
            return false;
        }

        public static string UpdateUrl(string Source, string OldUrl, string NewUrl)
        {
            if (string.IsNullOrEmpty(OldUrl))
            {
                return Source;
            }
            OldUrl = OldUrl.ToLower().Trim();
            List<SourceUpdate> updates = new List<SourceUpdate>();

            var doc = Kooboo.Dom.DomParser.CreateDom(Source);
            var els = GetElementsByUrl(doc, OldUrl);

            foreach (var item in els)
            {
                foreach (var att in item.attributes)
                {
                    if (!string.IsNullOrEmpty(att.value) && att.value.ToLower().Trim() == OldUrl)
                    {
                        att.value = NewUrl;
                        string newhtml = ReSerializeOpenTag(item);
                        updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenStartIndex, EndIndex = item.location.openTokenEndIndex, NewValue = newhtml });
                    }
                }
            }

            // this maybe a text with url directly in the field. in the case of text content media file.  
            els = Helper.ContentHelper.GetByTextContentMedia(doc, OldUrl);
            foreach (var item in els)
            {
                string value = item.InnerHtml;

                string newvalue = string.Empty;

                if (!string.IsNullOrEmpty(value) && value.ToLower().Trim() == OldUrl)
                {
                    newvalue = NewUrl;
                }
                else if (value.IndexOf(OldUrl, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    newvalue = Lib.Helper.StringHelper.ReplaceIgnoreCase(value, OldUrl, NewUrl);
                }

                updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenEndIndex + 1, EndIndex = item.location.endTokenStartIndex - 1, NewValue = newvalue });
            }

            if (updates.Count() > 0)
            {
                return UpdateSource(Source, updates);
            }
            return Source;
        }

        public static string DeleteUrl(string Source, string OldUrl)
        {
            if (string.IsNullOrEmpty(OldUrl))
            {
                return Source;
            }
            OldUrl = OldUrl.ToLower().Trim();
            List<SourceUpdate> updates = new List<SourceUpdate>();

            var doc = Kooboo.Dom.DomParser.CreateDom(Source);
            var els = GetElementsByUrl(doc, OldUrl);

            foreach (var item in els)
            {
                foreach (var att in item.attributes)
                {
                    if (!string.IsNullOrEmpty(att.value) && att.value.ToLower().Trim() == OldUrl)
                    {
                        //item.removeAttribute(att.name);
                        //string newhtml = ReSerializeOpenTag(item);
                        updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenStartIndex, EndIndex = item.location.endTokenEndIndex, NewValue = "" });
                        break;
                    }
                }
            }

            // this maybe a text with url directly in the field. in the case of text content media file.  
            els = Helper.ContentHelper.GetByTextContentMedia(doc, OldUrl);
            foreach (var item in els)
            {
                string value = item.InnerHtml;
                if (string.IsNullOrEmpty(value) || value.Length <= 1)
                {
                    continue;
                }
                value = value.Trim();
                string newvalue = string.Empty;

                if (!string.IsNullOrEmpty(value) && value.ToLower() == OldUrl)
                {
                    newvalue = "";
                    updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenEndIndex + 1, EndIndex = item.location.endTokenStartIndex - 1, NewValue = newvalue });
                }
                else if (value.IndexOf(OldUrl, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    // this is only the text content multile values.  
                    if (value.StartsWith("[") && value.EndsWith("]"))
                    {
                        try
                        {
                            List<string> files = Lib.Helper.JsonHelper.Deserialize<List<string>>(value);

                            if (files != null && files.Count() > 0)
                            {
                                List<int> removeindex = new List<int>();
                                int count = files.Count();
                                for (int i = 0; i < count; i++)
                                {
                                    var file = files[i];

                                    if (!string.IsNullOrWhiteSpace(file) && file.ToLower().Trim() == OldUrl)
                                    {
                                        removeindex.Add(i);
                                    }
                                }

                                if (removeindex.Count() > 0)
                                {
                                    foreach (var remove in removeindex.OrderByDescending(o => o))
                                    {
                                        files.RemoveAt(remove);
                                    }
                                    newvalue = Lib.Helper.JsonHelper.Serialize(files);
                                    updates.Add(new SourceUpdate() { StartIndex = item.location.openTokenEndIndex + 1, EndIndex = item.location.endTokenStartIndex - 1, NewValue = newvalue });

                                }
                            }

                        }
                        catch (Exception)
                        {
                        }

                    }
                }
            }

            if (updates.Count() > 0)
            {
                return UpdateSource(Source, updates);
            }
            return Source;
        }

        public static List<Element> GetElementsByUrl(Document doc, string url)
        {
            List<Element> result = new List<Element>();
            if (string.IsNullOrWhiteSpace(url))
            {
                return result;
            }

            _getElementByUrl(doc.documentElement, url.ToLower().Trim(), ref result);

            return result;
        }

        private static void _getElementByUrl(Element el, string Url, ref List<Element> result)
        {
            var link = DomUrlService.GetLinkOrSrc(el);
            if (!string.IsNullOrWhiteSpace(link) && link.Trim().ToLower() == Url)
            {
                result.Add(el);
            }

            foreach (var item in el.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var subel = item as Element;
                    _getElementByUrl(subel, Url, ref result);
                }
            }
        }

        public static List<string> GetElementPath(Element element)
        {
            if (element == null)
            {
                return new List<string>();
            }

            List<string> paths = new List<string>();

            while (element != null && element.tagName != "html")
            {
                var key = _GetElementKey(element);
                paths.Add(key);
                element = element.parentElement;
            }
            paths.Reverse();
            return paths;
        }

        private static string _GetElementKey(Element el)
        {
            if (el == null)
            {
                return null;
            }
            string key = el.tagName;
            if (!string.IsNullOrEmpty(el.id))
            {
                key += "||" + el.id;
            }
            var parent = el.parentElement;
            if (parent != null)
            {
                int count = 0;
                foreach (var item in parent.childNodes.item)
                {
                    var itemel = item as Element;
                    if (itemel != null)
                    {
                        if (itemel.isEqualNode(el))
                        {
                            break;
                        }
                        else
                        {
                            count += 1;
                        }
                    }
                }

                key = key + count.ToString();
            }

            return key.ToLower();

        }

        public static Element GetElementByPath(Document doc, List<string> Paths)
        {
            var el = doc.body;
            int index = 0;
            int pathcount = Paths.Count();

            if (pathcount == 0)
            {
                return null;
            }
            var bodykey = _GetElementKey(el);

            if (bodykey != Paths[0])
            {
                return null;
            }
             
            while (index <= pathcount - 1)
            {
                if (index == pathcount - 1)
                {
                    return el;
                }
                else
                {
                    Element sub = null;
                    var deeperkey = Paths[index + 1]; 
                    foreach (var item in el.childNodes.item)
                    {
                        var itemel = item as Element; 
                        if (itemel !=null)
                        {
                            var itemkey = _GetElementKey(itemel); 
                            if (itemkey == deeperkey)
                            {
                                sub = itemel; 
                            }
                        }
                    }

                    if (sub !=null )
                    {
                        el = sub;
                        index += 1; 
                    }
                    else
                    {
                        return null; 
                    }
                } 

            }
             

            return null; 


        }
          

        // el.tagName =='form' should have been checked before. 
        public static bool IsAspNetWebForm(Element el)
        { 
            foreach (var item in el.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var childel = item as Element;

                    if (childel.tagName == "input" && childel.id !=null && childel.id == "__VIEWSTATE")
                    {
                        return true; 
                    }

                    if (IsAspNetWebForm(childel))
                    {
                        return true; 
                    }
                }
            } 
            return false;   
        }
         
    }

}
