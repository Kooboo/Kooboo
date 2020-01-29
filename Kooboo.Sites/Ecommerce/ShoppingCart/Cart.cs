using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.ShoppingCart
{
    public class Cart : CoreObject
    { 
        private Guid _id; 
        [Kooboo.Attributes.SummaryIgnore]
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = Guid.NewGuid(); 
                }
                return _id;
            } 
            set
            {
                _id = value;
            }
        }
         
        public Guid SessionId { get; set; }
          
        public Guid CustomerId { get; set; }

        public DateTime CreattionTime { get; set; }

        public bool IsOrder { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal CartDiscount { get; set; }

        public List<string> DiscountReason { get; set; }

        private List<CartItem> _cartitems;
        public List<CartItem> CartItems
        {
            get
            {
                if (_cartitems == null)
                {
                    _cartitems = new List<CartItem>();
                }
                return _cartitems;
            }
            set
            {
                _cartitems = value;
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
