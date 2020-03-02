using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment
{
    public class PaymentRequest : CoreObject, IGolbalObject
    {
        public PaymentRequest()
        {
            this.ConstType = ConstObjectType.PaymentRequest;
        }

        private Guid _id;

        public override Guid Id
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

        //additional info if needed. 
        public string Description { get; set; }

        [KIgnore]
        [Obsolete]
        public Guid OrganizationId { get; set; }

        [KIgnore]
        [Obsolete]
        public Guid WebSiteId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; }

        public string Country { get; set; }

        public Guid OrderId { get; set; }

        public string Order { get; set; }

        public string UserIp { get; set; }

        public string PaymentMethod { get; set; }

        public bool Paid { get; set; }

        public bool Failed { get; set; }

        /// <summary>
        /// coupon or other codes.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The reference id at the payment provider if any. 
        /// </summary>
        public string ReferenceId { get; set; }

        public string ReturnPath { get; set; }

        private Dictionary<string, object> _Additional;
        public Dictionary<string, object> Additional
        {
            get
            {
                if (_Additional == null)
                {
                    _Additional = new Dictionary<string, object>();
                }
                return _Additional;
            }
            set
            {
                _Additional = value;
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Code + this.Currency + this.Description + this.Failed.ToString() + this.Paid.ToString();

            unique += this.Name + this.OrderId.ToString() + this.Order + this.PaymentMethod;
            unique += this.ReferenceId + this.TotalAmount.ToString() + this.UserIp;
            unique += this.LastModified.ToShortTimeString() + this.CreationDate.ToShortDateString();
            unique += this.ReturnPath;

            if (_Additional != null)
            {
                foreach (var item in _Additional)
                {
                    if (item.Key != null)
                    {
                        unique += item.Key;
                    }
                    if (item.Value != null)
                    {
                        unique += item.Value.ToString();
                    }
                }
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
