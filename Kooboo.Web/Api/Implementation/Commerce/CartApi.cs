using System.Linq;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Calculate;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class CartApi : CommerceApi
    {
        public override string ModelName => "Cart";

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public PagingResult List(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CartQuery>(body, Defaults.JsonSerializerOptions);
            return new CartService(commerce).List(model);
        }

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CartCreate>(body, Defaults.JsonSerializerOptions);
            var customer = commerce.Customer.Get(c => c.Id == model.CustomerId);
            if (customer == default) throw new Exception("Customer not found");
            commerce.Cart.AddOrUpdate(model.ToCart());
        }

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public object Calculate(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CalculateParams>(body, Defaults.JsonSerializerOptions);
            var calculateResult = new Calculator(commerce).Calculate(model);

            return new
            {
                DiscountAllocations = calculateResult.DiscountAllocations.ToArray(),
                ShippingAllocations = calculateResult.ShippingAllocations.ToArray(),
                calculateResult.ShippingAmount,
                calculateResult.SubtotalAmount,
                calculateResult.OriginalAmount,
                calculateResult.OriginalSubtotalAmount,
                calculateResult.InsuranceAmount,
                calculateResult.TotalAmount,
                calculateResult.TotalQuantity,
                calculateResult.RedeemPoints,
                calculateResult.CanRedeemPoints,
                calculateResult.PointsDeductionAmount,
                calculateResult.CanPointsDeductionAmount,
                calculateResult.EarnPoints,
                Lines = calculateResult.Lines.Select(s =>
                {
                    return new
                    {
                        s.Amount,
                        DiscountAllocations = s.DiscountAllocations.ToArray(),
                        Image = string.IsNullOrWhiteSpace(s.Variant.Image) ? s.Product.FeaturedImage : s.Variant.Image,
                        Options = s.Variant.SelectedOptions,
                        OriginalPrice = s.Variant.Price,
                        s.OriginalAmount,
                        s.Price,
                        ProductId = s.Product.Id,
                        s.Quantity,
                        s.TotalQuantity,
                        s.Variant.Sku,
                        s.Product.Title,
                        VariantId = s.Variant.Id,
                        s.Variant.Inventory,
                        s.GroupName,
                        s.IsMain,
                        s.Note,
                        s.ExtensionButton,
                        s.Product.IsDigital
                    };
                }).ToArray()
            };
        }

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CartEdit>(body, Defaults.JsonSerializerOptions);
            var entity = commerce.Cart.Get(p => p.Id == model.Id);
            model.UpdateCart(entity);
            var customer = commerce.Customer.Get(c => c.Id == model.CustomerId);
            if (customer == default) throw new Exception("Customer not found");
            commerce.Cart.AddOrUpdate(entity);
        }

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            commerce.Cart.Delete(p => ids.Contains(p.Id));
        }

        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public CartEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var entity = commerce.Cart.Get(p => p.Id == id);
            return new CartEdit(entity);
        }
    }
}
