using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareSetting : IPaymentSetting
    {
        public string Name => "SquarePay";

        public string Environment { get; set; }

        public string ApplicationId { get; set; }

        public string AccessToken { get; set; }

        public string LocationId { get; set; }
    }
}
