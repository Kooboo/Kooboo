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

        public Guid WebSiteId { get; set; }

        private SalesItem _item;

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

        public decimal TotalAmount
        {
            get; set;
        }

        public string Currency { get; set; }

        // Delivery method. 
        public string Delivery { get; set; }

        private OrderLine _Orderline;

        public OrderLine OrderLine
        {
            get
            {
                if (_Orderline == null)
                {
                    _Orderline = new OrderLine();
                }
                return _Orderline;

            }
            set { _Orderline = value; }
        }

        public bool IsPaid { get; set; }

        public bool IsCancel { get; set; }

        // Used for like domain name.. 
        public string Name { get; set; }

        // for coupon code or others... 
        public string Code { get; set; }

    }

    public enum OrderStatus
    {
        Unknown,
        Unpaid,
        Paid,
        Canceled,
        Delivered
    }

    public class OrderLine
    {
        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalAmount { get; set; }


    }

}
