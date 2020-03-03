using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Kooboo.Sites.Ecommerce.KScript
{

    public class KOrder
    {
        private CommerceContext context { get; set; }

        private OrderService service { get; set; }

        public KOrder(CommerceContext context)
        {
            this.context = context;
            this.service = Kooboo.Sites.Ecommerce.ServiceProvider.GetService<OrderService>(this.context.RenderContext);
        }

        [Description("Create an order based on current shopping cart items")]
        public Order Create()
        {
            var cartservice = ServiceProvider.GetService<CartService>(this.context.RenderContext);
            var cart = cartservice.GetOrCreateCart();
            if (cart.Items.Any())
            {
                var order = this.service.CreateOrder(cart, this.context.customer.Id);
                return order;
            }
            return null;
        }

      
        public Order CreateSelected(object[] selected)
        {
            List<Guid> selectedCartItem = new List<Guid>();
            foreach (var item in selected)
            {
                if (item !=null)
                {
                    var stritem = item.ToString();
                    
                    if (System.Guid.TryParse(stritem, out Guid id))
                    {
                        selectedCartItem.Add(id); 
                    }
                }
            }

            var cartservice = ServiceProvider.GetService<CartService>(this.context.RenderContext);
            var cart = cartservice.GetOrCreateCart();
            if (cart.Items.Any() && selectedCartItem.Any())
            { 
                var order = this.service.CreateOrder(cart.Items.FindAll(o=>selectedCartItem.Contains(o.Id)).ToList(), this.context.customer.Id);
                return order;
            }

            return null;
        }
        
        
    }





}
