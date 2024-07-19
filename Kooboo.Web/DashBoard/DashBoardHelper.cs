using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;

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

            var list = sitedb.VisitorLog.Take(1000);

            if (list != null)
            {
                return list.ToList();
            }

            return new List<VisitorLog>();
        }
    }
}
