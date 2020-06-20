//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
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
    public class TopPages : IDashBoard
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

            model.Model = getTopPages(Context);

            model.Link = "/_Admin/System/VisitorLogs?SiteId=" + Context.WebSite.Id.ToString() + "#TopPages";
            return model;
        }


        public List<ResourceCount> getTopPages(RenderContext Context)
        {
            var sitedb = Context.WebSite.SiteDb();

            List<ResourceCount> pagecountes = new List<ResourceCount>();

            List<TempCounter> temp = new List<TempCounter>(); 

            var logs = DashBoardHelper.GetLogs(Context);

            foreach (var item in logs.GroupBy(o => o.ObjectId))
            { 
                TempCounter one = new TempCounter();
                one.Item = item.First(); 
                one.Count = item.Count();
                one.ObjectId = item.Key;

                temp.Add(one);  

            }

            int counter = 0; 
            foreach (var item in temp.OrderByDescending(o=>o.Count))
            { 
                var page = sitedb.Pages.Get(item.ObjectId, true);
                if (page != null)
                {
                    ResourceCount count = new ResourceCount();
                    var pageurl = ObjectService.GetObjectRelativeUrl(sitedb, item.ObjectId, ConstObjectType.Page);
                    count.Name = pageurl;
                    count.Count = item.Count;
                    count.Size = item.Size; 
                    pagecountes.Add(count);

                    counter += 1; 
                    if (counter>=3)
                    {
                        return pagecountes; 
                    }
                }  
            }

            return pagecountes; 
        }

    }

    public class TempCounter
    {
        public Guid ObjectId { get; set; }

        public int Count { get; set; }

        public long Size { get; set; }

        public VisitorLog Item { get; set; }

    }

}
