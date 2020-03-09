using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Methods.Alipay.lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Alipay
{
    public class AlipayForm : IPaymentMethod<AlipayFormSetting>
    {
        public string Name => "AlipayForm";

        public string DisplayName => Data.Language.Hardcoded.GetValue("alipay", Context);

        public string Icon => "";//需要一张支付的图片


        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("CNY");
                return list;
            }
        }

        public AlipayFormSetting Setting { get; set; }

        public string IconType => "img";

        public RenderContext Context { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 1.50; 
charge.currency='CNY';
charge.name = 'green tea order'; 
charge.description = 'The best tea from Xiamen';  
var resForm = k.payment.alipayForm.charge(charge); 
</script>
 
<div class='jumbotron'>
	<div k-content='resForm.html'></div>
</div>'")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            if (this.Setting == null)
            {
                return null;
            }
            var dic = new AopDictionary();
            dic.Add("subject", request.Name);
            if (request.Description != null)
            {
                dic.Add("body", request.Description);
            }

            dic.Add("out_trade_no", request.Id.ToString("N"));
            dic.Add("product_code", "FAST_INSTANT_TRADE_PAY");//fix value

            var amount = Math.Round(request.TotalAmount, 2);

            dic.Add("total_amount", amount);
            dic.Add("time_expire", DateTime.Now.AddMinutes(10));

            string notifurl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);
            string returnurl = request.ReturnUrl ?? PaymentHelper.EnsureHttpUrl(this.Setting.ReturnUrl, Context);
            res.html = AlipayApi.Pay(dic, this.Setting, returnurl, notifurl);

            return res;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var dic = GetRequestPost(context);
            if (dic.Count > 0)
            {
                var data = new AlipayData();
                bool signVerified = data.RSACheckV1(dic, this.Setting.PublicKey, this.Setting.Charset); //调用SDK验证签名
                if (signVerified)
                {
                    var strPaymentRequestId = context.Request.GetValue("out_trade_no");
                    Guid paymentRequestId;
                    if (Guid.TryParse(strPaymentRequestId, out paymentRequestId))
                    {
                        var paymentRequest = PaymentManager.GetRequest(paymentRequestId, context);

                        decimal totalAmount = 0;//total amount
                        decimal.TryParse(context.Request.Get("total_amount"), out totalAmount);
                        var subject = context.Request.Get("subject");
                        var paymentStatus = context.Request.Get("trade_status");

                        if (paymentRequest == null || this.Setting == null)
                        {
                            return null;
                        }

                        var callback = new PaymentCallback()
                        {
                            RequestId = paymentRequestId,
                        };

                        if (totalAmount == Math.Round(paymentRequest.TotalAmount, 2) || subject == paymentRequest.Name)
                        {
                            if (paymentStatus == TradeStatus.TRADE_CLOSED)
                            {
                                callback.Status = PaymentStatus.Cancelled;
                            }
                            else if (paymentStatus == TradeStatus.TRADE_SUCCESS || paymentStatus == TradeStatus.TRADE_FINISHED)
                            {
                                callback.Status = PaymentStatus.Paid;
                            }
                            else if (paymentStatus == TradeStatus.WAIT_BUYER_PAY)
                            {
                                callback.Status = PaymentStatus.Pending;
                            }

                        }
                        else
                        {
                            callback.Status = PaymentStatus.NotAvailable;
                            //怎么让kooboo的前端打印输出“fail”
                        }

                        return callback;
                    }
                    else
                    {
                        return null;
                        //怎么让kooboo的前端打印输出“fail”
                    }
                }
            }

            return null;
        }

        public Dictionary<string, string> GetRequestPost(RenderContext context)
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            coll = context.Request.Forms;
            String[] requestItem = coll.AllKeys;
            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], context.Request.Forms[requestItem[i]]);
            }
            return sArray;

        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            if (request == null || request.Id == default(Guid) || this.Setting == null)
            {
                return result;
            }

            try
            {
                var dic = new AopDictionary();
                dic.Add("out_trade_no", request.Id.ToString("N"));
                var response = AlipayApi.Query(dic, this.Setting);
                var data = new AlipayData();
                var res = data.FromJson(response, Setting);
                var trade_state = res["trade_status"];
                //交易状态：WAIT_BUYER_PAY（交易创建，等待买家付款）、TRADE_CLOSED（未付款交易超时关闭，或支付完成后全额退款）、TRADE_SUCCESS（交易支付成功）、TRADE_FINISHED（交易结束，不可退款）

                if (trade_state != null)
                {
                    result.HasResult = true;
                    var code = trade_state.ToString().ToUpper();
                    if (code == "TRADE_SUCCESS" || code == "TRADE_FINISHED")
                    {
                        result.Status = PaymentStatus.Paid;
                    }
                    else if (code == "TRADE_CLOSED")
                    {
                        result.Status = PaymentStatus.Cancelled;
                    }
                    else if (code == "WAIT_BUYER_PAY")
                    {
                        result.Status = PaymentStatus.Pending;
                    }

                }
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return result;
        }
    }
}
