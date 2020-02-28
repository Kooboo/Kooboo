using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Braintree.lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Braintree
{
    public class BraintreeForm : IPaymentMethod<BraintreeSetting>
    {
        public BraintreeSetting Setting { get; set; }

        public string Name => "BraintreeForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("braintree", Context);

        public string IconType => "img";

        public string Icon => "";

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

        [Description(@"
<script engine='kscript'>
var charge = {};
charge.total = 1.50; 
charge.currency='USD';
charge.name = 'Tea from Xiamen'; 
var resForm = k.payment.braintreeForm.charge(charge);  
</script>  
<div k-content='resForm.html'></div>")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            var token = GenerateClientToken();

            res.html = GenerateHtml(token);

            return res;
        }

        public string CreateTransaction(RenderContext context)
        {
            if (this.Setting == null)
            {
                return null;
            }

            Decimal amount;

            try
            {
                amount = Convert.ToDecimal(context.Request.Forms["amount"]);

                var nonce = context.Request.Forms["payment_method_nonce"];
                var request = new TransactionRequest
                {
                    Amount = amount,
                    PaymentMethodNonce = nonce,
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };

                return new BraintreeAPI(Setting).Sale(request);
            }
            catch (FormatException e)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(e);
            }

            return null;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        public PaymentCallback Notify(RenderContext context)
        {
            throw new NotImplementedException();
        }

        private string GenerateClientToken()
        {
            if (this.Setting == null)
            {
                return null;
            }

            var service = new BraintreeAPI(Setting);

            return service.Generate();
        }

        private string GenerateHtml(string clientToken)
        {
            var createTransactionUrl = PaymentHelper.GetCallbackUrl(this, nameof(CreateTransaction), this.Context);
            return @"<script src='https://js.braintreegateway.com/web/dropin/1.22.1/js/dropin.js'></script>
<style>
.button {
  cursor: pointer;
  font-weight: 500;
  left: 3px;
  line-height: inherit;
  position: relative;
  text-decoration: none;
  text-align: center;
  border-style: solid;
  border-width: 1px;
  border-radius: 3px;
  -webkit-appearance: none;
  -moz-appearance: none;
  display: inline-block;
}

.button--small {
  padding: 10px 20px;
  font-size: 0.875rem;
}

.button--green {
  outline: none;
  background-color: #64d18a;
  border-color: #64d18a;
  color: white;
  transition: all 200ms ease;
}

.button--green:hover {
  background-color: #8bdda8;
  color: white;
}

</style>
<form id='payment-form' method='post' action=' / checkouts'>
    <div id='dropin-container'></div>
    <input id='nonce' name='payment_method_nonce' type='hidden' />
    <button id ='submit-button' class='button button--small button--green' type='submit'>Purchase</button>
</form>
<script>
var form = document.querySelector('#payment-form');

braintree.dropin.create({
  authorization: '"+ clientToken + @"',
  selector: '#dropin-container'
}, function (createErr, instance) {
        form.addEventListener('submit', function (event) {
            event.preventDefault();

            instance.requestPaymentMethod(function (err, payload) {
                if (err) {
                    console.log('Error', err);
                    return;
                }

                document.querySelector('#nonce').value = payload.nonce;
                form.submit();
            });
        });
});
</script>";
        }
    }
}