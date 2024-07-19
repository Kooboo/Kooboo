//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Lib.GeoLocation;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Render;
using Kooboo.Sites.Service;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Web.Security
{
    public static class AccessControl
    {
        //   types.Add("p",  ("public", call.Context));
        //   types.Add("o",  ("organization", call.Context));
        //   types.Add("m",  ("site user", call.Context));
        //   types.Add("u",  ("login user", call.Context));
        static readonly MemoryCache _devPasswordCache = new(new MemoryCacheOptions());

        public static bool HasCountryAccess(WebSite site, RenderContext context)
        {
            var ip = context.Request.IP;
            if (!AppSettings.IsOnlineServer || !site.EnableVisitorCountryRestriction || site.VisitorCountryRestrictions == null || string.IsNullOrEmpty(ip))
            {
                return true;
            }

            var country = IPLocation.instance.GetCountryCode(ip);
            if (country == null)
            {
#if DEBUG
                country = "cn";
# else
                return true;
#endif
            }

            if (site.VisitorCountryRestrictions.TryGetValue(country, out var _))
            {
                return false;
            }

            return true;
        }

        public static bool HasWebsiteAccess(WebSite site, RenderContext context)
        {
            switch (site.SiteType)
            {
                // public
                case Data.Definition.WebsiteType.p:
                    return true;
                case Data.Definition.WebsiteType.u:
                    {
                        if (IsPathException(site, context))
                        {
                            return true;
                        }

                        return context.User != null;
                    }
                case Data.Definition.WebsiteType.o:
                    {
                        if (IsPathException(site, context))
                        {
                            return true;
                        }

                        var user = context.User;
                        if (user == null)
                        { return false; }

                        if (user.Id == site.OrganizationId)
                        {
                            return true;
                        }

                        return Kooboo.Data.Cache.OrganizationUserCache.HasUser(site.OrganizationId, user.Id);
                    }

                case Data.Definition.WebsiteType.m:
                    {
                        if (IsPathException(site, context))
                        {
                            return true;
                        }

                        var siteUser = context.User;
                        if (siteUser == null)
                        { return false; }

                        if (siteUser.Id == site.OrganizationId)
                        {
                            return true;
                        }

                        var allUsers = site.SiteDb().SiteUser.All();
                        if (allUsers.Find(o => o.Id == siteUser.Id) != null)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    break;
            }

            return true;

        }

        public static bool IsPathException(WebSite site, RenderContext context)
        {
            //if (site.WhiteListPath != null && site.WhiteListPath.Any())
            //{
            //    var path = context.Request.RelativeUrl.ToLower();

            //    foreach (var item in site.WhiteListPath)
            //    {
            //        var lower = item.ToLower();
            //        if (path.StartsWith(lower))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;

            if (site.SpecialPath != null && site.SpecialPath.Any())
            {
                var path = context.Request.RelativeUrl.ToLower();

                if (!site.IncludePath)
                {
                    foreach (var item in site.SpecialPath)
                    {
                        if (IsMatch(item, path))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    foreach (var item in site.SpecialPath)
                    {
                        if (IsMatch(item, path))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        public static bool HasServerAccess(RenderContext context)
        {
            if (Data.AppSettings.AllowUsers != null && Data.AppSettings.AllowUsers.Any())
            {
                if (context.User != null)
                {
                    if (Data.AppSettings.AllowUsers.Contains(context.User.Id) || Data.AppSettings.AllowUsers.Contains(AppSettings.AllowAnyId))
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



        // pather with *. 
        public static bool IsMatch(string pattern, string testString)
        {
            try
            {
                pattern = pattern.Trim();
                if (!pattern.StartsWith('^')) pattern = '^' + pattern;
                if (!pattern.EndsWith('$')) pattern += '$';
                return Regex.IsMatch(testString, pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            }
            catch (System.Exception)
            {
                return false;
            }

        }




        #region  DEV Password

        public static bool RequireDevPassword(FrontContext context)
        {
            try
            {
                if (context?.WebSite?.SiteType != Data.Definition.WebsiteType.p) return false;

                var status = context?.WebSite?.Status ?? WebSite.SiteStatus.Published;

                //if (status == WebSite.SiteStatus.Development || status == WebSite.SiteStatus.Auditing)
                //{
                //var key = GetDevPasswordKey(context.RenderContext);
                //if (!_devPasswordCache.TryGetValue(key, out _))
                //{
                var body = WebSiteService.RenderCustomErrorPage(context, DataConstants.DevPassword).Result;
                context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(body);
                return true;
                // }
                //  }
            }
            catch { }

            return false;
        }

        public static bool ValidDevAccess(RenderContext context)
        {
            if (context.Request.Body == context.WebSite.DevPassword)
            {
                GrantDevAccess(context);
                return true;
            }

            return false;
        }

        public const string Dev_Cookie_key = "kooboo-dev-access";

        public static void GrantDevAccess(RenderContext context)
        {
            context.Response.AppendCookie(Dev_Cookie_key, "true", 180);
        }

        private static string GetDevPasswordKey(RenderContext context, Guid siteId = default)
        {
            if (siteId == default)
            {
                siteId = context?.WebSite?.Id ?? Guid.Empty;
            }
            var ip = context?.Request?.IP;

            if (ip == "0.0.0.1")
            {
                ip = "127.0.0.1";
            }

            return $"{ip}_{siteId}";
        }


        #endregion
    }
}
