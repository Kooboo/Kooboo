using Kooboo.Data.Context;
using Kooboo.ServerData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Payment
{
   public interface  IPaymentRequestStore
    {
        void Save(PaymentRequest request, RenderContext context);
    }
}
