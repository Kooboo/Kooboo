using Kooboo.Data;
using System;


namespace Kooboo.ServerData.Models
{
    public class PaymentRequest : IGolbalObject
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

        public Guid OrganizationId { get; set; }

        public Guid WebSiteId { get; set; }

        public string OrganizationName { get; set; }
         
        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "CNY"; 

        public Guid OrderId { get; set; }

        public string UserIp { get; set; }

        public string PaymentMethod { get; set; }  
          
        public bool IsPaid { get; set; }

        // When failed. set to true so that is completed. 
        public bool IsCancel { get; set; } 
   
        // coupon or other codes. 
        public string Code { get; set; }

        // The reference id at the payment provider. 
        public string ProviderReference { get; set; }

        public string ReturnPath { get; set; }

        public DateTime CreationTime { get; set; } = DateTime.Now; 

        public override int GetHashCode()
        { 
            string unique = this.Code + this.Currency + this.Description + this.IsCancel.ToString() + this.IsPaid.ToString();

            unique += this.Name + this.OrderId.ToString() + this.OrganizationId.ToString() + this.OrganizationName + this.PaymentMethod;

            unique += this.ProviderReference + this.TotalAmount.ToString() + this.UserIp + this.WebSiteId.ToString();

            unique += this.ReturnPath; 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }

    } 
}
