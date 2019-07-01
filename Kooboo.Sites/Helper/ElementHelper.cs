//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kooboo.Sites.Helper
{
    public static class ElementHelper
    {
        //public static DomElement ConvertToPageElement(Element element)
        //{
        //    DomElement pageelement = new DomElement();
        //    pageelement.KoobooId = Kooboo.Sites.Service.DomService.GetKoobooId(element);
        //    pageelement.NodeAttributes = element.attributes.ToDictionary(o => o.name, o => o.value);
        //    pageelement.Name = element.nodeName;
        //    pageelement.ParentPath = getParentPath(element);
        //    // pageelement.SubElements = GetSubElements(element); 
        //    //pageelement.SubElementHash = GetSubElementString(element).ToHashGuid(); 
        //    pageelement.SubElementString = GetSubElementString(element);
        //    pageelement.Depth = element.depth;
        //    pageelement.Sibling = element.siblingIndex;
        //    pageelement.OpenTagStartIndex = element.location.openTokenStartIndex;
        //    pageelement.EndTagEndIndex = element.location.endTokenEndIndex;
        //    pageelement.InnerHtmlHash = element.InnerHtml.ToHashGuid();
        //    return pageelement;
        //}

        //private bool isSamePageElement(DomElement x, DomElement y)
        //{
        //    if (x.ParentPathHash != y.ParentPathHash)
        //    {
        //        return false;
        //    }

        //    if (x.Name != y.Name)
        //    {
        //        return false;
        //    }

        //    if (x.InnerHtmlHash != y.InnerHtmlHash)
        //    {
        //        return false;
        //    }

        //    if (x.Depth != y.Depth)
        //    {
        //        return false;
        //    }

        //    if (x.NodeAttributeHash != y.NodeAttributeHash)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public static string GetParentPath(Element element)
        {
            var parent = element.parentElement;
            string path = "/" + element.tagName;
            if(!string.IsNullOrEmpty(element.id))
            {  path += "[" + element.id + "]"; }

            while (parent != null && parent.tagName != "html")
            {
                path = "/" + parent.tagName + path;
                if (!string.IsNullOrEmpty(parent.id))
                {
                    path += "[" + parent.id + "]";
                } 
                parent = parent.parentElement;
            }
            return path.ToLower();
        }


        public static string GetSubElementPath(Element element)
        {
            string sub = string.Empty; 
            foreach (var item in element.childNodes.item)
            {
                 if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var el = item as Element;
                    sub += el.tagName + el.id;  
                }
            }
            return sub; 
        }

        public static string GetSiblingElementsSimple(Element element, bool before = true)
        {
            string SiblingString = string.Empty;
            List<Element> siblings = new List<Element>();
            var parent = element.parentElement;
            if (parent == null || parent.tagName == "html")
            {
                return null;
            }
            List<Node> nodelist;
            if (before)
            {
                nodelist = parent.childNodes.item.Where(o => o.siblingIndex < element.siblingIndex).ToList();
            }
            else
            {
                nodelist = parent.childNodes.item.Where(o => o.siblingIndex > element.siblingIndex).ToList();
            }

            if (nodelist != null)
            {
                foreach (var item in nodelist)
                { 
                    if (item.nodeType == enumNodeType.ELEMENT)
                    {
                        Element el = item as Element;
                        siblings.Add(el);
                    }
                }
            }

            foreach (var item in siblings)
            {
                // for more strict one, this should include attributes. 
                if (before)
                {
                    SiblingString += item.tagName + item.id; 
                }
                else
                {
                    SiblingString += item.OuterHtml; 
                }
            }

            return SiblingString; 
        }

        public static Element FindSameElement(Element sourceElement, Document TargetDom)
        {
            if (TargetDom == null)
            {
                return null; 
            }

            var targets = FindElementsByDepth(TargetDom, sourceElement.depth);

            if (targets.Count() == 0)
            {
                return null;
            }

            // filter by same parentpath. 
            string parentpath = GetParentPath(sourceElement);

            var sameparent = targets.Where(o => GetParentPath(o) == parentpath).ToList();

            if (sameparent == null || sameparent.Count() == 0)
            {
                return null;
            }

            // filter by same inner html.  
            var sameinner = FindSameInnerElements(sourceElement, sameparent);  

            string siblingpath = GetSiblingElementsSimple(sourceElement); 

            foreach (var item in sameinner)
            {
                //check for sample front sibling. 
                var ItemSibling = GetSiblingElementsSimple(item); 
                if (ItemSibling == siblingpath)
                {
                    return item; 
                } 
            }

            siblingpath = GetSiblingElementsSimple(sourceElement, false);
            foreach (var item in sameinner)
            { 
                var ItemSibling = GetSiblingElementsSimple(item, false);
                if (ItemSibling == siblingpath)
                {
                    return item;
                }
            }

            return null;
        }

        internal static List<Element> FindElementsByDepth(Document doc, int depth)
        {
            HTMLCollection collection = new HTMLCollection();

            _FindByDepth(doc.documentElement, depth, ref collection);
            return collection.item.ToList();
        }

        private static void _FindByDepth(Node topEl, int depth, ref HTMLCollection col)
        {
            if (topEl.nodeType == enumNodeType.ELEMENT)
            {
                var el = topEl as Element;

                if (el.depth == depth)
                {
                    col.Add(el);
                }
                else if (topEl.depth < depth)
                {
                    foreach (var item in topEl.childNodes.item)
                    {
                        _FindByDepth(item, depth, ref col);
                    }
                }
            }

        }
        
        internal static List<Element> FindSameInnerElements(Element el, List<Element> targets)
        {
            //should remove all space between elements.  
            var exactmatch = targets.Where(o => o.InnerHtml == el.InnerHtml).ToList(); 
             
            if (exactmatch != null &&exactmatch.Count()>0)
            {
                return exactmatch; 
            } 
            // remove not possible items. 
            var elsub = GetSubElementPath(el); 
            var left = targets.Where(o => GetSubElementPath(o) == elsub).ToList(); 
            
            if (left !=null &&left.Count()>0)
            {
                //try to check without space.. 
                string nonspace = RemoveSpace(el.InnerHtml);

                return left.Where(o => RemoveSpace(o.InnerHtml) == nonspace).ToList();  
            }
                          
            return new List<Element>(); 
        } 

        private static string RemoveSpace(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty; 
            }
            input = input.Replace("\r", " ");
            input = input.Replace("\n", " ");
            input = Regex.Replace(input, @"\s+", " ");
            input = input.Trim(); 
            return input;       
        }
    }
}
