namespace Kooboo.Sites.Payment.Methods.Smart2Pay
{
    public class Smart2PaySetting : IPaymentSetting
    {
        public string Name => "Smart2PayPayment";

        public bool UseSandBox { get; set; }

        public string ReturnUrl { get; set; }

        public string RestApiSiteId { get; set; }

        public string RestApiApiKey { get; set; }

        public string Endpoint => UseSandBox ? "https://paytest.smart2pay.com" : "https://globalpay.smart2pay.com";
    }
}