using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Storage;
using Kooboo.Web.Api.Implementation.ThirdParty.Providers;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal class MediaSearchOnline : ThirdPartyApiBase<MediaFileViewModel>
{
    public override string ModelName => "online-media";

    [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
    [Kooboo.Attributes.RequireParameters("keyword", "provider")]
    public override InfiniteListViewModel<MediaFileViewModel> SearchOnline(ApiCall call)
    {
        return base.SearchOnline(call);
    }

    [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.VIEW)]
    public override IEnumerable<OnlineProvider> Providers(ApiCall call)
    {
        yield return OnlineProviders.Unsplash(call);

        yield return OnlineProviders.Shopify(call);
    }

    [Permission(Feature.MEDIA_LIBRARY, Action = Data.Permission.Action.EDIT)]
    [Attributes.RequireParameters("file", "path")]
    public override bool Download(ApiCall call)
    {
        var item = JsonHelper.Deserialize<ImageDownloadViewModel>(call.GetValue("file"));

        try
        {
            var bytes = DownloadHelper.DownloadFile(item.Url);
            var siteDb = call.WebSite.SiteDb();
            var storage = GetStorageProvider(call);
            var url = GetValidRouteName(call, item.Name, 0, storage);
            storage.UploadMediaFile(url, bytes, item.Alt);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override IThirdPartyProvider<MediaFileViewModel> GetProvider(string provider)
    {
        return provider.ToLower() switch
        {
            ProviderNames.Unsplash => new UnsplashProvider(),
            ProviderNames.Shopify => new ShopifyProvider(),
            _ => null,
        };
    }

    private IStorageProvider GetStorageProvider(ApiCall call)
    {
        var provider = call.GetValue("provider");
        return StorageProviderFactory.GetProvider(provider, call.Context);
    }
}
