using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment.Response
{
   public enum EnumResponseType
    {
        Pending =0,
        Paid = 1,
        Failed = 2,
        SubmitData = 3, 
        Redirect = 4,
        QrCode = 5,
        AutoFormPost = 6  // Return a form with hidden fields and values, and automatic submit. 
    }
}
