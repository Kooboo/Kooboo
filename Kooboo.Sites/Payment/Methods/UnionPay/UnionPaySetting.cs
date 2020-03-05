using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.UnionPay
{
    public class UnionPaySetting : IPaymentSetting
    {
        public string Name => "UnionPay";

        public string FrontTransactionUrl { get; set; }

        // need kooboo create a page to show pay result
        public string ReturnUrl { get; set; }

        public string NotifyURL { get; set; }

        public string MerchantID { get; set; }
    }
}
