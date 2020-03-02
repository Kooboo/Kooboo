using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareSetting : IPaymentSetting
    {
        public string Name => "SquarePay";

        public bool UseSandBox { get; set; }

        public string ApplicationId { get; set; }

        public string AccessToken { get; set; }

        public string LocationId { get; set; }

        public string BaseURL => UseSandBox ? "https://connect.squareupsandbox.com" : "https://connect.squareup.com";

        public string RedirectURL { get; set; }

        public string KscriptAPIURL { get; set; }
    }
}
