using Kooboo.Data.Context;
using Kooboo.Data.Server;
using Kooboo.Lib.Compatible;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Kooboo.Web.Frontend
{
    class CdnCacheMiddleware : IKoobooMiddleWare
    {
        public IKoobooMiddleWare Next
        {
            get; set;
        }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.Path.ToLower().StartsWith("/_cdn"))
            {
                var url = context.Request.QueryString.Get("url");
                if (string.IsNullOrWhiteSpace(url)) {
                   var referer= context.Request.Headers.Get("Referer");
                    //throw new ArgumentNullException("url");
                }
                var contentType = CompatibleManager.Instance.Framework.GetMimeMapping(url);
                if (contentType == "application/octet-stream")
                {
                    url += ".js";
                    contentType = CompatibleManager.Instance.Framework.GetMimeMapping(url);
                }
                var result = await HttpClientHelper.Client.GetAsync(url);
                context.Response.ContentType = contentType;
                context.Response.AppendString(await result.Content.ReadAsStringAsync());
            }
            else await Next.Invoke(context);
        }
    }
}
