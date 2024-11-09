//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Config;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using Kooboo.Sites.ScriptModules;
using Kooboo.Sites.ScriptModules.Models;
using Kooboo.Sites.ScriptModules.Render.View;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Web.Lighthouse;
using Kooboo.Web.ViewModel;


namespace Kooboo.Web.Api.Implementation
{
    public class SiteApi : IApi
    {
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
                return true;
            }
        }

        public string ModelName
        {
            get
            {
                return "Site";
            }
        }

        public Dictionary<string, string> Types(ApiCall call)
        {
            Dictionary<string, string> types = new Dictionary<string, string>();
            types.Add("p", Data.Language.Hardcoded.GetValue("public", call.Context));
            types.Add("o", Data.Language.Hardcoded.GetValue("organization", call.Context));
            types.Add("m", Data.Language.Hardcoded.GetValue("site user", call.Context));
            types.Add("u", Data.Language.Hardcoded.GetValue("login user", call.Context));
            return types;
        }

        public SiteCultureViewModel Langs(ApiCall request)
        {
            SiteCultureViewModel viewModel = new SiteCultureViewModel();
            string strid = request.GetValue("SiteId");
            if (string.IsNullOrEmpty(strid))
            {
                strid = request.GetValue("id");
            }

            Guid SiteId = default(Guid);

            if (Guid.TryParse(strid, out SiteId))
            {
                var site = Data.Config.AppHost.SiteRepo.Get(SiteId);
                if (site != null)
                {
                    var cultures = Kooboo.Data.Language.SiteCulture.List(SiteId);

                    string defaultName = site.DefaultCulture;

                    if (Data.Language.LanguageSetting.ISOTwoLetterCode.ContainsKey(site.DefaultCulture))
                    {
                        defaultName = Data.Language.LanguageSetting.ISOTwoLetterCode[site.DefaultCulture];
                    }

                    Dictionary<string, string> siteCultures = new Dictionary<string, string>();

                    if (site.EnableMultilingual)
                    {
                        if (site.Culture != null && site.Culture.Count() > 0)
                        {
                            foreach (var item in site.Culture)
                            {
                                siteCultures.Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            foreach (var item in site.Culture.Keys.ToList())
                            {
                                if (cultures.ContainsKey(item))
                                {
                                    siteCultures[item] = cultures[item];
                                }
                            }
                        }
                    }
                    else
                    {
                        siteCultures.Add(site.DefaultCulture, defaultName);
                    }

                    viewModel.Default = site.DefaultCulture;
                    viewModel.DefaultName = defaultName;
                    viewModel.Cultures = siteCultures;

                    return viewModel;
                }
            }
            return null;
        }

        public Dictionary<string, string> Cultures(ApiCall call)
        {
            return Kooboo.Data.Language.SiteCulture.List(call.WebSite.Id);
        }

        public virtual List<SiteSummaryViewModel> List(ApiCall apiCall)
        {
            var user = apiCall.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }

            var sites = WebSiteService.ListByUser(user);

            List<SiteSummaryViewModel> result = new List<SiteSummaryViewModel>();

            foreach (var item in sites)
            {
                var sitedb = item.SiteDb();
                SiteSummaryViewModel summary = new SiteSummaryViewModel();
                summary.SiteId = item.Id;
                summary.SiteName = item.Name;
                summary.SiteDisplayName = item.DisplayName;
                //  summary.PageCount = sitedb.Pages.Count();
                //  summary.ImageCount = sitedb.Images.Count();
                // if user has not right to access the site. present the preview link.  

                summary.Online = item.Published;
                //  summary.Visitors = sitedb.VisitorLog.QueryDescending(o => true).EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-12)).Count();

                try
                {
                    var allTasks = sitedb.TransferTasks.All();

                    if (allTasks != null && allTasks.Any())
                    {
                        var tasks = allTasks.FindAll(o => o != null && o.done == false);

                        foreach (var task in tasks)
                        {
                            if (task.CreationDate > DateTime.UtcNow.AddMinutes(-2))
                            {
                                summary.InProgress = true;
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }


                try
                {
                    var defaultRoute = ObjectRoute.GetDefaultRoute(item.SiteDb());

                    if (defaultRoute != null)
                    {
                        summary.HomePageLink = item.BaseUrl()?.TrimEnd('/') + defaultRoute.Name;
                    }
                }
                catch (Exception)
                {

                }

                result.Add(summary);
            }
            return result;
        }

        [Permission(Feature.SITE, Action = "export")]
        public virtual string ExportGenerate(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }
            var siteDb = site.SiteDb();

            var copyModeString = call.GetValue("copyMode");
            var copyMode = string.IsNullOrWhiteSpace(copyModeString)
                ? CopyMode.Normal
                : (CopyMode)Enum.Parse(typeof(CopyMode), copyModeString, true);

            if (copyMode == CopyMode.Normal)
            {
                var size = IOHelper.GetDirectorySize(siteDb.Name);
                if (size > AppSettings.MaxNormalExportSize * 5)
                {
                    throw new Exception(Hardcoded.GetValue("Site too large,please use full mode.", call.Context));
                }
            }

            string zipFile = ImportExport.ExportInter(siteDb, copyMode);

            var info = new System.IO.FileInfo(zipFile);

            if (copyMode == CopyMode.Normal && info != null && info.Length > AppSettings.MaxNormalExportSize)
            {
                throw new Exception(Hardcoded.GetValue("Site too large,please use full mode.", call.Context));
            }

            return System.IO.Path.GetFileName(zipFile);
        }


        [Permission(Feature.SITE, Action = "export")]
        public virtual BinaryResponse Export(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }

            var exportfile = call.GetValue("exportfile");
            var path =  System.IO.Path.Combine(AppSettings.TempDataPath, exportfile);
            string name = site.DisplayName;
            if (string.IsNullOrEmpty(name))
            {
                name = site.Name;
            }

            name = Lib.Helper.StringHelper.ToValidFileName(name);

            if (File.Exists(path))
            {
                BinaryResponse response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"attachment;filename={name}.zip");
                response.Stream = new FileStream(path, FileMode.Open);
                return response;
            }
            return null;
        }

        [Permission(Feature.SITE, Action = "export")]
        [Attributes.RequireParameters("stores")]
        public virtual string ExportStoreGenerate(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }

            var storeValue = call.GetValue("stores");

            var stores = storeValue.Split(',').ToList();
            var result= ImportExport.ExportInterSelected(site.SiteDb(), stores);
            return System.IO.Path.GetFileName(result);
        }


        [Permission(Feature.SITE, Action = "export")]
        public virtual BinaryResponse ExportStore(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }
            var exportFile = call.GetValue("exportfile");
            var path = System.IO.Path.Combine(AppSettings.TempDataPath, exportFile);

            if (File.Exists(path))
            {
                BinaryResponse response = new BinaryResponse();
                response.ContentType = "application/zip";
                response.Headers.Add("Content-Disposition", $"attachment;filename={site.Name}.zip");
                response.Stream = new FileStream(path, FileMode.Open);
                return response;
            }
            return null;
        }

        public List<ExportStoreNameViewModel> ExportStoreNames(ApiCall call)
        {
            // TODO: Change into attribute definition or Interface to define available store names for the menu. 

            var context = call.Context;

            List<ExportStoreNameViewModel> names = new List<ExportStoreNameViewModel>();
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.Page), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.View), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.Layout), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.Image), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.Script), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Models.Style), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Contents.Models.TextContent), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Contents.Models.ContentType), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Contents.Models.ContentFolder), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Contents.Models.HtmlBlock), context));
            names.Add(new ExportStoreNameViewModel(typeof(Sites.Contents.Models.Label), context));

            names.Add(new ExportStoreNameViewModel() { Name = "Menu", DisplayName = Hardcoded.GetValue("Menu", call.Context) });

            names.Add(new ExportStoreNameViewModel() { Name = "Storage", DisplayName = Hardcoded.GetValue("Database", call.Context) });

            names.Add(new ExportStoreNameViewModel() { Name = typeof(Code).Name, DisplayName = Hardcoded.GetValue("Code", call.Context) });

            names.Add(new ExportStoreNameViewModel() { Name = "Authentication", DisplayName = Hardcoded.GetValue("Authentication", call.Context) });

            names.Add(new ExportStoreNameViewModel() { Name = "CmsFile", DisplayName = Hardcoded.GetValue("File", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "SiteJob", DisplayName = Hardcoded.GetValue("Job", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "BusinessRule", DisplayName = Hardcoded.GetValue("Front Event", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "ScriptModule", DisplayName = Hardcoded.GetValue("Modules", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "Form", DisplayName = Hardcoded.GetValue("Form", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "OpenApi", DisplayName = Hardcoded.GetValue("OpenApi", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "KConfig", DisplayName = Hardcoded.GetValue("Text", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "SPAMultilingual", DisplayName = Hardcoded.GetValue("SPA Multilingual", call.Context) });
            names.Add(new ExportStoreNameViewModel() { Name = "TransferTask", DisplayName = Hardcoded.GetValue("Transfer task", call.Context) });

            return names;
        }

        [Permission(Feature.SITE, Action = Data.Permission.Action.EDIT)]
        public void SwitchStatus(ApiCall call)
        {
            var site = call.WebSite;

            if (site != null)
            {
                site.Published = !site.Published;
            }

            Data.Config.AppHost.SiteRepo.AddOrUpdate(site);
        }

        public void Preview(ApiCall call, Guid SiteId)
        {
            var site = Data.Config.AppHost.SiteRepo.Get(SiteId);
            if (site != null)
            {
                var baseurl = site.BaseUrl();

                if (!string.IsNullOrEmpty(baseurl))
                {
                    call.Context.Response.Redirect(301, baseurl);
                }
            }
        }

        public SiteViewModel Get(ApiCall call)
        {
            var webSite = call.WebSite;

            var result = new SiteViewModel
            {
                Site = webSite,
                IsAdmin = call.Context.User.IsAdmin || webSite.OrganizationId == call.Context.User.Id,
            };

            if (!string.IsNullOrEmpty(result.Site.PreviewUrl))
            {
                result.BaseUrl = result.Site.PreviewUrl;
                result.PrUrl = result.Site.PreviewUrl;
            }
            else
            {
                result.BaseUrl = result.Site.BaseUrl();
                result.PrUrl = Data.Service.WebSiteService.EnsureHttpsBaseUrlOnServer(result.BaseUrl, call.WebSite);
            }


            var sitedb = webSite.SiteDb();
            if (sitedb.TransferTasks.History()?.Any() ?? false)
            {
                result.ShowContinueDownload = true;
            }

            var user = PermissionService.GetSiteUser(call.Context);

            if (user != default && user.VisibleAdvancedMenus != default)
            {
                result.VisibleAdvancedMenus = user.VisibleAdvancedMenus.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                result.VisibleAdvancedMenus = webSite.VisibleAdvancedMenus;
            }

            if (!result.IsAdmin && user != null)
            {
                var role = PermissionService.GetRole(sitedb, user.SiteRole);
                result.Permissions = role?.Permissions;
            }

            SiteCoverService.Add(webSite.Id);


            var org = GlobalDb.Organization.Get(call.Context.WebSite.OrganizationId);
            result.ServiceLevel = org.ServiceLevel;
            if (WebSiteService.DevelopmentAccess.RequireDevelopmentPassword(call.Context, org))
            {
                result.Site.Status = WebSite.SiteStatus.Development; // Set as development so that the password will appear. 
            }

            try
            {
                result.ModuleMenus = GetModuleMenus(call, webSite, sitedb);
            }
            catch { }

            return result;
        }

        private static ModuleMenu[] GetModuleMenus(ApiCall call, WebSite webSite, SiteDb sitedb)
        {
            var moduleMenus = new List<ModuleMenu>();
            var modules = sitedb.ScriptModule.All().Where(w => w.Online).ToArray();
            foreach (var module in modules)
            {
                var moduleContext = ModuleContext.CreateNewFromRenderContext(call.Context, module);
                var diskHandle = DiskHandler.FromModuleContext(moduleContext, new ResourceType(EnumResourceType.root));
                if (!diskHandle.Exists("/", "module.config")) continue;
                var moduleConfig = diskHandle.Read("/", "module.config");
                var jsonRoot = JsonSerializer.Deserialize<JsonElement>(moduleConfig, Defaults.JsonSerializerOptions);
                if (!jsonRoot.TryGetProperty("name", out var moduleName)) continue;
                if (!jsonRoot.TryGetProperty("menu", out var moduleMenuJson)) continue;
                var moduleMenu = moduleMenuJson.Deserialize<ModuleMenu>(Defaults.JsonSerializerOptions);
                if (moduleMenu == default) continue;
                moduleMenu.Id = moduleName.GetString();

                if (string.IsNullOrWhiteSpace(moduleMenu.Url) || !moduleMenu.Url.StartsWith("http"))
                {
                    var modulePath = Settings.ModulePath(module.Name);
                    var startView = moduleMenu.Url;
                    if (string.IsNullOrWhiteSpace(startView))
                    {
                        startView = ViewRender.GetStartView(moduleContext);
                    }
                    if (string.IsNullOrWhiteSpace(modulePath) || string.IsNullOrWhiteSpace(startView)) continue;
                    moduleMenu.Url = $"{modulePath}/{startView}?SiteId={webSite.Id}";
                }

                if (!string.IsNullOrWhiteSpace(moduleMenu.Icon) || !(moduleMenu.Icon?.StartsWith("http") ?? true))
                {
                    var modulePath = Settings.ModulePath(module.Name);
                    if (string.IsNullOrWhiteSpace(modulePath)) continue;
                    moduleMenu.Icon = $"{modulePath}/img/{moduleMenu.Icon}?SiteId={webSite.Id}";
                }
                moduleMenus.Add(moduleMenu);
            }

            return [.. moduleMenus];
        }

        [Permission(Feature.SITE, Action = Data.Permission.Action.EDIT)]
        [Kooboo.Attributes.RequireModel(typeof(SiteUpdate))]
        public void Post(ApiCall call)
        {
            var siteid = call.GetValue<Guid>("siteid");

            var currentsite = Data.Config.AppHost.SiteRepo.Get(siteid);
            var org = GlobalDb.Organization.Get(currentsite.OrganizationId);


            if (currentsite != null)
            {
                var newinfo = call.Context.Request.Model as SiteUpdate;
                currentsite.DisplayName = newinfo.DisplayName;

                currentsite.EnableVisitorLog = newinfo.EnableVisitorLog;
                currentsite.EnableConstraintFixOnSave = newinfo.EnableConstraintFixOnSave;
                currentsite.EnableFrontEvents = newinfo.EnableFrontEvents;
                currentsite.EnableMultilingual = newinfo.EnableMultilingual;

                currentsite.CustomSettings = newinfo.CustomSettings;
                currentsite.Culture = newinfo.Culture;
                currentsite.SitePath = newinfo.SitePath;
                currentsite.EnableSitePath = newinfo.EnableSitePath;
                currentsite.DefaultCulture = newinfo.DefaultCulture;
                currentsite.AutoDetectCulture = newinfo.AutoDetectCulture;
                currentsite.ForceSSL = newinfo.ForceSSL;

                currentsite.EnableJsCssBrowerCache = newinfo.EnableJsCssBrowerCache;
                currentsite.EnableImageBrowserCache = newinfo.EnableImageBrowserCache;
                currentsite.EnableImageAlt = newinfo.EnableImageAlt;
                currentsite.ImageCacheDays = newinfo.ImageCacheDays;

                currentsite.EnableSPA = newinfo.EnableSPA;
                currentsite.EnableVideoBrowserCache = newinfo.EnableVideoBrowserCache;

                currentsite.EnableJsCssCompress = newinfo.EnableJsCssCompress;
                currentsite.EnableHtmlMinifier = newinfo.EnableHtmlMinifier;

                // if (currentsite.SiteType != newinfo.SiteType)
                // {
                //     CmsApiHelper.EnsureAdminRights(call, "SiteType");
                currentsite.SiteType = newinfo.SiteType;
                // }
                currentsite.WhiteListPath = newinfo.WhiteListPath;
                currentsite.SpecialPath = newinfo.SpecialPath;
                currentsite.IncludePath = newinfo.IncludePath;

                currentsite.EnableCORS = newinfo.EnableCORS;
                currentsite.EnableSqlLog = newinfo.EnableSqlLog;

                currentsite.PreviewUrl = newinfo.PreviewUrl;
                currentsite.EnableLighthouseOptimization = newinfo.EnableLighthouseOptimization;
                currentsite.LighthouseSettingsJson = newinfo.LighthouseSettingsJson;
                currentsite.DefaultDatabase = newinfo.DefaultDatabase;
                currentsite.Pwa = newinfo.Pwa;
                currentsite.CodeLogSettings = newinfo.CodeLogSettings;
                currentsite.SitemapSettings = newinfo.SitemapSettings;
                currentsite.UnocssSettings = newinfo.UnocssSettings;
                currentsite.LastUpdateTime = DateTime.UtcNow.Ticks;
                currentsite.EnableCssSplitByMedia = newinfo.EnableCssSplitByMedia;
                currentsite.DesktopMinWidth = newinfo.DesktopMinWidth;
                currentsite.MobileMaxWidth = newinfo.MobileMaxWidth;
                currentsite.TinymceToolbarSettings = newinfo.TinymceToolbarSettings;
                currentsite.TinymceSettings = newinfo.TinymceSettings;
                currentsite.EnableTinymceToolbarSettings = newinfo.EnableTinymceToolbarSettings;
                currentsite.SsoLogin = newinfo.SsoLogin;
                currentsite.AutomateCovertImageToWebp = newinfo.AutomateCovertImageToWebp;
                currentsite.CodeSuggestions = newinfo.CodeSuggestions;
                currentsite.RecordSiteLogVideo = newinfo.RecordSiteLogVideo;
                currentsite.EnableUpdateSimilarPage = newinfo.EnableUpdateSimilarPage;
                currentsite.EnableResourceCache = newinfo.EnableResourceCache;
                currentsite.ContinueDownload = newinfo.ContinueDownload;
                currentsite.EnableVisitorCountryRestriction = newinfo.EnableVisitorCountryRestriction;
                currentsite.VisitorCountryRestrictions = newinfo.VisitorCountryRestrictions;
                currentsite.VisitorCountryRestrictionPage = newinfo.VisitorCountryRestrictionPage;

                if (org.ServiceLevel > 0)
                {
                    currentsite.Status = newinfo.Status;
                }
                // the cluster... 
                Data.Config.AppHost.SiteRepo.AddOrUpdate(currentsite);
            }
        }

        public void UpdateAdvancedMenus(string[] menus, ApiCall call)
        {
            var siteDb = call.WebSite.SiteDb();
            var user = siteDb.SiteUser.Get(call.Context.User.Id);

            if (user != default)
            {
                user.VisibleAdvancedMenus = string.Join(',', menus);
                siteDb.SiteUser.AddOrUpdate(user);
            }
            else
            {
                call.WebSite.VisibleAdvancedMenus = menus;
                Data.Config.AppHost.SiteRepo.AddOrUpdate(call.WebSite);
            }

        }

        [Permission(Feature.SITE, Action = Data.Permission.Action.DELETE)]
        public bool BatchDelete(ApiCall call)
        {
            var ids = call.GetValue<Guid[]>("ids");
            var hasSuccess = false;
            foreach (var siteId in ids)
            {
                if (siteId == default(Guid) || siteId == Guid.Empty) continue;
                WebSiteService.Delete(siteId, call.Context.User);
                hasSuccess = true;
            }

            return hasSuccess;
        }

        [Permission(Feature.SITE, Action = Data.Permission.Action.DELETE)]
        public bool Delete(ApiCall call)
        {
            var SiteId = call.GetValue<Guid>(DataConstants.SiteId);
            if (SiteId != default(Guid))
            {
                WebSiteService.Delete(SiteId, call.Context.User);
                return true;
            }
            return false;
        }

        [Permission(Feature.SITE, Action = Data.Permission.Action.EDIT)]
        public void EnableCodeVideoLog(ApiCall call)
        {
            call.WebSite.RecordSiteLogVideo = true;
            AppHost.SiteRepo.AddOrUpdate(call.WebSite);
        }

        public WebSite Create(ApiCall call)
        {
            string fulldomain = call.GetValue("FullDomain");
            if (string.IsNullOrEmpty(fulldomain))
            {
                string RootDomain = call.GetValue("RootDomain");
                string SubDomain = call.GetValue("SubDomain");
                fulldomain = SubDomain + "." + RootDomain;
            }
            string sitename = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(sitename) || string.IsNullOrEmpty(fulldomain))
            {
                return null;
            }

            WebSite newsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(call.Context.User.CurrentOrgId, sitename, fulldomain, call.Context.User.Id, true);
            return newsite;
        }

        [Kooboo.Attributes.RequireParameters("rootdomain")]
        public bool CheckDomainBindingAvailable(ApiCall call)
        {
            string RootDomain = call.GetValue("RootDomain");
            string subdomain = call.GetValue("SubDomain");

            if (!string.IsNullOrEmpty(subdomain) && subdomain.ToLower() == "local" && RootDomain.ToLower() == "kooboo")
            {
                return false;
            }

            if (RootDomain != null && RootDomain.StartsWith("."))
            {
                RootDomain = RootDomain.Substring(1);
            }

            return Kooboo.Sites.Service.WebSiteService.DomainAvailable(RootDomain, subdomain);
        }


        public Guid ImportSite(ApiCall call)
        {
            string RootDomain = null;
            string SubDomain = null;
            string SiteName = null;
            string packageName = null;
            if (call.Context.Request.Forms != null)
            {
                RootDomain = call.Context.Request.Forms["RootDomain"];
                SubDomain = call.Context.Request.Forms["SubDomain"];
                SiteName = call.Context.Request.Forms["SiteName"];
                packageName = call.Context.Request.Forms["packageName"];
            }

            if (string.IsNullOrEmpty(SiteName) || string.IsNullOrEmpty(RootDomain) || string.IsNullOrEmpty(packageName))
            {
                return default(Guid);
            }

            string fulldomain = string.IsNullOrEmpty(SubDomain) ? RootDomain : SubDomain + "." + RootDomain;
            var packagePath = System.IO.Path.Combine(AppSettings.TempDataPath, packageName);
            using var fileStream = new FileStream(packagePath, FileMode.Open);
            var newsite = ImportExport.ImportZip(fileStream, call.Context.User.CurrentOrgId, SiteName, fulldomain, call.Context.User.Id);
            fileStream.Dispose();
            File.Delete(packagePath);
            return newsite.Id;
        }

        public void MultiChunkUpload(ApiCall call, Guid id)
        {
            var name = call.GetValue("name");
            var temDir = System.IO.Path.Combine(AppSettings.TempDataPath, id.ToString());
            IOHelper.EnsureDirectoryExists(temDir);

            if (name != default)
            {
                var fileName = System.IO.Path.Combine(AppSettings.TempDataPath, name);
                using var stream = new FileStream(fileName, FileMode.Append);

                var chunks = Directory.GetFiles(temDir).OrderBy(c =>
                {
                    c = System.IO.Path.GetFileName(c);
                    return int.Parse(c.Split(".")[0]);
                }).ToArray();

                foreach (var item in chunks)
                {
                    using var chunk = new FileStream(item, FileMode.Open);
                    chunk.CopyTo(stream);
                }
                stream.Dispose();
                Directory.Delete(temDir, true);
            }
            else
            {
                var index = call.GetIntValue("index");
                var path = System.IO.Path.Combine(temDir, $"{index}.chunk");
                using var stream = new FileStream(path, FileMode.Append);
                call.Context.Request.Files[0].CopyTo(stream);
            }
        }

        public Guid ImportUrl(string RootDomain, string SubDomain, string SiteName, string Url, ApiCall call)
        {

            var bytes = Kooboo.Lib.Helper.DownloadHelper.DownloadFileAsync(Url).Result;

            if (bytes != null)
            {
                string fulldomain = string.IsNullOrEmpty(SubDomain) ? RootDomain : SubDomain + "." + RootDomain;

                var newsite = ImportExport.ImportZip(new MemoryStream(bytes), call.Context.User.CurrentOrgId, SiteName, fulldomain, call.Context.User.Id);
                return newsite.Id;
            }

            return default(Guid);
        }

        public void RenewSite(ApiCall call, Guid SiteId, string replaceStores)
        {
            var files = call.Context.Request.Files;

            if (files == null || files.Count() == 0)
            {
                throw new Exception("No files uploaded");
            }

            var fileStream = new MemoryStream(files[0].Bytes);
            var site = Kooboo.Data.Config.AppHost.SiteRepo.Get(SiteId);
            var sitedb = site.SiteDb();
            var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);
            ImportExport.ImportInter(archive, sitedb, replaceStores.Split(',').ToList());
        }

        public bool IsUniqueName(ApiCall apiCall)
        {
            var sitename = apiCall.GetValue("SiteName", "Name");
            return Kooboo.Data.Config.AppHost.SiteService.CheckNameAvailable(sitename, apiCall.Context.User.CurrentOrgId);
        }

        public bool IsSubdomainAvailable(ApiCall apiCall)
        {
            string RootDomain = apiCall.GetValue("RootDomain");
            string SubDomain = apiCall.GetValue("SubDomain");
            string fulldomain = SubDomain + "." + RootDomain;

            var site = Kooboo.Data.Config.AppHost.BindingService.GetByFullDomain(fulldomain);

            return site == null || site.Count == 0;
        }

        public String GetName(ApiCall call)
        {
            string strsiteid = call.GetValue("siteid");
            if (!string.IsNullOrEmpty(strsiteid))
            {
                Guid siteid;
                if (System.Guid.TryParse(strsiteid, out siteid))
                {
                    var site = Data.Config.AppHost.SiteRepo.Get(siteid);
                    if (site != null)
                    {
                        return site.DisplayName;
                    }
                }
            }
            return null;
        }

        [Attributes.RequireParameters("SiteId")]
        public List<DataCenter> ClusterList(ApiCall call)
        {
            var orgid = call.Context.User.CurrentOrgId;
            var websiteid = call.GetValue<Guid>("SiteId");

            List<DataCenter> temp = new List<DataCenter>();
            temp.Add(new DataCenter() { Name = "CN", DisplayName = "China", IsCompleted = true, IsSelected = true, IsRoot = true });
            temp.Add(new DataCenter() { Name = "HK", DisplayName = "HongKong", IsCompleted = false, IsSelected = false });
            temp.Add(new DataCenter() { Name = "US", DisplayName = "America", IsCompleted = false, IsSelected = false });
            return temp;
        }

        readonly Type[] _numberType = new Type[] { typeof(int), typeof(float), typeof(double), typeof(decimal) };

        public List<Dictionary<string, object>> GetLighthouseItems(ApiCall apiCall)
        {
            var result = new List<Dictionary<string, object>>();
            var lighthouseItems = Manger.List();

            foreach (var lighthouseItem in lighthouseItems)
            {
                var lighthouseItemDic = new Dictionary<string, object>() {
                    { nameof(lighthouseItem.Name),lighthouseItem.Name },
                    { nameof(lighthouseItem.Description),lighthouseItem.Description },
                };

                var type = lighthouseItem.GetType();
                var settingType = Lib.Reflection.TypeHelper.GetGenericType(type);

                if (settingType != null)
                {
                    var settingDic = new List<Dictionary<string, object>>();
                    var instance = Activator.CreateInstance(settingType);
                    var properties = settingType.GetProperties();

                    foreach (var item in properties)
                    {
                        var description = item.CustomAttributes.FirstOrDefault(f => f.AttributeType == typeof(DescriptionAttribute))?.ConstructorArguments.FirstOrDefault();

                        var controlType = item.PropertyType == typeof(bool) ? "Switch" : _numberType.Contains(item.PropertyType) ? "Number" : "Text";

                        settingDic.Add(new Dictionary<string, object>
                        {
                            {  "Name", item.Name},
                            {  "DisplayName", description?.Value ?? item.Name},
                            {  "Value", item.GetValue(instance)},
                            {  "ControlType", controlType},
                        });
                    }

                    lighthouseItemDic.Add("Setting", settingDic);
                }
                else
                {
                    lighthouseItemDic.Add("Setting", null);
                }

                result.Add(lighthouseItemDic);
            }

            return result;
        }
    }

}
