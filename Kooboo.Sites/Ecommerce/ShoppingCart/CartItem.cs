using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.ShoppingCart
{
   public class CartItem
    {
        public Guid ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }  
    }
}
