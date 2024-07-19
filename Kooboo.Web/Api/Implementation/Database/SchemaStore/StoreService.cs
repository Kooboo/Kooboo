using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Database.SchemaStore;

public static class StoreService
{
    public static TableSchemaMappingRepository GetMappingStore(WebSite site)
    {
        return site.SiteDb().GetSiteRepository<TableSchemaMappingRepository>();
    }
}