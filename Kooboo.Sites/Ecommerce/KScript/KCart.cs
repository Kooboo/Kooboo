using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Promotion;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Ecommerce.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.KScript
{
    public class KCart
    {
        private Cart ShoppingCart { get; set; }
         
        private RenderContext context { get; set; }

        private ICartService service { get; set; }

        public KCart(RenderContext context)
        {
            this.context = context;
            this.service = ServiceProvider.Cart(this.context);
            this.ShoppingCart = this.service.GetOrCreateCart();
        }

        public void AddITem(object ProductVariantId, int quantity = 1)
        {
            Guid id = Lib.Helper.IDHelper.GetGuid(ProductVariantId);
            this.service.AddITem(id, quantity);
        }

        public void RemoveItem(object variantId)
        {
            Guid id = Lib.Helper.IDHelper.GetGuid(variantId);
            this.service.RemoveItem(id);
        }

        public void ChangeQuantity(object ProductVariantId, int newQuantity)
        {
            Guid id = Lib.Helper.IDHelper.GetGuid(ProductVariantId);
            this.service.ChangeQuantity(id, newQuantity);
        }


        public CartItemViewModel[] Items
        {
            get
            {
                List<CartItemViewModel> result = new List<CartItemViewModel>();
                foreach (var item in this.ShoppingCart.Items)
                {
                    CartItemViewModel model = new CartItemViewModel(item, this.context);
                    result.Add(model);
                }
                return result.ToArray();
            }
        }

        public Guid Id => this.ShoppingCart.Id;

        public Guid tempCustomerId => this.ShoppingCart.tempCustomerId;

        public Guid CustomerId => this.ShoppingCart.CustomerId;

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

        public DateTime CreattionTime => this.ShoppingCart.CreattionTime;

        public bool IsOrder => this.ShoppingCart.IsOrder;

        public decimal ItemSubTotal => this.ShoppingCart.ItemTotal;

        public decimal TotalAmount => this.ShoppingCart.TotalAmount;

        public Discount Discount => this.ShoppingCart.Discount; 
    }


}
