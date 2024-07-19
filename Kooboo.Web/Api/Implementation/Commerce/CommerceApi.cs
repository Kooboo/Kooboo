using Kooboo.Api;
using Kooboo.Sites.Commerce;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public abstract class CommerceApi : IApi
    {
        public abstract string ModelName { get; }

        public bool RequireSite => true;

        public bool RequireUser => false;

        public SiteCommerce GetSiteCommerce(ApiCall apiCall) => SiteCommerce.Get(apiCall.WebSite);
    }
}
