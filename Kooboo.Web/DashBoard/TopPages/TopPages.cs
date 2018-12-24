//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.DashBoard.TopPages
{
    public class TopPages :IDashBoard
    {
        public string Name
        {
            get
            {
                return "Top Pages"; 
            }
        }

        public string DisplayName(RenderContext Context)
        {
            return Data.Language.Hardcoded.GetValue("Top Pages", Context);  
        }

        public IDashBoardResponse Render(RenderContext Context)
        {
            DashBoardResponseModel model = new DashBoardResponseModel();
            var sitedb = Context.WebSite.SiteDb(); 
            model.Model = Kooboo.Sites.Service.VisitorLogService.TopPages(sitedb).OrderByDescending(o=>o.Count).Take(3);

            model.Link = "/_Admin/System/VisitorLogs?SiteId="+Context.WebSite.Id.ToString()+"#TopPages"; 
            return model; 
        }
    }
}
