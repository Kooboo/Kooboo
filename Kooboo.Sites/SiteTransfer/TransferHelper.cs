//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;
using Kooboo.Dom;
using Kooboo.Sites.Repository;
using Kooboo.Sites.SiteTransfer.Download;

namespace Kooboo.Sites.SiteTransfer
{
    public static class TransferHelper
    {
        private static object _object = new object();
        /// <summary>
        /// Get the possible domain part from the relative url. For the second import link, it will be attached with the domain. 
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string GetPossibleHostName(string relativeUrl)
        {
            if (relativeUrl.Length > 5)
            {
                int nextslash = relativeUrl.IndexOf("/", 2);

                string PossibleDomain;

                if (nextslash > 0)
                {
                    if (relativeUrl.StartsWith("/"))
                    {
                        PossibleDomain = relativeUrl.Substring(1, nextslash - 1);
                    }
                    else
                    {
                        PossibleDomain = relativeUrl.Substring(0, nextslash);
                    }
                }
                else
                {
                    if (relativeUrl.StartsWith("/"))
                    {
                        PossibleDomain = relativeUrl.Substring(1);
                    }
                    else
                    {
                        PossibleDomain = relativeUrl;
                    }
                }
                if (PossibleDomain.Contains("."))
                {
                    // and the part after dot must be like domains... 
                    int index = PossibleDomain.LastIndexOf(".");
                    if (index > -1)
                    {
                        var lastpart = PossibleDomain.Substring(index);
                        lastpart = lastpart.Trim();
                        if (lastpart.StartsWith("."))
                        {
                            lastpart = lastpart.Substring(1);
                        }
                        if (lastpart != null && OnlyAscii(lastpart.Trim()))
                        {
                            return PossibleDomain;
                        }
                    }

                }
            }

            return string.Empty;
        }

        private static bool OnlyAscii(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var currentchar = input[i];
                if (!Lib.Helper.CharHelper.IsAscii(currentchar))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// get all page links with http. if Base href is defined in the page, it will be used,
        /// otherwise gthe PageUrl parameter will be used as base url. 
        /// </summary>
        /// <param name="HtmlSource"></param>
        /// <param name="PageUrl"></param>
        /// <returns></returns>
        public static List<string> GetAbsoluteLinks(string HtmlSource, string PageUrl)
        {
            Document doc = Dom.DomParser.CreateDom(HtmlSource);
            string baseurl = doc.baseURI;
            if (string.IsNullOrEmpty(baseurl))
            {
                baseurl = PageUrl;
            }
            return GetAbsoluteLinks(doc, baseurl);
        }

        public static List<string> GetRelativeLinks(string HtmlSource)
        {
            return GetRelativeLinks(DomParser.CreateDom(HtmlSource));
        }

        public static List<Element> GetLinkElements(string htmlSource)
        {
            var doc = DomParser.CreateDom(htmlSource);
            return doc.Links.item;
        }

        public static List<string> GetRelativeLinks(Document doc)
        {
            List<string> RelativeUrlList = new List<string>();
            foreach (var item in doc.Links.item)
            {
                string href = Service.DomUrlService.GetLinkOrSrc(item);

                if (string.IsNullOrEmpty(href))
                {
                    continue;
                }

                href = Kooboo.Lib.Helper.UrlHelper.RemoveLocalLink(UrlHelper.RemoveSessionId(href));

                if (!RelativeUrlList.Contains(href))
                {
                    RelativeUrlList.Add(href);
                }
            }

            return RelativeUrlList;

        }

        /// <summary>
        /// get all page links with http. if Base href is defined in the page, it will be used,
        /// otherwise gthe PageUrl parameter will be used as base url. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="PageUrl"></param>
        /// <returns></returns>
        public static List<string> GetAbsoluteLinks(Document doc, string PageUrl)
        {
            var list = GetRelativeLinks(doc);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = UrlHelper.Combine(PageUrl, list[i]);
            }
            return list;
        }

