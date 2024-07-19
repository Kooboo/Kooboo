using System.Linq;
using Kooboo.Api;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Web.Api.Implementation.ThirdParty.Providers;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal class StyleSearchOnline : ThirdPartyApiBase<OnlinePackageItemViewModel>
{
    public override string ModelName => "online-style";

    [Permission(Feature.STYLE, Action = Data.Permission.Action.EDIT)]
    [Attributes.RequireParameters("file", "path")]
    public override bool Download(ApiCall call)
    {
        try
        {
            var item = JsonHelper.Deserialize<OnlinePackageItemViewModel>(call.GetValue("file"));
            var bytes = DownloadHelper.DownloadFile(item.FullUrl);
            var url = GetSaveUrl(call, item.Name);
            call.WebSite.SiteDb().Styles.Upload(bytes, url, call.Context.User.Id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
    public override IEnumerable<OnlineProvider> Providers(ApiCall call)
    {
        yield return OnlineProviders.BootCDN(call);
        yield return OnlineProviders.Cdnjs(call);
    }

    [Permission(Feature.STYLE, Action = Data.Permission.Action.VIEW)]
    [Kooboo.Attributes.RequireParameters("keyword", "provider")]
    public override InfiniteListViewModel<OnlinePackageItemViewModel> SearchOnline(ApiCall call)
    {
        return base.SearchOnline(call);
    }

    public override IThirdPartyProvider<OnlinePackageItemViewModel> GetProvider(string provider)
    {
        return provider.ToLower() switch
        {
            ProviderNames.cdnjs => new CdnjsProvider("css", siteDb => siteDb.Styles.All().Select(it => it.Name)),
            ProviderNames.BootCDN => new BootCDNProvider("css", siteDb => siteDb.Styles.All().Select(it => it.Name)),
            _ => null,
        };
    }
}
