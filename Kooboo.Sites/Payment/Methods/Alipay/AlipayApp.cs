using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Alipay.lib;
using Kooboo.Sites.Payment.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace Kooboo.Sites.Payment.Methods.Alipay
{
    public class AlipayApp : IPaymentMethod<AlipayAppSetting>
    {
        public AlipayAppSetting Setting { get; set; }

        public string Name => "AlipayApp";

        public string DisplayName => Data.Language.Hardcoded.GetValue("alipay", Context);

        public string Icon => "";

        public string IconType => "img";

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("CNY");
                return list;
            }
        }

        public RenderContext Context { get; set; }

        [Description(@"
var charge = {};
charge.totalAmount = 1.50; 
charge.name = 'green tea order'; 
 k.payment.alipayApp.charge(charge); 
")]
        [KDefineType(Return = typeof(StringResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            if (Setting == null) return null;

            var biz = new AopDictionary();
            biz.Add("subject", request.Name);
            biz.Add("out_trade_no", request.Id.ToString("N"));
            biz.Add("total_amount", Math.Round(request.TotalAmount, 2));

            var sys = new AopDictionary();
            sys.Add("method", "alipay.trade.app.pay");
            sys.Add("format", "json");
            sys.Add("version", "1.0");
            sys.Add("charset", "UTF-8");
            sys.Add("sign_type", Setting.SignType);
            sys.Add("app_id", Setting.APPId);
            sys.Add("timestamp", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"));
            sys.Add("alipay_sdk", "alipay-easysdk-net-2.0.0");
            sys.Add("biz_content", JsonHelper.Serialize(biz));
            sys.Add("notify_url", PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context));
            var data = new AlipayData();
            var sign = data.RSASign(sys, Setting.PrivateKey, "UTF-8", Setting.SignType);
            sys.Add("sign", sign);

            return new StringResponse
            {
                requestId = request.Id,
                Content = BuildQueryString(sys)
            };
        }

        [KDefineType(Params = new[] { typeof(string) })]
        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();

            if (request == null || request.Id == default(Guid) || this.Setting == null)
            {
                return result;
            }

            try
            {
                var biz = new AopDictionary();
                biz.Add("out_trade_no", request.Id.ToString("N"));

                var dic = new AopDictionary();
                dic.Add("app_id", Setting.APPId);
                dic.Add("method", "alipay.trade.query");
                dic.Add("charset", "utf-8");
                dic.Add("alipay_sdk", "alipay-easysdk-net-2.0.0");
                dic.Add("sign_type", Setting.SignType);
                dic.Add("timestamp", DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"));
                dic.Add("version", "1.0");
                dic.Add("biz_content", JsonHelper.Serialize(biz));

                var data = new AlipayData();
                var sign = data.RSASign(dic, Setting.PrivateKey, "UTF-8", Setting.SignType);
                dic.Add("sign", sign);

                var response = HttpService.DoPost(Setting.ServerUrl, dic, "UTF-8");
                var jobject = JsonHelper.Deserialize<JObject>(response);
                var rsaCheckContent = AlipaySignature.RSACheckContent(AlipayData.GetSignSourceData(response), jobject.Value<string>("sign"), Setting.PublicKey,
               "UTF-8", Setting.SignType);
                if (!rsaCheckContent) throw new AliPayException("sign check fail: check Sign and Data Fail!");
                var res = jobject["alipay_trade_query_response"];
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

        public PaymentCallback Notify(RenderContext context)
        {
            var dic = GetRequestPost(context);
            if (dic.Count > 0)
            {
                var data = new AlipayData();
                bool signVerified = data.RSACheckV1(dic, this.Setting.PublicKey, "UTF-8"); //调用SDK验证签名
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

        private Dictionary<string, string> GetRequestPost(RenderContext context)
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

        private string BuildQueryString(IDictionary<string, string> sortedMap)
        {
            StringBuilder content = new StringBuilder();
            int index = 0;
            foreach (var pair in sortedMap)
            {
                if (!string.IsNullOrEmpty(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                {
                    content.Append(index == 0 ? "" : "&")
                            .Append(pair.Key)
                            .Append("=")
                            .Append(HttpUtility.UrlEncode(pair.Value, Encoding.UTF8));
                    index++;
                }
            }
            return content.ToString();
        }
    }
}
