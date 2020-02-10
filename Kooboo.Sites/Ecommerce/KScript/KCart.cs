using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.KScript
{
    public class KCart
    {
        private RenderContext context { get; set; }

        private ICartService cartservice { get; set; }

        public KCart(RenderContext context)
        {
            this.context = context;
            this.cartservice = ServiceProvider.Cart(this.context);
        }

        public void AddITem(Guid ProductVariantId, int quantity = 1)
        {
            this.cartservice.AddITem(ProductVariantId, quantity);
        }

        public void RemoveItem(Guid variantId)
        {
            this.cartservice.RemoveItem(variantId);
        }

        public void ChangeQuantity(Guid ProductVariantId, int newQuantity)
        {
            this.cartservice.ChangeQuantity(ProductVariantId, newQuantity);
        }
    }


}
