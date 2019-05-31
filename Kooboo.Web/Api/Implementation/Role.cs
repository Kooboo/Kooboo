using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; 

namespace Kooboo.Web.Api.Implementation
{
    public class RoleApi : IApi
    {
        public string ModelName => "Role";

        public bool RequireSite => true;

        public bool RequireUser => true; 


        public List<Kooboo.Sites.Authorization.Model.PermissionViewModel> List(ApiCall call)
        {

            var db = call.WebSite.SiteDb();

            var items = db.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().All();
            items.Add(Kooboo.Sites.Authorization.Model.RolePermissionRepository.Master);
            items.Add(Kooboo.Sites.Authorization.Model.RolePermissionRepository.Developer);
            items.Add(Kooboo.Sites.Authorization.Model.RolePermissionRepository.ContentManager);

            return items.Select(o => Sites.Authorization.PermissionTreeHelper.ToViewModel(o)).ToList(); 
        }


        public Kooboo.Sites.Authorization.Model.PermissionViewModel GetEdit(ApiCall call)
        {
            string name = call.GetValue("name"); 
            if (string.IsNullOrWhiteSpace(name))
            {
                return Kooboo.Sites.Authorization.ApiPermission.MasterTemplate(); 
            }
            else
            {
                var db = call.WebSite.SiteDb();

                var item = db.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().Get(name); 
                
                if (item !=null)
                {
                    return Kooboo.Sites.Authorization.PermissionTreeHelper.ToViewModel(item); 
                }
                else
                {
                    return Kooboo.Sites.Authorization.ApiPermission.MasterTemplate();
                } 
            }
        }


        public void Post(ApiCall call, Sites.Authorization.Model.PermissionViewModel model)
        { 
            // save into it..... 
            



        }




    }
}
