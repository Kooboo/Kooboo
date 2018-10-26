using System;
 
using Kooboo.Data;
using Kooboo.Data.Attributes;

namespace Kooboo.Data.Models
{
    public class Order : IGolbalObject
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                    _id = Guid.NewGuid();
                return _id;
            }
            set { _id = value; }
        }
        public Guid OrganizationId { get; set; }

        private SalesItem _item;

        [SqlIngore]
        public SalesItem Item
        {
            get { return _item; }
            set
            {
                _saleItem = Lib.Helper.JsonHelper.Serialize(value);
                _item = value;
            }
        }

        private string _saleItem;

        public string SaleItem
        {
            get { return _saleItem; }
            set
            {
                _item = Lib.Helper.JsonHelper.Deserialize<SalesItem>(value);
                _saleItem = value;
            }
        }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; }

        public decimal TotalPrice
        {
            get;set;
        }

        public string Currency { get; set; }
        
    }

    public enum OrderStatus
    {
        Unknown,
        Unpaid,
        Paid,
        Canceled,
        Delivered
    }
}
