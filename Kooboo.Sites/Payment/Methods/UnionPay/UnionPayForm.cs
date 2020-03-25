using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.UnionPay.lib;
using Kooboo.Sites.Payment.Response;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using static Kooboo.Lib.Helper.ApiClient;

namespace Kooboo.Sites.Payment.Methods.UnionPay
{
    public class UnionPayForm : IPaymentMethod<UnionPaySetting>
    {
        public UnionPaySetting Setting { get; set; }

        public string Name { get => "UnionPayForm"; }

        public string DisplayName { get => Data.Language.Hardcoded.GetValue("UnionPay"); }

        public string Icon { get => "/_Admin/View/Market/Images/payment-UnionPay.png"; }

        public string IconType => "img";

        public List<string> supportedCurrency { get; set; }

        public RenderContext Context { get; set; }

        [Description(@"<script engine = 'kscript' >
    var charge = {};
    charge.total = 2.59; 
    charge.currency='CNY';
    charge.desciption='tea';
    var resForm = k.payment.unionPayForm.charge(charge);
</script>

<div class='jumbotron'>
	<div k-content='resForm.html'></div>
</div>")]
        [KDefineType(Return = typeof(HiddenFormResponse))]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var txnTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            request.ReferenceId = txnTime;

            HiddenFormResponse res = new HiddenFormResponse();
            res.html = GetHtmlForm(request, txnTime);
            res.method = "POST";

            return res;
        }

        private string GetHtmlForm(PaymentRequest request, string txnTime)
        {
            var callbackUrl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);

            // https://open.unionpay.com/tjweb/acproduct/APIList?acpAPIId=275&apiservId=448&version=V2.2&bussType=0#nav04
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["version"] = "5.1.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["txnType"] = "01";//交易类型 ，01：消费
            param["txnSubType"] = "01";//交易子类  01：自助消费
            param["bizType"] = "000201";//业务类型
            param["signMethod"] = "01";//签名方法 （表示采用RSA签名）
            param["channelType"] = "07";//渠道类型
            param["accessType"] = "0";//接入类型 0：商户直连接入
            param["frontUrl"] = Setting.FrontUrl;  //前台通知地址      

            // 需要用这行
            //param["backUrl"] = callbackUrl;  //后台通知地址
            param["backUrl"] = "https://46094a7d.ngrok.io/_api/paymentcallback/UnionPayForm_Notify?SiteId=50ecb05f-e985-7b0d-78de-c10ee111eb30";

            param["currencyCode"] = CurrencyCodes.GetNumericCode(request.Currency, string.Empty);//交易币种 156 人民币
            param["payTimeout"] = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");  // 订单超时时间。
            param["merId"] = Setting.MerchantID;//商户号
            param["orderId"] = request.Id.ToString("N");//商户订单号，8-32位数字字母，不能含“-”或“_”
            param["txnTime"] = txnTime;//订单发送时间，格式为YYYYMMDDhhmmss，取北京时间
            param["txnAmt"] = GetAmount(request.TotalAmount).ToString();//交易金额，单位分
            param["riskRateInfo"] = "{}";  // 请求方保留域 {}

            SignHelper.Sign(param, Encoding.UTF8, Setting.MerchantSignCertPFX.Bytes, Setting.SignCertPasswrod);
            string formHtml = CreateAutoFormHtml(Setting.FrontTransactionUrl, param, Encoding.UTF8);

            return formHtml;
        }

        public static string CreateAutoFormHtml(string reqUrl, Dictionary<string, string> reqData, Encoding encoding)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset={0}\" />", encoding);
            html.AppendLine("</head>");
            html.AppendLine("<body onload=\"OnLoadSubmit();\">");
            html.AppendFormat("<form id=\"pay_form\" action=\"{0}\" method=\"post\">", reqUrl);
            foreach (KeyValuePair<string, string> kvp in reqData)
            {
                html.AppendFormat("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", kvp.Key, kvp.Value);
            }
            html.AppendLine("</form>");
            html.AppendLine("<script type=\"text/javascript\">");
            html.AppendLine("function OnLoadSubmit()");
            html.AppendLine("{");
            html.AppendLine("document.getElementById(\"pay_form\").submit();");
            html.AppendLine("}");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            string result = html.ToString();
            return result;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            PaymentStatusResponse result = new PaymentStatusResponse();
            Dictionary<string, string> param = new Dictionary<string, string>();

            // https://open.unionpay.com/tjweb/acproduct/APIList?acpAPIId=278&apiservId=448&version=V2.2&bussType=0
            param["version"] = "5.1.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["signMethod"] = "01";//签名方法
            param["txnType"] = "00";//交易类型
            param["txnSubType"] = "00";//交易子类
            param["bizType"] = "000000";//业务类型
            param["accessType"] = "0";//接入类型
            param["channelType"] = "07";//渠道类型
            param["orderId"] = request.Id.ToString("N");//商户订单号
            param["merId"] = Setting.MerchantID;//商户号
            param["txnTime"] = request.ReferenceId;

            SignHelper.Sign(param, Encoding.UTF8, Setting.MerchantSignCertPFX.Bytes, Setting.SignCertPasswrod);

            string postData = SDKUtil.CreateLinkString(param, false, true, Encoding.UTF8);
            var apiResponse = DoPost(Setting.SingleQueryUrl, postData).Result;
            if (apiResponse.IsSuccessStatusCode)
            {
                Dictionary<String, String> rspData = SDKUtil.CoverStringToDictionary(apiResponse.Content, Encoding.UTF8);

                var isValid = SignHelper.Validate(rspData, Encoding.UTF8, Setting.RootCertCER.Bytes, Setting.MiddleCertCER.Bytes);

                if (rspData.Count != 0 && isValid && rspData["respCode"] == "00")
                {
                    result.Status = PaymentStatus.Paid;
                }
                else
                {
                    result.Status = PaymentStatus.Rejected;
                }

                return result;
            }

            return result;
        }

        public static async Task<ApiResponse> DoPost(string singleQueryUrl, string postData)
        {
            var client = ApiClient.Create();
            var result = await client.PostAsync(singleQueryUrl, postData, "application/x-www-form-urlencoded");
            return result;
        }

        private static int GetAmount(decimal totalAmount)
        {
            return (int)(totalAmount * 100);
        }

        public PaymentCallback Notify(RenderContext context)
        {
            Dictionary<string, string> resData = new Dictionary<string, string>();
            NameValueCollection coll = context.Request.Forms;
            string[] requestItem = coll.AllKeys;
            for (int i = 0; i < requestItem.Length; i++)
            {
                resData.Add(requestItem[i], context.Request.Forms[requestItem[i]]);
            }

            Guid orderId;
            if (SignHelper.Validate(resData, Encoding.UTF8, Setting.RootCertCER.Bytes, Setting.MiddleCertCER.Bytes) && Guid.TryParse(context.Request.Forms["orderId"], out orderId))
            {
                var response = new PaymentCallback
                {
                    RequestId = orderId,
                    RawData = context.Request.Body,
                };

                //00、A6为成功，其余为失败。其他字段也可按此方式获取。 https://open.unionpay.com/tjweb/support/faq/mchlist?id=327
                string respcode = resData["respCode"];
                if (respcode == "00" || respcode == "A6")
                {
                    response.Status = PaymentStatus.Paid;
                }
                else
                {
                    response.Status = PaymentStatus.Rejected;
                }

                return response;
            }
            else
            {
                return null;
            }
        }
    }
}
