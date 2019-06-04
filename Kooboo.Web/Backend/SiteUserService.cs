using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Backend
{
   public static  class SiteUserService
    { 

        public static Kooboo.Sites.Models.SiteUser GetSiteUser(RenderContext context)
        { 
            var sitedb = context.WebSite.SiteDb();

            return sitedb.SiteUser.Get(context.User.Id); 
        }

        public static Kooboo.Sites.Authorization.Model.RolePermission GetRolePermission(RenderContext context)
        {
            if (context.User == null)
            {
                return null; 
            }
            var sitedb = context.WebSite.SiteDb();

            var user = sitedb.SiteUser.Get(context.User.Id); 

            if (user  ==null)
            {
                return null; 
            }

            var role = sitedb.GetSiteRepository<Kooboo.Sites.Authorization.Model.RolePermissionRepository>().Get(user.SiteRole);

            return role; 
        }


    }




}
