using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Dwolla.lib;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
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

            var dwollaApi = new DwollaApi(Setting);
            var customer = dwollaApi.CreateCustomer(customerParameters).Result;
            var iavToken = dwollaApi.CreateIavToken(customer.Location.ToString()).Result;
            var response = new HiddenFormResponse
            {
                paymemtMethodReferenceId = Guid.NewGuid().ToString()
            };
            response.html = GetHtml(Setting.IsUsingSanbox, iavToken.Token);
            response.setFieldValues("iavToken", iavToken.Token);
            return response;
        }

        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse GetFundingSourceAddingStatus(RenderContext context)
        {
            var result = JsonConvert.DeserializeObject<FundingSourceAddingStatus>(context.Request.Body);
            IPaymentResponse response;
            if (result.FundingSource.Href != null)
            {
                var dwollaApi = new DwollaApi(Setting);
                var request = new CreateTransferRequest()
                {
                    Links = new Dictionary<string, Link>
                    {
                        { "source", new Link { Href = result.FundingSource.Href } },
                        { "destination", new Link { Href = new Uri(dwollaApi.ApiBaseAddress + "/funding-sources/" + Setting.FundingSourceId) }}
                    },
                    Amount = new Money
                    {
                        Currency = "USD",
                        Value = 100.00M
                    }
                };
                var createTransferResult = dwollaApi.CreateTransfer(request).Result;
                if (createTransferResult.Status == "Created")
                {
                    response = new PaidResponse();
                }
                else
                {
                    response = new FailedResponse("Create transfer failed");
                }
            }
            else
            {
                response = new FailedResponse("IAV failed");
            }

            return response;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        { 
            throw new NotImplementedException();
        }

        private string GetHtml(bool isisUsingSanbox, string iavToken)
        {
            var apiUrl = PaymentHelper.GetCallbackUrl(this, nameof(GetFundingSourceAddingStatus), Context);
            var html = GenerateHtml(iavToken, isisUsingSanbox, apiUrl);

            return html;
        }

        private string GenerateHtml(string iavToken, bool isUsingSanbox, string apiUrl)
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
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST','{2}',true);
    xmlhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    xmlhttp.send(JSON.stringify(res._links));
  }});
 </script> ", sanboxConfig, iavToken, apiUrl);

            return html;
        }
    }
}
