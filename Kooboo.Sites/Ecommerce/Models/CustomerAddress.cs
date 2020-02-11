using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.Ecommerce.Models
{
  public   class CustomerAddress : CoreObject
    { 
        public Guid CustomerId { get; set; }

        public string Address { get; set; }

        public string PostCode { get; set; }

        public string Address2 { get; set; }

        public string HouseNumber { get; set; }

        public string City { get; set; }

        public string Country { get; set; } 
    }
}
