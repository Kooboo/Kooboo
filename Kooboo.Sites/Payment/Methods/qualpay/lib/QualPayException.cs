using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.qualpay.lib
{
    public class QualPayException : Exception
    {
        public QualPayException(string msg) : base(msg)
        {

        }
    }
}
