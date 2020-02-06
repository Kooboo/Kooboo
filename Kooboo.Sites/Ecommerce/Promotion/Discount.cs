using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Promotion
{
    public class Discount
    {
        public decimal Total { get; set; }

        private List<DiscountItem> _items;
        public List<DiscountItem> items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<DiscountItem>();
                }
                return _items; 
            }
            set
            {
                _items = value; 
            }
        } 
    }

    public class DiscountItem
    {
        public Guid RuleId { get; set; }

        public Guid ProductVariantId { get; set; }

        public decimal Discount { get; set; }

        public int Quantity { get; set; }

        public bool CanCombine { get; set; }

        public decimal Total {
            get
            {
                return this.Quantity * this.Discount; 
            }
        }

        public string Reason { get; set; }
    }
}
