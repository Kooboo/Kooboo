using System;
using System.Collections.Generic;
using Kooboo.Data.Context;
using System.IO;
using System.Net; 
using Kooboo.Data.Attributes;
using System.ComponentModel;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods
{
    public class PaypalForm : IPaymentMethod<PaypalFormSetting>
    { 
        public   string Name { get => "PaypalForm"; }
        public   string DisplayName { get => Data.Language.Hardcoded.GetValue("Paypal"); }

        public   string Icon { get => "/_Admin/View/Market/Images/payment-paypal.jpg"; }

        private List<string> _currency;
        // see: https://developer.paypal.com/docs/classic/api/currency_codes/
        public   List<string> supportedCurrency
        {
            get
            {
                if (_currency == null)
                {
                    _currency = new List<string>();
                    _currency.Add("AUD");
                    _currency.Add("BRL");
                    _currency.Add("CAD");
                    _currency.Add("CZK");
                    _currency.Add("DKK");
                    _currency.Add("EUR");
                    _currency.Add("HKD");
                    _currency.Add("HUF");
                    _currency.Add("INR");
                    _currency.Add("ILS");
                    _currency.Add("JPY");
                    _currency.Add("MYR");
                    _currency.Add("MXN");
                    _currency.Add("NOK");
                    _currency.Add("NZD");
                    _currency.Add("PHP");
                    _currency.Add("PLN");
                    _currency.Add("GBP");
                    _currency.Add("RUB");
                    _currency.Add("SGD");
                    _currency.Add("SEK");
                    _currency.Add("CHF");
                    _currency.Add("TWD");
                    _currency.Add("THB");
                    _currency.Add("USD");
                }
                return _currency;
            }
        }

        public PaypalFormSetting Setting { get; set; }

        public string IconType => "img";
  
        public RenderContext Context { get; set; }

        // see: https://developer.paypal.com/docs/classic/api/currency_codes/
        private string ToCurrencyString(string Currency, decimal amount)
        {
            if (Currency != null)
            {
                var currencies = new List<string>();
                currencies.Add("HUF");
                currencies.Add("JPY");
                currencies.Add("TWD");
                var exist = currencies.Exists(c => c.Equals(Currency, StringComparison.OrdinalIgnoreCase));
                if (exist)
                {
                    return Math.Round(amount).ToString("0");
                }
            }
            return amount.ToString("0.00");
        }


        [Description(@"Pay by submitting hidden form values to Paypal. Example:
<script engine='kscript'>
var charge = {};
charge.total = 1.50; 
charge.currency='USD';
charge.name = 'Tea from Xiamen'; 
var resForm = k.payment.paypalForm.charge(charge);  
</script>  
<div k-content='resForm.html'></div>")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public  IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse(); 
            string currency = request.Currency;
            decimal total = request.TotalAmount;
             var data = GetFieldValues(request);
            res.fieldValues = new KScript.KDictionary(data);
            res.html = GetHtmlForm(request);  
            res.SubmitUrl =  this.Setting.PaypalUrl;
            res.method = "POST";  
            return res;
        }

        // see: https://developer.paypal.com/docs/classic/paypal-payments-standard/integration-guide/formbasics/ 
        private string GetHtmlForm(PaymentRequest request)
        {
            var notifyUrl = PaymentHelper.GetCallbackUrl(this, nameof(NofityUrl), this.Context);
            string returnurl = PaymentHelper.EnsureHttpUrl(this.Setting.ReturnUrl, this.Context);
            string cancalurl = PaymentHelper.EnsureHttpUrl(this.Setting.CancelUrl, this.Context);
            string imageurl = PaymentHelper.EnsureHttpUrl(this.Setting.LogoImage, this.Context);

            string paypalurl = this.Setting.PaypalUrl;

            string formhtml = "<form name='paypal' action='" + paypalurl + "' method='post'>\r\n";

            //  Buy Now buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick" >
            //Shopping cart buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_cart" >
            // Subscribe buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-subscriptions" >
            //Automatic Billing buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-auto-billing" >
            //Installment Plan buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-payment-plan" >
            //  Donate buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_donations" > 
            formhtml += "<input type = 'hidden' name = 'cmd' value = '_xclick' />\r\n";

            formhtml += "<input type = 'hidden' name = 'charset' value = 'utf-8' /> \r\n";
            formhtml += "<input type = 'hidden' name = 'business' value = '" + this.Setting.EmailAddress + "' />\r\n";
            formhtml += "<input type = 'hidden' name = 'no_shipping' value = '1' >\r\n";
            formhtml += "<input type = 'hidden' name = 'no_note' value = '1' />\r\n";
            formhtml += "<input type = 'hidden' name = 'currency_code' value = '" + request.Currency + "' /> \r\n";
            formhtml += "<input type = 'hidden' name = 'cancel_return' value = '" + cancalurl + "' />\r\n";
            formhtml += "<input type = 'hidden' name = 'notify_url' value = '" + notifyUrl + "' /> \r\n";
            formhtml += "<input type = 'hidden' name = 'return' value = '" + returnurl + "' /> \r\n";
            formhtml += "<input type = 'hidden' name = 'image_url' value = '" + imageurl + "' />\r\n";
            formhtml += "<input type = 'hidden' name = 'item_name' value = '" + GetRequestName(request.Name) + "' /> \r\n";
            formhtml += "<input type = 'hidden' name = 'amount' value = '" + this.ToCurrencyString(request.Currency, request.TotalAmount) + "' />\r\n";
             
            // FROM paypal: Pass-through variable for you to track product or service purchased or the contribution made. The value you specify is passed back to you upon payment completion. Required if you want PayPal to track either inventory or profit and loss for the item the button sells.

            formhtml += "<input type = 'hidden' name = 'item_number' value = '" + request.Id.ToString("N") + "' /> \r\n";

            //  formhtml += "<input type = 'hidden' name = 'on1' value = 'paymentrequestid' /> \r\n";
            //  formhtml += "<input type = 'hidden' name = 'os1' value = '" + request.Id.ToString() + "' /> \r\n";

            ////rm
            // Optional
            ///Return method.The FORM METHOD used to send data to the URL specified by the return variable.

            //Valid value is:

            //0.All shopping cart payments use the GET method.
            //1.The buyer's browser is redirected to the return URL by using the GET method, but no payment variables are included.
            //2.The buyer's browser is redirected to the return URL by using the POST method, and all payment variables are included.
            ///// 
            formhtml += "<input type = 'hidden' name = 'rm' value = '2' />\r\n";

            formhtml += "<input type = 'submit' name = 'submit' id='submit' style='display: none' value='please wait ...' />\r\n";
            formhtml += "<script>document.getElementById('submit').click() </script></form>"; 
            return formhtml;
        }

        private Dictionary<string, string> GetFieldValues(PaymentRequest request)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var notifyUrl = PaymentHelper.GetCallbackUrl(this, nameof(NofityUrl), this.Context);
            string returnurl = PaymentHelper.EnsureHttpUrl(this.Setting.ReturnUrl, this.Context);
            string cancalurl = PaymentHelper.EnsureHttpUrl(this.Setting.CancelUrl, this.Context);
            string imageurl = PaymentHelper.EnsureHttpUrl(this.Setting.LogoImage, this.Context);

            string paypalurl = this.Setting.PaypalUrl;

            //string formhtml = "<form name='paypal' action='" + paypalurl + "' method='post'>\r\n";

            //  Buy Now buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick" >
            //Shopping cart buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_cart" >
            // Subscribe buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-subscriptions" >
            //Automatic Billing buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-auto-billing" >
            //Installment Plan buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_xclick-payment-plan" >
            //  Donate buttons — < INPUT TYPE = "hidden" name = "cmd" value = "_donations" > 

            // formhtml += "<input type = 'hidden' name = 'cmd' value = '_xclick' />\r\n";
            result.Add("cmd", "_xclick");

            // formhtml += "<input type = 'hidden' name = 'charset' value = 'utf-8' /> \r\n";  
            // formhtml += "<input type = 'hidden' name = 'business' value = '" + this.Setting.EmailAddress + "' />\r\n"; 
            //formhtml += "<input type = 'hidden' name = 'no_shipping' value = '1' >\r\n"; 
            //formhtml += "<input type = 'hidden' name = 'no_note' value = '1' />\r\n";
            //formhtml += "<input type = 'hidden' name = 'currency_code' value = '" + request.Currency + "' /> \r\n";
            //formhtml += "<input type = 'hidden' name = 'cancel_return' value = '" + cancalurl + "' />\r\n";
            //formhtml += "<input type = 'hidden' name = 'notify_url' value = '" + notifyUrl + "' /> \r\n";
            //formhtml += "<input type = 'hidden' name = 'return' value = '" + returnurl + "' /> \r\n";
            //formhtml += "<input type = 'hidden' name = 'image_url' value = '" + imageurl + "' />\r\n";
            //formhtml += "<input type = 'hidden' name = 'item_name' value = '" + GetRequestName(request.Name) + "' /> \r\n";
            //formhtml += "<input type = 'hidden' name = 'amount' value = '" + this.ToCurrencyString(request.Currency, request.TotalAmount) + "' />\r\n";

            result.Add("no_shipping", "1");
            result.Add("no_note", "1");
            result.Add("currency_code", request.Currency);
            result.Add("cancel_return", cancalurl);
            result.Add("notify_url", notifyUrl);
            result.Add("return", "returnurl");
            result.Add("image_url", imageurl);
            result.Add("item_name", GetRequestName(request.Name));
            result.Add("amount", this.ToCurrencyString(request.Currency, request.TotalAmount));

            // FROM paypal: Pass-through variable for you to track product or service purchased or the contribution made. The value you specify is passed back to you upon payment completion. Required if you want PayPal to track either inventory or profit and loss for the item the button sells.

            // formhtml += "<input type = 'hidden' name = 'item_number' value = '" + request.Id.ToString("N") + "' /> \r\n";
            result.Add("item_number", request.Id.ToString("N"));

            //  formhtml += "<input type = 'hidden' name = 'on1' value = 'paymentrequestid' /> \r\n";
            //  formhtml += "<input type = 'hidden' name = 'os1' value = '" + request.Id.ToString() + "' /> \r\n";

            ////rm
            // Optional
            ///Return method.The FORM METHOD used to send data to the URL specified by the return variable.

            //Valid value is:

            //0.All shopping cart payments use the GET method.
            //1.The buyer's browser is redirected to the return URL by using the GET method, but no payment variables are included.
            //2.The buyer's browser is redirected to the return URL by using the POST method, and all payment variables are included.
            ///// 
            //formhtml += "<input type = 'hidden' name = 'rm' value = '2' />\r\n";

            //formhtml += "<input type = 'submit' name = 'submit' id='submit' style='display: none' value='please wait ...' />\r\n";
            //formhtml += "<script>document.getElementById('submit').click() </script></form>";


            result.Add("rm", "2");

            return result;
        }
         
        private string GetRequestName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                if (name.Length > 0)
                {
                    var ch = name[0];
                    //only support Latin
                    if (ch > 256)
                    {
                        return "name";
                    }
                }

            }
            return name;
        }

        public PaymentCallback NofityUrl(RenderContext context)
        {
            // PayPal HTTPS POSTs an IPN message to your listener that notifies it of an event.
            //Your listener returns an empty HTTP 200 response to PayPal.
            //Your listener HTTPS POSTs the complete, unaltered message back to PayPal; the message must contain the same fields (in the same order) as the original message and be encoded in the same way as the original message.
            //PayPal sends a single word back - either VERIFIED(if the message matches the original) or INVALID(if the message does not match the original).

            if (ValidateResponse(context))
            {
                var strPaymentRequestId = context.Request.GetValue("item_number");
                Guid paymentRequestId;
                if (Guid.TryParse(strPaymentRequestId, out paymentRequestId))
                {
                    var paymentRequest = PaymentManager.GetRequest(paymentRequestId, context);

                    if (paymentRequest == null || this.Setting == null)
                    {
                        return null;
                    }

                    decimal mcGross = 0;//total amount
                    decimal.TryParse(context.Request.Get("mc_gross"), out mcGross);
                    var currency = context.Request.Get("mc_currency");
                    var paymentStatus = context.Request.Get("payment_status");
                    var email = context.Request.Get("receiver_email");

                    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(paymentStatus))
                    {
                        return null;
                    }

                    if (this.Setting.EmailAddress.ToLower() == email.ToLower() && paymentRequest.Currency.ToLower() == currency.ToLower() &&
                        IsSameAmount(paymentRequest.Currency, paymentRequest.TotalAmount, mcGross))
                    {
                        var callback = new PaymentCallback()
                        {
                            RequestId = paymentRequestId,
                        };

                        if (paymentStatus != null)
                        {
                            var lower = paymentStatus.ToLower();

                            if (lower == "completed" || paymentStatus.ToLower() == "paid" || lower == "voided" || lower == "processed")
                            {
                                callback.Status = PaymentStatus.Paid;
                            }
                            else if (lower == "denied" || lower == "failed" || lower == "refused")
                            {
                                callback.Status = PaymentStatus.Rejected;
                            }
                            else if (lower.Contains("cancel") || lower == "expired")
                            {
                                callback.Status = PaymentStatus.Cancelled;
                            }
                            else if (lower == "pending")
                            {
                                callback.Status = PaymentStatus.Pending;
                            }

                        }


                        // TODO: add cancel here..

                        return callback;
                    }
                }

            }
            return null;
        }

        private bool IsSameAmount(string currency, decimal requestAmount, decimal mcGross)
        {
            return ToCurrencyString(currency, requestAmount) == ToCurrencyString(currency, mcGross);
        }

        private bool ValidateResponse(RenderContext context)
        {
            var strFormValues = context.Request.Body;
            string strNewValue = strFormValues + "&cmd=_notify-validate";

            var req = (HttpWebRequest)WebRequest.Create(this.Setting.IPNUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = strNewValue.Length;

            var stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(strNewValue);
            stOut.Close();

            string strResponse = "";
            try
            {
                using (var response = req.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                strResponse = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponse = "";
            }

            return strResponse != null && strResponse.Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            //            Before you can trust the contents of the message, you must first verify that the message came from PayPal.To verify the message, you must send back the contents in the exact order they were received and precede it with the command _notify - validate, as follows:

            //https://ipnpb.paypal.com/cgi-bin/webscr?cmd=_notify-validate&mc_gross=19.95&protection_eligibility=Eligible&address_status=confirmed&payer_id=LPLWNMTBWMFAY&tax=0.00&address_street=1+Main+St&payment_date=20%3A12%3A59+Jan+13%2C+2009+PST&payment_status=Completed&charset=windows-1252&address_zip=95131&first_name=Test&mc_fee=0.88&address_country_code=US&address_name=Test+User&notify_version=2.6&custom=&payer_status=verified&address_country=United+States&address_city=San+Jose&quantity=1&verify_sign=AtkOfCXbDm2hu0ZELryHFjY-Vb7PAUvS6nMXgysbElEn9v-1XcmSoGtf&payer_email=gpmac_1231902590_per%40paypal.com&txn_id=61E67681CH3238416&payment_type=instant&last_name=User&address_state=CA&receiver_email=gpmac_1231902686_biz%40paypal.com&payment_fee=0.88&receiver_id=S8XGHLYDW9T3S&txn_type=express_checkout&item_name=&mc_currency=USD&item_number=&residence_country=US&test_ipn=1&handling_amount=0.00&transaction_subject=&payment_gross=19.95&shipping=0.00
            //            copy
            //            PayPal will then send one single - word message, either VERIFIED, if the message is valid, or INVALID if the messages is not valid.

            //Important: After you have authenticated an IPN message(received a VERIFIED response from PayPal), you must perform these important checks before you can assume that the IPN is both legitimate and has not already been processed:

            //Check that the payment_status is Completed.
            //If the payment_status is Completed, check the txn_id against the previous PayPal transaction that you processed to ensure the IPN message is not a duplicate.
            //Check that the receiver_email is an email address registered in your PayPal account.
            //Check that the price(carried in mc_gross) and the currency(carried in mc_currency) are correct for the item (carried in item_name or item_number).





        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }

}



//Canceled: You canceled your payment, and the money was credited back to your account.
//Cleared: Money from an eCheck that you sent has been deposited in the recipient’s account.
//Cleared by Payment Review: We reviewed the transaction and the money is in the recipient’s account.
//Completed: The transaction was successful and the money is in the recipient’s account.
//Denied: The recipient didn’t accept your payment, and the money was credited back to your account.View the transaction details to see why your payment was denied or contact the recipient for more information. 
//Failed: Your payment didn’t go through.We recommend that you try your payment again.
//Held: We’re reviewing the transaction and your payment might be reversed. You should check the Resolution Center for more information.
//In progress: Your payment was sent, but the recipient hasn’t accepted it yet.
//On hold: We’re holding the money temporarily because either you filed a dispute or we’re reviewing the transaction. Look for an email from us with more information about this transaction.
//Paid: Someone requested money from you and you sent them a payment.
//Partially Refunded: Your payment was partially refunded.
//Pending Verification: We’re reviewing the transaction. We’ll send your payment to the recipient after your payment source has been verified.
//Placed: We have placed a temporary hold on your payment.Look for an email from us with more information.
//Processing: We’re processing your payment and the transaction should be completed shortly.
//Refunded: The recipient returned your payment.If you used a credit card to make your payment, the money will be returned to your credit card.It can take up to 30 days for the refund to appear on your statement.
//Refused: The recipient didn’t receive your payment. If you still want to make your payment, we recommend that you try again.
//Removed: The hold on your payment was removed and the transaction should be completed shortly.
//Returned: Money was returned to your account because the recipient didn’t claim your payment within 30 days.PayPal members can manually reverse unclaimed payments before the 30-day automatic reversal.
//Reversed: Either you canceled the transaction or we did.
//Temporary hold: Money from your account is being held temporarily during the authorization process.The recipient isn’t able to use or withdraw this money until the authorization is complete.
//Unclaimed: The recipient hasn’t accepted or received your payment. Unclaimed transactions are automatically canceled after 30 days.



//Currency Currency code Per transaction limit
//Australian Dollar   AUD
//Brazilian Real

//Note: This currency is supported as a payment currency and a currency balance for in-country PayPal accounts only.
//BRL
//Canadian Dollar CAD
//Czech Koruna CZK
//Danish Krone    DKK
//Euro    EUR
//Hong Kong Dollar    HKD
//Hungarian Forint

//Note: Decimal amounts are not supported for this currency.Passing a decimal amount will throw an error.
//HUF
//Indian Rupee

//Note: To use INR, merchants must have a PayPal India account.
//INR
//Israeli New Sheqel  ILS
//Japanese Yen

//Note: This currency does not support decimals. Passing a decimal amount will throw an error.
//JPY 1,000,000
//Malaysian Ringgit

//Note: This currency is supported as a payment currency and a currency balance for in-country PayPal accounts only.
//MYR
//Mexican Peso MXN
//Norwegian Krone NOK
//New Zealand Dollar  NZD
//Philippine Peso PHP
//Polish Zloty    PLN
//Pound Sterling GBP
//Russian Ruble   RUB For in-border payments (payments made within Russia), the Russian Ruble is the only accepted currency.If you use another currency for in-border payments, the transaction fails and returns the 10001 error code – Internal Error.
//Singapore Dollar    SGD
//Swedish Krona SEK
//Swiss Franc CHF
//Taiwan New Dollar

//Note: Decimal amounts are not supported for this currency.Passing a decimal amount will throw an error.
//TWD
//Thai Baht THB
//U.S.Dollar USD 