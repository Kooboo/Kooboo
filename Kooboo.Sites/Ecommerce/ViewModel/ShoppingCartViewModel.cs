using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Promotion;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class ShoppingCartViewModel
    {
        Cart shoppingcart { get; set; }
        RenderContext context { get; set; }

        public ShoppingCartViewModel(Cart cart, RenderContext context)
        {
            this.shoppingcart = cart;
            this.context = context;
        }

        public CartItemViewModel[] Items
        {
            get
            {
                List<CartItemViewModel> result = new List<CartItemViewModel>();
                foreach (var item in this.shoppingcart.Items)
                {
                    CartItemViewModel model = new CartItemViewModel(item, this.context);
                    result.Add(model);
                }
                return result.ToArray();
            }
        }

        public Guid Id => this.shoppingcart.Id;

        public Guid tempCustomerId => this.shoppingcart.tempCustomerId;

        public Guid CustomerId => this.shoppingcart.CustomerId;

        private Customer _customer;
        public Customer Customer
        {
            get
            {
                if (_customer == null && CustomerId != default(Guid))
                {
                    _customer = ServiceProvider.Customer(this.context).Get(this.CustomerId);
                }
                return _customer;
            } 
        }

        public DateTime CreattionTime => this.shoppingcart.CreattionTime;

        public bool IsOrder => this.shoppingcart.IsOrder;

        public decimal ItemTotal => this.shoppingcart.ItemTotal;

        public decimal TotalAmount => this.shoppingcart.TotalAmount;

        public Discount Discount => this.shoppingcart.Discount;

    }

    public class CartItemViewModel
    {
        RenderContext context { get; set; }

        public CartItemViewModel(CartItem item, RenderContext context)
        {
            this.ProductVariantId = item.ProductVariantId;
            this.ProductId = item.ProductId;
            this.Quantity = item.Quantity;
            this.UnitPrice = item.UnitPrice;
            this.Discount = item.Discount;
            this.ItemTotal = item.ItemTotal;
            this.context = context;
        }

        public ProductVariantsViewModel Variants
        {
            get
            {
                var varants = ServiceProvider.ProductVariants(this.context).Get(this.ProductVariantId);
                return new ProductVariantsViewModel(varants);
            }
        }


        public ProductViewModel Product
        {
            get
            {
                var product = ServiceProvider.Product(this.context).Get(this.ProductId);
                if (product != null)
                {

                    var type = ServiceProvider.ProductType(this.context).Get(product.ProductTypeId);

                    if (type != null)
                    {
                        return new ProductViewModel(product, this.context, type.Properties);
                    }
                }
                return null;
            }
        }

        public Guid ProductVariantId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }

        public Discount Discount { get; set; }

        public decimal ItemTotal
        {
            get; set;
        }

    }


}
