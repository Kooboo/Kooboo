using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KScript
{
    public class Kcommerce : IkScript
    {
        public string Name => "Ecommerce";

        public RenderContext context { get; set; }

        public Kooboo.Sites.Ecommerce.ShoppingCart.Cart Cart
        {
            get
            {
                return Kooboo.Sites.Ecommerce.ShoppingCart.CartManager.GetCart(context);
            } 
        }
         
    }
}
