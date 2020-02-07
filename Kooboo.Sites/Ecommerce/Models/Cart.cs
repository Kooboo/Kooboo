using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Promotion;
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

        public decimal ItemTotal
        {
            get
            {
                decimal total = 0;
                if (_cartitems != null)
                {
                    foreach (var item in _cartitems)
                    {
                        total += item.ItemTotal;
                    }
                }
                return total;
            }
        }

        public decimal TotalAmount
        {
            get
            {
                var total = ItemTotal; 

                if (Discount != null && Discount.Total > 0)
                {
                    total = total - Discount.Total;
                }
                if (total < 0)
                {
                    total = 0;
                }
                return total;
            }
        }

        public Discount Discount { get; set; }

        private List<CartItem> _cartitems;
        public List<CartItem> Items
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
