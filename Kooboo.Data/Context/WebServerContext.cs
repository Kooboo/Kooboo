//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Helper;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Microsoft.AspNetCore.Http;

namespace Kooboo.Data.Context
{
    public static class WebServerContext
    {
        private static Func<string, string, User> _validate;
        public static Func<string, string, User> Validate
        {
            get
            {
                if (_validate == null)
                {
                    _validate = GlobalDb.Users.Validate;
                }
                return _validate;
            }
            set
            {
                _validate = value;
            }
        }

        private static Func<Guid, User> _getuser;
        public static Func<Guid, User> GetUserFunc
        {
            get
            {
                if (_getuser == null)
                {
                    _getuser = GlobalDb.Users.Get;
                }
                return _getuser;
            }
            set
            {
                value = _getuser;
            }
        }


        public static Func<RenderContext, Guid> GetWebSiteFunc { get; set; }


        public static User GetUser(HttpRequest request, RenderContext context = null)
        {
            var user = _GetUserFromToken(request);
            // the user first login with token, should try to find the last page of this user.  
            if (user != null)
            {
                if (Kooboo.Data.Service.StartService.IsDefaultStartPage(request.RelativeUrl) && string.IsNullOrWhiteSpace(RequestManager.GetHttpValue(request, "returnurl")))
                {
                    var lasturl = Service.UserLoginService.GetLastPath(user.Id);
                    if (!string.IsNullOrEmpty(lasturl))
                    {
                        context.Response.Redirect(302, lasturl);
                        context.Response.End = true;
                    }
                }
            }
            else
            {
                user = _GetUserFromBasicAuthentication(request);
            }

            if (user != null && context != null)
            {
                string remember = context.Request.QueryString.Get("remember");
                int days = 1;
                if (!string.IsNullOrEmpty(remember))
                {
                    days = 60;
                }
                context.Response.AppendCookie(DataConstants.UserApiSessionKey, user.Id.ToString(), days);
            }

            if (user == null)
            {
                user = _GetUserFromCookie(request);
            }

            if (user == null)
            {
                //Two factors is only use to validate access to remote servers. 
                user = Kooboo.Data.Service.TwoFactorService.Validate(request);
                if (user != null)
                {
                    return user;
                }
            }

            if (string.IsNullOrEmpty(Data.Service.UserLoginService.GetUserPassword(user)))
            {
                return null;
            }

            return user;
        }

        internal static User _GetUserFromToken(HttpRequest request)
        {
            string token = RequestManager.GetHttpValue(request, "AccessToken");

            if (!string.IsNullOrEmpty(token))
            {
                Guid userid = Kooboo.Data.Cache.AccessTokenCache.GetUserId(token);

                if (userid != default(Guid))
                {
                    return GetUserFunc(userid);
                }
                var user = Kooboo.Data.GlobalDb.Users.GetByToken(token);
                if (user != null)
                {
                    Data.Cache.AccessTokenCache.SetToken(user.Id, token);
                }

                if (user != null)
                {
                    if (!Kooboo.Data.Service.UserLoginService.IsAllow(user.Id))
                    {
                        return null;
                    }
                }

                return user;
            }

            return null;
        }

        internal static User _GetUserFromCookie(HttpRequest request)
        {
            foreach (var item in request.Cookies)
            {
                if (item.Key == Kooboo.DataConstants.UserApiSessionKey)
                {
                    Guid userid = default(Guid);
                    if (System.Guid.TryParse(item.Value, out userid))
                    {
                        return GetUserFunc(userid);
                    }
                }
            }
            return null;
        }

        internal static User _GetUserFromBasicAuthentication(HttpRequest request)
        {
            // get from authentication... 
            string AuthorizationHeader = request.Headers.Get("Authorization");

            if (!string.IsNullOrEmpty(AuthorizationHeader))
            {
                var basicuser = ExtractUserFromBasicAuthorization(AuthorizationHeader);
                if (basicuser != null)
                {
                    return Validate(basicuser.UserName, basicuser.Password);
                }
            }
            return null;
        }


        internal static User _GetUserFromTwoFactor(HttpRequest request)
        {
            return Kooboo.Data.Service.TwoFactorService.Validate(request);
        }




