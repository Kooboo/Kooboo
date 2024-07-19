//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Web.Frontend
{

    public class SpaMultilingualMiddleware : IKoobooMiddleWare
    {
        private readonly MemoryCache cache = new(new MemoryCacheOptions()
        {

        });

        public IKoobooMiddleWare Next { get; set; }

        public async Task Invoke(RenderContext context)
        {
            if (context.Request.RelativeUrl.StartsWith("/_spa/lang")
                && context.WebSite != null)
            {
                var body = cache.GetOrCreate(context.WebSite.Id, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3);
                    return GetData(context);
                });

                CorsHelper.HandleHeaders(context);
                context.Response.ContentType = "application/json";
                context.Response.Body = body;
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private static byte[] GetData(RenderContext context)
        {
            var result = new Dictionary<string, string>();
            var lang = context.Request.RelativeUrl.Split('/').Last();
            var spaMultilingualRepository = context.WebSite.SiteDb().SpaMultilingual;
            var list = spaMultilingualRepository.All();

            foreach (var item in list)
            {
                item.Value.TryGetValue(lang, out string value);

                if (value == default)
                {
                    item.Value.TryGetValue(item.DefaultLang, out value);
                }

                if (value != default)
                {
                    result.Add(item.Name, value);
                }
            }

            return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(result);
        }
    }

}


