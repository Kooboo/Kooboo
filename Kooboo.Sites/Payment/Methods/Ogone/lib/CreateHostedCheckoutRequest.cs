using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Ogone.lib
{
    public class CreateHostedCheckoutRequest
    {
        public Order Order { get; set; }

        public HostedCheckoutSpecificInput HostedCheckoutSpecificInput { get; set; } = null;

        public CardPaymentMethodSpecificInputBase CardPaymentMethodSpecificInput { get; set; } = null;
    }

    public class CardPaymentMethodSpecificInputBase
    {
        public string AuthorizationMode { get; set; } = null;
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
        public bool? IsRecurring { get; set; } = null;

        public string Locale { get; set; } = null;

        public string ReturnUrl { get; set; } = null;

        public string Variant { get; set; } = null;

        public PaymentProductFiltersHostedCheckout PaymentProductFilters { get; set; } = null;

        public bool? ReturnCancelState { get; set; } = null;

        public bool? ShowResultPage { get; set; } = null;

        public string Tokens { get; set; } = null;
    }

    public class PaymentProductFiltersHostedCheckout
    {
        public PaymentProductFilter Exclude { get; set; } = null;

        public PaymentProductFilter RestrictTo { get; set; } = null;

        public bool? TokensOnly { get; set; } = null;
    }

    public class PaymentProductFilter
    {
        public IList<string> Groups { get; set; } = null;

        public IList<int> Products { get; set; } = null;
    }
}
