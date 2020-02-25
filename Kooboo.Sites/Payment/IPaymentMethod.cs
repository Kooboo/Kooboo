using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.Payment
{
    public interface IPaymentMethod<T> : IPaymentMethod where T:IPaymentSetting
    { 
        [Description("Account settig to be used for this payment method")]
        T Setting { get; set; } 
    }

    public interface IPaymentMethod
    {
        [Description("The name that can be used for k.payment.{name}")]
        string Name { get; }

        string DisplayName { get; }

        string Icon { get; }

        [Description("Img/Base64")] 
        string IconType { get; }
         
        /// <summary>
        /// The support currency code in capital.
        /// </summary>
        List<string> supportedCurrency { get; }
           
        RenderContext Context { get; set; }

        IPaymentResponse Charge(PaymentRequest request);

        PaymentStatusResponse checkStatus(PaymentRequest request);

    }

    public interface IPaymentSetting : ISiteSetting
    {

    }
     
    public interface IKoobooPayment
    {

    }
}
