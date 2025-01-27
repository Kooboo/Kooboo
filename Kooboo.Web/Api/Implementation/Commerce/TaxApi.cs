using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class TaxApiApi : CommerceApi
    {
        public override string ModelName => "Tax";

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.VIEW)]
        public Tax[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            return [.. commerce.Tax.Entities.OrderBy(o=>o.Country)];
        }

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.VIEW)]
        public object Schemas(ApiCall call)
        {
            return ProductTaxMatcher.Instance.GetOptionDetails(call.Context);
        }

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<Tax>(body, Defaults.JsonSerializerOptions);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(model.Id),
                Name = $"Tax:{model.Country}",
                Body = JsonSerializer.Serialize(model, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Tax
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<Tax>(body, Defaults.JsonSerializerOptions);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(model.Id),
                Name = $"Tax:{model.Country}",
                Body = JsonSerializer.Serialize(model, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Tax
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.VIEW)]
        public Tax GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Tax.Entities.FirstOrDefault(g => g.Id == id);
            return entity;
        }

        [Permission(Feature.COMMERCE_TAX, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            foreach (var id in ids)
            {
                var guid = Lib.Security.Hash.ComputeHashGuid(id);
                var exist = siteDb.CommerceData.Get(guid);
                if (exist == default)
                {
                    commerce.Tax.Delete(d => d.Id == id);
                }
                else
                {
                    siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                }
            }
        }
    }
}
