//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.IO;
using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Context.UserProviders;
using Kooboo.IndexedDB.Serializer.Simple;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.Mysql;
using Kooboo.Sites.Scripting.Global.SqlServer;
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
                               { "Authorization",$"bearer {UserProviderHelper.GetJtwTokentFromContext(call.Context)}" }
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
            var startpages = site.StartPages();
            if (startpages != null && startpages.Count() > 0)
            {
                foreach (var item in startpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            var allpages = site.SiteDb().Pages.All();

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
                {
                    Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                    if (route != null && !route.Name.Contains("{") && !route.Name.Contains("%"))
                    {
                        return route.Name;
                    }
                }
            }

            if (allpages != null && allpages.Count() > 0)
            {
                foreach (var item in allpages)
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

            var siteId = Kooboo.Lib.Helper.DictionaryHelper.GetValue<Guid>(data, "siteid");
            var website = Kooboo.Data.Config.AppHost.SiteRepo.Get(siteId);

            if (website == null)
            {
                throw new Exception("website not found");
            }
            var siteDb = website.SiteDb();
            TemplateTransferModel transfer = new TemplateTransferModel();

            CopyMode copyMode = CopyMode.Normal;

            var shareMethod = DictionaryHelper.GetString(data, "shareMethod");
            if (!string.IsNullOrEmpty(shareMethod) && shareMethod.ToLower() == "update")
            {
                transfer.IsUpdate = true;
                copyMode = CopyMode.Fast;
            }

            string exportFile = ImportExport.ExportInter(siteDb, copyMode);

            if (!File.Exists(exportFile))
            {
                return;
            }
            var zipBytes = IOHelper.ReadAllBytes(exportFile);

            if (zipBytes.Length > AppSettings.MaxTemplateSize)
            {
                var error = Data.Language.Hardcoded.GetValue("Exceed max template size", call.Context);
                error += " " + Lib.Utilities.CalculateUtility.GetSizeString(AppSettings.MaxTemplateSize);
                throw new Exception(error);
            }

            transfer.TypeName = DictionaryHelper.GetValue<string>(data, "typeName");
            transfer.Cover = DictionaryHelper.GetValue<string>(data, "coverimage");
            transfer.ScreenShot = DictionaryHelper.GetValue<string>(data, "ScreenShot");
            transfer.Name = DictionaryHelper.GetString(data, "sitename"); 

            transfer.ZhCover = DictionaryHelper.GetValue<string>(data, "zhCoverimage");
            transfer.ZhScreenShot = DictionaryHelper.GetValue<string>(data, "zhScreenShot");

            transfer.ZhName = DictionaryHelper.GetString(data, "zhSitename");

            transfer.Bytes = zipBytes;
            transfer.ByteHash = Kooboo.Lib.Security.Hash.ComputeGuid(zipBytes);

            transfer.IsPublic = siteDb.WebSite.SiteType == Data.Definition.WebsiteType.p; 

            transfer.TemplateId = DictionaryHelper.GetValue<Guid>(data, nameof(TemplateTransferModel.TemplateId));

            var updateItem = DictionaryHelper.GetString(data, "updateItem");

            if (updateItem != null)
            {
                if (updateItem.ToLower() == "onlyscreen")
                {
                    transfer.UpdateScreenOnly = true;
                }
                else if (updateItem.ToLower() == "onlybinary")
                {
                    transfer.UpdateBinaryOnly = true;
                }
            }

            if (transfer.UpdateBinaryOnly)
            {
                UpdateBinary(transfer, call);
                return;
            }

            if (transfer.UpdateScreenOnly)
            {
                UpdateScreen(transfer, call);
                return;
            }

            SimpleConverter<TemplateTransferModel> converter = new();
            var postBytes = converter.ToBytes(transfer);

            string url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateserver/share");

            var response = HttpHelper.PostData(url, Data.Helper.ApiHelper.GetAuthHeaders(call.Context), postBytes);

            if (!response)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Share template failed", call.Context));
            }
        }

        public void UpdateScreen(TemplateTransferModel model, ApiCall call)
        {
            ScreenUpdate update = new ScreenUpdate();
            update.TemplateId = model.TemplateId;
            update.Cover = model.Cover;
            update.ScreenShot = model.ScreenShot;
            update.ZhCover = model.ZhCover;
            update.ZhScreenShot = model.ZhScreenShot;

            update.ZHName = model.ZhName;
            update.Name = model.Name;


            string url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateupdate/screenshot");

            SimpleConverter<ScreenUpdate> converter = new SimpleConverter<ScreenUpdate>();

            var bytes = converter.ToBytes(update);

            var response = HttpHelper.PostData(url, loginHeader(call), bytes);

        }

        public void UpdateBinary(TemplateTransferModel model, ApiCall call)
        {
            BinaryUpdate update = new BinaryUpdate();
            update.TemplateId = model.TemplateId;
            update.Bytes = model.Bytes;

            update.ByteHash = Kooboo.Lib.Security.Hash.ComputeGuid(update.Bytes);

            SimpleConverter<BinaryUpdate> converter = new SimpleConverter<BinaryUpdate>();

            string url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateupdate/binary");

            var bytes = converter.ToBytes(update);

            var response = HttpHelper.PostData(url, loginHeader(call), bytes);
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
            string Url = Kooboo.Lib.Helper.UrlHelper.Combine(Kooboo.Data.AppSettings.ThemeUrl, "/_api/templateserver/Delete");
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("Id", call.ObjectId.ToString());
            var ok = HttpHelper.Post<bool>(Url, para, loginHeader(call));
            return;
        }


        public Guid Use(string SiteName, string RootDomain, string SubDomain, string id, ApiCall call)
        {

            if (!Data.Config.AppHost.SiteService.CheckNameAvailable(SiteName, call.Context.User.CurrentOrgId))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("SiteName is taken", call.Context));
            }


            string FullDomain = RootDomain;
            if (!string.IsNullOrEmpty(SubDomain))
            {
                if (FullDomain.StartsWith("."))
                {
                    FullDomain = SubDomain + FullDomain;
                }
                else
                {
                    FullDomain = SubDomain + "." + FullDomain;
                }
            }

            string url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateserver/download?id=" + id);

            if (call.Context.User != null)
            {
                url += "&userid=" + call.Context.User.Id.ToString();
            }

            var download = DownloadHelper.DownloadFile(url, "zip");
            if (download == null)
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("template package not found", call.Context));
            }

            MemoryStream memory = new MemoryStream(download);
            var newsite = ImportExport.ImportZip(memory, call.Context.User.CurrentOrgId, SiteName, FullDomain, call.Context.User.Id);
            return newsite.Id;
        }


        // used for update package. 
        public List<OwnPackage> Personal(ApiCall call)
        {
            //var mock = new List<OwnPackage>();
            //mock.Add(new OwnPackage() { Name = "coming feature", TemplateId = System.Guid.NewGuid() });
            ////mock.Add(new OwnPackage() { Name = "Test Name 2", TemplateId = System.Guid.NewGuid() });
            //return mock;


            var lang = GetLang(call);

            Dictionary<string, string> paras = new Dictionary<string, string>();

            if (lang != null)
            {
                paras["lang"] = lang;
            }

            string Url = UrlHelper.Combine(UrlSetting.AppStore, "/_api/templateserver/Personal");
            Url = UrlHelper.AppendQueryString(Url, paras);


            var fullList = HttpHelper.Get2<List<TemplateItemViewModel>>(Url, null, Data.Helper.ApiHelper.GetAuthHeaders(call.Context));

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
