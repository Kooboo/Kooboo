using Kooboo.Data.Cache;
using Kooboo.Data.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal class ThirdPartyCache : MemoryCacheBase
{
    private static readonly Lazy<ThirdPartyCache> _cache = new Lazy<ThirdPartyCache>(() => new ThirdPartyCache(WebHostServer.Services.GetService<IMemoryCache>()));
    public ThirdPartyCache(IMemoryCache cache) : base(cache)
    {
    }

    public override string Group => nameof(BootCDNProvider);

    internal static ThirdPartyCache Instance => _cache.Value;
}