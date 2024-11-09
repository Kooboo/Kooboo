using System.IO;
using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Commerce;
using Kooboo.Sites.Commerce.Condition;
using Kooboo.Sites.Commerce.DataStorage;
using Kooboo.Sites.Commerce.Entities;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Extensions;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class ProductApi : CommerceApi
    {
        public override string ModelName => "productManagement";

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public PagingResult List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var products = commerce.Product.Entities.OrderByDescending(o => o.CreatedAt).ToArray();
            var productIds = products.Select(s => s.Id).ToArray();
            var variants = commerce.ProductVariant.Entities.Where(w => productIds.Contains(w.ProductId)).ToArray();
            var result = new List<ProductListItem>();
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductQuery>(body, Defaults.JsonSerializerOptions);

            foreach (var item in products)
            {
                if (model.Excludes?.Contains(item.Id) ?? false) continue;
                if (!item.Match(model?.Keyword)) continue;
                var productVariants = variants.Where(w => w.ProductId == item.Id).ToArray();
                result.Add(new ProductListItem(item, productVariants));
            }

            var skipCount = (model.PageIndex - 1) * model.PageSize;
            var pageList = result.Skip(skipCount).Take(model.PageSize).ToArray();
            return new PagingResult(pageList, result.Count, model);
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public ProductPagingResult PagingList(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductQuery>(body, Defaults.JsonSerializerOptions);
            return new ProductService(commerce, apiCall.Context).List(model);
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public ProductVariant[] Variants(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            return commerce.ProductVariant.Entities.Where(w => w.ProductId == id).ToArray();
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.EDIT)]
        public string Create(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductCreate>(body, Defaults.JsonSerializerOptions);
            var product = model.ToProduct();
            var variants = model.ToVariants(product.Id);

            var categories = commerce.Category.Entities
              .Where(w => w.Type == FilterType.Manual && model.Categories.Contains(w.Id))
              .ToArray();

            var productCategories = categories.Select(s => new ProductCategoryModel
            {
                CategoryId = s.Id,
                ProductId = product.Id
            }).ToArray();

            siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
            {
                Id = Lib.Security.Hash.ComputeHashGuid(product.Id),
                Name = $"Product:{product.Title}",
                Body = JsonSerializer.Serialize(product, Defaults.JsonSerializerOptions),
                Type = Sites.Models.CommerceDataType.Product
            }, apiCall.Context.User.Id);

            foreach (var item in variants)
            {
                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = Lib.Security.Hash.ComputeHashGuid(item.Id),
                    Name = $"Variant:{product.Title}",
                    Body = JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.ProductVariant
                }, apiCall.Context.User.Id);
            }

            foreach (var item in productCategories)
            {
                var category = commerce.Category.Entities.FirstOrDefault(f => f.Id == item.CategoryId);
                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = item.GetHashGuid(),
                    Name = $"Product category:{category?.Title} with '{product.Title}'",
                    Body = JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.ProductCategory
                }, apiCall.Context.User.Id);
            }

            var tagService = new TagService(commerce);
            tagService.Append(TagType.Product, product.Tags);
            var variantTags = variants.SelectMany(s => s.Tags).ToArray();
            tagService.Append(TagType.Variant, variantTags);
            return product.Id;
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var siteDb = apiCall.WebSite.SiteDb();
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<ProductEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.Product.Entities.FirstOrDefault(p => p.Id == model.Id);
            IEnumerable<ProductVariant> oldVariants = commerce.ProductVariant.Entities.Where(w => w.ProductId == model.Id);
            var categories = commerce.Category.Entities
                .Where(w => w.Type == FilterType.Manual && model.Categories.Contains(w.Id))
                .ToArray();
            var productCategories = categories.Select(s => new ProductCategoryModel
            {
                CategoryId = s.Id,
                ProductId = model.Id
            }).ToArray();
            var oldProduct = entity.Clone() as Product;
            model.UpdateProduct(entity);
            oldProduct.UpdatedAt = entity.UpdatedAt;
            if (JsonSerializer.Serialize(oldProduct, Defaults.JsonSerializerOptions) != JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions))
            {
                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = Lib.Security.Hash.ComputeHashGuid(entity.Id),
                    Name = $"Product:{entity.Title}",
                    Body = JsonSerializer.Serialize(entity, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.Product
                }, apiCall.Context.User.Id);
            }

            var oldProductCategories = commerce.ProductCategory.Values.Where(w => w.ProductId == entity.Id).ToArray();

            foreach (var item in oldProductCategories.ToArray())
            {
                var guid = item.GetHashGuid();
                var exist = productCategories.Any(a => a.GetHashGuid() == guid);
                if (exist) continue;
                if (siteDb.CommerceData.Get(guid) == default)
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

            foreach (var item in productCategories)
            {
                var guid = item.GetHashGuid();
                var exist = oldProductCategories.Any(a => a.GetHashGuid() == guid);
                if (exist) continue;
                var category = commerce.Category.Entities.FirstOrDefault(f => f.Id == item.CategoryId);
                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = guid,
                    Name = $"Product category:'{category?.Title}' with '{entity.Title}'",
                    Body = JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.ProductCategory
                }, apiCall.Context.User.Id);
            }

            foreach (var item in oldVariants.ToArray())
            {
                var guid = Lib.Security.Hash.ComputeHashGuid(item.Id);
                var existVariant = model.Variants.FirstOrDefault(f => f.Id == item.Id);
                if (existVariant != default)
                {
                    var oldVariant = item.Clone() as ProductVariant;
                    existVariant.UpdateProductVariant(item);
                    oldVariant.UpdatedAt = item.UpdatedAt;
                    oldVariant.Inventory = item.Inventory;

                    if (JsonSerializer.Serialize(oldVariant, Defaults.JsonSerializerOptions) != JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions))
                    {
                        siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                        {
                            Id = guid,
                            Name = $"Variant:{model.Title}",
                            Body = JsonSerializer.Serialize(item, Defaults.JsonSerializerOptions),
                            Type = Sites.Models.CommerceDataType.ProductVariant
                        }, apiCall.Context.User.Id);
                    }
                    else
                    {
                        commerce.ProductVariant.AddOrUpdate(item);
                    }
                }
                else
                {
                    var exist = siteDb.CommerceData.Get(guid);
                    if (exist == default)
                    {
                        commerce.ProductVariant.Delete(d => d.Id == item.Id);
                        var productService = new ProductService(commerce, apiCall.Context);
                        productService.UpdateProductIndex(item.ProductId);
                        productService.DeleteVariantDigitalFiles(item.Id);
                    }
                    else
                    {
                        siteDb.CommerceData.Delete(guid, apiCall.Context.User.Id);
                    }
                }
            }

            foreach (var item in model.Variants)
            {
                var existVariant = oldVariants.FirstOrDefault(f => f.Id == item.Id);
                if (existVariant != default) continue;
                var appendVariant = item.ToProductVariant(model.Id);
                siteDb.CommerceData.AddOrUpdate(new Sites.Models.CommerceData
                {
                    Id = Lib.Security.Hash.ComputeHashGuid(appendVariant.Id),
                    Name = $"Variant:{model.Title}",
                    Body = JsonSerializer.Serialize(appendVariant, Defaults.JsonSerializerOptions),
                    Type = Sites.Models.CommerceDataType.ProductVariant
                }, apiCall.Context.User.Id);
            }

            var tagService = new TagService(commerce);
            tagService.Append(TagType.Product, model.Tags);
            var variantTags = model.Variants.SelectMany(s => s.Tags).ToArray();
            tagService.Append(TagType.Variant, variantTags);
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var productService = new ProductService(commerce, apiCall.Context);

            foreach (var id in ids)
            {
                productService.Delete(id);
            }
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public ProductEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Product.Entities.FirstOrDefault(p => p.Id == id);
            var variants = commerce.ProductVariant.Entities.Where(w => w.ProductId == id).ToArray();
            var categories = commerce.Category.Entities;
            var manualCategories = commerce.ProductCategory.Values.Where(w => w.ProductId == id);
            var context = new CategoryContext(entity, variants);

            var categoryIds = categories.Where(w =>
             {
                 if (w.Type == FilterType.Manual)
                 {
                     return manualCategories.Any(a => a.CategoryId == w.Id);
                 }
                 else if (w.Type == FilterType.Automated)
                 {
                     return CategoryMatcher.Instance.Match(context, w.Condition);
                 }
                 return false;
             }).Select(s => s.Id).ToArray();

            return new ProductEdit(entity, variants, categoryIds);
        }

        [Permission(Feature.COMMERCE_DISCOUNT, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.VIEW)]
        public ProductVariantItem[] ProductVariantItems(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var variants = commerce.ProductVariant.Entities.Where(v => ids.Contains(v.Id));
            var productIds = variants.Select(s => s.ProductId).Distinct().ToArray();
            var products = commerce.Product.Entities.Where(w => productIds.Contains(w.Id));
            var result = new List<ProductVariantItem>();

            foreach (var item in variants)
            {
                var product = products.First(f => f.Id == item.ProductId);
                var productVariantItem = new ProductVariantItem(product, item);

                if (string.IsNullOrWhiteSpace(productVariantItem.Image))
                {
                    productVariantItem.Image = product.FeaturedImage;
                }

                result.Add(productVariantItem);
            }

            return result.ToArray();
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.EDIT)]
        public void UploadDigitalFile(ApiCall call)
        {
            var commerce = SiteCommerce.Get(call.WebSite);
            var variantId = call.Context.HttpContext.Request.Form["variantId"];
            var directory = Path.Combine(commerce.RootPath, "digitalFiles", variantId);
            IOHelper.EnsureDirectoryExists(directory);

            foreach (var item in call.Context.HttpContext.Request.Form.Files)
            {
                var filePath = Path.Combine(directory, item.FileName);
                if (File.Exists(filePath)) File.Delete(filePath);
                using var fs = File.OpenWrite(filePath);
                item.CopyTo(fs);
            }
        }

        [Permission(Feature.COMMERCE_PRODUCT, Action = Data.Permission.Action.EDIT)]
        public void DeleteDigitalFile(string variantId, string fileName, ApiCall call)
        {
            var commerce = SiteCommerce.Get(call.WebSite);
            var filePath = Path.Combine(commerce.RootPath, "digitalFiles", variantId, fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }
}
