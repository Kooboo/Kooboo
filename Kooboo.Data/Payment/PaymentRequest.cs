using Kooboo.Data.Interface;
using System;

namespace Kooboo.Data.Models
{
    public class PaymentRequest : IGolbalObject, ISiteObject
    {
        public PaymentRequest()
        {
        }

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
            set => _id = value;
        }

        // Item name.
        public string Name { get; set; }

        // additional info if needed.
        public string Description { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid WebSiteId { get; set; }

        public string OrganizationName { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "CNY";

        public Guid OrderId { get; set; }

        public string Order { get; set; }

        public string UserIp { get; set; }

        public string PaymentMethod { get; set; }

        public bool IsPaid { get; set; }

        // When failed. set to true so that is completed.
        public bool IsCancel { get; set; }

        // coupon or other codes.
        public string Code { get; set; }

        // The reference id at the payment provider.
        public string Reference { get; set; }

        public string ReturnPath { get; set; }

        public byte ConstType { get; set; } = ConstObjectType.PaymentRequest;

        private DateTime _creationdate;

        public DateTime CreationDate
        {
            get
            {
                if (_creationdate == default(DateTime))
                {
                    _creationdate = DateTime.Now;
                }
                return _creationdate;
            }
            set => _creationdate = value;
        }

        private DateTime _lastmodified;

        public DateTime LastModified
        {
            get
            {
                if (_lastmodified == default(DateTime))
                {
                    _lastmodified = DateTime.Now;
                }
                return _lastmodified;
            }
            set => _lastmodified = value;
        }

        public override int GetHashCode()
        {
            string unique = this.Code + this.Currency + this.Description + this.IsCancel.ToString() + this.IsPaid.ToString();

            unique += this.Name + this.OrderId.ToString() + this.OrganizationId.ToString() + this.OrganizationName + this.PaymentMethod;

            unique += this.Reference + this.TotalAmount.ToString() + this.UserIp + this.WebSiteId.ToString();

            unique += this.LastModified.ToShortTimeString() + this.CreationDate.ToShortDateString();

            unique += this.ReturnPath;

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}