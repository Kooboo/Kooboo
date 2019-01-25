//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Sites.Render.Components;
using Kooboo.Sites.Render; 

namespace Kooboo.Sites.Service
{
    public static class PageService
    {

        /// <summary>
        /// The all the absolute url link with the page source. 
        /// </summary>
        /// <param name="pageHtmlSource"></param>
        /// <param name="pageOrBaseUrl"></param>
        /// <returns></returns>
        public static List<string> GetAllLinks(string pageHtmlSource, string pageOrBaseUrl)
        {
            Document doc = Dom.DomParser.CreateDom(pageHtmlSource);

            return GetAllLinks(doc, pageOrBaseUrl);
        }

        /// <summary>
        /// The all the absolute url link with the page source. 
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="pageOrBaseUrl"></param>
        /// <returns></returns>
        public static List<string> GetAllLinks(Document dom, string pageOrBaseUrl)
        {
            List<string> urlList = new List<string>();
            foreach (var link in dom.Links.item)
            {
                string href = link.getAttribute("href");

                if (string.IsNullOrEmpty(href) || href == "#")
                {
                    continue;
                }
                string url = UrlHelper.Combine(pageOrBaseUrl, href);

                urlList.Add(url);
            }
            return urlList;
        }

        /// <summary>
        /// Get all external style urls
        /// </summary>
        /// <param name="pageHtmlSource"></param>
        /// <param name="pageOrBaseUrl"></param>
        /// <returns></returns>
        public static List<string> GetAllExternalStyleUrls(string pageHtmlSource, string pageOrBaseUrl)
        {
            Document doc = Dom.DomParser.CreateDom(pageHtmlSource);
            return GetAllExternalStyleUrls(doc, pageOrBaseUrl);
        }

        /// <summary>
        /// Get all external style urls
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="pageOrBaseUrl"></param>
        /// <returns></returns>
        public static List<string> GetAllExternalStyleUrls(Document dom, string pageOrBaseUrl)
        {
            List<string> urlList = new List<string>();

            HTMLCollection styletags = dom.getElementsByTagName("link");

            foreach (var item in styletags.item)
            {
                if (item.hasAttribute("rel") && item.getAttribute("rel").ToLower().Contains("stylesheet"))
                {
                    string itemurl = item.getAttribute("href");

                    if (!string.IsNullOrEmpty(item.getAttribute("href")))
                    {
                        string url = UrlHelper.Combine(pageOrBaseUrl, itemurl);
                        urlList.Add(url);
                    }
                }
            }

            return urlList;

        }

        /// <summary>
        /// get all image links, that already combined with the pagebaseurl. 
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="pageOrBaseUrl"></param>
        /// <returns></returns>
        public static List<string> GetAllImageUrl(Document dom, string pageOrBaseUrl)
        {
            List<string> imgurls = new List<string>();

            foreach (var item in dom.images.item)
            {
                string itemsrc = item.getAttribute("src");

                if (!Kooboo.Lib.Utilities.DataUriService.isDataUri(itemsrc))
                {
                    string url = UrlHelper.Combine(pageOrBaseUrl, itemsrc);
                    imgurls.Add(url);
                }
            }
            return imgurls;
        }

        /// <summary>
        /// apply the style sheet to all Dom Elements. 
        /// </summary>
        /// <param name="page"></param>
        public static void ApplySiteStyle(Page page, Repository.SiteDb SiteDb)
        {
            var dom = page.Dom;

            if (!dom.hasParseCSS && dom.StyleSheets.item.Count == 0)
            {
                ParseSiteStyleSheet(page, SiteDb);
            }

            foreach (var item in dom.StyleSheets.item)
            {
                CSSStyleSheet stylesheet = item as CSSStyleSheet;
                if (stylesheet != null)
                {
                    dom.ApplyCssRules(stylesheet.cssRules, "");
                }
            }
        }

