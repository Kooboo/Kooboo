using Jint.Native.Function;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Callback;
using Kooboo.Sites.Payment.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WxPayAPI;

namespace Kooboo.Sites.Payment.Methods.wechat
{
    public class WeChatApp : IPaymentMethod<WeChatAppSetting>
    {
        public WeChatAppSetting Setting { get; set; }

        public string Name => "wechatApp";

        public string DisplayName => Data.Language.Hardcoded.GetValue("wechat", Context);

        public string Icon => "/_Admin/View/Market/Images/payment-wechat.jpg";

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
var result= k.payment.wechatApp.charge(charge); 
")]
        [KDefineType(Return = typeof(StringResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var root = new XElement("xml");
            root.Add(new XElement("appid", Setting.AppId));
            root.Add(new XElement("mch_id", Setting.MerchantId));
            root.Add(new XElement("nonce_str", WxPayApi.GenerateNonceStr()));
            root.Add(new XElement("body", request.Name));
            if (!string.IsNullOrWhiteSpace(request.Description)) root.Add(new XElement("detail", request.Description));
            root.Add(new XElement("out_trade_no", request.Id.ToString("N")));
            if (!string.IsNullOrWhiteSpace(request.Currency)) root.Add(new XElement("fee_type", request.Currency));
            root.Add(new XElement("total_fee", (int)(request.TotalAmount * 100)));
            root.Add(new XElement("spbill_create_ip", Context.Request.IP));
            root.Add(new XElement("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")));
            root.Add(new XElement("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss")));
            var notifurl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);
            root.Add(new XElement("notify_url", notifurl));
            root.Add(new XElement("trade_type", "APP"));
            root.Add(new XElement("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256));
            var xml = new XDocument(root);

            root.Add(new XElement("sign", MakeSign(xml, Setting.Key)));
            string responseStr = HttpService.Post(xml.ToString(), "https://api.mch.weixin.qq.com/pay/unifiedorder", false, 6, Setting.MerchantId);

            var response = XDocument.Parse(responseStr).Element("xml");
            CheckSign(response);


            if (response.Element("return_code").Value != "SUCCESS")
            {
                throw new Exception(response.Element("return_msg").Value);
            }

            var result = new SortedDictionary<string, object>
            {
                { "appid",Setting.AppId},
                { "partnerid",Setting.MerchantId},
                { "prepayid",response.Element("prepay_id").Value},
                { "package","Sign=WXPay"},
                { "noncestr",WxPayApi.GenerateNonceStr()},
                { "timestamp", DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeSeconds()},
            };

            string str = string.Join("&", result.Select(s => $"{s.Key}={s.Value}")) + "&key=" + Setting.Key;
            var sign = CalcHMACSHA256Hash(str, Setting.Key).ToUpper();
            result.Add("sign", sign);

            return new StringResponse
            {
                requestId = request.Id,
                Content = JsonHelper.Serialize(result)
            };
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var result = new PaymentCallback();

            if (context.Request.Method == "GET")
            {
                result.RawData = context.Request.RawRelativeUrl;
            }
            else
            {
                result.RawData = context.Request.Body;
            }

            var response = XDocument.Parse(result.RawData).Element("xml");

            try
            {
                CheckSign(response);
            }
            catch (Exception ex)
            {
                FailCallback(result, ex.Message);
                return result;
            }


            //检查支付结果中transaction_id是否存在
            if (response.Element("transaction_id") == null)
            {
                FailCallback(result, "支付结果中微信订单号不存在");
                return result;
            }

            var obj = response.Element("out_trade_no");

            Guid RequestId;

            if (obj == null || !System.Guid.TryParse(obj.Value.ToString(), out RequestId))
            {
                FailCallback(result, "订单查询失败");
                return result;
            }
            else
            {
                SuccessCallback(result, RequestId);
                var objcode = response.Element("result_code");

                if (objcode != null)
                {
                    string code = objcode.Value.ToUpper();
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
                var root = new XElement("xml");
                root.Add(new XElement("appid", Setting.AppId));
                root.Add(new XElement("mch_id", Setting.MerchantId));
                root.Add(new XElement("out_trade_no", request.Id.ToString("N")));
                root.Add(new XElement("nonce_str", WxPayApi.GenerateNonceStr()));
                root.Add(new XElement("sign_type", WxPayData.SIGN_TYPE_HMAC_SHA256));
                var xml = new XDocument(root);
                root.Add(new XElement("sign", MakeSign(xml, Setting.Key)));
                string responseStr = HttpService.Post(xml.ToString(), "https://api.mch.weixin.qq.com/pay/orderquery", false, 6, Setting.MerchantId);
                var response = XDocument.Parse(responseStr).Element("xml");
                CheckSign(response);

                if (response.Element("return_code").Value != "SUCCESS")
                {
                    throw new Exception(response.Element("return_msg").Value);
                }

                var trade_state = response.Element("trade_state").Value;
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
        
        private void CheckSign(XElement response)
        {
            var signElement = response.Element("sign");
            signElement.Remove();
            var sign = MakeSign(response.Document, Setting.Key);
            if (sign != signElement.Value) throw new Exception("sign check fail");
        }

        private void SuccessCallback(PaymentCallback result, Guid RequestId)
        {
            var root = new XElement("xml");
            root.Add(new XElement("return_code", "SUCCESS"));
            root.Add(new XElement("return_msg", "OK"));
            var xml = new XDocument(root);
            root.Add(new XElement("sign", MakeSign(xml, Setting.Key)));

            result.CallbackResponse = new CallbackResponse()
            {
                Content = xml.ToString(),
                ContentType = "Application/Xml"
            };

            result.RequestId = RequestId;
        }

        private void FailCallback(PaymentCallback result, string msg)
        {
            var root = new XElement("xml");
            root.Add(new XElement("return_code", "FAIL"));
            root.Add(new XElement("return_msg", msg));
            var xml = new XDocument(root);
            root.Add(new XElement("sign", MakeSign(xml, Setting.Key)));

            result.CallbackResponse = new CallbackResponse()
            {
                Content = xml.ToString(),
                ContentType = "Application/Xml"
            };
        }

        private string MakeSign(XDocument xml, string key)
        {
            var dic = xml.Element("xml").Elements().ToDictionary(k => k.Name.ToString(), v => v.Value);
            string str = string.Join("&", new SortedDictionary<string, string>(dic).Where(w => !string.IsNullOrWhiteSpace(w.Value)).Select(s => $"{s.Key}={s.Value}"));
            str += "&key=" + key;
            return CalcHMACSHA256Hash(str, key).ToUpper();
        }

        private string CalcHMACSHA256Hash(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.UTF8;
            byte[]
            baText2BeHashed = enc.GetBytes(plaintext),
            baSalt = enc.GetBytes(salt);
            HMACSHA256 hasher = new HMACSHA256(baSalt);
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }
    }
}
