using Kooboo.Api;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Storage;
using Kooboo.Sites.Storage.Kooboo;
using Kooboo.Web.Api.Implementation.ThirdParty.Providers;
using Kooboo.Web.ViewModel;
using IoPath = System.IO.Path;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

interface IThirdPartyApiBase<TModel> : IApi where TModel : class
{
    bool Download(ApiCall call);

    InfiniteListViewModel<TModel> SearchOnline(ApiCall call);

    IEnumerable<OnlineProvider> Providers(ApiCall call);

    IThirdPartyProvider<TModel> GetProvider(string provider);
}

internal abstract class ThirdPartyApiBase<TModel> : IThirdPartyApiBase<TModel> where TModel : class
{
    public abstract string ModelName { get; }

    public bool RequireSite => true;

    public bool RequireUser => true;

    protected static string GetSaveUrl(ApiCall call, string name)
    {
        string path = call.GetValue("path", "fullpath");
        if (string.IsNullOrEmpty(path) || path == "\\")
        {
            path = "/";
        }

        return $"{path.TrimEnd('/')}/{name}";
    }

    protected static string GetValidRouteName(ApiCall call, string name, int index, IStorageProvider storage = null)
    {
        string path = call.GetValue("path", "fullpath");
        if (string.IsNullOrEmpty(path) || path == "\\")
        {
            path = "/";
        }

        if (storage is KoobooStorageProvider)
        {
            path = KoobooStorageProvider.HandleCustomSettingPrefix(call.WebSite, path);
        }

        var fileName = GetFileName(name, index);
        var url = $"{path.TrimEnd('/')}/{fileName}";
        var route = call.WebSite.SiteDb().Routes.GetByUrl(url);
        if (route == null)
        {
            return url;
        }

        return GetValidRouteName(call, name, index + 1);
    }

    private static string GetFileName(string name, int index)
    {
        if (index > 0)
        {
            return $"{IoPath.GetFileNameWithoutExtension(name)}({index}){IoPath.GetExtension(name)}";
        }

        return name;
    }

    public abstract bool Download(ApiCall call);
    public abstract IThirdPartyProvider<TModel> GetProvider(string provider);
    public abstract IEnumerable<OnlineProvider> Providers(ApiCall call);
    public virtual InfiniteListViewModel<TModel> SearchOnline(ApiCall call)
    {
        var response = new InfiniteListViewModel<TModel>();
        var provider = call.GetValue("provider")?.Trim();
        var keyword = call.GetValue("keyword")?.Trim();
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(keyword))
        {
            return response;
        }

        var svc = GetProvider(provider);
        if (svc == null)
        {
            return response;
        }

        int pageSize = ApiHelper.GetPageSize(call, 20);
        int page = ApiHelper.GetPageNr(call);
        var siteDb = call.WebSite.SiteDb();

        return svc.Search(siteDb, keyword, page, pageSize);
    }
}
