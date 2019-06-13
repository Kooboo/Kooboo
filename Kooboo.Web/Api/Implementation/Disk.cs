//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.ViewModel;

namespace Kooboo.Web.Api.Implementation
{
    public class DiskApi : IApi
    {
        public string ModelName
        {
            get
            {
                return "Disk"; 
            }
        }

        public bool RequireSite
        {
            get
            {
                return true; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return true; 
            }
        }

        public DiskSize List(ApiCall call)
        {
            return call.WebSite.SiteDb().GetSize(); 
        }
        public DiskSize Size(ApiCall call)
        {
            return call.WebSite.SiteDb().GetSize();
        }

        public void CleanRepo(string repo, ApiCall call)
        {
            var sitedb = call.Context.WebSite.SiteDb();

            var repository = sitedb.GetRepository(repo); 
  
        }

        public void CleanLog(ApiCall call)
        {

        }

    }
}
