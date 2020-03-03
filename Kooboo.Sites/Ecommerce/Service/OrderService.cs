using Kooboo.Sites.Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Service
{
    public class OrderService : ServiceBase<Order>
    {
        //only order some of the shopping cart item... 
        public Order CreateOrder(List<CartItem> CartItems, Guid CustomerId)
        {
            Cart cart = new Cart();
            cart.Items = CartItems;
            ServiceProvider.Cart(this.Context).CalculatePromotion(cart);
            return CreateOrder(cart, CustomerId);
        }

        public Order CreateOrder(Cart cart, Guid CustomerId)
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

            this.Repo.AddOrUpdate(neworder);
            return neworder;
        }

        public bool AddAddress(Guid OrderId, Guid AddressId)
        {
            var order = this.Repo.Get(OrderId);
            if (order != null)
            {
                this.Repo.Store.UpdateColumn<Guid>(order.Id, o => o.AddressId, AddressId);
                return true;
            }
            return false;
        }

        public bool Paid(Guid OrderId)
        {
            // update an order status to paid.
            var order = this.Repo.Get(OrderId);
            if (order != null)
            {
                this.Repo.Store.UpdateColumn<bool>(order.Id, o => o.IsPaid, true);
                return true;
            }
            return false; 
        }

        public List<Order> List(int skip, int take)
        {
            var list = this.Repo.Store.Where().OrderByDescending().Skip(skip).Take(take);
            return list;
        } 
    }
}