        public static void ParseSiteStyleSheet(Page page, Repository.SiteDb SiteDb)
        {
            var dom = page.Dom;

            string pageRelativeUrl = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(SiteDb, page);

            string pageAbsoluteUrl = UrlHelper.Combine(SiteDb.WebSite.BaseUrl(), pageRelativeUrl);


            HTMLCollection styletags = dom.getElementsByTagName("link, style");

            HTMLCollection availablesheets = new HTMLCollection();

            foreach (var item in styletags.item)
            {
                if (item.tagName == "style")
                {
                    availablesheets.Add(item);
                }

                else if (item.hasAttribute("type"))
                {
                    if (item.getAttribute("type").ToLower().Contains("css"))
                    {
                        availablesheets.Add(item);
                    }
                }
                else if (item.hasAttribute("rel"))
                {
                    if (item.getAttribute("rel").ToLower().Contains("stylesheet"))
                    {
                        availablesheets.Add(item);
                    }
                }
            }

            foreach (var item in availablesheets.item)
            {
                if (item.tagName == "link")
                {
                    string href = item.getAttribute("href");

                    if (string.IsNullOrEmpty(href))
                    {
                        continue;
                    }

                    string cssrelativeUrl = UrlHelper.Combine(pageRelativeUrl, href);
                    cssrelativeUrl = UrlHelper.RelativePath(cssrelativeUrl);

                    var route = Sites.Routing.ObjectRoute.GetRoute(SiteDb, cssrelativeUrl);
                    if (route == null || route.DestinationConstType != ConstObjectType.Style)
                    {
                        continue;
                    }

                    string cssText = SiteDb.Styles.Get(route.objectId).Body;

                    string cssabsoluteUrl = UrlHelper.Combine(pageAbsoluteUrl, cssrelativeUrl);

                    if (string.IsNullOrEmpty(cssText))
                    {
                        continue;
                    }

                    CSSStyleSheet newStyleSheet = CSSParser.ParseCSSStyleSheet(cssText, cssabsoluteUrl, true);
                    newStyleSheet.ownerNode = item;

                    if (newStyleSheet != null)
                    {
                        newStyleSheet.ownerNode = item;

                        string media = item.getAttribute("media");
                        if (!string.IsNullOrEmpty(media))
                        {
                            string[] medialist = media.Split(',');
                            foreach (var mediaitem in medialist)
                            {
                                newStyleSheet.Medialist.appendMedium(mediaitem);
                            }
                        }
                        dom.StyleSheets.appendStyleSheet(newStyleSheet);
                    }

                }

                else if (item.tagName == "style")
                {
                    string cssText = item.InnerHtml;

                    CSSStyleSheet newStyleSheet = CSSParser.ParseCSSStyleSheet(cssText, pageAbsoluteUrl, true);

                    newStyleSheet.ownerNode = item;

                    string media = item.getAttribute("media");
                    if (!string.IsNullOrEmpty(media))
                    {
                        string[] medialist = media.Split(',');
                        foreach (var mediaitem in medialist)
                        {
                            newStyleSheet.Medialist.appendMedium(mediaitem);
                        }
                    }
                    dom.StyleSheets.appendStyleSheet(newStyleSheet);
                }

            }

            dom.hasParseCSS = true;
        }

        internal static string SetPreviewPara(Page Page, string Url)
        {
            if (string.IsNullOrEmpty(Url))
            {
                return Url;
            }

            int start = Url.IndexOf("{");

            if (start == -1)
            {
                return Url;
            }

            int end = Url.IndexOf("}", start);
            if (end == -1)
            {
                return Url;
            }

            string para = Url.Substring(start + 1, end - start - 1);
            if (string.IsNullOrWhiteSpace(para))
            {
                return Url;
            }

            string value = null;

            if (Page.Parameters.ContainsKey(para))
            {
                var paravalue = Page.Parameters[para];
                if (!Service.BindingService.IsBinding(paravalue))
                {
                    value = paravalue;
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                string newurl = Url.Substring(0, start) + value + Url.Substring(end + 1);
                return SetPreviewPara(Page, newurl);
            }
            else
            {
                return Url;
            }
        }

        public static string GetPreviewUrl(SiteDb SiteDb, Page page)
        {
            var siteId = SiteDb.WebSite.Id;

            var bindings = Data.GlobalDb.Bindings.GetByWebSite(siteId);
            var route = SiteDb.Routes.GetByObjectId(page.Id);
            if (bindings == null || bindings.Count < 1 || route == null)
            { 
                return "#";
            }
            var binding = bindings.FirstOrDefault();
            string PageRelativeUrl = SetPreviewPara(page, route.Name);

            string baseurl = SiteDb.WebSite.BaseUrl(); 
            if (string.IsNullOrEmpty(baseurl))
            {
                return null; 
            } 
            return Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, PageRelativeUrl); 
        }

