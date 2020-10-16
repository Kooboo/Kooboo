using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.wechat
{
    public class WeChatAppSetting : IPaymentSetting
    {
        public string Name => "WeChatAppPay";

        public string AppId { get; set; }

        public string MerchantId { get; set; }

        public string Key { get; set; }

        //public string AppSecret { get; set; }
    }
}
