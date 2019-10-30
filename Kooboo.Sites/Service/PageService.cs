//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Lib.Helper;
using Kooboo.Sites.DataSources.New.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Render.Components;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="siteDb"></param>
        public static void ApplySiteStyle(Page page, Repository.SiteDb siteDb)
        {
            var dom = page.Dom;

            if (!dom.hasParseCSS && dom.StyleSheets.item.Count == 0)
            {
                ParseSiteStyleSheet(page, siteDb);
            }

            foreach (var item in dom.StyleSheets.item)
            {
                if (item is CSSStyleSheet stylesheet)
                {
                    dom.ApplyCssRules(stylesheet.cssRules, "");
                }
            }
        }

        public static void ParseSiteStyleSheet(Page page, Repository.SiteDb siteDb)
        {
            var dom = page.Dom;

            string pageRelativeUrl = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(siteDb, page);

            string pageAbsoluteUrl = UrlHelper.Combine(siteDb.WebSite.BaseUrl(), pageRelativeUrl);

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

                    var route = Sites.Routing.ObjectRoute.GetRoute(siteDb, cssrelativeUrl);
                    if (route == null || route.DestinationConstType != ConstObjectType.Style)
                    {
                        continue;
                    }

                    string cssText = siteDb.Styles.Get(route.objectId).Body;

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

        internal static string SetPreviewPara(Page page, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            int start = url.IndexOf("{");

            if (start == -1)
            {
                return url;
            }

            int end = url.IndexOf("}", start);
            if (end == -1)
            {
                return url;
            }

            string para = url.Substring(start + 1, end - start - 1);
            if (string.IsNullOrWhiteSpace(para))
            {
                return url;
            }

            string value = null;

            if (page.Parameters.ContainsKey(para))
            {
                var paravalue = page.Parameters[para];
                if (!Service.BindingService.IsBinding(paravalue))
                {
                    value = paravalue;
                }
            }

            if (!string.IsNullOrEmpty(value))
            {
                string newurl = url.Substring(0, start) + value + url.Substring(end + 1);
                return SetPreviewPara(page, newurl);
            }
            else
            {
                return url;
            }
        }

        public static string GetPreviewUrl(SiteDb siteDb, Page page)
        {
            var siteId = siteDb.WebSite.Id;

            var bindings = Data.GlobalDb.Bindings.GetByWebSite(siteId);
            var route = siteDb.Routes.GetByObjectId(page.Id);
            if (bindings == null || bindings.Count < 1 || route == null)
            {
                return "#";
            }
            var binding = bindings.FirstOrDefault();
            string pageRelativeUrl = SetPreviewPara(page, route.Name);

            string baseurl = siteDb.WebSite.BaseUrl();
            if (string.IsNullOrEmpty(baseurl))
            {
                return null;
            }
            return Kooboo.Lib.Helper.UrlHelper.Combine(baseurl, pageRelativeUrl);
        }

        public static bool RequireRenderLink(Dom.Element hrefElement, Repository.SiteDb siteDb = null)
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

            if (siteDb == null)
            {
                return false;
            }

            if (siteDb.WebSite != null && siteDb.WebSite.EnableSitePath && siteDb.WebSite.SitePath != null && siteDb.WebSite.HasSitePath())
            {
                return true;
            }

            if (Service.DomUrlService.IsExternalLink(href))
            {
                return false;
            }

            var route = Routing.ObjectRoute.GetRoute(siteDb, href);

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
                var page = siteDb.Pages.Get(route.objectId);
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
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static List<string> GetUrlParas(SiteDb sitedb, Guid pageId)
        {
            var viewrelations = sitedb.Relations.GetRelations(pageId, ConstObjectType.View);
            List<Guid> viewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                viewIds.Add(item.objectYId);
            }

            return GetUrlParas(sitedb, viewIds);
        }

        public static List<string> GetUrlParas(SiteDb sitedb, List<Guid> viewIds)
        {
            List<string> allparameters = new List<string>();

            foreach (var viewid in viewIds)
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

            List<Guid> viewIds = new List<Guid>();

            foreach (var item in viewrelations)
            {
                viewIds.Add(item.objectYId);
            }

            return GetMetaBindings(sitedb, viewIds);
        }

        /// <summary>
        /// {GetById.Title}, {GetById.Summary}.... ..
        /// </summary>
        /// <param name="sitedb"></param>
        /// <param name="viewIds"></param>
        /// <returns></returns>
        public static List<string> GetMetaBindings(SiteDb sitedb, List<Guid> viewIds)
        {
            List<string> availableBindings = new List<string>();

            foreach (var viewId in viewIds)
            {
                var viewmethods = sitedb.ViewDataMethods.Query.Where(o => o.ViewId == viewId).SelectAll();

                foreach (var viewmethod in viewmethods)
                {
                    var fields = DataSources.DataSourceHelper.GetFields(sitedb, viewmethod.MethodId);

                    List<string> bindings = new List<string>();

                    if (!fields.Enumerable)
                    {
                        AnalyzeBinding(fields.ItemFields, viewmethod.AliasName, ref bindings);
                    }

                    foreach (var bind in bindings)
                    {
                        if (!availableBindings.Contains(bind))
                        {
                            availableBindings.Add(bind);
                        }
                    }
                }
            }

            return availableBindings;
        }

        private static void AnalyzeBinding(List<TypeFieldModel> fields, string parentTypeName, ref List<string> bindings)
        {
            foreach (var field in fields)
            {
                if (!field.IsComplexType)
                {
                    string bindingfield = parentTypeName + "." + field.Name;

                    if (!bindings.Contains(bindingfield))
                    {
                        bindings.Add(bindingfield);
                    }
                }
                else
                {
                    string parent = parentTypeName + "." + field.Name;
                    AnalyzeBinding(field.Fields, parent, ref bindings);
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

            var dom = DomParser.CreateDom(page.Body);
            var layoutTags = dom.getElementsByTagName("layout");

            if (layoutTags != null && layoutTags.item.Any())
            {
                var tag = layoutTags.item[0];
                return tag.id;
            }
            return null;
        }

        public static Dictionary<string, List<ComponentSetting>> GetComponentSettings(string htmlSource, string layoutNameOrId = null)
        {
            if (string.IsNullOrWhiteSpace(htmlSource))
            {
                return new Dictionary<string, List<ComponentSetting>>();
            }

            var dom = Kooboo.Dom.DomParser.CreateDom(htmlSource);

            if (!string.IsNullOrEmpty(layoutNameOrId))
            {
                var layoutelement = dom.getElementById(layoutNameOrId);
                return GetComponentSettingsFromElement(layoutelement);
            }
            else
            {
                var elements = dom.getElementsByTagName("layout");
                if (elements != null && elements.length > 0)
                {
                    var layoutelement = elements.item[0];
                    return GetComponentSettingsFromElement(layoutelement);
                }
            }

            return new Dictionary<string, List<ComponentSetting>>();
        }

        private static Dictionary<string, List<ComponentSetting>> GetComponentSettingsFromElement(Element layoutElement)
        {
            Dictionary<string, List<ComponentSetting>> result = new Dictionary<string, List<ComponentSetting>>();
            if (layoutElement == null)
            {
                return result;
            }

            foreach (var item in layoutElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    if (item is Element child && (child.tagName == "position" || child.tagName == "placeholder"))
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
                        { settingList = result[positionname]; }
                        else
                        {
                            settingList = new List<ComponentSetting>();
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

        public static string ToLayoutPageSource(string layoutName, Dictionary<string, List<ComponentSetting>> components)
        {
            string result = "<layout id='" + layoutName + "'>\r\n";

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