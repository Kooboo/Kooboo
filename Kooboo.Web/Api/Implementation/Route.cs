//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api; 
using Kooboo.Sites.Extensions; 
using Kooboo.Sites.Routing;
using Kooboo.Web.ViewModel; 
using System.Collections.Generic;
using System.Linq;
 

namespace Kooboo.Web.Api.Implementation
{
  public  class RouteApi: SiteObjectApi<Route>
    {
        public override List<object> List(ApiCall call)
        {
            List<RouteItemViewModel> result = new List<RouteItemViewModel>();
            var sitedb = call.WebSite.SiteDb();

            foreach (var item in sitedb.Routes.All())
            {
                RouteItemViewModel model = new RouteItemViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.ResourceType = ConstTypeContainer.GetName(item.DestinationConstType);
                model.ObjectId = item.objectId;

                model.LastModified = item.LastModified;

                model.Relations = Sites.Helper.RelationHelper.Sum(sitedb.Routes.GetUsedBy(item.Id));

                result.Add(model);
            }

            return result.ToList<object>();
        } 

    }
}
