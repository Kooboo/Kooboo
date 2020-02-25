using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Square;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Methods.Square.lib.Models.Checkout;
using Kooboo.Sites.Payment.Models;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareForm : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquareForm";

        public string DisplayName => throw new NotImplementedException();

        public string Icon => throw new NotImplementedException();

        public string IconType => throw new NotImplementedException();

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("USD");
                return list;
            }
        }

        public RenderContext Context { get; set; }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            var res = new PaidResponse();
            if (this.Setting == null)
            {
                return null;
            }

            // todo 需要转换为货币的最低单位 
            // square APi 货币的最小面额指定。例如，美元金额以美分指定，https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts
            var amount = new Money { Amount = SquareCommon.GetSquareAmount(request.TotalAmount), Currency = request.Currency };

            object squareResponseNonce;
            if (!request.Additional.TryGetValue("nonce", out squareResponseNonce))
            {
                return new FailedResponse("Not get nonce");
            }

            var result = PaymentsApi.CreatPayment(squareResponseNonce.ToString(), amount, Setting);

            var deserializeResult = JsonConvert.DeserializeObject<PaymentResponse>(result);

            if (deserializeResult.Payment.Status == "APPROVED" || deserializeResult.Payment.Status == "COMPLETED")
            {
                res.Type = EnumResponseType.paid;
                res.paymemtMethodReferenceId = deserializeResult.Payment.ID;
            }
            else if (deserializeResult.Payment.Status == "CANCELED" || deserializeResult.Payment.Status == "FAILED")
            {
                return new FailedResponse("FAILED");
            }

            return res;
        }

        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse GetHtmlDetail(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            res.html = GetForm(request);

            return res;
        }

        private string GetForm(PaymentRequest request)
        {
            var html = string.Empty;

            var currencySymbol = CurrencyHelper.GetCurrencySymbol(request.Currency);

            html = GenerateHtml(Setting.ApplicationId, Setting.LocationId, Setting.KscriptAPIURL, request.TotalAmount, currencySymbol);

            return html;
        }

        private string GenerateHtml(string applicationId, string locationId, string kscriptAPIURL, decimal amount, string currencySymbol)
        {
            return @"<script type='text/javascript' src='https://js.squareupsandbox.com/v2/paymentform'></script>
<script src='https://code.jquery.com/jquery-3.1.1.min.js'></script>
<div>
	<form id='nonce-form' novalidate='' action='' method='post'>
		<div class='sq-field'>
			<label class='sq-label'>Card Number</label>
			<div id='sq-card-number'></div>
		</div>
		<div class='sq-field-wrapper'>
			<div class='sq-field sq-field--in-wrapper'>
				<label class='sq-label'>CVV</label>
				<div id='sq-cvv'></div>
			</div>
			<div class='sq-field sq-field--in-wrapper'>
				<label class='sq-label'>Expiration</label>
				<div id='sq-expiration-date'></div>
			</div>
			<div class='sq-field sq-field--in-wrapper'>
				<label class='sq-label'>Postal</label>
				<div id='sq-postal-code'></div>
			</div>
		</div>
		<div class='sq-field'>
			<button id='sq-creditcard' class='sq-button' onclick='onGetCardNonce(event)'>
                    Pay " + currencySymbol + amount + @" Now
                </button>
		</div>
		<!--
          After a nonce is generated it will be assigned to this hidden input field.
        -->
		<div id='error'></div>
		<input type='hidden' id='card-nonce' name='nonce'>
	</form>
</div>
<div>
        <p k-content='applicationIdValue'></p>
</div>

<script>
	function onGetCardNonce(event) {
    // Don't submit the form until SqPaymentForm returns with a nonce
    event.preventDefault();

   // Request a nonce from the SqPaymentForm object
   paymentForm.requestCardNonce();
}

var paymentForm = new SqPaymentForm({
            // Initialize the payment form elements
            applicationId:'" + applicationId + @"',
            locationId: '" + locationId + @"',
            inputClass: 'sq-input',

            // Customize the CSS for SqPaymentForm iframe elements
            inputStyles: [{
                backgroundColor: 'transparent',
                color: '#333333',
                fontFamily: '""Helvetica Neue"", ""Helvetica"", sans-serif',
                fontSize: '16px',
                fontWeight: '400',
                placeholderColor: '#8594A7',
                placeholderFontWeight: '400',
                padding: '16px',
                _webkitFontSmoothing: 'antialiased',
                _mozOsxFontSmoothing: 'grayscale'
            }],

            // Initialize the credit card placeholders
            cardNumber: {
                elementId: 'sq-card-number',
                placeholder: '•••• •••• •••• ••••'
            },
            cvv: {
                elementId: 'sq-cvv',
                placeholder: 'CVV'
            },
            expirationDate: {
                elementId: 'sq-expiration-date',
                placeholder: 'MM/YY'
            },
            postalCode: {
                elementId: 'sq-postal-code'
            },

            // SqPaymentForm callback functions
            callbacks: {

                /*
                 * callback function: cardNonceResponseReceived
                 * Triggered when: SqPaymentForm completes a card nonce request
                 */
                cardNonceResponseReceived: function(errors, nonce, cardData, billingContact, shippingContact){
   if (errors)
    {
        var error_html = '';
        for (var i = 0; i < errors.length; i++)
        {
            error_html += '<li> ' + errors[i].message + ' </li>';
        }
        document.getElementById('error').innerHTML = error_html;
        document.getElementById('sq-creditcard').disabled = false;

        return;
    }
    else
    {
        document.getElementById('error').innerHTML = '';
    }

    // Assign the nonce value to the hidden form field
    document.getElementById('card-nonce').value = nonce;

    // POST the nonce form to the payment processing page
    // document.getElementById('nonce-form').submit();
    alert(document.getElementById('card-nonce').value)
                     $.get('" + kscriptAPIURL + @"?nonce=' + nonce, function(data, status){ });
              }
            }
        });
</script>
";
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            //https://connect.squareup.com/v2/payments/{payment_id} 

            // 创建订单后返回的订单编号  {payment_id}
            // request.ReferenceId = "nn2I3hGkDrBqU2MPQy103dzET5UZY";
            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }

            var requestURL = Setting.BaseURL + "/v2/payments/" + request.ReferenceId;

            var httpResult = PaymentsApi.DoHttpGetRequest(requestURL, Setting.AccessToken);

            var deserializeResult = JsonConvert.DeserializeObject<PaymentResponse>(httpResult);

            if (deserializeResult == null)
            {
                return null;
            }

            result = GetPaidStatus(result, deserializeResult.Payment.Status);

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            return SquareCommon.ProcessNotify(context);
        }

        private PaymentStatusResponse GetPaidStatus(PaymentStatusResponse result, string paymentStatus)
        {
            switch (paymentStatus)
            {
                case "APPROVED":
                    result.Status = PaymentStatus.Authorized;
                    break;
                case "COMPLETED":
                    result.Status = PaymentStatus.Paid;
                    break;
                case "CANCELED":
                    result.Status = PaymentStatus.Cancelled;
                    break;
                case "FAILED":
                    result.Status = PaymentStatus.Rejected;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
