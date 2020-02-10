using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment
{
    public enum PaymentStatus
    {
        NotAvailable = 0,
        Authorized =1,
        Pending = 2,
        Paid = 3,
        Cancelled = 4,
        Rejected = 5
    } 
}
