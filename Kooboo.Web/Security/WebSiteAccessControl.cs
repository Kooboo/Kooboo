//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Security
{ 
    public static class WebSiteAccessControl
    { 
        public static bool HasAccess(WebSite site, RenderContext context)
        {
            switch (site.SiteType)
            {
                case Data.Definition.WebsiteType.p:
                    return true;
                case Data.Definition.WebsiteType.o:
                    var user = context.User;   
                    if (user == null)
                    {    return false;   } 

                    return Kooboo.Data.Cache.OrganizationUserCache.HasUser(site.OrganizationId, user.Id);

                case Data.Definition.WebsiteType.m:

                    var siteuser = context.User;
                    if (siteuser == null)
                    { return false; }

                    if (siteuser.Id == site.OrganizationId)
                    {
                        return true; 
                    }

                    var allusers = site.SiteDb().SiteUser.All(); 
                    if (allusers.Find(o=>o.Id == siteuser.Id)!=null)
                    {
                        return true; 
                    }
                    else
                    {
                        return false; 
                    }        
                default:
                    break;
            }

            return true; 
   
        }
      
       
    }


}
