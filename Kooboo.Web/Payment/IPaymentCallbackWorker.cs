using Kooboo.Data.Context;

namespace Kooboo.Web.Payment
{
    public interface IPaymentCallbackWorker
    {
        void Process(PaymentCallback callback, RenderContext context);
    }
}