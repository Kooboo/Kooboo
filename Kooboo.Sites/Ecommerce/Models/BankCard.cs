using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.Models
{
    public class BankCard
    { 
        public Guid CustomerId { get; set; }
        //credit card number. 
        public string Account { get; set; }

        public int ExpirationYear { get; set; }

        public int ExpirationMonth { get; set; }

        public string CVC { get; set; }

        public string Address { get; set; } 
    }
}
