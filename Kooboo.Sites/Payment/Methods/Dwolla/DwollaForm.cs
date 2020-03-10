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

        public string DisplayName => Name;

        public string Icon => "https://cdn.dwolla.com/com/dist/images/footer/dwolla-logo-footer_d9270fdf.svg";

        public string IconType => "img";

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
            var email = Guid.NewGuid().ToString() + "@email.com";

            var customerParameters = new Customer()
            {
                FirstName = (string)firstName,
                LastName = (string)lastName,
                Email = email
            };

            var money = new Money
            {
                Currency = request.Currency,
                Value = request.TotalAmount
            };

            var dwollaApi = new DwollaApi(Setting);
            var failedResponse = new FailedResponse("Payment failed")
            {
                requestId = request.Id
            };

            var customer = dwollaApi.CreateCustomer(customerParameters).Result;
            if (!customer.IsSuccessStatusCode)
            {
                failedResponse.Message = customer.Content;
                return failedResponse;
            }

            var iavToken = dwollaApi.CreateIavToken(customer.Location.ToString()).Result;
            if (string.IsNullOrEmpty(iavToken.Token))
            {
                failedResponse.Message = "Getting IAV token failed";
                
                return failedResponse;
            }

            var response = new HiddenFormResponse
            {
                requestId = request.Id
            };
            response.html = GetHtml(Setting.IsUsingSanbox, iavToken.Token, money, request);
            return response;
        }

        public PaymentCallback CreateTransfer(RenderContext context)
        {
            var fundingSourceResponse = JsonConvert.DeserializeObject<AddFundingSourceResponse>(context.Request.Body);
            PaymentCallback response = null;
            var fundingSourceLink = fundingSourceResponse.Links.FundingSource.Href;
            if (fundingSourceLink != null)
            {
                var dwollaApi = new DwollaApi(Setting);
                var request = new CreateTransferRequest()
                {
                    Links = new Dictionary<string, Link>
                    {
                        { "source", new Link { Href = fundingSourceLink } },
                        { "destination", new Link { Href = new Uri(dwollaApi.ApiBaseAddress + "/funding-sources/" + Setting.FundingSourceId) } }
                    },
                    Amount = fundingSourceResponse.Money
                };
                var createTransferResult = dwollaApi.CreateTransfer(request).Result;
                if (createTransferResult.Status == "Created")
                {
                    response = new PaymentCallback();
                    var storedRequest = PaymentManager.GetRequest(fundingSourceResponse.RequestId, context);
                    storedRequest.ReferenceId = createTransferResult.TransferURL.ToString();
                    PaymentManager.UpdateRequest(storedRequest, context);
                }
            }
            
            return response;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var result = new PaymentStatusResponse();
            var dwollaApi = new DwollaApi(Setting);

            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }
            var response = dwollaApi.GetTransfer(request.ReferenceId).Result;
            
            if (response.Status == "processed")
            {
                result.Status = PaymentStatus.Paid;
            } 
            else if (response.Status == "failed")
            {
                result.Status = PaymentStatus.Rejected;
            } 
            else if (response.Status == "pending")
            {
                result.Status = PaymentStatus.Pending;
            }
            else if (response.Status == "cancelled")
            {
                result.Status = PaymentStatus.Cancelled;
            }
            
            return result;
        }

        private string GetHtml(bool isisUsingSanbox, string iavToken, Money money, PaymentRequest request)
        {
            var apiUrl = PaymentHelper.GetCallbackUrl(this, nameof(CreateTransfer), Context);
            var html = GenerateHtml(iavToken, isisUsingSanbox, apiUrl, money, request);

            return html;
        }

        private string GenerateHtml(string iavToken, bool isUsingSanbox, string apiUrl, Money money, PaymentRequest request)
        {
            var sanboxConfig = isUsingSanbox ? "dwolla.configure('sandbox')" : string.Empty;
            var redirect = string.IsNullOrEmpty(Setting.TransferCreatedUrl) ? "" : $"window.location.replace('{Setting.TransferCreatedUrl}')";
            var html = string.Format(@"
<script src=""https://cdn.dwolla.com/1/dwolla.js""></script>
<div id=""iavContainer""></div>

<script type=""text/javascript"">
  var request = {{
    money: {{
      currency: '{3}',
      value: {4}
    }},
    requestId: '{5}',
    _links: null
  }}
  {0}
  dwolla.iav.start('{1}', {{
    container: 'iavContainer',
    stylesheets: [
      'https://fonts.googleapis.com/css?family=Lato&subset=latin,latin-ext'
    ],
    microDeposits: 'false',
    fallbackToMicroDeposits: 'false'
  }}, function(err, res) {{
    console.log('Error: ' + JSON.stringify(err) + ' -- Response: ' + JSON.stringify(res));
    request._links = res._links
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST','{2}',true);
    xmlhttp.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    xmlhttp.send(JSON.stringify(request));
    {6}
  }});
 </script> ", sanboxConfig, iavToken, apiUrl, money.Currency, money.Value, request.Id.ToString(), redirect);

            return html;
        }
    }
}
