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

        public Guid PaymentId { get; set; }

        public Guid ShippingId { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsPaid { get; set; }
    }
}
