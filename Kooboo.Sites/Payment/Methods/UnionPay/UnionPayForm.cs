using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.UnionPay
{
    public class UnionPayForm : IPaymentMethod<UnionPaySetting>
    {
        public UnionPaySetting Setting { get; set; }

        public string Name { get => "UnionPayForm"; }

        public string DisplayName { get => Data.Language.Hardcoded.GetValue("UnionPay"); }

        public string Icon { get => "/_Admin/View/Market/Images/payment-UnionPay.png"; }

        public string IconType => "img";

        public List<string> supportedCurrency
        {
            get
            {
                var list = new List<string>();
                list.Add("CNY");
                list.Add("USD");
                return list;
            }
        }

        public RenderContext Context { get; set; }

        public IPaymentResponse Charge(PaymentRequest request)
        {
            HiddenFormResponse res = new HiddenFormResponse();
            string currency = request.Currency;
            decimal total = request.TotalAmount;
            //var data = GetFieldValues(request);
            //res.fieldValues = new KScript.KDictionary(data);
            res.html = GetHtmlForm(request);
            //res.SubmitUrl = this.Setting.PaypalUrl;
            res.method = "POST";
            return res;
        }


        private string GetHtmlForm(PaymentRequest request)
        {
            // https://open.unionpay.com/tjweb/acproduct/APIList?acpAPIId=275&apiservId=448&version=V2.2&bussType=0#nav04
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["version"] = "5.1.0";//版本号
            param["encoding"] = "UTF-8";//编码方式
            param["txnType"] = "01";//交易类型 ，01：消费
            param["txnSubType"] = "01";//交易子类  01：自助消费
            param["bizType"] = "000201";//业务类型
            param["signMethod"] = "01";//签名方法 （表示采用RSA签名）
            param["channelType"] = "08";//渠道类型
            param["accessType"] = "0";//接入类型 0：商户直连接入
            param["frontUrl"] = Setting.ReturnUrl;  //前台通知地址      
            param["backUrl"] = Setting.NotifyURL;  //后台通知地址
            param["currencyCode"] = GetCurrencyCode(request.Currency);//交易币种 156 人民币

            // 订单超时时间。
            // 超过此时间后，除网银交易外，其他交易银联系统会拒绝受理，提示超时。 跳转银行网银交易如果超时后交易成功，会自动退款，大约5个工作日金额返还到持卡人账户。
            // 此时间建议取支付时的北京时间加15分钟。
            // 超过超时时间调查询接口应答origRespCode不是A6或者00的就可以判断为失败。
            param["payTimeout"] = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            param["merId"] = Setting.MerchantID;//商户号
            param["orderId"] = Guid.NewGuid().ToString("N");//商户订单号，8-32位数字字母，不能含“-”或“_”

            param["txnTime"] = DateTime.Now.ToString("yyyyMMddHHmmss");//订单发送时间，格式为YYYYMMDDhhmmss，取北京时间
            param["txnAmt"] = GetSquareAmount(request.TotalAmount).ToString();//交易金额，单位分

            // 请求方保留域，
            // 透传字段，查询、通知、对账文件中均会原样出现，如有需要请启用并修改自己希望透传的数据。
            // 出现部分特殊字符时可能影响解析，请按下面建议的方式填写：
            // 1. 如果能确定内容不会出现&={}[]"'等符号时，可以直接填写数据，建议的方法如下。
            //param["reqReserved"] = "透传信息1|透传信息2|透传信息3";
            // 2. 内容可能出现&={}[]"'符号时：
            // 1) 如果需要对账文件里能显示，可将字符替换成全角＆＝｛｝【】“‘字符（自己写代码，此处不演示）；
            // 2) 如果对账文件没有显示要求，可做一下base64（如下）。
            //    注意控制数据长度，实际传输的数据长度不能超过1024位。
            //    查询、通知等接口解析时使用System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reqReserved))解base64后再对数据做后续解析。
            //param["reqReserved"] = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("任意格式的信息都可以"));

            param["riskRateInfo"] = "{commodityName=测试商品名称}";//

            string formHtml = CreateAutoFormHtml(Setting.FrontTransactionUrl, param, System.Text.Encoding.UTF8);

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
            html.AppendLine("<!--");
            html.AppendLine("function OnLoadSubmit()");
            html.AppendLine("{");
            html.AppendLine("document.getElementById(\"pay_form\").submit();");
            html.AppendLine("}");
            html.AppendLine("//-->");
            html.AppendLine("</script>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            string result = html.ToString();
            return result;
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            throw new NotImplementedException();
        }

        private static int GetSquareAmount(decimal totalAmount)
        {
            return (int)(totalAmount * 100);
        }

        private string GetCurrencyCode(string currency)
        {
            var result = string.Empty;

            var currentCodes = new Dictionary<string, string>
            {
                {"CNY", "156"},
                {"JPY", "392"},
                {"CAD", "124"},
                {"GBP", "826"},
                {"USD", "840"},
                {"EUR", "978"}
            };

            currentCodes.TryGetValue(currency, out result);

            return result;
        }
    }
}
