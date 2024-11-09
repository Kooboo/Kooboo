using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Commerce;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.CustomData;
using Kooboo.Sites.Commerce.DataStorage;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class CategoryApi : CommerceApi
    {
        public override string ModelName => "ProductCategory";

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public CategoryListItem[] List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var products = commerce.Product.Entities.ToArray();
            var variants = commerce.ProductVariant.Entities.ToArray();
            var productCategory = commerce.ProductCategory.Values.ToArray();
            var categoryService = new CategoryService(commerce, apiCall.Context);
            var fields = commerce.Settings.CategoryCustomFields;

            return commerce.Category.Entities.OrderByDescending(o => o.DragAndDrop).Select(s =>
            {
                var productCount = categoryService.GetProducts(s, products, variants).Count();
                var item = BuildCategoryListItem(s, productCount, fields);
                var parentId = item.CustomData.FirstOrDefault(f => "parentId".Equals(f.Key, StringComparison.CurrentCultureIgnoreCase));
                if (parentId.Value != default)
                {
                    foreach (var i in parentId.Value.ToArray())
                    {
                        var parent = commerce.Category.Entities.FirstOrDefault(f => f.Id == i.Value?.ToString());
                        if (parent != default)
                        {
                            parentId.Value[i.Key] = parent.Title;
                        }
                    }
                }
                return item;
            }).ToArray();
        }

        private CategoryListItem BuildCategoryListItem(Category category, int productCount, CustomField[] fields)
        {
            var item = new CategoryListItem(category, productCount);

            return Kooboo.Sites.Commerce.Helpers.BuildItem(item, category, fields);
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.EDIT)]
        public string Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CategoryCreate>(body, Defaults.JsonSerializerOptions);
            var category = model.ToCategory();
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(category.Id),
                Name = $"Category:{category.Title}",
                Body = JsonSerializer.Serialize(category, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Category
            }, apiCall.Context.User.Id);
            var tagService = new TagService(commerce);
            tagService.Append(TagType.Category, model.Tags);
            return category.Id;
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CategoryEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.Category.Entities.FirstOrDefault(g => g.Id == model.Id);
            model.UpdateCategory(entity);
            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                Name = $"Category:{entity.Title}",
                Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Category
            }, apiCall.Context.User.Id);
            var tagService = new TagService(commerce);
            tagService.Append(TagType.Category, model.Tags);
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var categoryService = new CategoryService(commerce, apiCall.Context);
            categoryService.Deletes(ids);
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.VIEW)]
        public CategoryEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Category.Entities.FirstOrDefault(g => g.Id == id);
            return new CategoryEdit(entity);
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.VIEW)]
        public object ConditionSchemas(ApiCall call)
        {
            return CategoryMatcher.Instance.GetOptionDetails(call.Context);
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.VIEW)]
        public IEnumerable<ProductListItem> Products(string categoryId, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var category = commerce.Category.Entities.FirstOrDefault(c => c.Id == categoryId);
            Product[] products = commerce.Product.Entities.ToArray();
            ProductVariant[] variants = commerce.ProductVariant.Entities.ToArray();
            var categoryService = new CategoryService(commerce, apiCall.Context);
            products = categoryService.GetProducts(category, products, variants);
            var result = new List<ProductListItem>();

            foreach (var item in products)
            {
                var productVariants = variants.Where(w => w.ProductId == item.Id).ToArray();
                result.Add(new ProductListItem(item, productVariants));
            }

            return result.ToArray();
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.EDIT)]
        public void EditProducts(string categoryId, string[] products, ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var category = commerce.Category.Entities.FirstOrDefault(c => c.Id == categoryId);
            if (category.Type != FilterType.Manual) return;
            var productCategoryList = commerce.ProductCategory.Values.Where(w => w.CategoryId == categoryId).ToArray();

            foreach (var item in productCategoryList.ToArray())
            {
                if (products.Contains(item.ProductId)) continue;
                var guid = item.GetHashGuid();
                var exist = siteDb.CommerceData.Get(guid);
                if (exist == default)
                {
                    commerce.ProductCategory.Delete(p => p.CategoryId == item.CategoryId && p.ProductId == item.ProductId);
                    var productService = new ProductService(commerce, apiCall.Context);
                    productService.UpdateProductIndex(item.ProductId);
                }
                else
                {
                    siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                }
            }

            foreach (var item in products.ToArray())
            {
                var exist = productCategoryList.Any(a => a.ProductId == item);
                if (exist) continue;

                var productCategory = new ProductCategoryModel
                {
                    CategoryId = categoryId,
                    ProductId = item
                };

                var product = commerce.Product.Entities.FirstOrDefault(f => f.Id == item);

                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = productCategory.GetHashGuid(),
                    Name = $"Product category:'{category.Title}' with '{product?.Title}'",
                    Body = JsonSerializer.Serialize(productCategory, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.ProductCategory
                }, apiCall.Context.User.Id);
            }
        }

        [Permission(Feature.COMMERCE_CATEGORY, Action = Data.Permission.Action.EDIT)]
        public void Move(ApiCall apiCall)
        {
            var model = JsonHelper.Deserialize<MoveCategory>(apiCall.GetValue("changes"));
            if (model.Source == default || (model.PrevId == default && model.NextId == default))
            {
                throw new Exception(Data.Language.Hardcoded.GetValue("Invalid Parameters", apiCall.Context));
            }

            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);

            var source = commerce.Category.Entities.FirstOrDefault(o => o.Id == model.Source);
            if (source == null) return;

            var categories = commerce.Category.Entities.ToList();
            var needUpdates = MoveCategoryHelper.CalculateToUpdateItems(categories, model);
            if (needUpdates.Any())
            {
                foreach (var item in needUpdates)
                {
                    siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                    {
                        Id = Lib.Security.Hash.ComputeHashGuid(item.Id),
                        Name = $"Category:{item.Title}",
                        Body = JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions),
                        Type = Sites.Models.CommerceDataType.Category
                    }, apiCall.Context.User.Id);
                }
            }
        }
    }
}
