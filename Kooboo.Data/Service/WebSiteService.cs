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
                int port = AppSettings.HttpPort;
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
            if (string.IsNullOrEmpty(HostDomain))
            {
                return null;
            }

            string org = null;
            if (Data.AppSettings.IsOnlineServer && Data.AppSettings.ServerSetting.ServerId > 0)
            {
                org = Data.AppSettings.ServerSetting.ServerId.ToString();
            }
            else
            {
                var siteorg = Data.GlobalDb.Organization.Get(site.OrganizationId);
                if (siteorg != null)
                {
                    org = siteorg.Name;
                }
            }

            string url = "_preview_" + site.Id.ToString() + "_" + org + "." + HostDomain;
            return url;
        }


        public static string GetSiteTempUrl(WebSite site, string ServerIdOrOrgName, string HostDomain)
        {
            string url = "_preview_" + site.Id.ToString() + "_" + ServerIdOrOrgName + "." + HostDomain;
            return url;
        }


        public static Guid GetTempUrlSiteId(string host)
        {
            if (host !=null && host.StartsWith("_preview_"))
            {
                var index = host.IndexOf("_", 15);
                if (index > 0)
                {
                    var part = host.Substring(0, index);

                    part = part.Substring("_preview_".Length);

                    if (Guid.TryParse(part, out Guid result))
                    {
                        return result;
                    }
                }
            }
            return default(Guid);
        }

        // used by account. 
        public static string GetSitePreviewServerIdOrOrgName(string host)
        {
            if (host.StartsWith("_preview_"))
            {
                var index = host.IndexOf("_", 15);
                if (index > 0)
                {
                    var part = host.Substring(0, index);

                    part = part.Substring("_preview_".Length);

                    if (Guid.TryParse(part, out Guid result))
                    {
                        var domainindex = host.IndexOf(".", index);
                        if (domainindex > 0)
                        {
                            var ServerOrg = host.Substring(index + 1, domainindex - index - 1);
                            return ServerOrg;
                        }
                    }
                }
            }
            return null;
        }

        public static string EnsureHttpsBaseUrlOnServer(string baseurl, WebSite site)
        {
            // Online server must use https even behind a NGINX server.  
            if (!string.IsNullOrWhiteSpace(baseurl) && Data.AppSettings.IsOnlineServer)
            {
                if (baseurl.ToLower().StartsWith("http://"))
                {
                    var tempurl = GetSiteTempUrl(site);
                    return "https://" + tempurl;
                }
            }
            return baseurl;
        }
    }

}
