using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Web.Payment.Models;
using System.Collections.Generic;

namespace Kooboo.Web.Payment
{
    public interface IPaymentMethod<T> : IPaymentMethod where T:IPaymentSetting
    {
     
    }

    public interface IPaymentMethod
    {
        string Name { get; }

        string DisplayName { get; }

        string Icon { get; }

        string IconType { get; }

        List<string> ForCurrency { get; }

        List<string> ExcludeCurrency { get; }

        IPaymentResponse MakePayment(PaymentRequest request, RenderContext Context);

        PaymentStatusResponse EnquireStatus(PaymentRequest request, RenderContext context);

        bool CanUse(RenderContext context); 
    }

    public interface IPaymentSetting : ISiteSetting
    {

    }
     
    public interface IKoobooPayment
    {

    }
}
