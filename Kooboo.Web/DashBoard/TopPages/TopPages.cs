//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System.Linq;

namespace Kooboo.Web.DashBoard.TopPages
{
    public class TopPages : IDashBoard
    {
        public string Name
        {
            get
            {
                return "Top Pages";
            }
        }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Top Pages", context);
        }

        public IDashBoardResponse Render(RenderContext context)
        {
            DashBoardResponseModel model = new DashBoardResponseModel();
            var sitedb = context.WebSite.SiteDb();
            model.Model = Kooboo.Sites.Service.VisitorLogService.TopPages(sitedb).OrderByDescending(o => o.Count).Take(3);

            model.Link = "/_Admin/System/VisitorLogs?SiteId=" + context.WebSite.Id.ToString() + "#TopPages";
            return model;
        }
    }
}