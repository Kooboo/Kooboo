using System.IO;
using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class ShippingApi : CommerceApi
    {
        public override string ModelName => "shipping";

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public object Schemas(ApiCall call)
        {
            return ShippingMatcher.Instance.GetOptionDetails(call.Context);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ShippingCreate>(body, Defaults.JsonSerializerOptions);
            var shipping = model.ToShipping();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(shipping.Id),
                Name = $"Shipping:{model.Name}",
                Body = JsonSerializer.Serialize(shipping, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Shipping
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public Shipping[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var shippings = commerce.Shipping.Entities;
            return shippings.ToArray();
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
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
                    commerce.Shipping.Delete(d => d.Id == id);
                }
                else
                {
                    siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                }
            }
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ShippingEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.Shipping.Entities.FirstOrDefault(g => g.Id == model.Id);
            model.UpdateShipping(entity);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                Name = $"Shipping:{model.Name}",
                Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Shipping
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        public ShippingEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Shipping.Entities.FirstOrDefault(g => g.Id == id);
            return new ShippingEdit(entity);
        }

        [Permission(Feature.COMMERCE_SHIPPING, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public Shipping Get(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var shipping = commerce.Shipping.Entities.FirstOrDefault(c => c.Id == id);
            if (shipping == default) shipping = commerce.Shipping.Entities.FirstOrDefault(f => f.IsDefault);
            return shipping;
        }

        public KeyValuePair<string, string>[] Countries()
        {
            return CountryService.List();
        }

        public void SetDefault(string id, ApiCall call)
        {
            var commerce = GetSiteCommerce(call);
            var shipping = commerce.Shipping.Entities.FirstOrDefault(f => f.Id == id);
            if (shipping == null || shipping.IsDefault) return;
            var oldDefaults = commerce.Shipping.Entities.Where(f => f.IsDefault).ToArray();

            foreach (var item in oldDefaults)
            {
                if (item.IsDefault)
                {
                    item.IsDefault = false;
                    commerce.Shipping.AddOrUpdate(item);
                }
            }

            shipping.IsDefault = true;
            commerce.Shipping.AddOrUpdate(shipping);
        }
    }
}