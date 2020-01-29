using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment
{
   public interface IPaymentCallbackWorker
    {
        void Process(PaymentCallback callback, RenderContext context);  
    }
}
