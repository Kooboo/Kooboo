using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.wechat
{
    public class WeChatAppSetting : IPaymentSetting, ISettingDescription
    {
        public string Name => "WeChatAppPay";

        public string AppId { get; set; }

        public string MerchantId { get; set; }

        public string Key { get; set; }

        //public string AppSecret { get; set; }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
