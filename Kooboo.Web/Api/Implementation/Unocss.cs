using System.Linq;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data.Server;
using Kooboo.Data.Unocss;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Helper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Kooboo.Web.Api.Implementation
{
    public class UnocssApi : IApi
    {
        public string ModelName => "Unocss";

        public bool RequireSite => true;

        public bool RequireUser => false;

        record CacheItem(string Version, string Value);

        public PlainResponse Style(ApiCall call)
        {
            CorsHelper.HandleHeaders(call.Context);
            var className = call.WebSite.SiteDb().CssClassName.All();
            var list = className.Select(s => s.ClassName).Distinct().ToArray();
            var memoryCache = WebHostServer.Services.GetService<IMemoryCache>();
            memoryCache.TryGetValue(call.WebSite.Id, out CacheItem item);
            var version = call.WebSite.SiteDb().DatabaseDb.Log.Store.LastKey;
            var key = call.WebSite.LastUpdateTime.ToString() + version;

            if (item == default || item.Version != key)
            {
                var value = UnocssExecutor.Generate(list, call.Context.WebSite.UnocssSettings);
                item = new CacheItem(key, value);

                memoryCache.Set(call.WebSite.Id, item, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(14)
                });
            }

            call.Context.Response.Headers.Add("Cache-Control", "public, max-age=31536000");
            return new PlainResponse
            {
                Content = item.Value,
                ContentType = "text/css",
            };
        }
    }
}