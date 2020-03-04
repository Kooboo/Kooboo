using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Dwolla.lib;
using Kooboo.Sites.Payment.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods.Dwolla
{
    public class DwollaForm : IPaymentMethod<DwollaSetting>
    {
        public DwollaSetting Setting { get; set; }

        public string Name => "DwollaForm";

        public string DisplayName => throw new NotImplementedException();

        public string Icon => throw new NotImplementedException();

        public string IconType => throw new NotImplementedException();

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>
                {
                    "USD",
                    "EUR"
                };
                return list;
            }
        }

        public RenderContext Context { get; set; }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            request.Additional.TryGetValue("firstName", out var firstName);
            request.Additional.TryGetValue("lastName", out var lastName);
            request.Additional.TryGetValue("email", out var email);

            var customerParameters = new Customer()
            {
                FirstName = (string)firstName,
                LastName = (string)lastName,
                Email = (string)email
            };

            var apiClient = new DwollaApi(Setting);
            var customer = apiClient.CreateCustomer(customerParameters).Result;
            var iavToken = apiClient.CreateIavToken(customer.Location.ToString()).Result;
            var response = new HiddenFormResponse
            {
                paymemtMethodReferenceId = Guid.NewGuid().ToString()
            };
            response.html = GetHtml(Setting.IsUsingSanbox, iavToken.Token);
            response.setFieldValues("iavToken", iavToken.Token);
            return response;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        { 
            throw new NotImplementedException();
        }

        private string GetHtml(bool isisUsingSanbox, string iavToken)
        {
            var html = GenerateHtml(iavToken, isisUsingSanbox);
            var kscriptAPIURL = PaymentHelper.GetCallbackUrl(this, nameof(checkStatus), this.Context);

            return html;
        }

        private string GenerateHtml(string iavToken, bool isUsingSanbox)
        {
            var sanboxConfig = isUsingSanbox ? "dwolla.configure('sandbox')" : "";
            var html = string.Format(@"
<script src=""https://cdn.dwolla.com/1/dwolla.js""></script>
<div id=""iavContainer""></div>

<script type=""text/javascript"">
  {0}
  dwolla.iav.start('{1}', {{
    container: 'iavContainer',
    stylesheets: [
      'https://fonts.googleapis.com/css?family=Lato&subset=latin,latin-ext'
    ],
    microDeposits: 'true',
    fallbackToMicroDeposits: 'true'
  }}, function(err, res) {{
    console.log('Error: ' + JSON.stringify(err) + ' -- Response: ' + JSON.stringify(res));
  }});
 </script> ", sanboxConfig, iavToken);

            return html;
        }
    }
}