        public static bool RequireRenderLink(Dom.Element hrefElement, Repository.SiteDb SiteDb = null)
        {
            if (hrefElement.hasAttribute(Kooboo.Sites.ConstTALAttributes.href))
            {
                return true;
            }
            var href = Service.DomUrlService.GetLinkOrSrc(hrefElement);
            if (string.IsNullOrEmpty(href))
            {
                return false;
            }
            if (href.Contains("{") && href.Contains("}"))
            {
                return true;
            }

            if (href.StartsWith("/__kb"))
            {
                return true;
            }

            if (SiteDb == null)
            {
                return false;
            }

            if (SiteDb.WebSite != null && SiteDb.WebSite.EnableSitePath && SiteDb.WebSite.SitePath != null && SiteDb.WebSite.HasSitePath())
            {
                return true;
            }

            if (Service.DomUrlService.IsExternalLink(href))
            {
                return false;
            }

            var route = Routing.ObjectRoute.GetRoute(SiteDb, href);

            if (route == null)
            {
                return false;
            }

            if (route.Parameters.Count > 0)
            {
                return true;
            }

            if (route.DestinationConstType == ConstObjectType.Page)
            {
                var page = SiteDb.Pages.Get(route.objectId);
                if (page != null && page.Parameters.Count > 0)
                { return true; }
            }

            return false;
        }

        public static string GetPageNameFromUrl(string relativeurl)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(relativeurl);
            if (string.IsNullOrEmpty(name))
            {
                relativeurl = relativeurl.Replace("\\", "/");
                string[] segments = relativeurl.Split('/');
                foreach (var item in segments.Reverse())
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        name = item;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(name))
            {
                return "untitled";
            }
            else
            {
                name = name.Replace("_", " ");
                name = name.Replace("-", " ");
            }

            return name;
        }


        /// <summary>
        /// Get available paras to be used on Url. 
        /// </summary>
        /// <param name="sitedb"></param>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public static List<string> GetUrlParas(SiteDb sitedb, Guid PageId)
        {
            var viewrelations = sitedb.Relations.GetRelations(PageId, ConstObjectType.View);
            List<Guid> ViewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                ViewIds.Add(item.objectYId);
            }

