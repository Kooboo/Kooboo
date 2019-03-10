//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System.Web;
using System.Collections.Generic;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class HtmlTag : IDiagnosis
    {
        public DiagnosisSession session { get; set; }

        public string Group(RenderContext context)
        {
            return Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("Check html tag for empty, too deep hierarchy, outdated tag and invalid nested tags", context);
        }

        private string _line;
        private string line
        {
            get
            {
                if (_line == null)
                {
                    _line = Hardcoded.GetValue("Line", this.session.context);
                }
                return _line;
            }

        }

        public void Check()
        {
            var sitedb = this.session.context.WebSite.SiteDb();

            this.session.Headline = Hardcoded.GetValue("Checking", session.context) + " " + Hardcoded.GetValue("Html tag", session.context) + "...";

            var allrepos = sitedb.ActiveRepositories();

            foreach (var repo in allrepos)
            {
                if (Lib.Reflection.TypeHelper.HasInterface(repo.ModelType, typeof(IDomObject)))

                {
                    var allitems = repo.All();

                    foreach (var item in allitems)
                    {
                        var domobj = item as IDomObject;
                        if (domobj != null)
                        {
                            var dom = domobj.Dom;

                            CheckEmptyTag(domobj as SiteObject, dom);

                            CheckDepth(domobj as SiteObject, dom);

                            CheckOutdateTag(domobj as SiteObject, dom);

                            CheckWrongNestedTag(domobj as SiteObject, dom); 
                        }
                    }
                }
            }
        }


        public void CheckEmptyTag(SiteObject siteobject, Kooboo.Dom.Document dom)
        {
            if (siteobject == null || dom == null)
            {
                return;
            }
            string name = Hardcoded.GetValue("Empty tag", session.context);
            HTMLCollection col = new HTMLCollection();
            getEmptyTag(dom.body, col);

            foreach (var item in col.item)
            {
                string message = HttpUtility.HtmlEncode(item.OuterHtml);
                message += DiagnosisHelper.DisplayUsedBy(session.context, siteobject);
                session.AddMessage(name, message, MessageType.Warning);
            }
        }

        private void getEmptyTag(Node topElement, HTMLCollection collection)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (IsEmptyTag(element))
                {
                    collection.Add(element);
                }
                else
                {
                    foreach (var item in topElement.childNodes.item)
                    {
                        getEmptyTag(item, collection);
                    }
                }
            }
        }

        public bool IsEmptyTag(Element el)
        {
            foreach (var item in el.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    return false;
                }
                else if (item.nodeType == enumNodeType.TEXT)
                {
                    var textnode = item as Text;
                    if (textnode != null && !string.IsNullOrWhiteSpace(textnode.data))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (Kooboo.Sites.Service.DomService.IsSelfCloseTag(el.tagName))
            {
                return false;
            }

            if (el.attributes != null && el.attributes.Count > 0)
            {
                return false;
            }

            return true;
        }

        public void CheckDepth(SiteObject siteobject, Kooboo.Dom.Document dom)
        {
            if (siteobject == null || dom == null)
            {
                return;
            }
            string name = Hardcoded.GetValue("Element too deep", session.context);

            HTMLCollection col = new HTMLCollection();
            getTooDepthElements(dom.documentElement, col);

            foreach (var item in col.item)
            {
                string message = GetTagInfo(item);

                message += DiagnosisHelper.DisplayUsedBy(session.context, siteobject);
                session.AddMessage(name, message, MessageType.Warning);
            }
        }

        private string GetTagInfo(Element el)
        {
            List<string> taglist = new List<string>();
            string opentag = Kooboo.Sites.Service.DomService.GetOpenTag(el);

            taglist.Add(opentag);

            var parent = el.parentElement;
            while (parent != null && parent.tagName.ToLower() != "html")
            {
                taglist.Add(parent.tagName);
                parent = parent.parentElement;
            }

            taglist.Reverse();
            string result = string.Join(" > ", taglist.ToArray());
            //this.line　+  " " + el.location + 
            return HttpUtility.HtmlEncode(result);
        }

        private void getTooDepthElements(Node topElement, HTMLCollection collection)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element.depth >= 12)
                {
                    collection.Add(element);
                }
                else
                {
                    foreach (var item in topElement.childNodes.item)
                    {
                        getTooDepthElements(item, collection);
                    }
                }
            }
        }



        public void CheckOutdateTag(SiteObject siteobject, Kooboo.Dom.Document dom)
        {
            if (siteobject == null || dom == null)
            {
                return;
            }
            string name = Hardcoded.GetValue("outdated tag", session.context);
            HTMLCollection col = new HTMLCollection();
            getOutdateTags(dom.body, col);

            foreach (var item in col.item)
            {
                string message = GetTagInfo(item);

                message += DiagnosisHelper.DisplayUsedBy(session.context, siteobject);
                session.AddMessage(name, message, MessageType.Warning);
            }
        }


        public void getOutdateTags(Node topElement, HTMLCollection collection)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element != null)
                {
                    if (element.tagName != null && outdatetags.Contains(element.tagName.ToLower()))
                    {
                        collection.Add(element);
                    }

                    foreach (var item in topElement.childNodes.item)
                    {
                        getOutdateTags(item, collection);
                    }
                }
            }
        }

        public void CheckWrongNestedTag(SiteObject siteobject, Kooboo.Dom.Document dom)
        {
            if (siteobject == null || dom == null)
            {
                return;
            }
            string name = Hardcoded.GetValue("Wrong nested tag", session.context);
            HTMLCollection col = new HTMLCollection();
            getWrongNestedTags(dom.body, col);

            foreach (var item in col.item)
            {
                string message = GetTagInfo(item);

                message += DiagnosisHelper.DisplayUsedBy(session.context, siteobject);
                session.AddMessage(name, message, MessageType.Warning);
            }
        }


        public void getWrongNestedTags(Node topElement, HTMLCollection col)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element != null)
                {
                    bool isInline = inline.Contains(element.tagName.ToLower());

                    if (isInline)
                    {
                        foreach (var item in element.childNodes.item)
                        {
                            getWrongBlockTags(item, col);
                        }
                    }
                    else
                    {
                        foreach (var item in element.childNodes.item)
                        {
                            getWrongNestedTags(item, col);
                        }
                    }


                }
            }
        }

        public void getWrongBlockTags(Node topElement, HTMLCollection col)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element != null)
                {
                    bool isblock = Block.Contains(element.tagName.ToLower());

                    if (isblock)
                    {
                        col.Add(element); 
                    }
                    else
                    {
                        foreach (var item in element.childNodes.item)
                        {
                            getWrongBlockTags(item, col);
                        }
                    } 
                }
            }
        }


        private HashSet<string> _outdatetags;
        private HashSet<string> outdatetags
        {
            get
            {
                if (_outdatetags == null)
                {
                    _outdatetags = new HashSet<string>();
                    string tags = "strike,u,acronym,tt,xmp,font,basefont,applet,bgsound,big,blink,center,dir,marquee,multicol,nextid,nobr,noembed,plaintext,spacer";

                    string[] list = tags.Split(',');
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            _outdatetags.Add(item.Trim());
                        }
                    }
                }
                return _outdatetags;
            }
        }

        private HashSet<string> _inline;
        private HashSet<string> inline
        {
            get
            {
                if (_inline == null)
                {
                    _inline = new HashSet<string>();
                    string tags = "a,abbr,acronym,b,bdo,big,br,cite,code,dfn,em,font,i,img,input,kbd,label,q,s,samp,select,small,span,strike,strong,sub,sup,textarea,tt,u,var";

                    string[] list = tags.Split(',');
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            _inline.Add(item.Trim());
                        }
                    }
                }
                return _inline;
            }
        }

        private HashSet<string> _block;
        private HashSet<string> Block
        {
            get
            {
                if (_block == null)
                {
                    _block = new HashSet<string>();
                    string tags = "address,blockquote,center,dir,div,dl,dt,dd,fieldset,form,h1,h2,h3,h4,h5,h6,hr,isindex,menu,noframes,noscript,ol,p,pre,table,ul";

                    string[] list = tags.Split(',');
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            _block.Add(item.Trim());
                        }
                    }
                }
                return _block;
            }
        }

    }
}
