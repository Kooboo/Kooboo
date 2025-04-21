//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Config;
using Kooboo.Data.Context.UserProviders;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.Mysql;
using Kooboo.Sites.Scripting.Global.SqlServer;
using Kooboo.Sites.Service;
using Kooboo.Sites.Store;
using Kooboo.Sites.Store.Model;
using Kooboo.Sites.Sync;
using KScript;

namespace Kooboo.Web.Api.Implementation
{
    public class TemplateApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Template";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return false;  // this needs to turn back...
            }
        }

        public string GetLang(ApiCall call)
        {
            if (call.Context.User != null & call.Context.User.Language != null)
            {
                return call.Context.User.Language;
            }
            return null;
        }

        private Dictionary<string, string> loginHeader(ApiCall call)
        {
            return new Dictionary<string, string> {
                               { "Authorization",$"bearer {UserProviderHelper.GetJwtTokenFromContext(call.Context)}" }
                            };
        }

        public TemplateItemViewModel Detail(Guid Id, ApiCall call)
        {
            string Url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateserver/Detail");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", Id.ToString());

            var lang = GetLang(call);

            if (lang != null)
            {
                para["lang"] = lang;
            }

            var detail = HttpHelper.Get2<TemplateItemViewModel>(Url, para, loginHeader(call));

            detail.Type.Name = Data.Language.Hardcoded.GetValue(detail.Type.Name, call.Context);

            return detail;
        }

        public object FullTextSearch(ApiCall call)
        {
            var lang = GetLang(call);
            var url = "/_api/templateserver/fulltextsearch";
            if (lang != null)
            {
                url = $"{url}?lang={lang}";
            }
            url = UrlHelper.Combine(Kooboo.Data.UrlSetting.AppStore, url);
            return HttpHelper.Post<object>(url, call.Context.Request.Body, loginHeader(call));
        }

        public object Type(ApiCall call)
        {
            return new KTemplate(call.Context).Types;
        }

        protected string GetStartRelativeUrl(Data.Models.WebSite site)
        {
            var startPages = site.StartPages();
            if (startPages != null && startPages.Count() > 0)
            {
                foreach (var item in startPages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            var allPages = site.SiteDb().Pages.All();

            if (allPages != null && allPages.Count() > 0)
            {
                foreach (var item in allPages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            if (allPages != null && allPages.Count() > 0)
            {
                foreach (var item in allPages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null)
                    {
                        return route.Name;
                    }
                }
            }

            return "/";
        }

        protected void SetThumbnailUrl(List<TemplateItemViewModel> items)
        {
            if (items == null)
            {
                return;
            }

            string imgBaseUrl = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.ThumbNail) && !item.ThumbNail.ToLower().StartsWith("http://"))
                {
                    item.ThumbNail = UrlHelper.Combine(imgBaseUrl, item.ThumbNail);
                }
                item.ThumbNail += "?width=200";
            }
        }

        protected void SetImageDownloadUrl(TemplateDetailViewModel detail)
        {
            if (detail == null)
            {
                return;
            }
            string imgBaseUrl = UrlHelper.Combine(AppSettings.ThemeUrl, "/_api/download/themeimg/");

            int count = detail.Images.Count();
            for (int i = 0; i < count; i++)
            {
                var current = detail.Images[i];
                var newUrl = UrlHelper.Combine(imgBaseUrl, current);
                detail.Images[i] = newUrl;
            }
        }

        protected SiteDb GetSiteDb(ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            if (siteDb == null)
            {
                Guid SiteId = call.GetGuidValue("SiteId");

                var website = Data.Config.AppHost.SiteRepo.Get(SiteId);
                siteDb = website != null ? website.SiteDb() : null;
            }
            return siteDb;
        }

        public virtual void Share(ApiCall call)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(call.Context.Request.Body);
            TemplateService.Share(data, call.Context);
        }

        public void UpdateScreen(TemplateTransferModel model, ApiCall call)
        {
            TemplateService.UpdateScreen(model, call.Context);
        }

        public void UpdateBinary(TemplateTransferModel model, ApiCall call)
        {
            TemplateService.UpdateBinary(model, call.Context);
        }

        public ValidateResult ShareValidate(Guid siteid, ApiCall call)
        {
            var website = Kooboo.Data.Config.AppHost.SiteRepo.Get(siteid);

            if (website == null || !Security.AccessControl.HasWebsiteAccess(website, call.Context))
            {
                throw new Exception("Access denied");
            }

            var siteDb = website.SiteDb();

            ValidateResult result = new ValidateResult();
            result.IsSecure = true;

            var error = Data.Language.Hardcoded.GetValue("Contains sensitive data such as payment information or database connection", call.Context);

            var mysql = siteDb.CoreSetting.GetSetting<MysqlSetting>();
            if (mysql != null)
            {
                result.IsSecure = false;
                result.Message = error;
                result.Violation = mysql.ConnectionString;
            }

            var sql = siteDb.CoreSetting.GetSetting<SqlServerSetting>();

            if (sql != null)
            {
                result.IsSecure = false;
                result.Message = error;
                result.Violation = sql.ConnectionString;
            }

            var mongo = siteDb.CoreSetting.GetSetting<MongoSetting>();

            if (mongo != null)
            {
                result.IsSecure = false;
                result.Message = error;
                result.Violation = mongo.ConnectionString;
            }

            // result.ShowScreenShot = siteDb.WebSite.SiteType != Data.Definition.WebsiteType.p;
            //Temp
            result.ShowScreenShot = true;

            return result;

        }

        public void Delete(ApiCall call)
        {
            TemplateService.Delete(call.ObjectId.ToString(), call.Context);
        }


        public Guid Use(string SiteName, string RootDomain, string SubDomain, string id, ApiCall call)
        {
            if (!Data.Config.AppHost.SiteService.CheckNameAvailable(SiteName, call.Context.User.CurrentOrgId))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("SiteName is taken", call.Context));
            }

            string FullDomain = ConfigHelper.ToFullDomain(RootDomain, SubDomain);

            var guid = System.Guid.Parse(id);

            Guid UserId = default(Guid);
            if (call.Context.User != null)
            {
                UserId = call.Context.User.Id;
            }

            var memory = TemplateService.DownloadTemplate(guid, UserId);

            var newSite = ImportExport.ImportZip(memory, call.Context.User.CurrentOrgId, SiteName, FullDomain, call.Context.User.Id);
            Sites.Scripting.Global.Koobox.KFavorite.Add(call.Context, newSite.Id);
            return newSite.Id;
        }


        // used for update package. 
        public List<OwnPackage> Personal(ApiCall call)
        {
            //var mock = new List<OwnPackage>();
            //mock.Add(new OwnPackage() { Name = "coming feature", TemplateId = System.Guid.NewGuid() });
            ////mock.Add(new OwnPackage() { Name = "Test Name 2", TemplateId = System.Guid.NewGuid() });
            //return mock;

            var fullList = TemplateService.Personal(call.Context);

            if (fullList != null)
            {
                return fullList.Select(o => new OwnPackage() { TemplateId = o.Id, Name = o.Name }).ToList();
            }

            return null;

        }


    }

    public class ValidateResult
    {
        public bool IsSecure { get; set; }

        public string Message { get; set; }

        public string Violation { get; set; }

        public bool ShowScreenShot { get; set; }
    }


    public class OwnPackage
    {
        public string Name { get; set; }

        public Guid TemplateId { get; set; }
    }
}