            return GetUrlParas(sitedb, ViewIds);
        }

        public static List<string> GetUrlParas(SiteDb sitedb, List<Guid> ViewIds)
        {
            List<string> allparameters = new List<string>();

            foreach (var viewid in ViewIds)
            {
                var viewparas = Kooboo.Sites.Routing.PageRoute.GetViewParameters(sitedb, viewid);
                foreach (var para in viewparas)
                {
                    if (!allparameters.Contains(para) && !SpecialParaNames(para))
                    {
                        allparameters.Add(para);
                    }
                }
            }
            return allparameters;
        }

        private static bool SpecialParaNames(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            string lower = input.ToLower();
            lower = lower.Replace("{", "");
            lower = lower.Replace("}", ""); 
            if (lower.Contains("."))
            {
                int index = lower.LastIndexOf(".");
                lower = lower.Substring(index + 1); 
            }
            
            if (lower == "folderid" || lower == "sortfield" || lower == "limit" || lower == "isascending" || lower == "enablepaging" || lower == "pagesize" || lower == "pagenumber" || lower == "contentlistparams")
            {
                return true; 
            }
            return false; 
        }
         

        /// <summary>
        /// {TextContent.Title}, {TextContent.Summary} 
        /// </summary>
        /// <param name="sitedb"></param>
        /// <param name="PageId"></param>
        /// <returns></returns>
        public static List<string> GetMetaBindings(SiteDb sitedb, Guid PageId)
        {
            var viewrelations = sitedb.Relations.GetRelations(PageId, ConstObjectType.View);

            List<Guid> ViewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                ViewIds.Add(item.objectYId);
            }

            return GetMetaBindings(sitedb, ViewIds); 
        }

        /// <summary>
        /// {GetById.Title}, {GetById.Summary}.... .. 
        /// </summary>
        /// <param name="sitedb"></param>
        /// <param name="ViewIds"></param>
        /// <returns></returns>
        public static List<string> GetMetaBindings(SiteDb sitedb, List<Guid> ViewIds)
        {
            List<string> AvailableBindings = new List<string>();

            foreach (var ViewId in ViewIds)
            {
                var viewmethods = sitedb.ViewDataMethods.Query.Where(o => o.ViewId == ViewId).SelectAll();

                foreach (var viewmethod in viewmethods)
                {
                    var fields = DataSources.DataSourceHelper.GetFields(sitedb, viewmethod.MethodId);

                    List<string> bindings = new List<string>();

                    if (!fields.Enumerable)
                    {
                        analyzeBinding(fields.ItemFields, viewmethod.AliasName, ref bindings);
                    }

                    foreach (var bind in bindings)
                    {
                        if (!AvailableBindings.Contains(bind))
                        {
                            AvailableBindings.Add(bind);
                        }
                    }
                }

            }

            return AvailableBindings;

        }

        private static void analyzeBinding(List<TypeFieldModel> Fields, string ParentTypeName, ref List<string> bindings)
        {

            foreach (var field in Fields)
            {
                if (!field.IsComplexType)
                {
                    string bindingfield = ParentTypeName + "." + field.Name;

                    if (!bindings.Contains(bindingfield))
                    {
                        bindings.Add(bindingfield);
                    }
                }

                else
                {
                    string parent = ParentTypeName + "." + field.Name;
                    analyzeBinding(field.Fields, parent, ref bindings);
                }

            }
        }

        public static string GetLayoutName(Page page)
        {
          if (page == null)
            {
                return null; 
            }
          if (!string.IsNullOrEmpty(page.LayoutName))
            {
                return page.LayoutName; 
            }

            var dom =  DomParser.CreateDom(page.Body);
            var layoutTags = dom.getElementsByTagName("layout");
             
            if (layoutTags != null && layoutTags.item.Count()>0)
            {
                var tag = layoutTags.item[0];
                return tag.id; 
            }
            return null; 
        }

        public static Dictionary<string, List<ComponentSetting>> GetComponentSettings(string HtmlSource, string LayoutNameOrId = null)
        { 
            if (string.IsNullOrWhiteSpace(HtmlSource))
            {
                return new Dictionary<string, List<ComponentSetting>>(); 
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(HtmlSource); 
            
            if (!string.IsNullOrEmpty(LayoutNameOrId))
            {
                var layoutelement = dom.getElementById(LayoutNameOrId);
                return GetComponentSettingsFromElement(layoutelement); 
            }
            else
            {
                var elements = dom.getElementsByTagName("layout"); 
                if (elements!= null && elements.length >0)
                {
                    var layoutelement = elements.item[0];
                    return GetComponentSettingsFromElement(layoutelement); 
                }
            }
           
            return new Dictionary<string, List<ComponentSetting>>();
        }

        private static Dictionary<string, List<ComponentSetting>> GetComponentSettingsFromElement(Element LayoutElement)
        {
            Dictionary<string, List<ComponentSetting>> result = new Dictionary<string, List<ComponentSetting>>(); 
            if (LayoutElement == null)
            {
                return result; 
            }
             
            foreach (var item in LayoutElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    Element child = item as Element;

                    if (child.tagName == "position" || child.tagName == "placeholder")
                    {
                        string positionname = child.id;
                        if (string.IsNullOrEmpty(positionname))
                        {
                            positionname = child.getAttribute("name");
                        }
                        if (string.IsNullOrEmpty(positionname))
                        {
                            continue;
                        }

                        List<ComponentSetting> settingList;
                        if (result.ContainsKey(positionname))
                        {  settingList = result[positionname];   }  
                        else
                        { settingList = new List<ComponentSetting>();
                            result[positionname] = settingList; 
                        }
                         
                         
                        foreach (var comNodeItem in child.childNodes.item)
                        {
                            if (comNodeItem.nodeType == enumNodeType.ELEMENT)
                            {
                                Element comElement = comNodeItem as Element;
                                if (Manager.IsComponent(comElement))
                                {
                                    var setting = ComponentSetting.LoadFromElement(comElement);
                                    settingList.Add(setting); 
                                }
                            }
                        } 
                         
                    }

                }
            }

            return result;
        }

        public static string ToLayoutPageSource(string LayoutName, Dictionary<string, List<ComponentSetting>> components)
        {
            string result = "<layout id='" + LayoutName + "'>\r\n";

            foreach (var item in components)
            {
                result += "<placeholder id='" + item.Key + "'>\r\n";

                foreach (var onecomponent in item.Value)
                {
                    result += "<" + onecomponent.TagName + " id='" + onecomponent.NameOrId + "'>\r\n";

                    foreach (var setting in onecomponent.Settings)
                    {
                        result += "<" + setting.Key + ">" + setting.Value + "></" + setting.Key + ">"; 
                    }

                    result += "</" + onecomponent.TagName + ">\r\n"; 
                }

                 result += "</placeholder>\r\n"; 
            }
            result += "\r\n</layout>";

            return result; 
        }
    }
}
