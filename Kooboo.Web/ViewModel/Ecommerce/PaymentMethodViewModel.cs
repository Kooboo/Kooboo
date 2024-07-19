using Kooboo.Data.Context;
using Kooboo.Sites.Payment;

namespace Kooboo.ServerData.ViewModel
{
    public class PaymentMethodViewModel
    {
        public PaymentMethodViewModel(IPaymentMethod method, RenderContext context)
        {
            this.Icon = method.Icon;
            this.IconType = method.IconType;
            this.DisplayName = method.Name;
        }

        //???? what is this???
        public string Type { get; set; }

        public string IconType { get; set; }

        public string Icon { get; set; }

        public string DisplayName { get; set; }
    }
}
