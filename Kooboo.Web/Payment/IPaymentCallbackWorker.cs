using Kooboo.Data.Context;
using Kooboo.ServerData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment
{
   public interface IPaymentCallbackWorker
    {
        void Process(PaymentCallback callback, RenderContext context); 

    }
}
