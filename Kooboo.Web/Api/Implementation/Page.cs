//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Kooboo.Web.Api.Implementation
{
    public class PageApi : SiteObjectApi<Page>
    {
        public PageListViewModel All(ApiCall apiCall)
        {          
            var sitedb = apiCall.WebSite.SiteDb();       
       
            PageListViewModel model = new PageListViewModel();
            model.Layouts = sitedb.Layouts.All();
            model.BaseUrl = sitedb.WebSite.BaseUrl();

            foreach (var item in sitedb.Pages.All().OrderBy(o => o.Name))
            {
                PageViewModel pagemodel = ToPageModel(sitedb, item);

                model.Pages.Add(pagemodel);
            }
            return model;
        }

        private PageViewModel ToPageModel(SiteDb sitedb, Page item)
        {
            PageViewModel pagemodel = new PageViewModel()
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Online = item.Online,
                Path = Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, item),
                PreviewUrl = PageService.GetPreviewUrl(sitedb, item),
                Linked = sitedb.Relations.GetReferredBy(item).Count(),
                LayoutId = GetLayoutId(item),
                PageView = sitedb.VisitorLog.QueryDescending(o => o.ObjectId == item.Id).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-24)).Take(999999).Count(),
                LastModified = item.LastModified,
                StartPage = item.DefaultStart,
                InlineUrl = "/_api/redirect/inline?siteid=" + sitedb.WebSite.Id + "&pageid=" + item.Id
            };

            var relations = sitedb.Relations.GetRelations(item.Id);

            if (relations != null && relations.Count() > 0)
            {
                var relationresult = pagemodel.Relations;
                foreach (var onerelation in relations.Where(o => o.ConstTypeY == ConstObjectType.Layout || o.ConstTypeY == ConstObjectType.View || o.ConstTypeY == ConstObjectType.Form || o.ConstTypeY == ConstObjectType.HtmlBlock || o.ConstTypeY == ConstObjectType.Menu))
                {
                    var objecttypename = ConstTypeService.GetModelType(onerelation.ConstTypeY).Name;
                    if (relationresult.ContainsKey(objecttypename))
                    {
                        var value = relationresult[objecttypename];
                        value = value + 1;
                        relationresult[objecttypename] = value;
                    }
                    else
                    { relationresult.Add(objecttypename, 1); }
                }
            }

            return pagemodel;
        }

        public Guid GetLayoutId(Page page)
        {
            string layoutname = Kooboo.Sites.Service.PageService.GetLayoutName(page);
            if (string.IsNullOrEmpty(layoutname))
            {
                return default(Guid);
            }
            else
            {
                return Data.IDGenerator.Generate(layoutname, ConstObjectType.Layout);
            }
        }

        internal static PageViewModel ToPageViewModel(SiteDb SiteDb, Page page)
        {
            return new PageViewModel()
            {
                Id = page.Id,
                Name = page.Name,
                Online = page.Online,
                Path = ObjectService.GetObjectRelativeUrl(SiteDb, page),
                PreviewUrl = PageService.GetPreviewUrl(SiteDb, page),
                Linked = SiteDb.Relations.GetReferredBy(page).Count(),
                PageView = SiteDb.VisitorLog.QueryDescending(o => o.ObjectId == page.Id).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-24)).Take(999999).Count(),
                LastModified = page.LastModified,
                InlineUrl = "/_api/redirect/inline?siteid=" + SiteDb.WebSite.Id + "&pageid=" + page.Id
            };
        }

        public PageEditViewModel GetEdit(ApiCall call)
        {
            // optional type.  
            string baseurl = call.Context.WebSite.BaseUrl();
            baseurl = Kooboo.Data.Service.WebSiteService.EnsureHttpsBaseUrlOnServer(baseurl, call.Context.WebSite);  
            Guid PageId = call.ObjectId;
            var sitedb = call.WebSite.SiteDb();

            if (PageId == default(Guid))
            {
                string layoutid = call.GetValue("layoutid");
                string type = call.GetValue("type");

                var result = new PageEditViewModel();

                if (!string.IsNullOrEmpty(layoutid))
                {
                    var Layout = sitedb.Layouts.GetByNameOrId(layoutid);
                    if (Layout != null)
                    {
                        result.Body = InitPageLayoutSource(Layout);
                    }
                }
                else if (!string.IsNullOrEmpty(type) && (type.ToLower() == "richtext"))
                {
                    result.Body = "";
                }
                else
                {
                    result.Body = "<html><head></head><body><div></div></body></html>";
                    result.Body = HtmlHeadService.SetBaseHref(result.Body, baseurl);
                }

                return result;
            }

            var page = sitedb.Pages.Get(PageId);
            var route = sitedb.Routes.GetByObjectId(page.Id);

            var model = new PageEditViewModel();
            model.Id = page.Id;
            model.Name = page.Name;
            model.Published = page.Online;
            model.UrlPath = route == null ? null : route.Name; 
            model.Type = page.Type; 

            if (page.Type == PageType.RichText)
            {
                string body = page.Body;
                var doc = Kooboo.Dom.DomParser.CreateDom(body);
                var titletag = doc.head.getElementsByTagName("title");
                if (titletag != null && titletag.length > 0)
                {
                    model.Title = titletag.item[0].InnerHtml;
                }
                model.Body = doc.body.InnerHtml; 
            }

            else
            {
                if (!page.HasLayout)
                {
                    // HtmlHeadService.AppendHeader(page.Body, page.Headers);
                }

                string body = string.IsNullOrEmpty(page.Body) ? string.Empty : HtmlHeadService.SetBaseHref(page.Body, baseurl);

                model.LayoutName = page.LayoutName;
                model.LayoutId = Kooboo.Data.IDGenerator.GetOrGenerate(page.LayoutName, ConstObjectType.Layout); 
                model.Metas = page.Headers.Metas;
                model.CustomHeader = page.Headers.CustomHeader;
                model.MetaBindings = PageService.GetMetaBindings(sitedb, PageId);
                model.UrlParamsBindings = PageService.GetUrlParas(sitedb, PageId);
                model.ContentTitle = page.Headers.Titles;
                model.Parameters = page.Parameters;
                model.Body = body;
                model.Scripts = page.Headers.Scripts;
                model.Styles = page.Headers.Styles;
            }

            return model;
        }

        private string InitPageLayoutSource(Layout layout)
        {
            var dom = layout.Dom;
            HashSet<string> placeholderNames = new HashSet<string>();
            List<string> attributenames = new List<string>();
            attributenames.Add("tal-placeholder");
            attributenames.Add("position");
            attributenames.Add("placeholder");
            attributenames.Add("k-placeholder");
            foreach (var item in attributenames)
            {
                var elements = dom.getElementByAttribute(item);
                if (elements != null && elements.length > 0)
                {
                    foreach (var elementItem in elements.item)
                    {
                        string PositionValue = elementItem.getAttribute(item);
                        if (!string.IsNullOrEmpty(PositionValue))
                        {
                            placeholderNames.Add(PositionValue);
                        }
                    }
                }
            }

            string result = "<layout id=\"" + layout.Name + "\">\r\n";
            foreach (var item in placeholderNames)
            {
                result += "<placeholder id=\"" + item + "\"></placeholder>\r\n";
            }
            result += "</layout>";

            return result;
        }

        [Kooboo.Attributes.RequireModel(typeof(PageUpdateViewModel))]
        public override Guid Post(ApiCall call)
        {
            var model = call.Context.Request.Model as PageUpdateViewModel;

            var page = new Page();
            page.Id = model.Id;
            page.Name = model.Name;
            page.Parameters = new Dictionary<string, string>(model.Parameters, StringComparer.OrdinalIgnoreCase);
            page.Headers.Metas = model.Metas;
            page.Headers.Styles = model.Styles;
            page.Headers.Scripts = model.Scripts;
            page.Headers.CustomHeader = model.CustomHeader;
            page.Headers.Titles = model.ContentTitle;

            string body = HtmlHeadService.RemoveBaseHrel(model.Body);

            page.Body = body;

            page.LayoutName = PageService.GetLayoutName(page);

            if (!string.IsNullOrEmpty(page.LayoutName))
            {
                page.Type = PageType.Layout; 
            }

            string routename = string.IsNullOrWhiteSpace(model.UrlPath) ? page.Name : model.UrlPath;
            if (!string.IsNullOrEmpty(routename))
            {
                routename = System.Web.HttpUtility.UrlDecode(routename);
            }

            routename = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(routename);

            var sitedb = call.Context.WebSite.SiteDb(); 
             
            if (!sitedb.Routes.Validate(routename, page.Id))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Url occupied", call.Context));
            }

            //----
            if (model.Id == default(Guid))
            {    
                sitedb.Routes.AddOrUpdate(routename, page, call.Context.User.Id);
                sitedb.Pages.AddOrUpdate(page, call.Context.User.Id);
            }
            else
            {
                var oldpage = sitedb.Pages.Get(model.Id);
                if (oldpage == null)
                {
                    return model.Id;
                }

                page.DefaultStart = oldpage.DefaultStart;
                page.IsSecure = oldpage.IsSecure; 
     
                sitedb.Pages.AddOrUpdate(page, call.Context.User.Id);

                var route = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, oldpage);

                if (string.IsNullOrWhiteSpace(route))
                {
                    sitedb.Routes.AddOrUpdate(routename, page, call.Context.User.Id);
                    return page.Id;
                }

                if (route != routename)
                {
                    sitedb.Routes.ChangeRoute(route, routename);
                }
            }               

            return page.Id;
        }
               
        public Guid PostRichText(string name, string title, string body, string url, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();
                                                   
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(body))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Page body and Url are required", call.Context));
            }

            url = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(url); 
            string PageBody = "<html>";
            if (!string.IsNullOrEmpty(title))
            {
                PageBody += "<head><title>" + title + "</title></head><body>";
            }
            PageBody += body + "</body></html>";


            Guid PageId = call.ObjectId;

            if (!sitedb.Routes.Validate(url, PageId))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Url occupied", call.Context)); 
            }

            if (PageId == default(Guid))
            {
                // new add. 
                Page newpage = new Page() { Name = name, Body = PageBody, Type = PageType.RichText };
                sitedb.Routes.AddOrUpdate(url, newpage, call.Context.User.Id);
                sitedb.Pages.AddOrUpdate(newpage, call.Context.User.Id);
                PageId = newpage.Id;
            }
            else
            {
                var oldpage = sitedb.Pages.Get(PageId);
                if (oldpage == null)
                {
                    return PageId;
                }

                oldpage.Name = name;
                oldpage.Body = PageBody;         
  
                sitedb.Pages.AddOrUpdate(oldpage, call.Context.User.Id);

                var route = Kooboo.Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, oldpage);
                if (route != url)
                {
                    sitedb.Routes.ChangeRoute(route, url);
                }

                PageId = oldpage.Id; 
            }
            return PageId;
        }
                 

        public void ConvertFile(ApiCall call)
        {
            var files = Kooboo.Lib.NETMultiplePart.FormReader.ReadFile(call.Context.Request.PostData);

            if (files != null && files.Count() > 0)
            {
                foreach (var f in files)
                {
                    var bytes = f.Bytes;
                    string filename = f.FileName;

                    string extension = System.IO.Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(extension))
                    {
                        extension = extension.ToLower();
                    }
                    if (extension == ".aspx" || extension == ".axd" || extension == ".asx" || extension == ".ashx" || extension == ".asmx" || extension == ".asp" || extension == ".cfm" || extension == ".yaws" || extension == ".html" || extension == ".htm" || extension == ".shtml" || extension == ".xhtml" || extension == ".jhtml" || extension == ".cshtml" || extension == ".jsp" || extension == ".jspx" || extension == ".wss" || extension == ".do" || extension == ".action" || extension == ".pl" || extension == ".php" || extension == ".php3" || extension == ".php4" || extension == ".phtml" || extension == ".py" || extension == ".cgi" || extension == ".dll" || extension == ".rb" || extension == ".rhtml")
                    {  
                        // import single page. 
                        var sitedb = call.WebSite.SiteDb();
                        var routeurl = GetAvailableRoute(sitedb, filename);
                        string textbody = System.Text.Encoding.UTF8.GetString(bytes);
                        var page = new Page() { Name = filename, Body = textbody };

                        sitedb.Routes.AddOrUpdate(routeurl, page,call.Context.User.Id);
                        sitedb.Pages.AddOrUpdate(page, call.Context.User.Id);
                        return;
                    }
                    else if (extension == ".zip" || extension == ".rar")
                    {
                        MemoryStream memory = new MemoryStream(bytes);
                    
                        Sites.Sync.ImportExport.ImportZip(memory, call.WebSite, call.Context.User.Id);
                        return;
                    }
                    // upload to api...
                    string api = Data.AppSettings.ConvertApiUrl + "/_api/converter/Convert";

                    Dictionary<string, string> header = new Dictionary<string, string>();
                    header.Add("filename", System.Net.WebUtility.UrlEncode(filename));
                    var response = Lib.Helper.HttpHelper.ConvertKooboo(api, bytes, header);

                    if (response != null)
                    {
                        MemoryStream memory = new MemoryStream(response);
                        Sites.Sync.ImportExport.ImportZip(memory, call.WebSite, call.Context.User.Id);
                    }
                }
            }

        }

        private string GetAvailableRoute(SiteDb sitedb, string filename)
        {
            string relativeurl = filename.Replace("\\", "/");

            if (relativeurl.StartsWith("/"))
            {
                relativeurl = relativeurl.Substring(1);
            }

            for (int i = 0; i < 999; i++)
            {
                string routerul;
                if (i > 0)
                {
                    routerul = "/" + i.ToString() + relativeurl;
                }
                else
                {
                    routerul = "/" + relativeurl;
                }

                var route = sitedb.Routes.GetByUrl(routerul);
                if (route == null)
                {
                    return routerul;
                }
            }

            return null;

        }

        public string GetAccessToken(ApiCall call)
        {
            return Kooboo.Data.Cache.AccessTokenCache.GetNewToken(call.Context.User.Id);
        }

        public PageDefaultRouteViewModel DefaultRoute(ApiCall call)
        {
            PageDefaultRouteViewModel model = new PageDefaultRouteViewModel();

            var startpages = call.WebSite.StartPages();
            if (startpages != null && startpages.Count() > 0)
            {
                model.StartPage = startpages.First().Id;
            }

            var pagenotfound = WebSiteService.GetCustomErrorUrl(call.WebSite, 404);
            Guid PageNotFoundId = default(Guid);
            if (!string.IsNullOrEmpty(pagenotfound))
            {
                if (!Guid.TryParse(pagenotfound, out PageNotFoundId))
                {
                    var DbPageNotFound = call.WebSite.SiteDb().Pages.GetByUrl(pagenotfound);
                    if (DbPageNotFound != null)
                    {
                        PageNotFoundId = DbPageNotFound.Id;
                    }
                }
            }

            var pageerror = WebSiteService.GetCustomErrorUrl(call.WebSite, 500);

            Guid PageErrorId = default(Guid);

            if (!string.IsNullOrEmpty(pageerror))
            {
                if (!Guid.TryParse(pageerror, out PageErrorId))
                {
                    var DbPageError = call.WebSite.SiteDb().Pages.GetByUrl(pageerror);
                    if (DbPageError != null)
                    {
                        PageErrorId = DbPageError.Id;
                    }
                }
            }

            model.NotFound = PageNotFoundId;
            model.Error = PageErrorId;
            return model;
        }

        [Kooboo.Attributes.RequireModel(typeof(PageDefaultRouteViewModel))]
        public void DefaultRouteUpdate(ApiCall call)
        {
            PageDefaultRouteViewModel model = call.Context.Request.Model as PageDefaultRouteViewModel;
            var sitedb = call.WebSite.SiteDb();
            var website = call.WebSite;

            if (model.StartPage != default(Guid))
            {
                var startpage = sitedb.Pages.Get(model.StartPage);
                startpage.DefaultStart = true;

                var allstartpages = sitedb.WebSite.StartPages();

                foreach (var item in allstartpages)
                {
                    if (item.Id != startpage.Id)
                    {
                        item.DefaultStart = false;
                        sitedb.Pages.AddOrUpdate(item);
                    }
                }
                sitedb.Pages.AddOrUpdate(startpage);
            }


            if (model.NotFound != default(Guid))
            {
                website.CustomErrors[404] = model.NotFound.ToString();
            }
            else
            {
                website.CustomErrors.Remove(404);
            }

            if (model.Error != default(Guid))
            {
                website.CustomErrors[500] = model.Error.ToString();
            }
            else
            {
                website.CustomErrors.Remove(500);
            }
            Data.GlobalDb.WebSites.AddOrUpdate(website);
        }

        [Kooboo.Attributes.RequireParameters("id", "name", "url")]
        public PageViewModel Copy(ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            var page = sitedb.Pages.Get(call.ObjectId);
            if (page != null)
            {
                var newpage = Lib.Serializer.Copy.DeepCopy<Page>(page);
                newpage.CreationDate = DateTime.UtcNow;
                newpage.LastModified = DateTime.UtcNow; 

                newpage.Name = call.GetValue("name");
                newpage.Id = default(Guid);
                string url = call.GetValue("url");

                if (!sitedb.Routes.Validate(url, default(Guid)))
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("Url occupied", call.Context));
                }
                 
                sitedb.Routes.AddOrUpdate(url, newpage, call.Context.User.Id);

                sitedb.Pages.AddOrUpdate(newpage, call.Context.User.Id);
                return ToPageModel(sitedb, newpage);
            }
            return null;
        }
    }
}
