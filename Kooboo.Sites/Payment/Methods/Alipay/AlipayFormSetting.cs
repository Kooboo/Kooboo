using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Payment.Methods.Alipay;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class AlipayFormSetting : IPaymentSetting, ISettingDescription
    {
        public string Name => "AlipayFormPayment";

        public string APPId { get; set; }
        public string Format { get; set; }
        public string Charset { get; set; }
        public string SignType { get; set; }
        public string Version { get; set; }
        public string ReturnUrl { get; set; }
        public bool UseSandBox { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }

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

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
