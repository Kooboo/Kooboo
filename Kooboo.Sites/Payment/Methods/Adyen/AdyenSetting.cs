namespace Kooboo.Sites.Payment.Methods.Adyen
{
    public class AdyenSetting : IPaymentSetting
    {
        public string Name => "AdyenPayment";

        public bool UseSandBox { get; set; }

        //public string BackUrl { get; set; }

        public string XApiKey { get; set; }

        public string MerchantAccount { get; set; }

        public string LiveUrlPrefix { get; set; }

        public string ReturnUrl { get; set; }

        public string CheckoutEndpoint => UseSandBox
            ? "https://checkout-test.adyen.com"
            : $"https://{LiveUrlPrefix}-checkout-live.adyenpayments.com/checkout";

        public string NotifyUrlUserName { get; set; }

        public string NotifyUrlPassword { get; set; }
    }
}