using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareSetting : IPaymentSetting
    {
        public string Name => "SquarePay";

        public string ApplicationId { get; set; }

        public string AccessToken { get; set; }

        public string LocationId { get; set; }

        public string PaymentURL { get; set; }
    }
}
