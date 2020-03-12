using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Square;
using Kooboo.Sites.Payment.Methods.Square.lib;
using Kooboo.Sites.Payment.Methods.Square.lib.Models;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kooboo.Sites.Payment.Methods
{
    public class SquareForm : IPaymentMethod<SquareSetting>
    {
        public SquareSetting Setting { get; set; }

        public string Name => "SquareForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("square", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-square.png";

        public string IconType => "img";

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

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 2.59; 
    charge.currency='USD';
    charge.desciption='tea';
    var resForm = k.payment.squareForm.charge(charge);
</script>

<div class='jumbotron'>
	<div k-content='resForm.html'></div>
</div>")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            res.html = GetForm(request);

            return res;
        }

        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse CreatPayment(RenderContext context)
        {
            if (this.Setting == null)
            {
                return null;
            }

            // square 货币的最小面额指定。https://developer.squareup.com/docs/build-basics/working-with-monetary-amounts
            var currency = context.Request.Get("currency");
            var totalAmount = decimal.Parse(context.Request.Get("totalAmount"));
            var requestIdStr = context.Request.Get("paymentRequestId");
            var amount = new Money
            {
                Amount = CurrencyDecimalPlaceConverter.ToMinorUnit(currency, totalAmount),
                Currency = currency
            };

            var deserializeResult = PaymentsApi.CreatPayment(context.Request.Get("nonce"), amount, Setting, requestIdStr);

            // 把paymentID赋值到request referenceID 为了后面 checkStatus 使用
            var paymentRequestIdStr = context.Request.Get("paymentRequestId");
            Guid paymentRequestId;
            if (Guid.TryParse(paymentRequestIdStr, out paymentRequestId))
            {
                var request = PaymentManager.GetRequest(paymentRequestId, context);
                request.ReferenceId = deserializeResult.Payment.ID;
                PaymentManager.UpdateRequest(request, context);
            }

            if (deserializeResult.Payment.Status == "APPROVED" || deserializeResult.Payment.Status == "COMPLETED")
            {
                var res = new PaidResponse();
                res.paymemtMethodReferenceId = deserializeResult.Payment.ID;
                return res;
            }
            else if (deserializeResult.Payment.Status == "CANCELED" || deserializeResult.Payment.Status == "FAILED")
            {
                return new FailedResponse("FAILED");
            }
            else
            {
                // TODO: please check.
                return new FailedResponse("No response");
            }
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            //https://connect.squareup.com/v2/payments/{payment_id} 
            // 创建订单后返回的订单编号  {payment_id}
            if (string.IsNullOrEmpty(request.ReferenceId))
            {
                return result;
            }

            var requestURL = Setting.BaseURL + "/v2/payments/" + request.ReferenceId;
            var httpResult = ApiClient.Create("Bearer", Setting.AccessToken)
                .GetAsync(requestURL).Result.Content;

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

        private string GetForm(PaymentRequest request)
        {
            var html = string.Empty;

            var currencySymbol = CurrencyHelper.GetCurrencySymbol(request.Currency);
            var kscriptAPIURL = PaymentHelper.GetCallbackUrl(this, nameof(CreatPayment), this.Context);

            html = GenerateHtml(Setting.ApplicationId, Setting.LocationId, kscriptAPIURL, request.TotalAmount, request.Currency, currencySymbol, request.Id);

            return html;
        }

        private string GenerateHtml(string applicationId, string locationId, string kscriptAPIURL, decimal amount, string currency, string currencySymbol, Guid requestId)
        {
            return @"<script type='text/javascript' src='https://js.squareupsandbox.com/v2/paymentform'></script>
<script src='https://code.jquery.com/jquery-3.1.1.min.js'></script>
<style>
.sq-field-wrapper {
  display: flex;
  flex-flow: row nowrap;
  margin-bottom: 16px;
}

.sq-field {
  margin-bottom: 16px;
  width: 100%;
}

.sq-field:first-child {
  margin-left: 0;
}

.sq-field:last-child {
  margin-right: 0;
}

.sq-field--in-wrapper {
  flex-grow: 1;
  margin: 0 8px;
}
.sq-label {
  margin-bottom: 8px;
  text-transform: uppercase;
}

.sq-input {
  background-color: #fff;
  border-style: solid;
  border-width: 1px;
  overflow: hidden;
  transition: border-color 0.25s ease;
  width: 100%;
}

.sq-input--focus {
  background-color: #fbfdff;
}

.sq-input--error {
  background-color: #fbfdff;
}

.sq-button {
  color: #fff;
  padding: 16px;
  width: 100%;
}

.sq-button:active {
  color: #fff;
}
.sq-payment-form {
  max-width: 100%;
  padding: 20px 20px 5px;
  width: 380px;
}

.sq-label {
  color: #000000;
  font-size: 14px;
  font-family: '""Helvetica Neue"", ""Helvetica"", sans-serif;
  font - weight: 500;
            letter - spacing: 0.5px;
        }

.sq-input {
  border-color: #E0E2E3;
  border-radius: 4px;
}

.sq-input--focus {
  border-color: #4A90E2;
}

.sq-input--error {
  border-color: #e02e2f;
}

.sq-button {
  background: #4A90E2;
  border-radius: 4px;
  font-size: 16px;
  font-weight: 600;
  letter-spacing: 1px;
}

.sq-button:active {
  background: #4A90E2;
}
.sq-wallet-divider {
  margin: 24px 0;
  position: relative;
  text-align: center;
  width: 100%;
}

.sq-wallet-divider:after, .sq-wallet-divider::after, .sq-wallet-divider:before, .sq-wallet-divider::before {
  background: #bbb;
  content: '';
  display: block;
  height: 1px;
  left: 0;
  position: absolute;
  right: 0;
  top: 9px;
}

.sq-wallet-divider:after, .sq-wallet-divider::after {
  right: 65%;
}

.sq-wallet-divider:before, .sq-wallet-divider::before {
  left: 65%;
}

.sq-wallet-divider__text {
  color: #bbb;
  padding: 10px;
  text-transform: uppercase;
}

.sq-button:hover {
  cursor: pointer;
  background-color: #4281CB;
}

#error {
  width: 100%;
  margin-top: 16px;
  font-size: 14px;
  color: red;
  font-weight: 500;
  text-align: center;
  opacity: 0.8;
}

</style>
<div class='sq-payment-form'>
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
                     $.get('" + kscriptAPIURL + "?totalAmount=" + amount + "&currency=" + currency + "&paymentRequestId=" + requestId + @"&nonce=' + nonce, function(data, status){ });
              }
            }
        });
</script>
";
        }
    }
}
