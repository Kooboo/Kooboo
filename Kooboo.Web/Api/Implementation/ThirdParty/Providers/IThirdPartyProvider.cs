using Kooboo.Sites.Repository;
using Kooboo.Web.ViewModel;

namespace Kooboo.Web.Api.Implementation.ThirdParty.Providers
{
    public interface IThirdPartyProvider<TModel> where TModel : class
    {
        InfiniteListViewModel<TModel> Search(SiteDb siteDb, string keyword, int page, int pageSize);
    }
}
