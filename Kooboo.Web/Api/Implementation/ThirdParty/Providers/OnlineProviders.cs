using Kooboo.Api;
using Kooboo.Data.Language;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal static class OnlineProviders
{
    public static OnlineProvider Cdnjs(ApiCall call) => new()
    {
        Value = ProviderNames.cdnjs,
        Label = Hardcoded.GetValue(nameof(ProviderNames.cdnjs), call.Context),
        Url = "https://cdnjs.com/libraries"
    };

    public static OnlineProvider BootCDN(ApiCall call) => new()
    {
        Value = ProviderNames.BootCDN,
        Label = Hardcoded.GetValue(nameof(ProviderNames.BootCDN), call.Context),
        Url = "https://www.bootcdn.cn/"
    };

    public static OnlineProvider Unsplash(ApiCall call) => new()
    {
        Value = ProviderNames.Unsplash,
        Label = Hardcoded.GetValue(nameof(ProviderNames.Unsplash), call.Context),
        Url = "https://unsplash.com"
    };

    public static OnlineProvider Shopify(ApiCall call) => new()
    {
        Value = ProviderNames.Shopify,
        Label = Hardcoded.GetValue(nameof(ProviderNames.Shopify), call.Context),
        Url = "https://burst.shopify.com"
    };

    public static OnlineProvider Kooboo(ApiCall call) => new()
    {
        Value = ProviderNames.Kooboo,
        Label = Hardcoded.GetValue(nameof(ProviderNames.Kooboo), call.Context),
        Url = null // we don't want to expose the real URL
    };
}

internal class ProviderNames
{
    // Script/Style
    internal const string cdnjs = "cdnjs";
    internal const string BootCDN = "bootcdn";

    // Image
    internal const string Unsplash = "unsplash";
    internal const string Pexels = "pexels";
    internal const string Pixabay = "pixabay";
    internal const string Shopify = "shopify";

    // Module
    internal const string Kooboo = "kooboo";
}
