//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Api;
using Kooboo.Sites.Models;
using Kooboo.Web.ViewModel;
using Kooboo.Sites.Extensions;
using System.Linq;
using System;
using Kooboo.Data.Models;

namespace Kooboo.Web.Api.Implementation
{
  public  class ExternalResourceApi: SiteObjectApi<ExternalResource>
    { 
        public override List<object> List(ApiCall call)
        { 
            List<ExternalResourceItemViewModel> result = new List<ExternalResourceItemViewModel>();
            var sitedb = call.WebSite.SiteDb();   

            foreach (var item in sitedb.ExternalResource.All())
            {
                ExternalResourceItemViewModel model = new ExternalResourceItemViewModel();
                model.Id = item.Id;
                model.FullUrl = item.FullUrl;
                model.ResourceType = ConstTypeContainer.GetName(item.DestinationObjectType);
                model.LastModified = item.LastModified;

               model.Relations =  Sites.Helper.RelationHelper.Sum(sitedb.ExternalResource.GetUsedBy(item.Id));
 
                result.Add(model); 
            }

            return result.ToList<object>(); 
        } 
 
         
    }
}
