//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.RateLimits;
using Kooboo.Data.SSL;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Kooboo.Sites.Render;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Service;
using Kooboo.Sites.SiteTransfer;
using Kooboo.Web.Frontend;

namespace Kooboo.Web.FrontRequest
{
    public class KoobooMiddleware : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get; set;
        }
        public async Task Invoke(RenderContext context)
        {
            Kooboo.Web.SystemStart.InitHeaders(context);

            FrontContext koobooContext = new FrontContext(context);

            if (HandleBySsoLogin(context)) return;

            if (context.WebSite != null)
            {
                if (!Security.AccessControl.HasCountryAccess(context.WebSite, context) && !CheckIsBackEndOrImageUrl(koobooContext.RenderContext.Request.RelativeUrl) && koobooContext.RenderContext.Request.Channel == RequestChannel.Default)
                {
                    var pageRoute = koobooContext.WebSite.SiteDb().Pages.GetByUrl(koobooContext.RenderContext.Request.RelativeUrl);

                    if (pageRoute != null)
                    {
                        if (!string.IsNullOrWhiteSpace(context.WebSite.VisitorCountryRestrictionPage))
                        {
                            var route = context.WebSite.SiteDb().Routes.GetByUrl(context.WebSite.VisitorCountryRestrictionPage);
                            if (route != null)
                            {
                                koobooContext.Route = route;
                                await PageRenderer.RenderAsync(koobooContext);
                                context.Response.StatusCode = 451;
                                context.Response.End = true;
                                return;
                            }
                        }
                        var errorBody = await WebSiteService.RenderCustomError(koobooContext, 451);
                        if (!string.IsNullOrWhiteSpace(errorBody))
                        {
                            koobooContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorBody);
                        }
                        context.Response.End = true;
                        return;
                    }
                }

                if (context.Request.Binding != default && !string.IsNullOrWhiteSpace(context.Request.Binding.Redirect) && !SSLHelper.IsAcmePath(context.Request.RawRelativeUrl))
                {
                    var fullDomain = context.Request.Binding.Redirect;
                    var isHttps = GlobalDb.SslCertificate.GetByDomain(fullDomain) != default;
                    fullDomain = (isHttps ? "https://" : "http://") + fullDomain + context.Request.RawRelativeUrl;
                    context.Response.Redirect(301, fullDomain);
                    return;
                }

                if (!Kooboo.Web.Security.AccessControl.HasWebsiteAccess(context.WebSite, context))
                {
                    if (!CheckIsBackEndOrImageUrl(koobooContext.RenderContext.Request.RelativeUrl))
                    {
                        if (context.User == null)
                        {
                            RedirectToLogin(context);
                            return;
                        }
                        else
                        {
                            var errorBody = await WebSiteService.RenderCustomError(koobooContext, 403);
                            if (!string.IsNullOrWhiteSpace(errorBody))
                            {
                                koobooContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorBody);
                            }
                            context.Response.End = true;
                            return;
                        }
                    }
                }

                var isBackendOrImage = CheckIsBackEndOrImageUrl(koobooContext.RenderContext.Request.RelativeUrl);
                if (koobooContext.RenderContext.IsSiteBinding || !isBackendOrImage)
                {
                    ObjectRoute.Parse(koobooContext);
                    if (koobooContext.Route != null && koobooContext.Route.objectId != default(Guid))
                    {
                        var success = true;

                        if (!isBackendOrImage)
                        {
                            success = AuthenticationHelper.Authentication(koobooContext);
                        }

                        if (success) await ExecuteKooboo(koobooContext);
                        return;
                    }
                }


                if (koobooContext.Route == null && !String.IsNullOrEmpty(koobooContext.WebSite.LocalRootPath))
                {
                    await Next.Invoke(context);
                    return;
                }

                if (!isBackendOrImage)
                {
                    if (koobooContext.Route == null || koobooContext.Route.objectId == default(Guid))
                    {
                        if (koobooContext.SiteDb.WebSite.ContinueDownload)
                        {
                            if (Data.AppSettings.IsOnlineServer && !Kooboo.Web.Security.ActionControl.CanServerDownloadMorePages(koobooContext.SiteDb, koobooContext.RenderContext.Request.RelativeUrl))
                            {
                                koobooContext.RenderContext.Response.StatusCode = 402;
                                var errorBody = await WebSiteService.RenderCustomError(koobooContext, 402);
                                if (!string.IsNullOrWhiteSpace(errorBody))
                                {
                                    koobooContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorBody);
                                }
                                return;
                            }
                            else
                            {

                                var continueDownload = await TransferManager.continueDownload(koobooContext.SiteDb, koobooContext.RenderContext.Request.RawRelativeUrl, koobooContext.RenderContext);
                                if (continueDownload != null)
                                {
                                    ObjectRoute.Parse(koobooContext);
                                }
                            }
                        }

                        var sitemapSettings = context.WebSite.SitemapSettings;

                        if (sitemapSettings.Enable && context.Request.RelativeUrl.StartsWith(sitemapSettings.Path))
                        {
                            SitemapHelper.HandleSitemap(context, sitemapSettings);
                            return;
                        }
                    }

                    if (koobooContext.Route != null && koobooContext.Route.objectId != default(Guid))
                    {
                        await ExecuteKooboo(koobooContext);
                        return;
                    }
                    else
                    {
                        if (!koobooContext.WebSite.EnableSPA || !koobooContext.WebSite.StartPages().Any(o => o.DefaultStart))
                        {
                            koobooContext.RenderContext.Response.StatusCode = 404;
                        }
                        var errorBody = await WebSiteService.RenderCustomError(koobooContext, 404);
                        if (!string.IsNullOrWhiteSpace(errorBody))
                        {
                            koobooContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorBody);
                            if (koobooContext.WebSite.EnableVisitorLog && koobooContext.RenderContext.Request.Channel == RequestChannel.Default)
                            {

                                var errorLog = new Data.Models.SiteErrorLog
                                {
                                    ClientIP = koobooContext.RenderContext.Request.IP,
                                    StatusCode = 404,
                                    Url = koobooContext.RenderContext.Request.RawRelativeUrl,
                                    StartTime = koobooContext.StartTime,
                                };

                                Kooboo.Sites.Worker.LogWriter.Instance.Add(koobooContext.SiteDb, errorLog);

                                // koobooContext.SiteDb.ErrorLog.Add(); 

                            }
                        }
                        return;
                    }
                }
                else
                {
                    koobooContext.RenderContext.IsBackendView = true;

                    // reEnsure siteId..in case of the same url used for login. 
                    var siteId = Kooboo.Data.Context.WebServerContext.RequestSiteId(context.Request);
                    if (siteId != koobooContext.RenderContext.WebSite.Id)
                    {
                        var site = Data.Config.AppHost.SiteRepo.Get(siteId);
                        if (site != null)
                        {
                            koobooContext.WebSite = site;
                        }
                    }

                    if (koobooContext.RenderContext.Request.SitePath != null)
                    {
                        koobooContext.RenderContext.Response.Redirect(302, "/_Admin/");
                        return;
                    }
                }


            }

            if (RenderThumbnail(koobooContext))
            { return; }

            // access control for allow users
            await Next.Invoke(context);
        }

        private bool HandleBySsoLogin(RenderContext context)
        {
            if (!"/_admin/login".Equals(context.Request.Path, StringComparison.CurrentCultureIgnoreCase)) return false;
            if (context.Request.QueryString.Get("sso") != "1") return false;
            var token = context.Request.QueryString.Get("access_token");
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Request.Cookies.TryGetValue("jwt_token", out token);
            }
            else
            {
                context.HttpContext.Response.Cookies.Append(DataConstants.UserJwtToken, token, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });
            }
            if (string.IsNullOrWhiteSpace(token)) return false;
            var server = context.Request.QueryString.Get("server");
            var returnUrl = context.Request.QueryString.Get("returnurl");
            var redirectUrl = $"{server}/_api/v2/user/ssoLogin?accessToken={token}&returnurl={returnUrl}";
            context.Response.Redirect(302, redirectUrl);
            context.Response.End = true;
            return true;
        }

        private void RedirectToLogin(RenderContext context)
        {
            var returnUrl = System.Web.HttpUtility.UrlEncode(context.Request.RawRelativeUrl);
            var redirectUrl = $"/_admin/login?permission={context.WebSite.SiteType}&returnurl={returnUrl}";
            if (context.WebSite.SsoLogin)
            {
                redirectUrl = $"{UrlSetting.SsoLogin}{redirectUrl}&sso=1&server={context.Request.Scheme}://{context.Request.Host}";
            }
            context.Response.Redirect(302, redirectUrl);
            context.Response.End = true;
        }

        private static bool CheckIsBackEndOrImageUrl(string RelativeUrl)
        {
            string relativeUrl = RelativeUrl.ToLower();

            if (relativeUrl.StartsWith("/_api/") ||
                relativeUrl.StartsWith("/_admin/", StringComparison.CurrentCultureIgnoreCase) ||
                relativeUrl.Equals("/_admin", StringComparison.CurrentCultureIgnoreCase) ||
                relativeUrl.StartsWith("/_spa/") ||
                relativeUrl.StartsWith("/_pwa/") ||
                relativeUrl.StartsWith("/_thumbnail/") ||
                relativeUrl.StartsWith("/.well-known/") ||
                relativeUrl.StartsWith("/_start/", StringComparison.CurrentCultureIgnoreCase) ||
                relativeUrl.StartsWith("/_start?", StringComparison.CurrentCultureIgnoreCase) ||
                relativeUrl.Equals("/_start", StringComparison.CurrentCultureIgnoreCase)

                )
            {
                return true;
            }

            return false;
        }

        public async Task ExecuteKooboo(FrontContext frontContext)
        {
            DateTime endtime = default(DateTime);

            if (frontContext.RenderContext.Request.Channel == RequestChannel.Default && !frontContext.WebSite.Published)
            {
                frontContext.RenderContext.Response.StatusCode = 503;
            }

            if (frontContext.Route.DestinationConstType == ConstObjectType.Page && (!RateLimitService.Acquire(frontContext.RenderContext) || !AccessLimitService.Acquire(frontContext.RenderContext)))
            {
                frontContext.RenderContext.Response.StatusCode = 429;
            }

            if (frontContext.RenderContext.Response.StatusCode == 200)
            {
                try
                {
                    await RouteRenderers.RenderAsync(frontContext);
                    endtime = DateTime.UtcNow;
                    // check for rights...
                    await CheckUserBandwidth(frontContext);
                }
                catch (Exception ex)
                {
                    Kooboo.Data.Log.Instance.Exception.Write(frontContext.RenderContext.WebSite.Name + " " + frontContext.RenderContext.Request.Url);
                    Kooboo.Data.Log.Instance.Exception.WriteException(ex);

                    frontContext.RenderContext.Response.StatusCode = 500;

                    frontContext.Log.AddEntry("500", ex.Message, DateTime.UtcNow, DateTime.UtcNow, 500, ex.Message);
                }
            }


            if (frontContext.RenderContext.Response.StatusCode != 200)
            {
                var custom = frontContext.RenderContext.GetItem<CustomStatusCode>();
                if (custom == null)
                {
                    var errorBody = await WebSiteService.RenderCustomError(frontContext, frontContext.RenderContext.Response.StatusCode);
                    if (!string.IsNullOrWhiteSpace(errorBody))
                    {
                        frontContext.RenderContext.Response.Body = Encoding.UTF8.GetBytes(errorBody);
                    }
                    else if (string.IsNullOrEmpty(frontContext.RenderContext.Response.RedirectLocation))
                    {
                        frontContext.RenderContext.Response.RedirectLocation = WebSiteService.GetCustomErrorUrl(frontContext.WebSite, frontContext.RenderContext.Response.StatusCode);
                    }
                }
            }

            if (frontContext.WebSite.EnableVisitorLog && frontContext.RenderContext.Request.Channel == Data.Context.RequestChannel.Default)
            {
                if (frontContext.Page != null)
                {
                    string referer = frontContext.RenderContext.Request.Headers.Get("Referer");
                    if (!string.IsNullOrEmpty(referer))
                    {
                        frontContext.Log.Referer = referer;
                    }

                    frontContext.Log.Url = frontContext.RenderContext.Request.RawRelativeUrl;
                    frontContext.Log.UserAgent = frontContext.RenderContext.Request.Headers.Get("User-Agent");
                    frontContext.Log.ClientIP = frontContext.RenderContext.Request.IP;
                    frontContext.Log.Begin = frontContext.StartTime;
                    if (endtime == default(DateTime))
                    {
                        endtime = DateTime.UtcNow;
                    }
                    frontContext.Log.End = endtime;
                    frontContext.Log.TimeSpan = (endtime - frontContext.StartTime).TotalMilliseconds;

                    frontContext.Log.StatusCode = Convert.ToInt16(frontContext.RenderContext.Response.StatusCode);
                    frontContext.Log.ObjectId = frontContext.Route != null ? frontContext.Route.objectId : default(Guid);
                    if (frontContext.RenderContext.Response.Body != null)
                    {
                        frontContext.Log.Size = frontContext.RenderContext.Response.Body.Length;
                    }
                    // frontContext.SiteDb.VisitorLog.Add(frontContext.Log);

                    Kooboo.Sites.Worker.LogWriter.Instance.Add(frontContext.SiteDb, frontContext.Log);
                }

                if (frontContext.RenderContext.Response.StatusCode >= 400)
                {
                    var log = new Data.Models.SiteErrorLog();
                    log.ClientIP = frontContext.RenderContext.Request.IP;
                    log.Url = frontContext.RenderContext.Request.RawRelativeUrl;
                    log.StatusCode = frontContext.RenderContext.Response.StatusCode;

                    Kooboo.Sites.Worker.LogWriter.Instance.Add(frontContext.SiteDb, log);
                    // frontContext.SiteDb.ErrorLog.Add(log);
                }
            }
        }

        public bool RenderThumbnail(FrontContext frontContext)
        {
            string relativeUrl = frontContext.RenderContext.Request.RelativeUrl;

            if (relativeUrl.StartsWith(Kooboo.Sites.SiteConstants.ThumbnailRootPath, StringComparison.OrdinalIgnoreCase))
            {
                var thumbnail = ThumbnailService.GetThumbnail(frontContext.RenderContext.WebSite.SiteDb(), relativeUrl);
                if (thumbnail != null)
                {
                    frontContext.RenderContext.Response.ContentType = thumbnail.ContentType;
                    frontContext.RenderContext.Response.Body = thumbnail.ContentBytes;
                }
                return true;
            }
            return false;
        }


        public async Task CheckUserBandwidth(FrontContext frontContext)
        {
            bool shouldCheck = false;
            if (frontContext.RenderContext.Response.StatusCode == 200)
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    shouldCheck = true;
                }

