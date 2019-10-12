using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;

namespace Kooboo.Web.Payment
{
    public interface IPaymentRequestStore
    {
        void Save(PaymentRequest request, RenderContext context);

        PaymentRequest Get(Guid PaymentRequestId, RenderContext context);
    }
}