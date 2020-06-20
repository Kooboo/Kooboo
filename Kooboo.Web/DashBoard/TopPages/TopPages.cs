//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Service;

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
            model.Model = getTopPages(sitedb).OrderByDescending(o=>o.Count).Take(3);

            model.Link = "/_Admin/System/VisitorLogs?SiteId="+Context.WebSite.Id.ToString()+"#TopPages"; 
            return model; 
        }
         

        public   List<ResourceCount> getTopPages(SiteDb sitedb)
        {
            List<ResourceCount> pagecountes = new List<ResourceCount>();

            var logs =  sitedb.VisitorLog.Take(false, 0, 300);
             
            foreach (var item in logs.GroupBy(o => o.ObjectId))
            {
                var objectid = item.Key;
                var page = sitedb.Pages.Get(objectid, true);
                if (page != null)
                {
                    ResourceCount count = new ResourceCount();
                    var pageurl = ObjectService.GetObjectRelativeUrl(sitedb, objectid, ConstObjectType.Page);
                    count.Name = pageurl;
                    count.Count = item.Count();
                    count.Size = item.First().Size;
                    pagecountes.Add(count);
                }
            }

            return pagecountes.OrderByDescending(o => o.Count).ToList();
        }


    }
}
