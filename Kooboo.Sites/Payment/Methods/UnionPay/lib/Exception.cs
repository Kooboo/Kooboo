using System;
using System.Collections.Generic;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class UnionPayException : Exception 
    {
        public UnionPayException(string msg) : base(msg) 
        {

        }
     }
}