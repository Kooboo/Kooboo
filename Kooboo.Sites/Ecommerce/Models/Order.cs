using Kooboo.Sites.Ecommerce.Promotion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Models
{
    public class Order : Kooboo.Sites.Models.CoreObject
    {
        public Order()
        {
            this.ConstType = ConstObjectType.Order;
        }
        private Guid _id;
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

        public Guid CustomerId { get; set; }

        public Guid PaymentRequestId { get; set; }

        public Guid AddressId { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsPaid { get; set; }

        public decimal ItemTotal
        {
            get
            {
                decimal total = 0;
                if (_items != null)
                {
                    foreach (var item in _items)
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

                total += this.ShippingCost;

                if (total < 0)
                {
                    total = 0;
                }
                return total;
            }
        }

        public Discount Discount { get; set; }

        private List<OrderLine> _items;
        public List<OrderLine> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<OrderLine>();
                }
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        public decimal ShippingCost { get; set; }
    }
}
