//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Service
{
    public static class WebSiteService
    {
        public static string GetBaseUrl(WebSite site, bool ForceSsl = false)
        {
            if (site == null)
            {
                return string.Empty;
            }

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

                if (site.ForceSSL || ForceSsl)
                {
                    var uri = new UriBuilder("https", hostname, 443);
                    return uri.Uri.AbsoluteUri;
                }
                else
                {
                    var uri = new UriBuilder("http", hostname, port);
                    return uri.Uri.AbsoluteUri;
                }
            }

            return "/";
        }


        // This is only for webserver behind NGINX.. 
        public static string GetSiteTempUrl(WebSite site)
        {
            if (Data.AppSettings.IsOnlineServer)
            {
                return GetSiteTempUrl(site, Data.AppSettings.HostDomain); 
            }
            return null;
        }

        public static string GetSiteTempUrl(WebSite site, string HostDomain)
        {
            if (!string.IsNullOrWhiteSpace(HostDomain))
            {
                string url = "_temp_" + site.Id.ToString() + "." + HostDomain;
                return url;
            }
            return null;
        }

        public static Guid GetTempUrlSiteId(string host)
        { 
            if (host.StartsWith("_temp_"))
            {
                var index = host.IndexOf(".");
                if (index > 0)
                {
                    var part = host.Substring(0, index);

                    part = part.Substring("_temp_".Length);  

                    if (Guid.TryParse(part, out Guid result))
                    {
                        return result; 
                    }
                }  
            } 
            return default(Guid); 
        }
         
        public static string EnsureHttpsBaseUrlOnServer(string baseurl, WebSite site)
        {  
            // Online server must use https even behind a NGINX server.  
            if (!string.IsNullOrWhiteSpace(baseurl) && Data.AppSettings.IsOnlineServer)
            {
                if (baseurl.ToLower().StartsWith("http://"))
                {
                    var tempurl = GetSiteTempUrl(site);
                    return tempurl; 
                } 
            }
            return baseurl; 
        }
    }

}
 