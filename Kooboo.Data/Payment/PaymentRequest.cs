using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    public class PaymentRequest : IGolbalObject, ISiteObject
    {   
        public PaymentRequest() { }
         
        private Guid _id; 
        
        public Guid Id {
            get {
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
        
        // Item name. 
        public string Name { get; set; }

        // additional info if needed. 
        public string Description { get; set; }

        [KIgnore]
        [Obsolete]
        public Guid OrganizationId { get; set; }

        [KIgnore]
        [Obsolete]
        public Guid WebSiteId { get; set; }
          
        public decimal TotalAmount { get; set; }

        public string Currency { get; set; }
         
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
         
        public byte ConstType { get; set; } = ConstObjectType.PaymentRequest;
         
        private DateTime _creationdate; 
        public DateTime CreationDate {
            get
            {
                if (_creationdate == default(DateTime))
                {
                    _creationdate = DateTime.Now; 
                }
                return _creationdate; 
            }
            set
            {
                _creationdate = value;  
            }
        }
         
        private DateTime _lastmodified; 
        public DateTime LastModified {
            get
            {
                if (_lastmodified == default(DateTime))
                {
                    _lastmodified = DateTime.Now; 
                }
                return _lastmodified; 
            }
            set
            {
                _lastmodified = value; 
            }

        }

        private Dictionary<string, object> _Additional; 
        public Dictionary<string,object> Additional {
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

            unique += this.Name + this.OrderId.ToString() + this.PaymentMethod; 
            unique += this.ReferenceId + this.TotalAmount.ToString() + this.UserIp; 
            unique += this.LastModified.ToShortTimeString() + this.CreationDate.ToShortDateString();  
            unique += this.ReturnPath; 

            if (_Additional !=null)
            {
                foreach (var item in _Additional)
                {
                    if (item.Key !=null)
                    {
                        unique += item.Key; 
                    }
                    if (item.Value !=null)
                    {
                        unique += item.Value.ToString(); 
                    }
                } 
            } 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }

    } 
}
