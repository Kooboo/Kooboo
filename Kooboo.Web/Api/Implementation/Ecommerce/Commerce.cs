//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Api.Implementation.Ecommerce
{
    public class Commerce : IApi
    {
        public string ModelName => "Commerce";

        public bool RequireSite => false;

        public bool RequireUser => true; 
         

        public List<Kooboo.Web.ViewModel.Ecommerce.CommerceSiteViewModel> list(ApiCall call)
        {

            var user = call.Context.User;
            if (user.CurrentOrgId == default(Guid))
            {
                return null;
            }

            var sites = Kooboo.Sites.Service.WebSiteService.ListByUser(user);

            List<ViewModel.Ecommerce.CommerceSiteViewModel> result = new List<ViewModel.Ecommerce.CommerceSiteViewModel>();

            foreach (var item in sites)
            {
                ViewModel.Ecommerce.CommerceSiteViewModel model = new ViewModel.Ecommerce.CommerceSiteViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.Enable = item.EnableECommerce;  
                result.Add(model);
            }
            return result; 

        }
        
        public void Enable(Guid id, ApiCall call)
        {
            if (id != default(Guid))
            {

            Kooboo.Data.GlobalDb.WebSites.UpdateBoolColumn(id, o => o.EnableECommerce, true);

                 
            } 
        }

        public void EnableSites(List<Guid> ids, ApiCall call)
        {
            foreach(Guid id in ids)
            {
                Kooboo.Data.GlobalDb.WebSites.UpdateBoolColumn(id, o => o.EnableECommerce, true);
            }
        }


        public void Disable(Guid id, ApiCall call)
        {
            if (id != default(Guid))
            {

                Kooboo.Data.GlobalDb.WebSites.UpdateBoolColumn(id, o => o.EnableECommerce, false); 

            }
        }

        public void DisableSites(List<Guid> ids, ApiCall call)
        {
            foreach (Guid id in ids)
            {
                Kooboo.Data.GlobalDb.WebSites.UpdateBoolColumn(id, o => o.EnableECommerce, false);
            }
        }
    }
}
