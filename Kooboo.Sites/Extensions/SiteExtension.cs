//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Extensions
{
    public static class SiteExtension
    {
        public static SiteDb SiteDb(this WebSite website)
        {
            return Cache.WebSiteCache.GetSiteDb(website);
        }

        public static string FullStartUrl(this WebSite site)
        {
            var binding = Data.GlobalDb.Bindings.GetByWebSite(site.Id).FirstOrDefault();

            string starturl = string.Empty;
            if (binding != null)
            {
                starturl = "http://" + binding.FullName;

                if (AppSettings.CurrentUsedPort != 80 && AppSettings.CurrentUsedPort > 0)
                {
                    starturl = starturl + ":" + AppSettings.CurrentUsedPort;
                }
                starturl = starturl + "/";
                starturl = Lib.Helper.UrlHelper.Combine(starturl, site.StartRoutePath());
            }
            return starturl;
        }

        public static string BaseUrl(this WebSite site, string path = null)
        {
            if (site == null)
            {
                return string.Empty;
            }
            if (path == null)
            {
                path = string.Empty;
            }
            var split = path.Split('?');
            var p = split[0];
            var binding = Data.GlobalDb.Bindings.GetByWebSite(site.Id).FirstOrDefault();
            if (binding != null)
            {
                int port = AppSettings.CurrentUsedPort;
                if (binding.Port > 0)
                {
                    port = binding.Port;
                }
                string hostname = null;
                if (binding.DomainId != default(Guid))
                {
                    hostname = binding.FullName;
                }

                if (string.IsNullOrEmpty(hostname))
                {
                    var ip = Lib.Helper.NetworkHelper.GetLocalIpAddress();
                    if (ip == "127.0.0.1")
                    {
                        hostname = "localhost";
                    }
                    else
                    {
                        hostname = ip;
                    }
                }

                if (site.ForceSSL)
                {
                    var uri = new UriBuilder("https", hostname, 443, p, path.Substring(p.Length));
                    return uri.Uri.AbsoluteUri;
                }
                else
                {
                    var uri = new UriBuilder("http", hostname, port, p, path.Substring(p.Length));
                    return uri.Uri.AbsoluteUri;
                }
            }

            return "/";
        }

        public static List<Page> StartPages(this WebSite site)
        {
            return site.SiteDb().Pages.Query.Where(o => o.DefaultStart == true).SelectAll();
        }

        public static string StartRoutePath(this WebSite site)
        {
            foreach (var item in site.StartPages())
            {
                Route route = site.SiteDb().Routes.Query.Where(o => o.objectId == item.Id).FirstOrDefault();

                if (route != null)
                {
                    return route.Name;
                }
            }

            return string.Empty;
        }
    }
}