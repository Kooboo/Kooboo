using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Web.Payment.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Web.Payment
{
    public interface IPaymentMethod<T> : IPaymentMethod where T:IPaymentSetting
    { 
        [Description("Account settig to be used for this payment method")]
        T Setting { get; set; } 
    }

    public interface IPaymentMethod
    {
        string Name { get; }

        string DisplayName { get; }

        string Icon { get; }

        [Description("Img/Base64")] 
        string IconType { get; }

        [KIgnore]
        List<string> ForCurrency { get; }

        [KIgnore]
        List<string> ExcludeCurrency { get; }
         
        RenderContext Context { get; set; }

        IPaymentResponse Charge(PaymentRequest request);

        PaymentStatusResponse EnquireStatus(PaymentRequest request);
         

    }

    public interface IPaymentSetting : ISiteSetting
    {

    }
     
    public interface IKoobooPayment
    {

    }
}
