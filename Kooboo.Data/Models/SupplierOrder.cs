using System;

namespace Kooboo.Data.Models
{
    public class SupplierOrder : IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public Guid SupplierId { get; set; }

        public Guid SupplierUserId { get; set; }

        public string SupplierUserName { get; set; }

        public string Expertise { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public DateTime CreateTime { get; set; }

        public SupplierOrderStatus Status { get; set; }
    }

    public enum SupplierOrderStatus
    {
        Pending=0,
        UnFinished=1,
        Finished=2
    }
}
