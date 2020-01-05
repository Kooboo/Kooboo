using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.ShoppingCart
{
    public class Cart: ISiteObject
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public Guid CustomerId { get; set; }

        public DateTime CreattionTime { get; set; }

        public bool IsOrder { get; set; }

        public decimal TotalAmount { get; set; }

        private Dictionary<Guid, decimal> _itemsdiscounts;
        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public Dictionary<Guid, Decimal> ItemsDiscounts
        {
            get
            {
                if (_itemsdiscounts == null)
                {
                    _itemsdiscounts = new Dictionary<Guid, decimal>();
                }
                return _itemsdiscounts; 
            }
            set
            {
                _itemsdiscounts = value;
            }
        }

        public decimal CartDiscount { get; set; }

        public List<string> DiscountReason { get; set; }

        private List<CartItem> _cartitems; 
        public List<CartItem> CartItems {
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

        public byte ConstType { get; set; }
        public DateTime CreationDate { get; set; }

        private DateTime _lastmodified; 
        public DateTime LastModified {
            get
            {
                if (_lastmodified == default(DateTime))
                {
                    _lastmodified = DateTime.Now; 
                }
                return _lastmodified; 
            }
            set
            {
                _lastmodified = value; 
            }
        }

        private long lastmodifiedtick; 
        public long LastModifiedTick
        {
            get;set;
        }
         
        public string Name { get; set; }
  
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
