//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Web.Api.Implementation
{
    public class ExternalResourceApi : SiteObjectApi<ExternalResource>
    {
        public override List<object> List(ApiCall call)
        {
            List<ExternalResourceItemViewModel> result = new List<ExternalResourceItemViewModel>();
            var sitedb = call.WebSite.SiteDb();

            foreach (var item in sitedb.ExternalResource.All())
            {
                ExternalResourceItemViewModel model = new ExternalResourceItemViewModel
                {
                    Id = item.Id,
                    FullUrl = item.FullUrl,
                    ResourceType = ConstTypeContainer.GetName(item.DestinationObjectType),
                    LastModified = item.LastModified,
                    Relations = Sites.Helper.RelationHelper.Sum(sitedb.ExternalResource.GetUsedBy(item.Id))
                };


                result.Add(model);
            }

            return result.ToList<object>();
        }
    }
}