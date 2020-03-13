using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class CreateHostedCheckoutRequest
    {
        public Order Order { get; set; }
    }

    public class Order
    {
        public AmountOfMoney AmountOfMoney { get; set; }

        public Customer Customer { get; set; }
    }

    public class Customer
    {
        public Address BillingAddress { get; set; }

        public string MerchantCustomerId { get; set; }
    }

    public class Address
    {
        public string CountryCode { get; set; }
    }

    public class AmountOfMoney
    {
        public string Amount { get; set; }

        public string CurrencyCode { get; set; }
    }

    public  class HostedCheckoutSpecificInput
    {
        public bool? IsRecurring { get; set; }

        public string Locale { get; set; }

        public string ReturnUrl { get; set; }

        public string Variant { get; set; }
    }
}
