//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualFile;

namespace Kooboo.Web.Api.Implementation
{
    public class SystemApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "system";
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
                return false;
            }
        }

        public SystemVersion Version(ApiCall call)
        {
            Guid SiteId = call.GetValue<Guid>("SiteId");

            SystemVersion version = new SystemVersion();
            version.Admin = Data.AppSettings.Version.ToString();
            version.SiteVersions = new Dictionary<Guid, string>();

            if (SiteId != default(Guid))
            {
                var site = Data.GlobalDb.WebSites.Get(SiteId);
                if (site == null || site.OrganizationId != call.Context.User.CurrentOrgId)
                {
                    throw new Exception(Data.Language.Hardcoded.GetValue("User does not own website", call.Context));
                }
                var db = site.SiteDb();
                var last = db.Log.Store.LastKey;
                version.SiteVersions.Add(site.Id, last.ToString());
            }
            else
            {
                if (call.Context.User != null)
                {
                    var sites = Data.GlobalDb.WebSites.AllSites.Where(o => o.Value.OrganizationId == call.Context.User.CurrentOrgId).ToList();

                    foreach (var item in sites)
                    {
                        var site = item.Value;
                        var db = site.SiteDb();
                        var last = db.Log.Store.LastKey;
                        version.SiteVersions.Add(site.Id, last.ToString());
                    }
                }
            }
            return version;

        }

        [Kooboo.Attributes.RequireModel(typeof(List<string>))]
        public Dictionary<string, string> LoadFile(ApiCall call)
        {
            var urls = call.Context.Request.Model as List<string>;

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in urls)
            {
                var jsstring = GetString(item);
                result.Add(item, jsstring);
            }

            return result;
        }


        public string LoadOneJs(string url, ApiCall call)
        {
            return GetString(url);
        }

        private string GetString(string url)
        {
            string root = Kooboo.Data.AppSettings.RootPath;

            string fullpath = Lib.Compatible.CompatibleManager.Instance.System.CombinePath(root, url);

            if (VirtualResources.FileExists(fullpath))
            {
                return VirtualResources.ReadAllText(fullpath);
            }
            else
            {
                fullpath = Kooboo.Render.Controller.ModuleFile.FindFile(fullpath);

                if (!string.IsNullOrWhiteSpace(fullpath) && VirtualResources.FileExists(fullpath))
                {
                    return VirtualResources.ReadAllText(fullpath);
                }
            }

            return null;
        }

        public Dictionary<string, string> Info()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var apires = Kooboo.Data.AppSettings.ApiResource;

            if (apires != null)
            {
                result.Add("account", apires.AccountUrl);
                result.Add("resource", apires.ThemeUrl);
                result.Add("converter", apires.ConvertUrl);
            }
            else
            {
                result.Add("ApiResource", null);
            }

            if (Data.AppSettings.ServerSetting != null)
            {
                result.Add("setting", Lib.Helper.JsonHelper.Serialize(Data.AppSettings.ServerSetting));
            }

            return result;
        }
    }

    public class SystemVersion
    {
        public string Admin { get; set; }

        public Dictionary<Guid, string> SiteVersions { get; set; }
    }
}