#if DEBUG

                shouldCheck = true;

#endif
            }

            if (shouldCheck)
            {
                long length = 0;

                if (frontContext.RenderContext.Response.Body != null)
                {
                    length = frontContext.RenderContext.Response.Body.Length;
                }

                if (length == 0)
                {
                    if (frontContext.RenderContext.Response.Stream != null)
                    {
                        try
                        {
                            length = frontContext.RenderContext.Response.Stream.Length;
                        }
                        catch (Exception ex)
                        {
                            Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.Source + ex.StackTrace);
                        }
                    }
                }

                if (length > 0)
                {
                    var orgId = frontContext.RenderContext.WebSite.OrganizationId;
                    var testOk = Kooboo.Data.Infrastructure.InfraManager.Test(orgId, Data.Infrastructure.InfraType.Bandwidth, length);

                    if (!testOk)
                    {
                        frontContext.RenderContext.Response.StatusCode = 402;
                        var errorBody = await WebSiteService.RenderCustomError(frontContext, 402);
                        if (!string.IsNullOrWhiteSpace(errorBody))
                        {
                            frontContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorBody);
                        }
                    }
                    else
                    {
                        string url = frontContext.RenderContext.Request.Host + frontContext.RenderContext.Request.RawRelativeUrl;
                        Kooboo.Data.Infrastructure.InfraManager.Add(orgId, Data.Infrastructure.InfraType.Bandwidth, length, url);
                    }
                }

            }

        }

    }
}
