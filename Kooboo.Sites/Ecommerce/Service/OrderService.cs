using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class OrderService : ServiceBase<Order>
    {
        //only order some of the shopping cart item... 
        public Order CreateOrder(List<CartItem> CartItems, Guid CustomerId, Guid AddressId)
        {
            Cart cart = new Cart();
            cart.Items = CartItems;
            ServiceProvider.Cart(this.Context).CalculatePromotion(cart); 
            return CreateOrder(cart, CustomerId, AddressId);
        }

        public Order CreateOrder(Cart cart, Guid CustomerId, Guid AddressId)
        {
            var shipping = ServiceProvider.Shipping(this.Context).CalculateCost(cart);

            Order neworder = new Order();
            foreach (var item in cart.Items)
            {
                neworder.Items.Add(Mapper.ToOrderLine(item));
            }
            neworder.Discount = cart.Discount;
            neworder.ShippingCost = shipping;

            neworder.CustomerId = CustomerId;
            neworder.AddressId = AddressId;

            this.Repo.AddOrUpdate(neworder);
            return neworder;
        }
    }
}
