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

        public void CleanRepository(ApiCall call)
        {
            // 
        }

        public void CleanLog(ApiCall call)
        {

        }

    }
}
