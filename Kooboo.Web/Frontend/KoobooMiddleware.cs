//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Threading.Tasks;
using Kooboo.Sites.Render;
using Kooboo.Sites.Routing;
using Kooboo.Sites.SiteTransfer;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Service;
using Kooboo.Data.Context;
using Kooboo.Data.Server;

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
            FrontContext kooboocontext = new FrontContext();
            context.SetItem<FrontContext>(kooboocontext);
            kooboocontext.RenderContext = context;

            if (context.WebSite != null)
            {
                if (!Kooboo.Web.Security.AccessControl.HasWebsiteAccess(context.WebSite, context))
                {
                    if (!CheckIsBackEndOrImageUrl(kooboocontext.RenderContext.Request.RelativeUrl))
                    {
                        if (context.User == null)
                        {
                            context.Response.Redirect(302, "/_admin/account/login?type=site&returnurl=" + System.Web.HttpUtility.UrlEncode(context.Request.RawRelativeUrl));
                            context.Response.End = true;
                            return;
                        }
                        else
                        {
                            context.Response.Redirect(302, Kooboo.DataConstants.Default403Page);
                            context.Response.End = true;
                            return;
                        }
                    }
                }


                if (kooboocontext.RenderContext.IsSiteBinding || !CheckIsBackEndOrImageUrl(kooboocontext.RenderContext.Request.RelativeUrl))
                {
                    ObjectRoute.Parse(kooboocontext);
                    if (kooboocontext.Route != null && kooboocontext.Route.objectId != default(Guid))
                    {
                        await ExecuteKooboo(kooboocontext);
                        return;
                    }
                }


                if (kooboocontext.Route == null && !String.IsNullOrEmpty(kooboocontext.WebSite.LocalRootPath))
                {
                    await Next.Invoke(context);
                    return;
                }

                if (!CheckIsBackEndOrImageUrl(kooboocontext.RenderContext.Request.RelativeUrl))
                {
                    if (kooboocontext.Route == null || kooboocontext.Route.objectId == default(Guid))
                    {
                        if (kooboocontext.SiteDb.WebSite.ContinueDownload)
                        {
                            if (Data.AppSettings.IsOnlineServer && !Kooboo.Web.Security.ActionControl.CanServerDownloadMorePages(kooboocontext.SiteDb, kooboocontext.RenderContext.Request.RelativeUrl))
                            {
                                kooboocontext.RenderContext.Response.StatusCode = 402;
                                var errorbody = await WebSiteService.RenderCustomError(kooboocontext, 402);
                                if (!string.IsNullOrWhiteSpace(errorbody))
                                {
                                    kooboocontext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorbody);
                                }
                                return;
                            }
                            else
                            {
                                var continuedownload = await TransferManager.continueDownload(kooboocontext.SiteDb, kooboocontext.RenderContext.Request.RawRelativeUrl);
                                if (continuedownload != null)
                                {
                                    ObjectRoute.Parse(kooboocontext);
                                }
                            }
                        }
                    }

                    if (kooboocontext.Route != null && kooboocontext.Route.objectId != default(Guid))
                    {
                        await ExecuteKooboo(kooboocontext);
                        return;
                    }
                    else
                    {
                        kooboocontext.RenderContext.Response.StatusCode = 404;
                        var errorbody = await WebSiteService.RenderCustomError(kooboocontext, 404);
                        if (!string.IsNullOrWhiteSpace(errorbody))
                        {
                            kooboocontext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorbody);
                        }
                        return;
                    }
                }
                else
                {
                    // reensure siteid... in case of the same url used for login. 
                    var siteid = Kooboo.Data.Context.WebServerContext.RequestSiteId(context.Request);
                    if (siteid != kooboocontext.RenderContext.WebSite.Id)
                    {
                        var site = Kooboo.Data.GlobalDb.WebSites.Get(siteid);
                        if (site != null)
                        {
                            kooboocontext.WebSite = site;
                        }
                    }

                }
            }

            if (RenderThumbnail(kooboocontext))
            { return; }

            // access control for allow users...   
            await Next.Invoke(context);
        }

        private static bool CheckIsBackEndOrImageUrl(string Relativeurl)
        {
            string relativeUrl = Relativeurl.ToLower();

            if (relativeUrl.StartsWith("/_api/") ||
                relativeUrl.StartsWith("/_admin/") ||
                relativeUrl.StartsWith("/_spa/") ||
                 relativeUrl.StartsWith("/_thumbnail/") ||
                 relativeUrl.StartsWith("/.well-known/acme-challenge")
                )
            {
                return true;
            }

            return false;
        }

        public async Task ExecuteKooboo(FrontContext frontContext)
        {
            DateTime endtime = default(DateTime);

            if (!frontContext.WebSite.Published && frontContext.RenderContext.Request.Channel == Data.Context.RequestChannel.Default)
            {
                if ((frontContext.Route != null && frontContext.Route.DestinationConstType == ConstObjectType.Page) || frontContext.RenderContext.User == null)
                {
                    frontContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes("WebSite set to offline");
                    frontContext.RenderContext.Response.StatusCode = 503;
                    return;
                }
            }

            if (frontContext.RenderContext.Response.StatusCode == 200)
            {
                try
                {
                    await RouteRenderers.RenderAsync(frontContext);
                    endtime = DateTime.UtcNow;
                    // check for rights...
                    CheckUserBandwidth(frontContext);
                }
                catch (Exception ex)
                { 
                    Kooboo.Data.Log.Instance.Exception.Write(frontContext.RenderContext.WebSite.Name + " "+ frontContext.RenderContext.Request.Url);   
                    Kooboo.Data.Log.Instance.Exception.WriteException(ex);

                    frontContext.RenderContext.Response.StatusCode = 500;

                    var errorbody = await WebSiteService.RenderCustomError(frontContext, 500);
                    if (!string.IsNullOrWhiteSpace(errorbody))
                    {
                        frontContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorbody);
                    }
                    frontContext.Log.AddEntry("500",ex.Message, DateTime.UtcNow, DateTime.UtcNow, 500, ex.Message); 
                }
            }


            if (frontContext.RenderContext.Response.StatusCode != 200)
            {
                if (string.IsNullOrEmpty(frontContext.RenderContext.Response.RedirectLocation))
                {
                    frontContext.RenderContext.Response.RedirectLocation = WebSiteService.GetCustomErrorUrl(frontContext.WebSite, frontContext.RenderContext.Response.StatusCode);
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
                    frontContext.SiteDb.VisitorLog.Add(frontContext.Log);
                }

                if (frontContext.RenderContext.Response.StatusCode != 200)
                {
                    var log = new Data.Models.SiteErrorLog();
                    log.ClientIP = frontContext.RenderContext.Request.IP;
                    log.Url = frontContext.RenderContext.Request.RawRelativeUrl;
                    log.StatusCode = frontContext.RenderContext.Response.StatusCode;
                    frontContext.SiteDb.ErrorLog.Add(log);
                }
            }
        }

        public bool RenderThumbnail(FrontContext frontcontext)
        {
            string relativeUrl = frontcontext.RenderContext.Request.RelativeUrl;

            if (relativeUrl.StartsWith(Kooboo.Sites.SiteConstants.ThumbnailRootPath, StringComparison.OrdinalIgnoreCase))
            {
                var thumbnail = ThumbnailService.GetThumbnail(frontcontext.RenderContext.WebSite.SiteDb(), relativeUrl);
                if (thumbnail != null)
                {
                    frontcontext.RenderContext.Response.ContentType = thumbnail.ContentType;
                    frontcontext.RenderContext.Response.Body = thumbnail.ContentBytes;
                }
                return true;
            }
            return false;
        }


        public async void CheckUserBandwidth(FrontContext frontContext)
        {
            bool shouldcheck = false;
            if (frontContext.RenderContext.Response.StatusCode == 200)
            {
                if (Data.AppSettings.IsOnlineServer)
                {
                    shouldcheck = true;
                }

#if DEBUG

                shouldcheck = true;

#endif
            }

            if (shouldcheck)
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
                    var orgid = frontContext.RenderContext.WebSite.OrganizationId;
                    var testok = Kooboo.Data.Infrastructure.InfraManager.Test(orgid, Data.Infrastructure.InfraType.Bandwidth, length);

                    if (!testok)
                    {
                        frontContext.RenderContext.Response.StatusCode = 402;
                        var errorbody = await WebSiteService.RenderCustomError(frontContext, 402);
                        if (!string.IsNullOrWhiteSpace(errorbody))
                        {
                            frontContext.RenderContext.Response.Body = System.Text.Encoding.UTF8.GetBytes(errorbody);
                        }
                    }
                    else
                    {
                        string url = frontContext.RenderContext.Request.Host + frontContext.RenderContext.Request.RawRelativeUrl;
                        Kooboo.Data.Infrastructure.InfraManager.Add(orgid, Data.Infrastructure.InfraType.Bandwidth, length, url);
                    }
                }

            }

        }

    }
}
