//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Pwa;
using Kooboo.Sites.Helper;

namespace Kooboo.Web.Frontend
{
    public class PwaMiddleware : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            var enablePwa = context?.WebSite?.Pwa?.Enable ?? false;
            var isPwaRequest = context?.Request?.RelativeUrl?.StartsWith("/_pwa") ?? false;

            if (isPwaRequest && context.Request.RelativeUrl.StartsWith(Manifest.DefaultLogoPath))
            {
                context.Response.ContentType = "image/png";
                context.Response.Body = Manifest.DefaultLogo;
            }
            else if (enablePwa && isPwaRequest)
            {
                CorsHelper.HandleHeaders(context);

                if (context.Request.RelativeUrl == Manifest.FilePath)
                {
                    context.Response.Body = Encoding.UTF8.GetBytes(context.WebSite.Pwa.Manifest);
                }

                if (context.Request.RelativeUrl == ServiceWorker.RegisterFilePath)
                {
                    context.Response.ContentType = "application/javascript";
                    context.Response.Body = ServiceWorker.RegisterJs;
                }

                if (context.Request.RelativeUrl == ServiceWorker.FilePath)
                {
                    context.Response.ContentType = "application/javascript";
                    context.Response.Body = Encoding.UTF8.GetBytes(ServiceWorker.Get(context));
                }
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }

}


