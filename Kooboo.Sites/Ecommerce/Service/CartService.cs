using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.ShoppingCart;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class CartService : ServiceBase<Cart>
    {

        public Cart GetCart()
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

        }

        public void RemoveItem(Guid CartItemId)
        {

        }

        public void ChangeQuantity(Guid CartItemId, int newQuantity)
        {

        }


    }
}
