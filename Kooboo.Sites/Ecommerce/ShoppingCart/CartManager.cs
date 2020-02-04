using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Repository;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Scripting.Global;
using KScript;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.ShoppingCart
{
    public static class CartManager
    {
        static CartManager()
        {
            SessionCart = new Dictionary<Guid, Cart>();
        }

        // temp solution  
        private static Dictionary<Guid, Cart> SessionCart { get; set; }

        public static Cart GetCart(RenderContext context)
        {
            //var session = new Session(context);

            //if (SessionCart.ContainsKey(id))
            //{
            //    return SessionCart[id];
            //}

            //else
            //{
            //    Cart usercart = new Cart();

            //    SessionCart[id] = usercart;
            //    return usercart;
            //} 

            return null;
        }
    }
}
