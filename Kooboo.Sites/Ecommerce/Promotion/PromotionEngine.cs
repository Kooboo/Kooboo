using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
    public class PromotionEngine
    {

        public PromotionResult CalculateProduct(Guid ProductVariantId, RenderContext context)
        {
            return null;
        }

        public PromotionResult CalculateProudct(ProductVariants variants)
        {
            return null;
        }

        public void CalculateShoppingCart(Cart cart, RenderContext context)
        {
            return; 
        }

        
    }
}
