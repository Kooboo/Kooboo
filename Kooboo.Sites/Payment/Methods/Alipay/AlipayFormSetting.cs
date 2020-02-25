using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class AlipayFormSetting : IPaymentSetting
    {
        public string Name => "AlipayFormPayment";

        public string APPId { get; set; }
        public string Format { get; set; }
        public string Charset { get; set; }
        public string SignType { get; set; }
        public string Version { get; set; }
        public string ReturnUrl { get; set; }
        public string ServerUrl { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
    }
}
