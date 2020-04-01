using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.UnionPay
{
    public class UnionPaySetting : IPaymentSetting
    {
        public string Name => "UnionPay";

        public bool UseSandBox { get; set; }

        public string FrontTransactionUrl
        {
            get
            {
                if (UseSandBox)
                {
                    return "https://gateway.test.95516.com/gateway/api/frontTransReq.do";
                }
                return "https://gateway.95516.com/gateway/api/frontTransReq.do";

            }
        }

        /// <summary>
        /// 前台通知地址  need kooboo create a page to show pay result
        /// </summary>
        public string FrontUrl { get; set; }

        public string SingleQueryUrl { get; set; }

        public string MerchantID { get; set; }

        public SettingFile MerchantSignCertPFX { get; set; }

        public SettingFile RootCertCER { get; set; }

        public SettingFile MiddleCertCER { get; set; }

        public string SignCertPasswrod { get; set; }
    }
}
