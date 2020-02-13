using System;
using System.Collections.Generic;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class AliPayException : Exception 
    {
        public AliPayException(string msg) : base(msg) 
        {

        }
     }
}