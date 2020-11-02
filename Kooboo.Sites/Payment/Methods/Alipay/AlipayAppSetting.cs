using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Alipay
{
    public class AlipayAppSetting :IPaymentSetting
    {
        public  string Name => "AlipayAppPayment";
        public string APPId { get; set; }
        public string SignType { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public bool UseSandBox { get; set; }

        public string ServerUrl
        {
            get
            {
                if (UseSandBox)
                {
                    return "https://openapi.alipaydev.com/gateway.do";
                }
                return "https://openapi.alipay.com/gateway.do";

            }
        }
    }
}
