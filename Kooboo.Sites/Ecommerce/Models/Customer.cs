using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Models
{
  public class Customer : CoreObject
    {
        public Customer()
        {
            this.ConstType = ConstObjectType.Customer;  
        }

        private Guid _id; 
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    { 
                        _id = Data.IDGenerator.GetId(Name);
                    } 
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }
          
        // Hashed password..
        public string Password { get; set; }
         
        public string EmailAddress { get; set; }

        public Guid EmailId { get; set; }
         
        public string Telephone { get; set; }

        public Guid TelHash { get; set; }

        public Guid DefaultShippingAddress { get; set; }

        public Guid DefaultPaymentId { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string Address2 { get; set; }

        public string HouseNumber { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
