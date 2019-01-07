//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using System.Linq;

namespace Kooboo.Web.Security
{ 
    public static class AccessControl
    { 
        public static bool HasWebsiteAccess(WebSite site, RenderContext context)
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
      
        public static bool HasServerAccess(RenderContext context)
        {
            if (Data.AppSettings.AllowUsers !=null && Data.AppSettings.AllowUsers.Any())
            {
                if (context.User !=null)
                {
                    if (Data.AppSettings.AllowUsers.Contains(context.User.Id))
                    {
                        return true; 
                    }
                    else
                    {
                        if (Data.AppSettings.DefaultUser != null && Data.AppSettings.DefaultUser.UserName == context.User.UserName)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            } 
            return true; 
        }
       
    }
}
