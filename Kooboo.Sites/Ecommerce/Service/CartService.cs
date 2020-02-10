using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class CartService : ServiceBase<Cart>, ICartService
    {
        public Cart GetOrCreateCart()
        {
            var cart = this.Repo.Query.Where(o => o.CustomerId == this.CommerceContext.customer.Id).OrderByDescending().FirstOrDefault();
            if (cart != null && !cart.IsOrder)
            {
                return cart;
            }
            else
            {
                Cart newcart = new Cart();
                newcart.CustomerId = this.CommerceContext.customer.Id;
                this.Repo.AddOrUpdate(newcart);
                return newcart;
            }
        }

        public void AddITem(Guid ProductVariantId, int quantity = 1)
        {
            var variantService = ServiceProvider.GetService<ProductVariantsService>(this.Context);

            var variant = variantService.Get(ProductVariantId);

            if (variant != null)
            {
                var cart = GetOrCreateCart();
                var find = cart.Items.Find(o => o.ProductVariantId == ProductVariantId);
                if (find == null)
                {
                    CartItem item = new CartItem();
                    item.ProductVariantId = ProductVariantId;
                    item.UnitPrice = variant.Price;
                    item.ProductId = variant.ProductId;

                    cart.Items.Add(item);
                }
                else
                {
                    find.Quantity += 1;
                }

                UpdateCart(cart);
            }
        }

        public void RemoveItem(Guid variantId)
        {
            var cart = GetOrCreateCart();
            cart.Items.RemoveAll(o => o.ProductVariantId == variantId);
            UpdateCart(cart);
        }

        public void ChangeQuantity(Guid ProductVariantId, int newQuantity)
        {
            var variantService = ServiceProvider.GetService<ProductVariantsService>(this.Context);

            var variant = variantService.Get(ProductVariantId);

            if (variant != null)
            {
                var cart = GetOrCreateCart();
                var find = cart.Items.Find(o => o.ProductVariantId == ProductVariantId);
                if (find == null)
                {
                    CartItem item = new CartItem();
                    item.ProductVariantId = ProductVariantId;
                    item.UnitPrice = variant.Price;
                    item.ProductId = variant.ProductId;
                    item.Quantity = newQuantity;

                    cart.Items.Add(item);
                }
                else
                {
                    find.Quantity = newQuantity;
                }

                UpdateCart(cart);
            }
        }

        public void UpdateCart(Cart cart)
        {
            //calculate discount before updates...
            CalculatePromotion(cart);
            this.Repo.AddOrUpdate(cart);
        }
         
        public void CalculatePromotion(Cart cart)
        {
            Promotion.PromotionEngine.CalculatePromotion(cart, this.Context);
        }
    }
}
