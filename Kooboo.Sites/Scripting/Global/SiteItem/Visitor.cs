using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class Visitor
    {

        [KIgnore]
        internal RenderContext context { get; set; }

        internal SiteDb siteDb { get; set; }

        public Visitor(RenderContext Context)
        {
            this.context = Context;
            this.siteDb = context.WebSite.SiteDb();
        }

        public List<VisitorLog> Top(int count)
        {
            return siteDb.VisitorLog.Take(false, 0, count);
        }

        public VisitorLog[] thisWeek(int count)
        {
            var weekname = siteDb._GetWeekName(DateTime.Now);
            var log = siteDb.LogByWeek<VisitorLog>(weekname);
            return log.Take(false, 0, count).ToArray();
        }

        [Description(@"Weekname in the format of year + week number, for example: 2020-12")]
        public VisitorLog[] ByWeek(string weekname, int count)
        {
            var log = siteDb.LogByWeek<VisitorLog>(weekname);
            if (log != null)
            {
                return log.Take(false, 0, count).ToArray();
            } 
            return null;
        }
    }
} 