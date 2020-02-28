using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kooboo.Sites.Payment.Methods.Braintree.lib
{
    public class BraintreeAPI
    {
        private const string SALE = "sale";

        private readonly BraintreeService Service;

        public BraintreeSetting setting;

        public BraintreeAPI(BraintreeSetting setting)
        {
            this.setting = setting;
            Service = new BraintreeService(setting);
        }

        public virtual string Sale(TransactionRequest request)
        {
            request.Type = SALE;
            XmlNode response = Service.Post(string.Format("{0}/merchants/{1}/transactions", setting.ServerUrl, setting.MerchantId), request);

            return "";
        }

        public string Generate(ClientTokenRequest request = null)
        {
            if (request == null) request = new ClientTokenRequest();
            VerifyOptions(request);
            XmlNode response = Service.Post(string.Format("{0}/merchants/{1}/client_token", setting.ServerUrl, setting.MerchantId), request);

            if (response.Name.Equals("client-token"))
            {
                return Regex.Unescape(response.InnerText);
            }
            else
            {
                throw new ArgumentException(response.SelectSingleNode("message").InnerText);
            }
        }

        private void VerifyOptions(ClientTokenRequest request)
        {
            if (request.Options != null && request.CustomerId == null)
            {
                var invalidOptions = new List<string> { };

                if (request.Options.VerifyCard != null)
                {
                    invalidOptions.Add("VerifyCard");
                }
                if (request.Options.MakeDefault != null)
                {
                    invalidOptions.Add("MakeDefault");
                }
                if (request.Options.FailOnDuplicatePaymentMethod != null)
                {
                    invalidOptions.Add("FailOnDuplicatePaymentMethod");
                }

                if (invalidOptions.Count != 0)
                {
                    var message = "Following arguments are invalid without customerId: ";
                    foreach (var invalidOption in invalidOptions)
                    {
                        message += " " + invalidOption;
                    }
                    throw new ArgumentException(message);
                }
            }
        }
    }
}
