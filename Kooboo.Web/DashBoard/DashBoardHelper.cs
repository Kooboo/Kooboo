using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.DashBoard
{
    public static class DashBoardHelper
    {

        public static List<VisitorLog> GetLogs(RenderContext context)
        { 
           return context.GetItem<List<VisitorLog>>("_lastlog", Last1000);  
        }

        public static List<VisitorLog> Last1000(RenderContext context)
        {
            var sitedb = context.WebSite.SiteDb();

            var repo = sitedb.LogByWeek<VisitorLog>();
            var list = repo.Take(false, 0, 1000);
            repo.Close();
            return list;
        }
         

    }
}
