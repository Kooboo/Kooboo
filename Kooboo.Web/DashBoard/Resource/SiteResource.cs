//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.DashBoard.Resource
{
    public class SiteResource : IDashBoard
    {
        public string Name
        {
            get
            {
                return "Resource";
            }
        }

        public string DisplayName(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Resource", context);
        }

        public IDashBoardResponse Render(RenderContext Context)
        {
            var model = new SiteResourceViewModel();
            var sitedb = Context.WebSite.SiteDb();
            model.Images = sitedb.Images.Count();
            model.Contents = sitedb.TextContent.Count();
            model.Pages = sitedb.Pages.Count();
            model.Views = sitedb.Views.Count();

            var result = new DashBoardResponseModel();
            result.Model = model;
            return result;
        }
    }

    public class SiteResourceViewModel
    {
        public int Images { get; set; }
        public int Views { get; set; }
        public int Pages { get; set; }
        public int Contents { get; set; }
    }
}
