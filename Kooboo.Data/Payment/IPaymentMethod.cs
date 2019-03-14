using Kooboo.ServerData.Models;
using Kooboo.ServerData.Payment.Models;
using System.Collections.Generic;

namespace Kooboo.ServerData.Payment.Methods
{
    public interface IPaymentMethod
    {
        string Name { get;  }

        string DisplayName { get; }

        string Icon { get;  }

        string IconType { get; }

        List<string> ForCurrency { get;   }

        List<string> ExcludeCurrency { get; }

        IPaymentResponse MakePayment(PaymentRequest request);

        PaymentStatusResponse EnquireStatus(PaymentRequest request); 
        
    }
}