        //https://en.wikipedia.org/wiki/Basic_access_authentication
        public static BasicUser ExtractUserFromBasicAuthorization(string AuthorizationCookieValue)
        {
            //Authorization: Basic QWxhZGRpbjpPcGVuU2VzYW1l 
            if (string.IsNullOrWhiteSpace(AuthorizationCookieValue))
            {
                return null;
            }
            string value = AuthorizationCookieValue.Trim();

            if (!string.IsNullOrEmpty(value) && value.ToLower().StartsWith("basic") && value.Length > 5)
            {
                value = value.Substring(5).Trim();
                var UserPassString = Encoding.UTF8.GetString(Convert.FromBase64String(value));

                int seperatorIndex = UserPassString.IndexOf(":");
                if (seperatorIndex > 0 && value.Length > seperatorIndex)
                {
                    BasicUser user = new BasicUser();
                    user.UserName = UserPassString.Substring(0, seperatorIndex);
                    user.Password = UserPassString.Substring(seperatorIndex + 1);
                    return user;
                }
            }
            return null;
        }

        public static string GenerateBasicAuthorization(string UserName, string Password)
        {
            //Authorization: Basic QWxhZGRpbjpPcGVuU2VzYW1l
            //client.AddDefaultHeader("Authorization", "Basic " + Convert.ToBase64String(bytes)); 
            string UserPassString = UserName + ":" + Password;
            var bytes = Encoding.UTF8.GetBytes(UserPassString);
            return "Basic " + Convert.ToBase64String(bytes);
        }

        public static WebSite GetWebSite(RenderContext context)
        {
            var siteid = _GetSiteIdByUrl(context.Request);

            if (siteid == default(Guid))
            {
                siteid = Kooboo.Data.Service.WebSiteService.GetTempUrlSiteId(context.Request.Host);
            }

            if (siteid == default(Guid))
            {
                siteid = _GetSiteIdBySiteIdQuery(context.Request);
            }
            else
            {
                context.IsSiteBinding = true;
            }

            if (siteid == default(Guid))
            {
                siteid = _GetSiteIdByDefaultPort(context.Request);
                if (siteid != default(Guid))
                {
                    context.IsSiteBinding = true;
                }
            }

            if (siteid == default(Guid) && GetWebSiteFunc != null)
            {
                siteid = GetWebSiteFunc(context);
                if (siteid != default(Guid))
                {
                    context.IsSiteBinding = true;
                }
            }

            if (siteid != default(Guid))
            {
                var site = Kooboo.Data.GlobalDb.WebSites.Get(siteid);
                if (site != null)
                {
                    if (context.IsSiteBinding && site.ForceSSL)
                    {
                        if (context.Request.Scheme != null && context.Request.Scheme.ToLower() != "https")
                        {
                            if (!context.Request.Headers.AllKeys.Contains("https") && !context.Request.Headers.AllKeys.Contains("ishttp"))
                            {
                                string newurl = "https://" + context.Request.Host;
                                if (context.Request.Port != 80)
                                {
                                    newurl += newurl + ":" + context.Request.Port;
                                }
                                newurl += context.Request.RawRelativeUrl;
                                context.Response.Redirect(301, newurl);
                                context.Response.End = true;
                            }
                        }
                    }

                    RequestManager.ParseQueryString(site, context.Request);
                    RequestManager.PraseSitePath(site, context.Request);

                    return site;
                }
            }
            return null;
        }

        public static Guid RequestSiteId(HttpRequest request)
        {
            var siteid = _GetSiteIdBySiteIdQuery(request);
            if (siteid == default(Guid))
            {
                siteid = _GetSiteIdByDefaultPort(request);
            }
            return siteid;
        }

        internal static Guid _GetSiteIdBySiteIdQuery(HttpRequest request)
        {
            Guid siteid = default(Guid);
            string value = request.GetValue(DataConstants.SiteId);

            if (!string.IsNullOrEmpty(value))
            {
                System.Guid.TryParse(value, out siteid);
            }
            return siteid;
        }

