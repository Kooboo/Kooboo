using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public class CommerceContext
    {
        public RenderContext RenderContext { get; set; }

        public Customer customer
        {
            get; set;
        }

        public ShoppingCart.Cart ShoppingCart {get;set;} 
    }
}
