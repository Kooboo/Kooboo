using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla
{
    public class DwollaSetting : IPaymentSetting
    {
        public string Name => "DwollaPayment";

        public string Key { get; set; }

        public string Secret { get; set; }

        public bool IsUsingSanbox { get; set; }

        public string FundingSourceId { get; set; }

        public string ReturnUrl { get; set; }
    }
}
