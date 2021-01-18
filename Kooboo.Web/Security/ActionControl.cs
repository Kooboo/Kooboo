//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Repository; 
using System.Linq; 

namespace Kooboo.Web.Security
{
    public static class ActionControl 
    {
        public static int OnlineMaxPage { get; set; } = 200; 

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
                    if (pages.Count > OnlineMaxPage)   // only allow additional download of 20 more pages.. 
                    {
                        return false;
                    }
                }
            }
            return true;
        } 
    }
}
