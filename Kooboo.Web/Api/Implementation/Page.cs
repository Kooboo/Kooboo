//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class PageApi : SiteObjectApi<Page>
    {
        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.MENU, Action = Data.Permission.Action.EDIT)]
        public List<PageViewModel> All(ApiCall apiCall)
        {
            var sitedb = apiCall.WebSite.SiteDb();
            var result = new List<PageViewModel>();

            var pages = sitedb.Pages.All();

            foreach (var item in pages.SortByNameOrLastModified(apiCall))
            {
                PageViewModel model = ToPageModel(sitedb, item);

                result.Add(model);
            }

            return result;
        }

        protected PageViewModel ToPageModel(SiteDb sitedb, Page item)
        {
            PageViewModel pageModel = new PageViewModel();

            pageModel.Id = item.Id;
            pageModel.Name = item.Name;
            pageModel.Type = item.Type;
            pageModel.Online = item.Online;

            if (item.HasLayout)
            {
                pageModel.LayoutId = Data.IDGenerator.Generate(item.LayoutName, ConstObjectType.Layout);
            }

            pageModel.LastModified = item.LastModified;
            pageModel.StartPage = item.DefaultStart;
            pageModel.InlineUrl = "/_api/v2/redirect/inline?siteid=" + sitedb.WebSite.Id + "&pageid=" + item.Id;

            pageModel.Path = Sites.Service.ObjectService.GetObjectRelativeUrl(sitedb, item);
            pageModel.PreviewUrl = PageService.GetPreviewUrl(sitedb, item);
            pageModel.Linked = sitedb.Relations.GetReferredBy(item).Count();
            pageModel.HasParameter = PageService.GetUrlParas(sitedb, item.Id).Any();

            if (item.Type == PageType.RichText)
            {
                string body = item.Body;
                var doc = Kooboo.Dom.DomParser.CreateDom(body);
                var titleTag = doc.head.getElementsByTagName("title");
                if (titleTag != null && titleTag.length > 0)
                {
                    pageModel.Title = titleTag.item[0].InnerHtml;
                }
            }

            var relations = sitedb.Relations.GetRelations(item.Id);

            if (relations != null && relations.Count() > 0)
            {
                var relationResult = pageModel.Relations;
                foreach (var oneRelation in relations.Where(o => o.ConstTypeY == ConstObjectType.Layout || o.ConstTypeY == ConstObjectType.View || o.ConstTypeY == ConstObjectType.Form || o.ConstTypeY == ConstObjectType.HtmlBlock || o.ConstTypeY == ConstObjectType.Menu))
                {
                    var objectTypeName = ConstTypeService.GetModelType(oneRelation.ConstTypeY).Name;
                    if (relationResult.ContainsKey(objectTypeName))
                    {
                        var value = relationResult[objectTypeName];
                        value = value + 1;
                        relationResult[objectTypeName] = value;
                    }
                    else
                    { relationResult.Add(objectTypeName, 1); }
                }
            }

            return pageModel;
        }

        public static Guid GetLayoutId(Page page)
        {
            string layoutName = Kooboo.Sites.Service.PageService.GetLayoutName(page);
            if (string.IsNullOrEmpty(layoutName))
            {
                return default(Guid);
            }
            else
            {
                return Data.IDGenerator.Generate(layoutName, ConstObjectType.Layout);
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
                //PageView = SiteDb.VisitorLog.QueryDescending(o => o.ObjectId == page.Id).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-24)).Take(999999).Count(),
                LastModified = page.LastModified,
                InlineUrl = "/_api/redirect/inline?siteid=" + SiteDb.WebSite.Id + "&pageid=" + page.Id
            };
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public virtual PageEditViewModel GetEdit(ApiCall call)
        {
            // optional type.  
            Guid PageId = call.ObjectId;
            var sitedb = call.WebSite.SiteDb();

            if (PageId == default)
            {
                string layoutId = call.GetValue("layoutid");
                string type = call.GetValue("type");

                var result = new PageEditViewModel();

                if (!string.IsNullOrEmpty(layoutId))
                {
                    var Layout = sitedb.Layouts.GetByNameOrId(layoutId);
                    if (Layout != null)
                    {
                        result.Body = InitPageLayoutSource(Layout);
                        result.Type = PageType.Layout;
                    }
                }
                else if (!string.IsNullOrEmpty(type) && (type.ToLower() == "richtext"))
                {
                    result.Body = "";
                }
                else
                {
                    result.Body = $@"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>

<body>
    <div></div>
</body>

</html>";
                }

                if (Enum.TryParse<PageType>(type, out var pageType))
                {
                    result.Type = pageType;
                }

                return result;
            }

            var page = sitedb.Pages.Get(PageId);
            var route = sitedb.Routes.GetByObjectId(page.Id);

            var model = new PageEditViewModel
            {
                Id = page.Id,
                Name = page.Name,
                Published = page.Online,
                UrlPath = route == null ? null : route.Name,
                Type = page.Type,
                EnableCache = page.EnableCache,
                DisableUnocss=page.DisableUnocss,
                CacheMinutes = page.CacheMinutes,
                CacheQueryKeys = page.CacheQueryKeys,
                CacheByVersion = page.CacheByVersion,
                Version = page.Version,
                PreviewUrl = PageService.GetPreviewUrl(sitedb, page),
                DesignConfig = page.DesignConfig
            };

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
                model.LayoutName = page.LayoutName;
                model.LayoutId = Kooboo.Data.IDGenerator.GetOrGenerate(page.LayoutName, ConstObjectType.Layout);
                model.Metas = page.Headers.Metas;
                model.CustomHeader = page.Headers.CustomHeader;
                model.MetaBindings = PageService.GetMetaBindings(sitedb, PageId);
                model.UrlParamsBindings = PageService.GetUrlParas(sitedb, PageId);
                model.ContentTitle = page.Headers.Titles;
                model.Parameters = page.Parameters;
                model.Scripts = page.Headers.Scripts;
                model.Styles = page.Headers.Styles;
                model.Body = page.Body;
                model.DesignConfig = page.DesignConfig;
            }

            return model;
        }

        public string GetDesignTemplate(ApiCall call)
        {
            var html = $@"<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Document</title>
</head>

<body>
    <div></div>
</body>

</html>";
            var script = VisualEditorHelper.GetInjects(call);

            var dom = DomParser.CreateDom(html);
            var changes = new List<SourceUpdate> {
                new SourceUpdate
                {
                    StartIndex = dom.head.location.endTokenStartIndex - 1,
                    EndIndex = dom.head.location.endTokenStartIndex - 1,
                    NewValue = $"\t{script}\n"
                },
                new SourceUpdate
                {
                    StartIndex = dom.body.location.openTokenEndIndex + 1,
                    EndIndex = dom.body.location.endTokenStartIndex - 1,
                    NewValue = "<ve-placeholder></ve-placeholder>"
                }
            };

            return DomService.UpdateSource(html, changes);
        }

        protected string InitPageLayoutSource(Layout layout)
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
        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public override Guid Post(ApiCall call)
        {
            var model = call.Context.Request.Model as PageUpdateViewModel;
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("invalid name", call.Context));
            }

            var page = new Page
            {
                Id = model.Id,
                Name = model.Name,
                EnableCache = model.EnableCache,
                DisableUnocss=model.DisableUnocss,
                CacheByVersion = model.CacheByVersion,
                CacheMinutes = model.CacheMinutes,
                CacheQueryKeys = model.CacheQueryKeys,
                Online = model.Published,
                Parameters = new Dictionary<string, string>(model.Parameters, StringComparer.OrdinalIgnoreCase)
            };
            page.Headers.Metas = model.Metas;
            page.Headers.Styles = model.Styles;
            page.Headers.Scripts = model.Scripts;
            page.Headers.CustomHeader = model.CustomHeader;
            page.Headers.Titles = model.ContentTitle;
            page.DesignConfig = model.DesignConfig;

            string body = HtmlHeadService.RemoveBaseHrel(model.Body);

            page.Body = body;

            page.LayoutName = PageService.GetLayoutName(page);
            if (string.IsNullOrEmpty(page.LayoutName) && !string.IsNullOrEmpty(model.LayoutName))
            {
                page.LayoutName = model.LayoutName;
            }

            if (model.Type != null)
            {
                page.Type = model.Type.Value;
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
            if (model.Id == default)
            {
                sitedb.Routes.AddOrUpdate(routename, page, call.Context.User.Id);
                sitedb.Pages.AddOrUpdate(page, call.Context.User.Id);
            }
            else
            {
                var oldpage = sitedb.Pages.Get(model.Id);
                if (oldpage == null) return model.Id;

                (model as IDiffChecker).CheckDiff(oldpage);

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

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public Guid PostRichText(string name, string body, string url, bool enableCache, bool cacheByVersion, int cacheMinutes, string cacheQueryKeys, bool published, ApiCall call)
        {
            var sitedb = call.WebSite.SiteDb();

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(body))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Page body and Url are required", call.Context));
            }

            url = Kooboo.Sites.Helper.RouteHelper.ToValidRoute(url);
            string PageBody = "<html>";
            var title = call.Context.Request.GetValue("title");
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
                Page newpage = new Page() { Name = name, Body = PageBody, Type = PageType.RichText, EnableCache = enableCache, CacheByVersion = cacheByVersion, CacheMinutes = cacheMinutes, CacheQueryKeys = cacheQueryKeys };
                newpage.Online = published;
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
                oldpage.EnableCache = enableCache;
                oldpage.CacheByVersion = cacheByVersion;
                oldpage.CacheMinutes = cacheMinutes;
                oldpage.CacheQueryKeys = cacheQueryKeys;
                oldpage.Online = published;

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

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public void ConvertFile(ApiCall call)
        {
            var files = call.Context.Request.Files;

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
                    var textFileExtensions = new string[] { ".aspx", ".axd", ".asx", ".ashx", ".asmx", ".asp",
                        ".cfm", ".yaws", ".html", ".htm", ".shtml", ".shtm",".xhtml",".ehtml",
                        ".jhtml", ".cshtml", ".jsp", ".jspx", ".wss", ".do",
                        ".action", ".pl", ".php", ".php3", ".php4", ".phtml",
                        ".py", ".cgi", ".dll", ".rb", ".rhtml" };

                    if (textFileExtensions.Contains(extension))
                    {
                        // import single page. 
                        var sitedb = call.WebSite.SiteDb();
                        var routeurl = GetAvailableRoute(sitedb, filename);
                        string textbody = System.Text.Encoding.UTF8.GetString(bytes);
                        var page = new Page() { Name = filename, Body = textbody };

                        sitedb.Routes.AddOrUpdate(routeurl, page, call.Context.User.Id);
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
                    string api = Data.UrlSetting.Converter + "/_api/converter/Convert";

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

        [Permission(Feature.PAGES, Action = "router")]
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
        [Permission(Feature.PAGES, Action = "router")]
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
            Data.Config.AppHost.SiteRepo.AddOrUpdate(website);
        }

        [Kooboo.Attributes.RequireParameters("id", "name", "url")]
        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
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

                var model = JsonSerializer.Deserialize<JsonElement>(call.Context.Request.Body);

                var nodes = model.GetProperty("pageStructure")
                    .Deserialize<PageCopyService.NodeViewModel[]>(
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        }
                    );
                var apply = new List<System.Action>();
                newpage.Body = PageCopyService.ApplyCopyStructure(newpage.Body, nodes, sitedb, apply);
                apply.ForEach(f => f());
                var layoutNode = nodes.FirstOrDefault(f => f.Type == "layout" && f.Name == newpage.LayoutName);
                if (layoutNode != default && layoutNode.Selected) newpage.LayoutName = layoutNode.NewName;
                sitedb.Routes.AddOrUpdate(url, newpage, call.Context.User.Id);
                sitedb.Pages.AddOrUpdate(newpage, call.Context.User.Id);
                return ToPageModel(sitedb, newpage);
            }
            return null;
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public override Guid AddOrUpdate(ApiCall call)
        {
            return base.AddOrUpdate(call);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.DELETE)]
        public override bool Delete(ApiCall call)
        {
            return base.Delete(call);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.DELETE)]
        public override bool Deletes(ApiCall call)
        {
            return base.Deletes(call);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public override object Get(ApiCall call)
        {
            return base.Get(call);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.VIEW)]
        public override List<object> List(ApiCall call)
        {
            return base.List(call);
        }

        [Permission(Feature.PAGES, Action = Data.Permission.Action.EDIT)]
        public override Guid put(ApiCall call)
        {
            return base.put(call);
        }

        public override bool IsUniqueName(ApiCall call)
        {
            var page = call.Context.WebSite.SiteDb()
                    .Pages
                    .Query
                    .Where(p => p.Name == call.NameOrId)
                    .FirstOrDefault();

            return page == null;
        }

        public PageCopyService.Node[] PageStructure(ApiCall call)
        {
            Guid pageId = call.ObjectId;
            var siteDb = call.Context.WebSite.SiteDb();
            var page = siteDb.Pages.Get(pageId);
            if (page == default) throw new Exception("Page not found");
            return PageCopyService.AnalyzeStructure(page.Body, siteDb, call.Context.Culture);
        }
    }
}
