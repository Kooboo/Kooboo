using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Sites.Payment.Methods.XMLCommon;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class TransactionRequest : Request
    {
        public const string CREATE_TRANSACTION = "create_transaction";

        public TransactionCreditCardRequest CreditCard { get; set; }
        public decimal Amount { get; set; }
        public TransactionOptionsRequest Options { get; set; }
        public string PaymentMethodNonce { get; set; }
        public string Type { get; set; }

        public TransactionRequest()
        {
        }

        public override string Kind()
        {
            return CREATE_TRANSACTION;
        }

        public override string ToXml()
        {
            return ToXml("transaction");
        }

        public override string ToXml(string root)
        {
            return BuildRequest(root).ToXml();
        }

        public override string ToQueryString()
        {
            return ToQueryString("transaction");
        }

        public override string ToQueryString(string root)
        {
            return BuildRequest(root).ToQueryString();
        }

        protected virtual RequestBuilder BuildRequest(string root)
        {
            var builder = new RequestBuilder(root);

            if (Amount != 0) builder.AddElement("amount", Amount);
            builder.AddElement("payment-method-nonce", PaymentMethodNonce);
            if (!string.IsNullOrEmpty(Type))
                builder.AddElement("type", Type.ToString().ToLower());
            builder.AddElement("options", Options);
            return builder;
        }
    }
}
