//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Dom;
using Kooboo.Extensions;
using Kooboo.Sites.Models;


namespace Kooboo.Sites.SiteElements
{
    public class PageElementManager
    {
        public static DomElement ConvertToPageElement(Element element)
        {
            DomElement pageelement = new DomElement();
            pageelement.KoobooId = Kooboo.Sites.Service.DomService.GetKoobooId(element);
            pageelement.NodeAttributes = element.attributes.ToDictionary(o => o.name, o => o.value);
            pageelement.Name = element.nodeName;
            pageelement.ParentPath = getParentPath(element);
            // pageelement.SubElements = GetSubElements(element); 
            //pageelement.SubElementHash = GetSubElementString(element).ToHashGuid(); 
            pageelement.SubElementString = GetSubElementString(element); 
            pageelement.Depth = element.depth;
            pageelement.Sibling = element.siblingIndex;
            pageelement.OpenTagStartIndex = element.location.openTokenStartIndex;
            pageelement.EndTagEndIndex = element.location.endTokenEndIndex; 
            pageelement.InnerHtmlHash = element.InnerHtml.ToHashGuid(); 

            return pageelement;
        }

        private static string getParentPath(Element element)
        {
            var parent = element.parentElement;

            string path = "/" + element.tagName;

            while (parent != null && parent.tagName != "html")
            {
                path = "/" + parent.tagName + path;

                parent = parent.parentElement;
            }
            return path;
        }

        private static string GetSubElementString(Element element)
        {
            string sub = string.Empty;
            foreach (var item in element.childNodes.item)
            {
                string subvalue = string.Empty;
                switch (item.nodeType)
                {
                    case enumNodeType.ELEMENT:
                        Element e = item as Element;

                        if (!Kooboo.Sites.Tag.TagGroup.isText(e.tagName))
                        {
                            subvalue = e.tagName;
                            if (e.attributes.Count > 0)
                            {
                                subvalue += "[";
                                foreach (var att in e.attributes)
                                {
                                    subvalue += att.name + "=" + att.value + "|";
                                }
                                if (subvalue.EndsWith("|"))
                                {
                                    subvalue = subvalue.TrimEnd('|');
                                }
                                subvalue += "]";
                            }
                        }
                        else
                        {
                            subvalue += e.tagName + ":" + Lib.Security.Hash.ComputeIntCaseSensitive(e.InnerHtml);
                        }
                        break;
                    case enumNodeType.ATTRIBUTE:
                        break;
                    case enumNodeType.TEXT:
                        Kooboo.Dom.Text text = item as Kooboo.Dom.Text;
                        subvalue += "text:" + Lib.Security.Hash.ComputeIntCaseSensitive(text.data);
                        break;
                    case enumNodeType.CDATA_SECTION:
                        break;
                    case enumNodeType.ENTITY_REFERENCE:
                        break;
                    case enumNodeType.ENTITY:
                        break;
                    case enumNodeType.PROCESSING_INSTRUCTION:
                        break;
                    case enumNodeType.COMMENT:
                        Kooboo.Dom.Comment comment = item as Comment;
                        subvalue += "comment:" + Lib.Security.Hash.ComputeIntCaseSensitive(comment.data);
                        break;
                    case enumNodeType.DOCUMENT:
                        break;
                    case enumNodeType.DOCUMENT_TYPE:
                        break;
                    case enumNodeType.DOCUMENT_FRAGMENT:
                        break;
                    case enumNodeType.NOTATION:
                        break;
                    default:
                        break;
                }
                sub = sub + "/" + subvalue;
            }
            return sub;
        }

        private static List<KeyValuePair<string, string>> GetSubElements(Element element)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            foreach (var item in element.childNodes.item)
            {
                switch (item.nodeType)
                {
                    case enumNodeType.ELEMENT:
                        Element e = item as Element;

                        if (!Kooboo.Sites.Tag.TagGroup.isText(e.tagName))
                        {
                           string subvalue = string.Empty;
                           if (e.attributes.Count > 0)
                           {
                               foreach (var att in e.attributes)
                               {
                                   subvalue += att.name + "=" + att.value;
                               }
                           }
                           result.Add(new KeyValuePair<string, string>(e.siblingIndex.ToString() + e.tagName, subvalue)); 
                        }
                   
                        else
                        {
                            result.Add(new KeyValuePair<string, string>(e.tagName, "text:" + Lib.Security.Hash.ComputeIntCaseSensitive(e.InnerHtml)));
                        }
                        break;
                    case enumNodeType.ATTRIBUTE:
                        break;
                    case enumNodeType.TEXT:

                        Kooboo.Dom.Text text = item as Kooboo.Dom.Text;
                        string data = text.data.Replace(" ", string.Empty);
                        data = data.Replace(Environment.NewLine, string.Empty);

                        result.Add(new KeyValuePair<string, string>("text", Lib.Security.Hash.ComputeIntCaseSensitive(data).ToString()));

                        break;
                    case enumNodeType.CDATA_SECTION:
                        break;
                    case enumNodeType.ENTITY_REFERENCE:
                        break;
                    case enumNodeType.ENTITY:
                        break;
                    case enumNodeType.PROCESSING_INSTRUCTION:
                        break;
                    case enumNodeType.COMMENT:
                        Kooboo.Dom.Comment comment = item as Comment;
                        result.Add(new KeyValuePair<string, string>("comment", Lib.Security.Hash.ComputeIntCaseSensitive(comment.data).ToString()));
                        break;
                    case enumNodeType.DOCUMENT:
                        break;
                    case enumNodeType.DOCUMENT_TYPE:
                        break;
                    case enumNodeType.DOCUMENT_FRAGMENT:
                        break;
                    case enumNodeType.NOTATION:
                        break;
                    default:
                        break;
                }
            }

            return result; 
        }
         
    }
}
