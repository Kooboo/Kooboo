using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using WxPayAPI;
using Kooboo.Data.Attributes;
using System.ComponentModel;
using Kooboo.Sites.Payment.Response;
using Kooboo.Sites.Payment.Callback;

namespace Kooboo.Sites.Payment.Methods
{
    public class WeChatNative : IPaymentMethod<WeChatSetting>
    {
        public string Name => "wechat";

        public string DisplayName => Data.Language.Hardcoded.GetValue("wechat", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-wechat.jpg";


        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("CNY");
                return list;
            }
        }

        public WeChatSetting Setting { get; set; }

        public string IconType => "img";

        public RenderContext Context { get; set; }

        [KDefineType(Return = typeof(QRCodeResponse))]
        [Description(@"Pay by wechat barcode scan. Example:
<script engine=""kscript"">
var charge = {};
charge.total = 1.50; 
charge.name = ""green tea order""; 
charge.description = ""The best tea from Xiamen"";  
var res = k.payment.wechat.charge(charge);
</script>
<div k-content=""res.html""></div>
<div id=""paymentRefId"" style=""display:none;"" k-content=""res.requestId""></div>
<script>
function checkStatus()
{ var url = ""/_api/payment/CheckStatus?id=""+ document.getElementById(""paymentRefId"").innerText; 
  $.ajax({url: url }).done(function(res) {
   if (res.model.paid)
   { alert('pay successfully.'); clearInterval(timerid);  } }); } 
var timerid = setInterval(checkStatus, 1000);  
</script>")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            WxPayData data = new WxPayData();

            if (this.Setting == null)
            {
                return null;
            }

            data.SetValue("body", request.Name);
            if (request.Description != null)
            {
                data.SetValue("attach", request.Description);//附加数据
            }
            //out_trade_no can't have "-"
            data.SetValue("out_trade_no", request.Id.ToString("N"));//随机字符串

            var amount = request.TotalAmount;

            //if (request.Currency != "CNY")
            //{
            //    amount = Currency.ExConverter.To(amount, request.Currency, "CNY");
            //}

            amount = amount * 100;  // convert to cents. 

            data.SetValue("total_fee", (int)amount);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间

            data.SetValue("goods_tag", request.Name);//商品标记

            data.SetValue("trade_type", "NATIVE"); //交易类型

            data.SetValue("product_id", request.Id.ToString("N"));//商品ID


            //异步通知url未设置，则使用配置文件中的url
            // string notifurl = Manager.GetCallbackUrl(this, "Notify", site);

            string notifurl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);

            data.SetValue("notify_url", notifurl); //异步通知url

            WxPayData result = WxPayApi.UnifiedOrder(data, this.Setting);//调用统一下单接口

            string url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("Payment failed, please try again");
            }

            return new QRCodeResponse(url, request.Id);
        }

        public PaymentCallback Notify(RenderContext context)
        {
            string postdata = context.Request.Body;

            var result = new PaymentCallback();

            if (context.Request.Method == "GET")
            {
                result.RawData = context.Request.RawRelativeUrl;
            }
            else
            {
                result.RawData = postdata;
            }


            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(postdata);
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                // Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
                // page.Response.Write(res.ToXml());
                // page.Response.End(); 
                result.CallbackResponse = new CallbackResponse()
                {
                    Content = res.ToXml(),
                    ContentType = "Application/Xml"
                };
                return result;
            }

            //  Log.Info(this.GetType().ToString(), "Check sign success");
            // return data;


            //检查支付结果中transaction_id是否存在
            if (!data.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                // Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                // page.Response.Write(res.ToXml());
                // page.Response.End();

                result.CallbackResponse = new CallbackResponse()
                {
                    Content = res.ToXml(),
                    ContentType = "Application/Xml"
                };
                 
                return result;
            }

            var obj = data.GetValue("out_trade_no");

            Guid RequestId;

            if (obj == null || !System.Guid.TryParse(obj.ToString(), out RequestId))
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                result.CallbackResponse = new CallbackResponse()
                {
                    Content = res.ToXml(),
                    ContentType = "Application/Xml"
                };
                return result;
            }
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                result.CallbackResponse = new CallbackResponse()
                {
                    Content = res.ToXml(),
                    ContentType = "Application/Xml"
                };

                result.RequestId = RequestId;

                var objcode = data.GetValue("result_code");

                if (objcode != null)
                {
                    string code = objcode.ToString().ToUpper();
                    if (code == "SUCCESS")
                    {
                        result.Status = PaymentStatus.Paid;
                    }

                    if (code == "FAIL")
                    {
                        result.Status = PaymentStatus.Rejected;
                    }
                    //业务结果 result_code 是 String(16)	SUCCESS SUCCESS/ FAIL 
                }

                return result;
            }

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
                var data = new WxPayData();
                data.SetValue("out_trade_no", request.Id.ToString("N"));
                var response = WxPayApi.OrderQuery(data, this.Setting);

                var trade_state = response.GetValue("trade_state");
                //trade_state:SUCCESS,REFUND,NOTPAY,CLOSED,REVOKED,USERPAYING,PAYERROR

                if (trade_state != null)
                {
                    result.HasResult = true;
                    var code = trade_state.ToString().ToUpper();
                    if (code == "SUCCESS")
                    {
                        result.Status = PaymentStatus.Paid;
                    }
                    else if (code == "REFUND" || code == "NOTPAY")
                    {
                        result.Status = PaymentStatus.Cancelled;
                    }
                    else if (code == "CLOSE" || code == "REVOKED" || code == "PAYERROR")
                    {
                        result.Status = PaymentStatus.Cancelled;
                    }
                    else if (code == "USERPAYING")
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
