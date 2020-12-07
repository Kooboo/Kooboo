using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla
{
    public class DwollaSetting : IPaymentSetting, ISettingDescription
    {
        public string Name => "DwollaPayment";

        public string Key { get; set; }

        public string Secret { get; set; }

        public bool IsUsingSanbox { get; set; }

        public string FundingSourceId { get; set; }

        public string ReturnUrl { get; set; }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
