namespace Kooboo.Sites.Payment.Methods.Mollie
{
    public class MollieSetting : IPaymentSetting
    {
        public string Name => "MolliePayment";

        public string RedirectUrl { get; set; }

        public string ApiToken { get; set; }
    }
}