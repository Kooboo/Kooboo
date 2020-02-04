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

        public decimal Discount { get; set; }

        public decimal ItemTotal { get; set; }
          
        public List<Guid> DiscountRules { get; set; }

        public List<string> DiscountReasons { get; set; }
    }
}
