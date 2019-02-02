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
using Kooboo.HttpServer;
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

        public static async Task<RenderContext> GetRenderContext(HttpContext httpContext)
        {
            RenderContext context = new RenderContext();
            context.Request = await GetRequest(httpContext);
            return context;
        }

        public static User GetUser(HttpRequest request, RenderContext context = null)
        { 
            var user = _GetUserFromToken(request); 
            // the user first login with token, should try to find the last page of this user.  
            if (user !=null)
            {  
                if (Kooboo.Data.Service.StartService.IsDefaultStartPage(request.RelativeUrl) &&  string.IsNullOrWhiteSpace(RequestManager.GetHttpValue(request, "returnurl")))
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

            //if (user != null && user.PasswordHash == default(Guid))
            //{
            //    return null;
            //}

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

                if (user !=null)
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
                siteid = _GetSiteIdBySiteIdQuery(context.Request);
            }
            else
            {
                context.IsSiteBinding = true;
            }

            if (siteid == default(Guid))
            {
                siteid = _GetSiteIdByDefaultPort(context.Request);
            }

            if (siteid == default(Guid) && GetWebSiteFunc != null)
            {
                siteid = GetWebSiteFunc(context);
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

        //internal static List<Binding> _fastGetBinding(string host)
        //{

        //}

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

        private static async Task<HttpRequest> GetRequest(HttpContext context)
        {
            var header = context.Features.Request.Headers as Kooboo.HttpServer.Http.HttpRequestHeaders;

            HttpRequest httprequest = new HttpRequest();

            // httprequest.Host

            if (header != null && header.HeaderHost.Any())
            {
                string host = header.HeaderHost.First();

                int delimiterIndex = host.IndexOf(":");
                if (delimiterIndex > 0)
                {
                    host = host.Substring(0, delimiterIndex);
                }
                httprequest.Host = host;
            }

            httprequest.Path = context.Features.Request.Path;
            var url = GetUrl(context.Features.Request.RawTarget);

            httprequest.Url = url;
            httprequest.RawRelativeUrl = url;
            httprequest.Method = context.Features.Request.Method.ToUpper();
            httprequest.IP = context.Features.Connection.RemoteEndPoint.Address.ToString();
            httprequest.Port = context.Features.Connection.LocalEndPoint.Port;
            httprequest.Scheme = context.Features.Request.Scheme;

            foreach (var item in header)
            {
                httprequest.Headers.Add(item.Key, item.Value);
            }

            foreach (var item in context.Features.Request.Query)
            {
                httprequest.QueryString.Add(item.Key, item.Value);
            }
            httprequest.Cookies = GetCookie(context);

            if (httprequest.Method != "GET")
            {
                if (context.Features.Request.Body != null && context.Features.Request.Body.CanRead)
                {
                    MemoryStream ms = new MemoryStream();
                    var body = context.Features.Request.Body;
                    await context.Features.Request.Body.CopyToAsync(ms);
                    httprequest.PostData = ms.ToArray();
                    ms.Dispose();
                }


                var contenttype = httprequest.Headers.Get("Content-Type");
                if (contenttype != null && contenttype.ToLower().Contains("multipart"))
                {
                    var formresult = Kooboo.Lib.NETMultiplePart.FormReader.ReadForm(httprequest.PostData);

                    httprequest.Forms = new NameValueCollection();
                    if (formresult.FormData != null)
                    {
                        foreach (var item in formresult.FormData)
                        {
                            httprequest.Forms.Add(item.Key, item.Value);
                        }
                    }

                    httprequest.Files = formresult.Files;

                }
                else
                {
                    httprequest.Forms.Add(GetForm(httprequest.PostData));
                }

            }
            return httprequest;
        }

        private static string GetUrl(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return "/";
            }
            else
            {
                return System.Net.WebUtility.UrlDecode(target);
            }
        }


        internal static NameValueCollection GetForm(byte[] inputstream)
        {
            NameValueCollection result = new NameValueCollection();

            string text = System.Text.Encoding.UTF8.GetString(inputstream);

            //text = Uri.UnescapeDataString(text); 

            //text = System.Net.WebUtility.UrlDecode(text);
            //text = System.Net.WebUtility.HtmlDecode(text);

            int textLength = text.Length;
            int equalIndex = text.IndexOf('=');
            if (equalIndex == -1)
            {
                equalIndex = textLength;
            }
            int scanIndex = 0;
            while (scanIndex < textLength)
            {
                int delimiterIndex = text.IndexOf("&", scanIndex);
                if (delimiterIndex == -1)
                {
                    delimiterIndex = textLength;
                }
                if (equalIndex < delimiterIndex)
                {
                    while (scanIndex != equalIndex && char.IsWhiteSpace(text[scanIndex]))
                    {
                        ++scanIndex;
                    }
                    string name = text.Substring(scanIndex, equalIndex - scanIndex);
                    name= Uri.UnescapeDataString(name);
                    
                    string value = text.Substring(equalIndex + 1, delimiterIndex - equalIndex - 1);
                    value= Uri.UnescapeDataString(value);

                    result.Add(name, value);
                    equalIndex = text.IndexOf('=', delimiterIndex);
                    if (equalIndex == -1)
                    {
                        equalIndex = textLength;
                    }
                }
                scanIndex = delimiterIndex + 1;
            }

            return result;
        }

        private static Dictionary<string, string> GetCookie(HttpContext context)
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();

            foreach (var item in context.Features.Request.Cookies)
            {
                if (!cookies.ContainsKey(item.Key))
                {
                    cookies.Add(item.Key, item.Value);
                }
            }
            return cookies;
        }

        public static async Task SetResponse(HttpContext context, RenderContext renderContext)
        {
            var response = renderContext.Response;

            var header = context.Features.Response.Headers as Kooboo.HttpServer.Http.HttpResponseHeaders;

            context.Features.Response.StatusCode = response.StatusCode;

            if (response.StatusCode == 200)
            {
                context.Features.Response.Headers["Server"] = "http://www.kooboo.com";

                foreach (var item in response.Headers)
                {
                    context.Features.Response.Headers[item.Key] = item.Value;
                }

                foreach (var item in response.DeletedCookieNames)
                {
                    // context.Features.Response.Cookies.Delete(item);
                    var options = new CookieOptions()
                    {
                        Domain = renderContext.Request.Host,
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-30)
                    };
                    context.Features.Response.Cookies.Append(item, "", options);

                    response.AppendedCookies.RemoveAll(o => o.Name == item);
                }

                foreach (var item in response.AppendedCookies)
                {
                    if (string.IsNullOrEmpty(item.Domain))
                    {
                        item.Domain = renderContext.Request.Host;
                    }
                    if (string.IsNullOrEmpty(item.Path))
                    {
                        item.Path = "/";
                    }

                    //if (item.Expires == default(DateTime))
                    //{
                    //    context.Features.Response.Cookies.Append(item.Name, item.Value);  
                    //}
                    //else
                    //{

                    if (item.Expires == default(DateTime))
                    {
                        var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
                        time = time.AddSeconds(-1);
                        item.Expires = time;
                    }

                    var options = new CookieOptions()
                    {
                        Domain = item.Domain,
                        Path = item.Path,
                        Expires = item.Expires
                    };

                    context.Features.Response.Cookies.Append(item.Name, item.Value, options);
                    // }   
                }

                header.HeaderContentType = response.ContentType;

                if (response.Body != null && context.Features.Request.Method.ToLower() != "head")
                {
                    //int len = response.Body.Length;
                    //if (len > 0)
                    //{
                    try
                    {
                        header.ContentLength = response.Body.Length;
                        await context.Features.Response.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        context.Features.Response.Body.Close();
                    }
                    //}
                }
                else
                {
                    if (response.Stream != null)
                    {
                        response.Stream.CopyTo(context.Features.Response.Body);
                    }
                    else if (response.ContentType !=null && response.ContentType.ToLower().Contains("javascript"))
                    {
                        // TODO:???? what is this???? 
                    }
                    else

                    {
                        // 404.   
                        string filename = Lib.Helper.IOHelper.CombinePath(AppSettings.RootPath, Kooboo.DataConstants.Default404Page) + ".html";
                        if (System.IO.File.Exists(filename))
                        {
                            string text = System.IO.File.ReadAllText(filename);
                            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                            header.ContentLength = bytes.Length;
                            context.Features.Response.StatusCode = 404;
                            await context.Features.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                        }  

                    }
                }
            }
            else
            {

                string location = response.RedirectLocation;

                foreach (var item in response.DeletedCookieNames)
                {
                    var options = new CookieOptions()
                    {
                        Domain = renderContext.Request.Host,
                        Path = "/",
                        Expires = DateTime.Now.AddDays(-30)
                    };

                    context.Features.Response.Cookies.Append(item, "", options);
                }

                foreach (var item in response.AppendedCookies)
                {
                    if (string.IsNullOrEmpty(item.Domain))
                    {
                        item.Domain = renderContext.Request.Host;
                    }
                    if (string.IsNullOrEmpty(item.Path))
                    {
                        item.Path = "/";
                    }

                    var options = new CookieOptions()
                    {
                        Domain = item.Domain,
                        Path = item.Path,
                        Expires = item.Expires
                    };

                    context.Features.Response.Cookies.Append(item.Name, item.Value, options);
                }

                if (!string.IsNullOrEmpty(location))
                {
                    location = GetEncodedLocation(location);

                    var host = renderContext.Request.Port == 80 || renderContext.Request.Port == 443
                        ? renderContext.Request.Host
                        : string.Format("{0}:{1}", renderContext.Request.Host, renderContext.Request.Port);
                    string BaseUrl = renderContext.Request.Scheme + "://" + host + renderContext.Request.Path;
                    var newUrl = UrlHelper.Combine(BaseUrl, location);
                    if (response.StatusCode != 200)
                    {
                        context.Features.Response.StatusCode = response.StatusCode;
                    }
                    //status code doesn't start with 3xx,it'will not redirect.
                    if (!response.StatusCode.ToString().StartsWith("3"))
                    {
                        context.Features.Response.StatusCode = StatusCodes.Status302Found;
                    }
                    
                    header.HeaderLocation = newUrl;

                    context.Features.Response.Body.Dispose();

                    Log(renderContext);
                    return;
                }

                if (response.Body != null && response.Body.Length > 0)
                {
                    context.Features.Response.StatusCode = response.StatusCode;
                    await context.Features.Response.Body.WriteAsync(response.Body, 0, response.Body.Length).ConfigureAwait(false);

                    context.Features.Response.Body.Dispose();
                    Log(renderContext);
                    return;
                }

                context.Features.Response.StatusCode = response.StatusCode;
                string responsebody = null;
                switch (response.StatusCode)
                {
                    case 404:
                        responsebody = " The requested resource not found";
                        break;
                    case 301:
                        responsebody = " The requested resource has moved.";
                        break;
                    case 302:
                        responsebody = " The requested resource has moved.";
                        break;
                    case 401:
                        responsebody = " Unauthorized access";
                        break;
                    case 403:
                        responsebody = " Unauthorized access";
                        break;
                    case 407:
                        responsebody = "Reach Limitation";
                        break;
                    case 500:
                        responsebody = "Internal server error";
                        break;
                    case 503:
                        responsebody = " Service Unavailable";
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(responsebody))
                {
                    if (response.StatusCode >= 400 && response.StatusCode < 500)
                    {
                        responsebody = " Client error";
                    }
                    else if (response.StatusCode >= 500)
                    {
                        responsebody = " Server error";
                    }
                    else
                    {
                        responsebody = " Unknown error";
                    }
                }
                var bytes = Encoding.UTF8.GetBytes(responsebody);
                await context.Features.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }


            context.Features.Response.Body.Dispose();
            Log(renderContext);
            context = null;
        }

        internal static string GetEncodedLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                return location;
            var builder = new StringBuilder();
            Uri uri;
            if(Uri.TryCreate(location, UriKind.Absolute,out uri))
            {
                var baseUrl = uri.Scheme + "://" + uri.Authority;
                builder.Append(baseUrl);
                location = location.Replace(baseUrl,"");
            }

            var queryString = string.Empty;

            int questionmark = location.IndexOf("?");
            if (questionmark > -1)
            {
                queryString = location.Substring(questionmark);
                location = location.Substring(0, questionmark);
                
            }
            var segments = location.Split('/');

            for(var i=0;i<segments.Length;i++)
            {
                var seg = segments[i];
                builder.Append(System.Net.WebUtility.UrlEncode(seg));
                if (segments.Length - 1 != i)
                {
                    builder.Append("/");
                }
                
            }
            if(!string.IsNullOrEmpty(queryString))
            {
                builder.Append(queryString);
            }
            return builder.ToString();
        }
        public static void Log(RenderContext context)
        {
            if (Data.AppSettings.Global.EnableLog)
            {
                string log = context.Response.StatusCode.ToString() + " " + context.Request.IP + ": " + DateTime.Now.ToLongTimeString() + " " + context.Request.Host + " " + context.Request.Method + " " + context.Request.Url;

                Kooboo.Data.Log.HttpLog.Write(log);
            }
        }
    }


}
