using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;

namespace Kooboo.Web.Api.Implementation.Ecommerce.ViewModel
{
    public class ShoppingCartWebViewModel
    {

        public ShoppingCartWebViewModel(Cart cart, Kooboo.Data.Context.RenderContext context)
        {  
            if (Lib.Helper.IDHelper.IsTempGuid(cart.CustomerId))
            {
                this.UserName = "NoLogin";
            }
            else
            {
                var customerService = Kooboo.Sites.Ecommerce.ServiceProvider.Customer(context);
                var user = customerService.Get(cart.CustomerId);
                if (user != null)
                {
                    this.UserName = user.Name;
                } 
            }

            if (string.IsNullOrWhiteSpace(this.UserName))
            {
                this.UserName = "TEMP"; 
            }

            this.CreationDate = cart.CreationDate;

            var productService = Kooboo.Sites.Ecommerce.ServiceProvider.Product(context);

            string products = null; 
            foreach (var item in cart.Items)
            {
                var product = productService.Get(item.ProductId);
                if (product !=null)
                {
                    string name = product.Name; 
                    if (string.IsNullOrWhiteSpace(name))
                    { name = product.UserKey;  }

                    products += name + " [" + item.Quantity + "]"; 
                }
            }
            this.Products = products; 

        }

        public string UserName { get; set; }

        public decimal Total { get; set; }

        public string Products { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
