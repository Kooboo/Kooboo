using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Sites.Payment.Methods.XMLCommon;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class TransactionCreditCardRequest : BaseCreditCardRequest
    {
        public string Token { get; set; }

        protected override RequestBuilder BuildRequest(string root)
        {
            return base.BuildRequest(root).AddElement("token", Token);
        }
    }
}
