using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return true;
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
                var sites = Data.GlobalDb.WebSites.AllSites.Where(o => o.Value.OrganizationId == call.Context.User.CurrentOrgId).ToList();

                foreach (var item in sites)
                {
                    var site = item.Value;
                    var db = site.SiteDb();
                    var last = db.Log.Store.LastKey;
                    version.SiteVersions.Add(site.Id, last.ToString());
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

            url = url.TrimStart('/');
            string path = url.Replace("/", "\\");

            string fullpath = System.IO.Path.Combine(root, path);

            if (System.IO.File.Exists(fullpath))
            {
                return System.IO.File.ReadAllText(fullpath);
            }
            return null;
        }
    }

    public class SystemVersion
    {
        public string Admin { get; set; }

        public Dictionary<Guid, string> SiteVersions { get; set; }
    }
}
