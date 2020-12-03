using Kooboo.Data.Context;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Payment.Methods.Mollie
{
    public class MollieSetting : IPaymentSetting,ISettingDescription
    {
        public string Name => "MolliePayment";

        public string RedirectUrl { get; set; }

        public string ApiToken { get; set; }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}