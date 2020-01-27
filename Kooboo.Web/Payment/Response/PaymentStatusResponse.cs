using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.Payment.Models
{
   public class PaymentStatusResponse
    { 
        public bool HasResult { get; set; }

        public bool Paid { get; set; }

        public bool Failed { get; set; }
    
        public  PaymentStatus Status
        {
            get;set;
        }

        public string Message { get; set; }
    }
}