        internal static Guid _GetSiteIdByUrl(HttpRequest request)
        {
            if (string.IsNullOrEmpty(request.Host))
            {
                return default(Guid);
            }

            List<Binding> bindings = null;

            var firsttry = _fastFirstDomain(request.Host);

            if (firsttry != null)
            {
                bindings = Data.GlobalDb.Bindings.GetByDomainResult(firsttry);

                if (bindings == null || !bindings.Any())
                {
                    var secondtry = _fastSecondDomain(request.Host);
                    if (secondtry != null)
                    {
                        bindings = Data.GlobalDb.Bindings.GetByDomainResult(secondtry);
                    }
                }
            }

            if (bindings == null || !bindings.Any())
            {
                bindings = Data.GlobalDb.Bindings.GetByFullDomain(request.Host);
            }

            if (bindings != null && bindings.Any())
            {
                if (AppSettings.SupportSiteDeviceBinding)
                {
                    string UserAgent = request.Headers.Get("User-Agent");
                    return _GetWebSiteId(bindings, UserAgent);
                }
                else
                {
                    return bindings.First().WebSiteId;
                }
            }



            return default(Guid);
        }


        internal static DomainResult _fastFirstDomain(string host)
        {
            int len = host.Length;
            bool DotFound = false;
            for (int i = len - 1; i >= 0; i--)
            {
                if (host[i] == '.')
                {
                    if (!DotFound)
                    {
                        DotFound = true;
                    }
                    else
                    {
                        string domain = host.Substring(i + 1);
                        string sub = host.Substring(0, i);
                        return new DomainResult() { Domain = domain, SubDomain = sub };
                    }
                }
            }

            if (DotFound)
            {
                if (host.ToLower().EndsWith(".kooboo"))
                {
                    var result = new DomainResult() { Domain = "kooboo" };
                    result.SubDomain = host.Substring(0, host.Length - 7);
                    return result;
                }

                return new DomainResult() { Domain = host };
            }
            return null;
        }

        internal static DomainResult _fastSecondDomain(string host)
        {
            int len = host.Length;
            bool DotFound = false;
            bool DoubuleDotFound = false;
            for (int i = len - 1; i >= 0; i--)
            {
                if (host[i] == '.')
                {
                    if (!DotFound)
                    {
                        DotFound = true;
                    }
                    else if (!DoubuleDotFound)
                    {
                        DoubuleDotFound = true;
                    }
                    else
                    {
                        string domain = host.Substring(i + 1);
                        string sub = host.Substring(0, i);
                        return new DomainResult() { Domain = domain, SubDomain = sub };
                    }
                }
            }

            if (DoubuleDotFound)
            {
                return new DomainResult() { Domain = host };
            }
            return null;
        }


        internal static Guid _GetWebSiteId(List<Binding> bindings, string UserAgent)
        {
            if (bindings == null || bindings.Count == 0)
            {
                return default(Guid);
            }

            if (string.IsNullOrEmpty(UserAgent))
            {
                return bindings.FirstOrDefault().WebSiteId;
            }

            Binding rightbinding = null;
            bool devicematch = false;

            // check the user-agent. 
            foreach (var item in bindings)
            {
                if (!string.IsNullOrEmpty(item.Device))
                {
                    string[] devices = item.Device.Split('|');
                    foreach (var jtem in devices)
                    {
                        if (UserAgent.Contains(jtem))
                        {
                            rightbinding = item;
                            devicematch = true;
                            break;
                        }
                    }
                }

                if (devicematch)
                {
                    break;
                }
                else
                {
                    rightbinding = item;
                }
            }

            if (rightbinding != null)
            {
                return rightbinding.WebSiteId;
            }

            return default(Guid);
        }

        internal static Guid _GetSiteIdByDefaultPort(HttpRequest request)
        {
            if (request.Port > 0)
            {
                var all = GlobalDb.Bindings.GetByDomain(default(Guid));
                if (all != null && all.Count() > 0)
                {
                    foreach (var item in all)
                    {
                        if ((string.IsNullOrEmpty(item.SubDomain) || item.DefaultPortBinding) && item.Port == request.Port)
                        {
                            return item.WebSiteId;
                        }
                    }
                }
            }
            return default(Guid);
        }




    }


}
