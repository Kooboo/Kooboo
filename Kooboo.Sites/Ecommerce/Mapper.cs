using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public static class Mapper
    {
        public static OrderLine ToOrderLine(CartItem cartitem)
        {
            OrderLine line = new OrderLine();
            line.ProductId = cartitem.ProductId;
            line.ProductVariantId = cartitem.ProductVariantId;
            line.Quantity = cartitem.Quantity;
            line.UnitPrice = cartitem.UnitPrice;
            line.Discount = cartitem.Discount;
            return line; 
        }
    }
}
