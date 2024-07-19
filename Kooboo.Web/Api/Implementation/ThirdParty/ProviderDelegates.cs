using Kooboo.Sites.Repository;

namespace Kooboo.Web.Api.Implementation.ThirdParty;

internal delegate string NameBuilder(string name, string version, string fileType);

internal delegate IEnumerable<string> ExistsFetcher(SiteDb siteDb);