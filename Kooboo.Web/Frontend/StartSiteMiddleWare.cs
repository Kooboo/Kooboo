using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Sites.Render;
using Kooboo.Sites.Routing;
using Kooboo.Sites.Service;
using Kooboo.Web.FrontRequest;

namespace Kooboo.Web.Frontend
{
    public class StartSiteMiddleWare : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get; set;
        }

        private KoobooMiddleware KoobooSiteRender { get; set; } = new KoobooMiddleware();

        public async Task Invoke(RenderContext context)
        {
            // enable start as the default URL. 
            if (context.Request.RelativeUrl == "/" || context.Request.RelativeUrl == "\\")
            {
                context.Request.RelativeUrl = "/_start/";
            }

            if (context.Request.RelativeUrl.ToLower().StartsWith(StartSiteService.StartPath))
            {
                var webSite = StartSiteService.GetOrInstallWebSite();

                if (webSite != null)
                {
                    context.WebSite = webSite;

                    FrontContext koobooContext = new FrontContext(context);

                    ObjectRoute.Parse(koobooContext);


                    if (koobooContext.Route == null)
                    {
                        var lowerRelative = context.Request.RelativeUrl.ToLower();
                        if (lowerRelative == StartSiteService.StartPath || lowerRelative == StartSiteService.StartPath + "/")
                        {
                            if (koobooContext.Route == null)
                            {
                                var defaultRoute = ObjectRoute.GetDefaultRoute(koobooContext.SiteDb);
                                if (defaultRoute != null)
                                {
                                    koobooContext.Route = defaultRoute;
                                }
                            }

                        }
                    }

                    if (koobooContext.Route != null && koobooContext.Route.objectId != default(Guid))
                    {
                        await KoobooSiteRender.ExecuteKooboo(koobooContext);
                        return;
                    }
                }
            }

            await Next.Invoke(context);
        }
    }
}
