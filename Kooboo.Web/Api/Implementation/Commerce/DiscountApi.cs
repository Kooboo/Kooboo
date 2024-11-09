using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class DiscountApi : CommerceApi
    {
        public override string ModelName => "discount";

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        public object OrderLineSchemas(ApiCall call)
        {
            return OrderLineMatcher.Instance.GetOptionDetails(call.Context);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        public object OrderSchemas(ApiCall call)
        {
            return OrderMatcher.Instance.GetOptionDetails(call.Context);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<DiscountCreate>(body, Defaults.JsonSerializerOptions);
            var discount = model.ToDiscount();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(discount.Id),
                Name = $"Discount:{model.Title}",
                Body = JsonSerializer.Serialize(discount, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Discount
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        public DiscountListItem[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var discounts = commerce.Discount.Entities;

            return discounts.Select(s => new DiscountListItem(s)).ToArray();
        }


        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.EDIT)]
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
                    commerce.Discount.Delete(d => d.Id == id);
                }
                else
                {
                    siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                }
            }
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<DiscountEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.Discount.Entities.FirstOrDefault(g => g.Id == model.Id);
            model.UpdateDiscount(entity);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                Name = $"Discount:{model.Title}",
                Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Discount
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        public DiscountEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Discount.Entities.FirstOrDefault(g => g.Id == id);
            return new DiscountEdit(entity);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        public string[] Codes(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var codes = commerce.Discount.Entities.Where(d => d.Method == DiscountMethod.DiscountCode).Select(s => s.Code).Distinct().ToArray();
            return codes;
        }
    }
}