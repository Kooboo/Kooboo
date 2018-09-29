using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Security
{
    public static class ActionControl
    {
        //online server must has max download pages control. otherwise, too many pages may be downloaded. 
        public static bool CanServerDownloadMorePages(SiteDb sitedb, string relativeurl)
        {
            var filetype = Lib.Helper.UrlHelper.GetFileType(relativeurl);
            if (filetype == Lib.Helper.UrlHelper.UrlFileType.PageOrView)
            {
                var allpages = sitedb.TransferPages.All();  

                foreach (var item in allpages.GroupBy(o => o.taskid))
                {
                    var pages = item.ToList();
                    if (pages.Count > 15)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
