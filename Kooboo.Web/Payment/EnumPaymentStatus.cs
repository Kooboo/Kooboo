using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment
{
    public enum PaymentStatus
    {
        NotAvailable = 0,
        Pending = 1,
        Success = 2,
        //Complete = 3,
        Cancelled = 4,
        Rejected = 5
    } 
}
