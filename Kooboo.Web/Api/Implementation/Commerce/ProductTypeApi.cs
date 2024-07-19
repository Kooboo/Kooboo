using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class ProductTypeApi : CommerceApi
    {
        public override string ModelName => "ProductType";

        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public ProductType[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            return commerce.ProductType.Query();
        }

        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductTypeCreate>(body, Defaults.JsonSerializerOptions);
            var siteDb = apiCall.WebSite.SiteDb();
            var productType = model.ToProductType();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(productType.Id),
                Name = $"Product type:{productType.Name}",
                Body = JsonSerializer.Serialize(productType, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.ProductType
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductTypeEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.ProductType.Get(g => g.Id == model.Id);
            model.UpdateProductType(entity);
            var siteDb = apiCall.WebSite.SiteDb();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                Name = $"Product type: {entity.Name}",
                Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.ProductType
            }, apiCall.Context.User.Id);
        }

        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            foreach (var item in ids)
            {
                var id = Lib.Security.Hash.ComputeHashGuid(item);
                var exist = siteDb.CommerceData.Get(id);
                if (exist == default)
                {
                    commerce.ProductType.Delete(p => p.Id == item);
                }
                else
                {
                    siteDb.CommerceData.Delete(id, apiCall.Context.User.Id);
                }
            }
        }

        [Permission(Feature.COMMERCE_PRODUCT_TYPE, Action = Data.Permission.Action.VIEW)]
        public ProductTypeEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.ProductType.Get(g => g.Id == id);
            return new ProductTypeEdit(entity);
        }
    }
}
