//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kooboo.Dom;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render
{
    public class HeaderRenderTask : IRenderTask
    {
        private string HeaderHtml { get; set; }

        private string HeaderStartTag { get; set; } = "<head>";
        private string HeaderEndTag { get; set; } = "</head>";

        private List<HeaderRenderItem> HeaderItems { get; set; }

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public HeaderRenderTask(Element HeaderElement = null)
        {
            if (HeaderElement != null)
            {
                if (!Service.DomService.IsFakeElement(HeaderElement))
                {
                    this.HeaderHtml = HeaderElement.OuterHtml;
                    this.HeaderItems = this.ProcessItems(HeaderElement);
                }
            }
        }

        public List<HeaderRenderItem> ProcessItems(Element head)
        {
            List<HeaderRenderItem> result = new List<HeaderRenderItem>();

            foreach (var item in head.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var element = item as Element;

                    if (element.tagName.ToLower() == "title")
                    {
                        HeaderRenderItem headerItem = new HeaderRenderItem();
                        headerItem.OriginalHtml = element.OuterHtml;
                        headerItem.isTitle = true;
                        headerItem.Content = element.InnerHtml;
                        foreach (var att in element.attributes)
                        {
                            headerItem.Attributes[att.name] = att.value;
                        }
                        result.Add(headerItem);
                    }

                    else if (element.tagName.ToLower() == "meta")
                    {
                        HeaderRenderItem headerItem = new HeaderRenderItem();
                        headerItem.OriginalHtml = element.OuterHtml;

                        headerItem.MetaName = element.getAttribute("name");
                        headerItem.HttpEquive = element.getAttribute("http-equiv");
                        headerItem.CharSet = element.getAttribute("charset");
                        headerItem.Content = element.getAttribute("content");

                        headerItem.IsMeta = true;

                        foreach (var att in element.attributes)
                        {
                            headerItem.Attributes[att.name] = att.value;
                        }
                        result.Add(headerItem);
                    }
                    else if (element.tagName == "script" || element.tagName == "link")
                    {
                        // get the render task. 
                        var plan = Kooboo.Sites.Render.RenderEvaluator.Evaluate(element.OuterHtml, new EvaluatorOption());
                        if (plan != null)
                        {
                            HeaderRenderItem headeritem = new HeaderRenderItem();
                            headeritem.IsRenderTask = true;
                            headeritem.renderTasks = plan;
                            result.Add(headeritem);
                        }
                        else
                        {
                            HeaderRenderItem headeritem = new HeaderRenderItem();
                            headeritem.OriginalHtml = element.OuterHtml;
                            headeritem.NoRender = true;
                            result.Add(headeritem);
                        }
                    }
                    else
                    {
                        HeaderRenderItem headeritem = new HeaderRenderItem();
                        headeritem.OriginalHtml = element.OuterHtml;
                        headeritem.NoRender = true;
                        result.Add(headeritem);

                    }
                }

                else if (item.nodeType == enumNodeType.TEXT)
                {
                    var textndoe = item as Kooboo.Dom.Text;

                    HeaderRenderItem headeritem = new HeaderRenderItem();
                    headeritem.OriginalHtml = textndoe.data;
                    headeritem.NoRender = true;
                    result.Add(headeritem);
                }
            }
            return result;
        }

        public string Render(RenderContext context)
        {
            var headerbinding = context.HeaderBindings;
            if (headerbinding == null)
            {
                return this.HeaderHtml;
            }

            StringBuilder sb = new StringBuilder();

            if (this.HeaderItems != null)
            {
                foreach (var item in this.HeaderItems)
                {
                    if (item.isTitle)
                    {
                        string newcontent = null;
                        var binding = headerbinding.Find(o => o.IsTitle);
                        if (binding != null)
                        {
                            newcontent = GetBindingContent(binding, context);
                            headerbinding.Remove(binding);
                        }
                        sb.Append(RenderTitle(item, newcontent));
                    }
                    else if (item.NoRender)
                    {
                        sb.Append(item.OriginalHtml);
                    }
                    else if (item.IsMeta)
                    {
                        sb.Append(RenderHeaderMetaItem(item, headerbinding, context));
                    }
                    else if (item.IsRenderTask)
                    {
                        var renderresult = Kooboo.Sites.Render.RenderHelper.Render(item.renderTasks, context);
                        sb.Append(renderresult);
                    }
                }
            }

            foreach (var item in headerbinding)
            {
                sb.Append(RenderBindingItem(item, context));
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, this.HeaderStartTag);
                sb.Append("</head>");
            }

            return sb.ToString();
        }

        private string RenderHeaderMetaItem(HeaderRenderItem item, List<HeaderBindings> bindings, RenderContext context)
        {
            if (item.IsRenderTask)
            {
                return item.renderTasks.Render(context);
            }

            if (bindings == null || bindings.Count() == 0)
            {
                return item.OriginalHtml;
            }

            HeaderBindings finditem = null;
            if (!string.IsNullOrEmpty(item.MetaName))
            {
                finditem = bindings.Find(o => Lib.Helper.StringHelper.IsSameValue(item.MetaName, o.MetaName));
            }
            else if (!string.IsNullOrEmpty(item.HttpEquive))
            {
                finditem = bindings.Find(o => Lib.Helper.StringHelper.IsSameValue(item.HttpEquive, o.HttpEquiv));
            }
            else if (!string.IsNullOrEmpty(item.CharSet))
            {
                finditem = bindings.Find(o => !string.IsNullOrEmpty(o.CharSet));
            }

            if (finditem != null)
            {
                bindings.Remove(finditem);
                return RenderBindingItem(finditem, context);
            }
            else
            {
                return item.OriginalHtml;
            }
        }

        private string RenderBindingItem(HeaderBindings binding, RenderContext context)
        {
            if (binding == null)
            {
                return null;
            }

            string newcontent = GetBindingContent(binding, context);

            if (newcontent == null)
            {
                return null;
            }

            if (binding.IsTitle)
            {
                return "<Title>" + newcontent + "</Title>";
            }
            else if (binding.IsCustomHeader)
            {
                return newcontent;
            }
            else
            {
                string result = "<Meta";

                if (!string.IsNullOrEmpty(binding.MetaName))
                {
                    result += " name=\"" + binding.MetaName + "\"";
                }

                if (!string.IsNullOrEmpty(binding.HttpEquiv))
                {
                    result += " http-equiv=\"" + binding.HttpEquiv + "\"";
                }

                if (!string.IsNullOrEmpty(binding.CharSet))
                {
                    result += " charset=\"" + binding.CharSet + "\"";
                }

                if (!string.IsNullOrEmpty(newcontent))
                {
                    result += " content=\"" + newcontent + "\"";
                }

                result = result + " />";

                return result;
            }

        }

        private string RenderTitle(HeaderRenderItem item, string newContent = null)
        {
            if (item == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(newContent))
            {
                return item.OriginalHtml;
            }

            string att = string.Empty;

            foreach (var oneatt in item.Attributes)
            {
                att += " " + oneatt.Key + "=\"" + oneatt.Value + "\"";
            }

            return "<Title" + att + ">" + newContent + "</Title>";

        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            RenderResult headerresult = new RenderResult();
            HeaderLazyRender lazy = new HeaderLazyRender(context, this);
            headerresult.RenderString = lazy.Render;
            result.Add(headerresult);
        }

        public string GetBindingContent(HeaderBindings binding, RenderContext context)
        {
            return binding.GetContent(context);
        }

        public bool IsRenderTask(Element el)
        {
            // only process style and script now. 
            if (el.tagName == "script" || el.tagName == "link")
            {
                if (Kooboo.Sites.Render.Components.Manager.IsComponent(el))
                {
                    return true;
                }

                if (el.hasAttribute("k-version"))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class HeaderLazyRender
    {
        private RenderContext _context;
        private HeaderRenderTask _headertask;
        public HeaderLazyRender(RenderContext context, HeaderRenderTask headerRender)
        {
            this._headertask = headerRender;
            this._context = context;
        }

        public string Render()
        {
            return this._headertask.Render(this._context);
        }

    }

    public class HeaderRenderItem
    {
        public bool isTitle { get; set; }

        public bool IsMeta { get; set; }

        public bool IsRenderTask { get; set; }

        public List<IRenderTask> renderTasks { get; set; }

        /// <summary>
        ///  Node that does not require render... 
        /// </summary>
        public bool NoRender { get; set; }

        public string MetaName { get; set; }

        public string HttpEquive { get; set; }

        public string CharSet { get; set; }

        public string Content { get; set; }

        public string OriginalHtml { get; set; }

        private Dictionary<string, string> _attributes;
        public Dictionary<string, string> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }
    }

}
