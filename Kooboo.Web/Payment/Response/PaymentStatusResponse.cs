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

        public bool IsPaid { get; set; }

        public bool IsCancel { get; set; }

        public string Status
        {
            get
            {
                if (IsPaid)
                {
                    return "PAID"; 
                }
                else if (IsCancel)
                {
                    return "CANCELLED"; 
                }
                else
                {
                    return "OPEN"; 
                }

            }
        }
    }
}
