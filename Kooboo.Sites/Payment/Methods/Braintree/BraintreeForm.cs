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

            if (string.IsNullOrEmpty(token))
                return null;
            res.html = GenerateHtml(token, request.TotalAmount, request.Id, request.ReturnUrl);

            return res;
        }

        public PaymentCallback CreateTransaction(RenderContext context)
        {
            decimal amount;
            PaymentCallback res = null;

            try
            {
                amount = Convert.ToDecimal(context.Request.GetValue("amount"));

                var nonce = context.Request.GetValue("nonce");
                var orderId = context.Request.GetValue("orderId");
                var request = new TransactionRequest
                {
                    Transaction = new TransactionRequestChildren
                    {
                        Amount = amount,
                        PaymentMethodNonce = nonce,
                        Options = new TransactionOptionsRequest
                        {
                            SubmitForSettlement = true
                        },
                        OrderId = orderId
                    }
                };

                var result = new BraintreeAPI(Setting).Sale(request);

                var strPaymentRequestId = orderId;
                Guid paymentRequestId;

                if (Guid.TryParse(strPaymentRequestId, out paymentRequestId))
                {

                    if (result != null && result.Transaction.Status.Equals("submitted_for_settlement", StringComparison.OrdinalIgnoreCase))
                    {
                        res = new PaymentCallback();
                        var paymentRequest = PaymentManager.GetRequest(paymentRequestId, context);
                        paymentRequest.ReferenceId = result.Transaction.Id;
                        PaymentManager.UpdateRequest(paymentRequest, context);
                    }
                }
            }
            catch (FormatException e)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(e);
            }

            return res;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();
            var id = request.ReferenceId;
            var resp = new BraintreeAPI(Setting).Find(id);

            result = GetPaidStatus(result, resp.Transaction.Status);

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var forms = context.Request.Forms;
            var data = new BraintreeAPI(Setting).Parse(forms["bt_signature"],
                forms["bt_payload"]);

            var result = new PaymentCallback();
            if (data != null)
            {
                var transactions = data.Subject.Subscription.Transactions;
                for (int i = 0; i < transactions.Length; i++)
                {
                    var strPaymentRequestId = transactions[i].Transaction.OrderId;
                    Guid paymentRequestId;
                    if (Guid.TryParse(strPaymentRequestId, out paymentRequestId))
                    {
                        result.RequestId = paymentRequestId;
                        if (string.Equals(data.kind, "transaction_settled", StringComparison.OrdinalIgnoreCase))
                        {
                            result.Status = PaymentStatus.Paid;
                        }
                        else
                        {
                            result.Status = PaymentStatus.Cancelled;
                        }
                    }

                }
            }
            return result;
        }

        private static PaymentStatusResponse GetPaidStatus(PaymentStatusResponse result, string orderStatus)
        {
            if (orderStatus.Equals("failed", StringComparison.OrdinalIgnoreCase)
                || orderStatus.Equals("gateway_rejected", StringComparison.OrdinalIgnoreCase)
                || orderStatus.Equals("settlement_declined", StringComparison.OrdinalIgnoreCase))
            {
                result.Status = PaymentStatus.NotAvailable;
            }

            if (orderStatus.Equals("authorized", StringComparison.OrdinalIgnoreCase)
                || orderStatus.Equals("submitted_for_settlement", StringComparison.OrdinalIgnoreCase))
            {
                result.Status = PaymentStatus.Pending;
            }

            if (orderStatus.Equals("settled", StringComparison.OrdinalIgnoreCase))
            {
                result.Status = PaymentStatus.Paid;
            }

            return result;
        }

        private string GenerateClientToken()
        {
            try
            {
                if (this.Setting == null)
                {
                    return null;
                }

                var service = new BraintreeAPI(Setting);

                return service.Generate();
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
                return null;
            }
        }

        private string GenerateHtml(string clientToken, decimal amount, Guid orderId, string returnUrl)
        {
            var createTransactionUrl = PaymentHelper.GetCallbackUrl(this, nameof(CreateTransaction), this.Context);
            return @"<script src='https://js.braintreegateway.com/web/dropin/1.22.1/js/dropin.js'></script>
<script src='https://code.jquery.com/jquery-3.1.1.min.js'></script>
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
    <input id='amount' name='amount' type='hidden' value='" + amount + @"'/>
    <input id='orderId' name='orderId' type='hidden' value='" + orderId + @"'/>
    <div id='dropin-container'></div>
    <input id='nonce' name='payment_method_nonce' type='hidden' />
    <button style='display: none;' id ='submit-button' class='button button--small button--green' type='submit'>Purchase</button>

<script>
var button = document.querySelector('#submit-button');

braintree.dropin.create({
  authorization: '" + clientToken + @"',
  selector: '#dropin-container'
}, function (createErr, instance) {
        document.getElementById('submit-button').style.display = 'block';
        button.addEventListener('click', function (event) {
            document.getElementById('submit-button').style.display = 'none';
            event.preventDefault();
             
            instance.requestPaymentMethod(function (err, payload) {
                if (err) {
                    console.log('Error', err);
                    return;
                }

                document.querySelector('#nonce').value = payload.nonce;
                $.post('" + createTransactionUrl + @"',
                 {
                      amount:'" + amount + @"',
                      nonce:payload.nonce,
                      orderId:'" + orderId + @"'
                 },
                 function(data)
                 {" + Redirect(returnUrl) + @"
                 });
            });
        });
});
</script>";
        }

        public string Redirect(string returnUrl)
        {
            string html = "";
            if (!string.IsNullOrEmpty(Setting.FailureRedirectURL))
            {
                html += @"if (data && !data.success)
                       { window.location.replace('" + Setting.FailureRedirectURL + @"'); } ;";
            }

            if (!string.IsNullOrEmpty(Setting.SucceedRedirectURL) || !string.IsNullOrEmpty(returnUrl))
            {
                html += @"if (!data)
                       { window.location.replace('" + returnUrl ?? Setting.SucceedRedirectURL + @"'); } ;";
            }

            return html;
        }
    }
}