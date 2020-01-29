using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Response
{
   public enum EnumResponseType
    {
        pending =0,
        paid = 1,
        failed = 2,
        submitdata = 3, 
        redirect = 4,
        qrcode = 5,
        hiddenform = 6  // Return a form with hidden fields and values, and automatic submit. 
    }
}
