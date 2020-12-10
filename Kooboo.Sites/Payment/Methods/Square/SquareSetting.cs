using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareSetting : IPaymentSetting, ISettingDescription
    {
        public string Name => "SquarePay";

        public bool UseSandBox { get; set; }

        public string ApplicationId { get; set; }

        public string AccessToken { get; set; }

        public string LocationId { get; set; }

        public string BaseURL => UseSandBox ? "https://connect.squareupsandbox.com" : "https://connect.squareup.com";

        public string RedirectURL { get; set; }

        public string Group => "Payment";

        public string GetAlert(RenderContext renderContext)
        {
            return string.Empty;
        }
    }
}
