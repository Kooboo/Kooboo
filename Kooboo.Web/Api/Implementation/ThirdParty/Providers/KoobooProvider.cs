using System.Linq;
using System.Web;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Repository;
using Kooboo.Sites.ViewModel;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers;

internal sealed class KoobooProvider : IThirdPartyProvider<OnlinePackageItemViewModel>
{
    public InfiniteListViewModel<OnlinePackageItemViewModel> Search(SiteDb siteDb, string keyword, int page, int pageSize)
    {
        var response = new InfiniteListViewModel<OnlinePackageItemViewModel>
        {
            PageNr = 1,
            PageSize = pageSize,
        };
        var keywordArg = HttpUtility.UrlEncode(keyword);
        var url = $"{Data.UrlSetting.AppStore}/_api/Package/search?type=module&count=100&keyword={keywordArg}";

        var exists = siteDb.ScriptModule.All().Select(it => StringHelper.ToValidFileName(it.Name));

        var list = HttpHelper.Get<List<RemoteTemplatePackage>>(url);
        response.List = list.Select(it =>
        {
            var item = new OnlinePackageItemViewModel
            {
                Id = it.Id,
                Description = it.Description,
                Name = string.IsNullOrWhiteSpace(it.Title) ? it.Name : it.Title,
                PackageName = it.Name,
                Installed = exists.Contains(StringHelper.ToValidFileName(it.Name), StringComparer.OrdinalIgnoreCase),
                Type = it.Type,
                Version = "0.0.0"
            };
            return item;
        });

        return response;
    }
}
