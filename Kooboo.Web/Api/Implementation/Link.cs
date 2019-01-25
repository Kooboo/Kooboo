//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation
{
    public class Link : IApi
    {
        public string ModelName
        {
            get
            {
                return "link";
            }
        }

        public bool RequireSite
        {
            get
            {
                return true; 
            }
        }

        public bool RequireUser
        {
            get
            {
                return true; 
            }
        }

        public  LinkViewModel All(ApiCall call)
        {
            LinkViewModel model = new LinkViewModel(); 
             
            var allpageroutes = call.WebSite.SiteDb().Routes.GetByType(ConstObjectType.Page);

            foreach (var item in allpageroutes)
            {
                if (item.objectId != default(Guid))
                {
                    LinkItem linkitem = new LinkItem();
                    linkitem.Url = item.Name;
                    linkitem.Parameters = item.Parameters.Keys.ToList();
                    model.Pages.Add(linkitem);
                }

            }

            var allviews = call.WebSite.SiteDb().Views.All();

            foreach (var item in allviews)
            {
                LinkItem linkitem = new LinkItem();
                linkitem.Url = ObjectService.GetObjectRelativeUrl(call.WebSite.SiteDb(), item);
             //  linkitem.Parameters = Sites.Routing.PageRoute.GetViewParameters(call.SiteDb, item.Id);
                model.Views.Add(linkitem);  
            }

            return model;    

        }
    }
}
