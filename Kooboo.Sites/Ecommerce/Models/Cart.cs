using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Ecommerce.Models
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
                    _id = Lib.Helper.IDHelper.NewTimeGuid(DateTime.Now); 
                }
                return _id;
            } 
            set
            {
                _id = value;
            }
        }
         
        /// <summary>
        /// for users that is not login yet. 
        /// </summary>
        public Guid tempCustomerId { get; set; }
          
        public Guid CustomerId { get; set; }

        public DateTime CreattionTime { get; set; }

        public bool IsOrder { get; set; }

        public decimal TotalAmount { get; set; }

        public List<Guid> DiscountRules { get; set; }

        public decimal  Discount { get; set; }

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
    }
}