        public static void AddPageRoute(SiteDb sitedb, Guid PageId, string absoluteUrl, string originalimporturl)
        {
            bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(originalimporturl, absoluteUrl);
            string relativeurl = UrlHelper.RelativePath(absoluteUrl, issamehost);

            AddPageRoute(sitedb, PageId, relativeurl);
        }

        public static void AddPageRoute(SiteDb sitedb, Guid PageId, string relativeurl)
        {
            var oldroute = sitedb.Routes.GetByObjectId(PageId);
            if (oldroute != null)
            {
                if (!Lib.Helper.StringHelper.IsSameValue(oldroute.Name, relativeurl))
                { 
                    Routing.Route newroute = new Routing.Route();
                    newroute.Name = relativeurl;
                    newroute.objectId = oldroute.Id;
                    newroute.DestinationConstType = ConstObjectType.Route;
                    sitedb.Routes.appendRoute(newroute, default(Guid));
                }
            }
            else
            {
                Routing.Route route = new Routing.Route();
                route.Name = relativeurl;
                route.objectId = PageId;
                route.DestinationConstType = ConstObjectType.Page;
                sitedb.Routes.appendRoute(route, default(Guid));
            }
        }

        public static SiteObject AddDownload(DownloadManager manager, DownloadContent download, string absoluteUrl, bool DefaultStartPage, bool AnalyzePage, string originalimporturl = "")
        {
            var sitedb = manager.SiteDb;

            bool issamehost = UrlHelper.isSameHost(originalimporturl, absoluteUrl);

            string relativeurl = UrlHelper.RelativePath(absoluteUrl, issamehost);

            byte consttype = 0;

            var FileType = Kooboo.Lib.Helper.UrlHelper.GetFileType(relativeurl, download.ContentType);

            switch (FileType)
            {
                case UrlHelper.UrlFileType.Image:
                    {
                        consttype = ConstObjectType.Image;
                    }
                    break;
                case UrlHelper.UrlFileType.JavaScript:
                    {
                        consttype = ConstObjectType.Script;
                    }
                    break;
                case UrlHelper.UrlFileType.Style:
                    {
                        consttype = ConstObjectType.Style;
                    }
                    break;
                case UrlHelper.UrlFileType.File:
                    {
                        consttype = ConstObjectType.CmsFile;
                    }
                    break;
                default:
                    {
                        string htmlsource = download.GetString();

                        if (string.IsNullOrEmpty(htmlsource) || Kooboo.HtmlUtility.HasHtmlOrBodyTag(htmlsource))
                        {
                            consttype = ConstObjectType.Page;
                        }
                        else
                        {
                            consttype = ConstObjectType.View;
                        }
                    }
                    break;
            }

            switch (consttype)
            {
                case ConstObjectType.Page:
                    {
                        Page page = new Page();
                        page.IsStatic = true;
                        page.DefaultStart = DefaultStartPage;

                        page.Name = Service.PageService.GetPageNameFromUrl(relativeurl);

                        string htmlsource = UrlHelper.ReplaceMetaCharSet(download.GetString());

                        if (AnalyzePage)
                        {
                            var context = AnalyzerManager.Execute(htmlsource, absoluteUrl, page.Id, page.ConstType, manager, originalimporturl);
                            htmlsource = context.HtmlSource;
                        }
                        page.Body = htmlsource;

                        if (page.Name == "untitled")
                        {
                            var titleel = Service.DomService.GetTitleElement(page.Dom);
                            if (titleel != null)
                            {
                                string title = titleel.InnerHtml;
                                if (!string.IsNullOrEmpty(title))
                                {
                                    title = Lib.Helper.StringHelper.SementicSubString(title, 0, 30);
                                    page.Name = title.Trim();
                                }

                            }
                        }

                        sitedb.Routes.AddOrUpdate(relativeurl, page, manager.UserId);
                        sitedb.Pages.AddOrUpdate(page, manager.UserId);

                        return page;
                    }

                case ConstObjectType.Style:
                    {
                        string csstext = download.GetString();
                        if (AnalyzePage)
                        {
                            CssManager.ProcessResource(ref csstext, absoluteUrl, manager, default(Guid));
                        }
                        var style = new Style();
                        style.Body = csstext;
                        sitedb.Routes.AddOrUpdate(relativeurl, style, manager.UserId);
                        sitedb.Styles.AddOrUpdate(style, manager.UserId);
                        return style;
                    }

                case ConstObjectType.Script:
                    {
                        Script script = new Script { ConstType = ConstObjectType.Script, Body = download.GetString() };

                        sitedb.Routes.AddOrUpdate(relativeurl, script, manager.UserId);
                        sitedb.Scripts.AddOrUpdate(script, manager.UserId);
                        return script;
                    }

                case ConstObjectType.Image:
                    {
                        Image koobooimage = new Image
                        {
                            ContentBytes = download.DataBytes,
                            Extension = UrlHelper.FileExtension(relativeurl),
                            Name = UrlHelper.FileName(relativeurl),
                        };


                        sitedb.Routes.AddOrUpdate(relativeurl, koobooimage, manager.UserId);
                        sitedb.Images.AddOrUpdate(koobooimage, manager.UserId);
                        return koobooimage;
                    }
                case ConstObjectType.View:
                    {
                        var view = new View();

                        string name = System.IO.Path.GetFileNameWithoutExtension(absoluteUrl);
                        if (string.IsNullOrEmpty(name))
                        {
                            name = "untitlted";
                        }
                        view.Name = name;

                        string htmlsource = UrlHelper.ReplaceMetaCharSet(download.GetString());
                        if (AnalyzePage)
                        {
                            var context = AnalyzerManager.Execute(htmlsource, absoluteUrl, view.Id, view.ConstType, manager, originalimporturl);
                            htmlsource = context.HtmlSource;
                        }

                        view.Body = htmlsource;

                        sitedb.Routes.AddOrUpdate(relativeurl, view, manager.UserId);
                        sitedb.Views.AddOrUpdate(view, manager.UserId);
                        return view;
                    }

                default:
                    {
                        /// default treat it as file. 
                        var kooboofile = new CmsFile
                        {
                            ContentType = download.ContentType,
                            ContentBytes = download.DataBytes,
                            ContentString = download.ContentString,
                            Extension = UrlHelper.FileExtension(relativeurl),
                            Name = UrlHelper.FileName(relativeurl)
                        };
                        sitedb.Routes.AddOrUpdate(relativeurl, kooboofile, manager.UserId);
                        sitedb.Files.AddOrUpdate(kooboofile, manager.UserId);
                        return kooboofile;
                    }
            }
        }


        public static bool IsPageUrl(string url)
        {
            var filetype = Lib.Helper.UrlHelper.GetFileType(url);
            if (filetype == UrlHelper.UrlFileType.PageOrView)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsLowerPrioUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            List<string> keywords = new List<string>();
            keywords.Add("login");
            keywords.Add("logon");
            keywords.Add("signin");
            keywords.Add("signup");
            keywords.Add("forgotpassword");
            keywords.Add("forgetpassword");
            keywords.Add("profile");
            keywords.Add("privacy");
            keywords.Add("cookie");

            var lower = url.ToLower();

            var relative = Lib.Helper.UrlHelper.RelativePath(lower);

            if (string.IsNullOrEmpty(relative) || relative.Length < 5)
            {
                return false;
            }

            foreach (var item in keywords)
            {
                if (relative.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static string TrimQuestionMark(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            int index = input.IndexOf("?");

            if (index > -1)
            {
                return input.Substring(0, index);
            }
            return input;
        }

    }
}
