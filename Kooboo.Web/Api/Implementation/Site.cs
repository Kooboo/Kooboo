//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Sites.Sync;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            Dictionary<string, string> types = new Dictionary<string, string>
            {
                {"p", Data.Language.Hardcoded.GetValue("public", call.Context)},
                {"o", Data.Language.Hardcoded.GetValue("private", call.Context)},
                {"m", Data.Language.Hardcoded.GetValue("member", call.Context)}
            };
            return types;
        }

        public SiteCultureViewModel Langs(ApiCall request)
        {
            SiteCultureViewModel viewmodel = new SiteCultureViewModel();
            string strid = request.GetValue("SiteId");
            if (string.IsNullOrEmpty(strid))
            {
                strid = request.GetValue("id");
            }

            if (Guid.TryParse(strid, out var SiteId))
            {
                var site = GlobalDb.WebSites.Get(SiteId);
                if (site != null)
                {
                    var cultures = Kooboo.Data.Language.SiteCulture.List(SiteId);

                    string defaultname = site.DefaultCulture;

                    if (Data.Language.LanguageSetting.ISOTwoLetterCode.ContainsKey(site.DefaultCulture))
                    {
                        defaultname = Data.Language.LanguageSetting.ISOTwoLetterCode[site.DefaultCulture];
                    }

                    Dictionary<string, string> sitecultures = new Dictionary<string, string>();

                    if (site.EnableMultilingual)
                    {
                        if (site.Culture != null && site.Culture.Any())
                        {
                            foreach (var item in site.Culture)
                            {
                                sitecultures.Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            foreach (var item in site.Culture.Keys.ToList())
                            {
                                if (cultures.ContainsKey(item))
                                {
                                    sitecultures[item] = cultures[item];
                                }
                            }
                        }
                    }
                    else
                    {
                        sitecultures.Add(site.DefaultCulture, defaultname);
                    }

                    viewmodel.Default = site.DefaultCulture;
                    viewmodel.DefaultName = defaultname;
                    viewmodel.Cultures = sitecultures;

                    return viewmodel;
                }
            }
            return null;
        }

        public Dictionary<string, string> Cultures(ApiCall call)
        {
            return Kooboo.Data.Language.SiteCulture.List(call.WebSite.Id);
        }

        public List<SiteSummaryViewModel> List(ApiCall apiCall)
        {
            var user = apiCall.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }

            var sites = Kooboo.Sites.Service.WebSiteService.ListByUser(user);

            List<SiteSummaryViewModel> result = new List<SiteSummaryViewModel>();

            foreach (var item in sites)
            {
                var sitedb = item.SiteDb();

                SiteSummaryViewModel summary = new SiteSummaryViewModel
                {
                    SiteId = item.Id,
                    SiteName = item.Name,
                    SiteDisplayName = item.DisplayName,
                    PageCount = sitedb.Pages.Count(),
                    ImageCount = sitedb.Images.Count(),
                    Online = item.Published,
                    Visitors = sitedb.VisitorLog.QueryDescending(o => true)
                        .EndQueryCondition(o => o.Begin < DateTime.UtcNow.AddHours(-12)).Count()
                };
                // if user has not right to access the site. present the preview link.


                var alltask = sitedb.TransferTasks.All();

                if (alltask != null && alltask.Any())
                {
                    foreach (var ttask in alltask.Where(o => o.done == false))
                    {
                        if (ttask.CreationDate > DateTime.UtcNow.AddMinutes(-2))
                        {
                            summary.InProgress = true;
                            break;
                        }
                    }
                }

                summary.HomePageLink = item.BaseUrl();
                result.Add(summary);
            }
            return result;
        }

        public virtual BinaryResponse Export(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }
            var exportfile = ImportExport.ExportInter(site.SiteDb());
            var path = System.IO.Path.GetFullPath(exportfile);

            if (File.Exists(exportfile))
            {
                var allbytes = System.IO.File.ReadAllBytes(path);

                BinaryResponse response = new BinaryResponse {ContentType = "application/zip"};
                response.Headers.Add("Content-Disposition", $"attachment;filename={site.Name}.zip");
                response.BinaryBytes = allbytes;
                return response;
            }
            return null;
        }

        [Attributes.RequireParameters("stores")]
        public virtual BinaryResponse ExportStore(ApiCall call)
        {
            var site = call.WebSite;
            if (site == null)
            {
                return null;
            }

            var storevalue = call.GetValue("stores");

            var stores = storevalue.Split(',').ToList();

            var exportfile = ImportExport.ExportInterSelected(site.SiteDb(), stores);
            var path = System.IO.Path.GetFullPath(exportfile);

            if (File.Exists(exportfile))
            {
                var allbytes = System.IO.File.ReadAllBytes(path);

                BinaryResponse response = new BinaryResponse {ContentType = "application/zip"};
                response.Headers.Add("Content-Disposition", $"attachment;filename={site.Name}.zip");
                response.BinaryBytes = allbytes;
                return response;
            }
            return null;
        }

        public List<ExportStoreNameViewModel> ExportStoreNames(ApiCall call)
        {
            List<ExportStoreNameViewModel> names = new List<ExportStoreNameViewModel>
            {
                new ExportStoreNameViewModel()
                {
                    Name = "Page", DisplayName = Hardcoded.GetValue("Page", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "View", DisplayName = Hardcoded.GetValue("View", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Layout", DisplayName = Hardcoded.GetValue("Layout", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Image", DisplayName = Hardcoded.GetValue("Image", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Script", DisplayName = Hardcoded.GetValue("Script", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Style", DisplayName = Hardcoded.GetValue("Style", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "TextContent", DisplayName = Hardcoded.GetValue("TextContent", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "ContentType", DisplayName = Hardcoded.GetValue("ContentType", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "ContentFolder", DisplayName = Hardcoded.GetValue("ContentFolder", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "HtmlBlock", DisplayName = Hardcoded.GetValue("HtmlBlock", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Label", DisplayName = Hardcoded.GetValue("Label", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Menu", DisplayName = Hardcoded.GetValue("Menu", call.Context)
                },
                new ExportStoreNameViewModel()
                {
                    Name = "Storage", DisplayName = Hardcoded.GetValue("Storage", call.Context)
                }
            };

            return names;
        }

        public void SwitchStatus(ApiCall call)
        {
            var site = Data.GlobalDb.WebSites.Get(call.ObjectId);
            if (site != null)
            {
                site.Published = !site.Published;
            }
            Data.GlobalDb.WebSites.AddOrUpdate(site);
        }

        public void Preview(ApiCall call, Guid siteId)
        {
            var site = Kooboo.Data.GlobalDb.WebSites.Get(siteId);
            if (site != null)
            {
                var baseurl = site.BaseUrl();

                if (!string.IsNullOrEmpty(baseurl))
                {
                    call.Context.Response.Redirect(301, baseurl);
                }
            }
        }

        public WebSite Get(ApiCall call)
        {
            string strsiteid = call.GetValue("siteid");
            if (!string.IsNullOrEmpty(strsiteid))
            {
                if (System.Guid.TryParse(strsiteid, out var siteid))
                {
                    return GlobalDb.WebSites.Get(siteid);
                }
            }
            return null;
        }

        [Kooboo.Attributes.RequireModel(typeof(SiteUpdate))]
        public void Post(ApiCall call)
        {
            var siteid = call.GetValue<Guid>("siteid");

            var currentsite = Kooboo.Data.GlobalDb.WebSites.Get(siteid);

            bool shouldinitDisk = false;

            if (currentsite != null)
            {
                var newinfo = call.Context.Request.Model as SiteUpdate;

                if (!currentsite.EnableDiskSync && newinfo.EnableDiskSync)
                {
                    shouldinitDisk = true;
                }

                currentsite.DiskSyncFolder = newinfo.DiskSyncFolder;
                currentsite.DisplayName = newinfo.DisplayName;

                currentsite.EnableVisitorLog = newinfo.EnableVisitorLog;
                currentsite.EnableConstraintFixOnSave = newinfo.EnableConstraintFixOnSave;
                currentsite.EnableFrontEvents = newinfo.EnableFrontEvents;
                currentsite.EnableDiskSync = newinfo.EnableDiskSync;
                currentsite.EnableMultilingual = newinfo.EnableMultilingual;

                currentsite.CustomSettings = newinfo.CustomSettings;
                currentsite.Culture = newinfo.Culture;
                currentsite.SitePath = newinfo.SitePath;
                currentsite.EnableSitePath = newinfo.EnableSitePath;
                currentsite.DefaultCulture = newinfo.DefaultCulture;
                currentsite.AutoDetectCulture = newinfo.AutoDetectCulture;
                currentsite.ForceSSL = newinfo.ForceSSL;

                currentsite.SiteType = newinfo.SiteType;

                // the cluster...

                GlobalDb.WebSites.AddOrUpdate(currentsite);
            }

            if (shouldinitDisk)
            {
                WebSiteService.InitDiskSync(currentsite, true);
            }
        }

        public bool Delete(ApiCall call)
        {
            Guid siteId = call.GetGuidValue("SiteId");

            if (siteId == default(Guid) && call.ObjectId != default(Guid))
            {
                siteId = call.ObjectId;
            }

            if (siteId != default(Guid))
            {
                WebSiteService.Delete(siteId, call.Context.User);
                return true;
            }
            return false;
        }

        public virtual bool Deletes(ApiCall call)
        {
            string json = call.GetValue("ids");
            if (string.IsNullOrEmpty(json))
            {
                json = call.Context.Request.Body;
            }

            List<Guid> ids = Lib.Helper.JsonHelper.Deserialize<List<Guid>>(json);

            if (ids != null && ids.Any())
            {
                foreach (var item in ids)
                {
                    WebSiteService.Delete(item, call.Context.User);
                }
                return true;
            }
            return false;
        }

        public SiteDiskSyncViewModel DiskSyncGet(ApiCall apiCall)
        {
            SiteDiskSyncViewModel model = new SiteDiskSyncViewModel
            {
                Folder = apiCall.WebSite.SiteDb().WebSite.DiskSyncFolder,
                EnableDiskSync = apiCall.WebSite.SiteDb().WebSite.EnableDiskSync
            };


            if (model.EnableDiskSync)
            {
                model.DiskFileCount = IOHelper.CountFiles(model.Folder);
            }

            return model;
        }

        [Kooboo.Attributes.RequireParameters("localpath")]
        public void DiskSyncUpdate(ApiCall call)
        {
            Data.Models.WebSite website = null;

            Guid siteId = call.GetGuidValue("SiteId");
            website = Kooboo.Data.GlobalDb.WebSites.Get(siteId);
            if (siteId == default(Guid) || website == null)
            {
                return;
            }

            bool enable = call.GetBoolValue("EnableDiskSync");
            string path = call.GetValue("localpath");

            bool hasSamePath = Lib.Helper.StringHelper.IsSameValue(website.DiskSyncFolder, path);

            if (website.EnableDiskSync != enable || Lib.Helper.StringHelper.IsSameValue(website.DiskSyncFolder, path) == false)
            {
                website.EnableDiskSync = enable;
                website.DiskSyncFolder = path;
                GlobalDb.WebSites.AddOrUpdate(website);
            }

            if (enable)
            {
                // init disk..
                if (!hasSamePath)
                {
                    DiskSyncFolderWatcher.StopDiskWatcher(website);
                    DiskSyncFolderWatcher.StartDiskWatcher(website);
                }
                WebSiteService.InitDiskSync(website, true);
            }
        }

        public WebSite Create(ApiCall call)
        {
            Sites.DataSources.DataSourceHelper.InitIDataSource();
            string fulldomain = call.GetValue("FullDomain");
            if (string.IsNullOrEmpty(fulldomain))
            {
                string rootDomain = call.GetValue("RootDomain");
                string subDomain = call.GetValue("SubDomain");
                fulldomain = subDomain + "." + rootDomain;
            }
            string sitename = call.GetValue("SiteName");

            if (string.IsNullOrEmpty(sitename) || string.IsNullOrEmpty(fulldomain))
            {
                return null;
            }

            WebSite newsite = Kooboo.Sites.Service.WebSiteService.AddNewSite(call.Context.User.CurrentOrgId, sitename, fulldomain, call.Context.User.Id);
            return newsite;
        }

        [Kooboo.Attributes.RequireParameters("rootdomain")]
        public bool CheckDomainBindingAvailable(ApiCall call)
        {
            string rootDomain = call.GetValue("RootDomain");
            string subdomain = call.GetValue("SubDomain");

            if (!string.IsNullOrEmpty(subdomain) && subdomain.ToLower() == "local" && rootDomain.ToLower() == "kooboo")
            {
                return false;
            }

            var bindings = GlobalDb.Bindings.GetByDomain(rootDomain);
            foreach (var item in bindings)
            {
                if (Lib.Helper.StringHelper.IsSameValue(item.SubDomain, subdomain))
                {
                    return false;
                }
            }
            return true;
        }

        public Guid ImportSite(ApiCall call)
        {
            var formresult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(call.Context.Request.PostData);

            if (formresult.Files.Count == 0)
            {
                return default(Guid);
            }

            string rootDomain = null;
            string subDomain = null;
            string siteName = null;
            if (formresult.FormData.ContainsKey("RootDomain"))
            {
                rootDomain = formresult.FormData["RootDomain"];
            }
            if (formresult.FormData.ContainsKey("SubDomain"))
            {
                subDomain = formresult.FormData["SubDomain"];
            }
            if (formresult.FormData.ContainsKey("SiteName"))
            {
                siteName = formresult.FormData["SiteName"];
            }

            if (string.IsNullOrEmpty(siteName) || string.IsNullOrEmpty(rootDomain))
            {
                return default(Guid);
            }

            string fulldomain = string.IsNullOrEmpty(subDomain) ? rootDomain : subDomain + "." + rootDomain;

            var newsite = ImportExport.ImportZip(new MemoryStream(formresult.Files[0].Bytes), call.Context.User.CurrentOrgId, siteName, fulldomain, call.Context.User.Id);

            return newsite.Id;
        }

        public bool IsUniqueName(ApiCall apiCall)
        {
            var sitename = apiCall.GetValue("SiteName", "Name");
            return Kooboo.Data.GlobalDb.WebSites.CheckNameAvailable(sitename, apiCall.Context.User.CurrentOrgId);
        }

        public bool IsSubdomainAvailable(ApiCall apiCall)
        {
            string rootDomain = apiCall.GetValue("RootDomain");
            string subDomain = apiCall.GetValue("SubDomain");
            string fulldomain = subDomain + "." + rootDomain;

            var site = Kooboo.Data.GlobalDb.Bindings.GetByFullDomain(fulldomain);
            return site == null;
        }

        public String GetName(ApiCall call)
        {
            string strsiteid = call.GetValue("siteid");
            if (!string.IsNullOrEmpty(strsiteid))
            {
                if (System.Guid.TryParse(strsiteid, out var siteid))
                {
                    var site = GlobalDb.WebSites.Get(siteid);
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

            List<DataCenter> temp = new List<DataCenter>
            {
                new DataCenter()
                {
                    Name = "CN",
                    DisplayName = "China",
                    IsCompleted = true,
                    IsSelected = true,
                    IsRoot = true
                },
                new DataCenter() {Name = "HK", DisplayName = "HongKong", IsCompleted = false, IsSelected = false},
                new DataCenter() {Name = "US", DisplayName = "America", IsCompleted = false, IsSelected = false}
            };
            return temp;
        }
    }
}