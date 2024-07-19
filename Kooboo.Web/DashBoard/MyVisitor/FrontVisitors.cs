//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.DashBoard.MyVisitor
{

    public class FrontVisitors : IDashBoard
    {
        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Visitor", context);
        }

        public string Name
        {
            get
            {
                return "Visitors";
            }
        }

        public IDashBoardResponse Render(RenderContext Context)
        {
            var sitedb = Context.WebSite.SiteDb();

            var logs = DashBoardHelper.GetLogs(Context);

            SiteVisitorModel model = new SiteVisitorModel();
            if (logs.Count() > 0)
            {
                model.Total = logs.Count();
                model.Ips = logs.GroupBy(o => o.ClientIP).Count();
                model.Pages = logs.GroupBy(o => o.ObjectId).Count();
                model.AvgSize = (int)(logs.Average(o => o.Size) / 1024);
            }
            Kooboo.Data.Models.DashBoardResponseModel response = new Data.Models.DashBoardResponseModel();
            response.Model = model;
            return response;
        }
    }

    public class SiteVisitorModel
    {
        public int Total { get; set; }
        public int Ips { get; set; }

        public int Pages { get; set; }

        public int AvgSize { get; set; }
    }


}
