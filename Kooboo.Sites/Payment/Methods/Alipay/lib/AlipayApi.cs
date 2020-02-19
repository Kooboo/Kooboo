using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Payment.Methods.Alipay.lib
{
    public class AlipayApi
    {
        public const string CHARSET = "charset";
        public const string PayMethod = "alipay.trade.page.pay";
        public const string ALIPAY_QUERY = "alipay.trade.query";
        /**
        * 跳转支付宝页面直接进行支付
        */
        public static string Pay(AopDictionary bizContent, AlipayFormSetting setting, string returnUrl, string noticeUrl)
        {
            try
            {
                PayValidation(bizContent);
                var data = new AlipayData();
                var request = RequestBase(setting, PayMethod);
                request.Add("biz_content", data.ToJson(bizContent));
                request.Add("notify_url", noticeUrl);
                request.Add("return_url", returnUrl);
                request.Add("sign", data.RSASign(request, setting.PrivateKey, setting.Charset, setting.SignType));

                var body = BuildHtmlRequest(request, "POST", setting);

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static string Query(AopDictionary bizContent, AlipayFormSetting setting)
        {
            try
            {
                QueryValidation(bizContent);
                var data = new AlipayData();
                var request = RequestBase(setting, ALIPAY_QUERY);
                request.Add("biz_content", data.ToJson(bizContent));

                request.Add("sign", data.RSASign(request, setting.PrivateKey, setting.Charset, setting.SignType));

                var body = HttpService.DoPost(setting.ServerUrl + "?" + CHARSET + "=" + setting.Charset, request, setting.Charset);

                return body;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static string BuildHtmlRequest(IDictionary<string, string> dicPara, string strMethod, AlipayFormSetting setting)
        {
            //待请求参数数组
            var sbHtml = new StringBuilder();
            //sbHtml.Append("<head><meta http-equiv=\"Content-Type\" content=\"text/html\" charset= \"" + charset + "\" /></head>");

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + setting.ServerUrl + "?charset=" + setting.Charset +
                          "' method='" + strMethod + "'>");
            ;
            foreach (var temp in dicPara)
                sbHtml.Append("<input  name='" + temp.Key + "' value='" + temp.Value + "'/>");

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='" + strMethod + "' style='display:none;'></form>");
            // sbHtml.Append("<input type='submit' value='" + strButtonValue + "'></form></div>");

            //表单实现自动提交
            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        private static void QueryValidation(AopDictionary data)
        {
            if (!data.ContainsKey("out_trade_no") && data.ContainsKey("trade_no"))
            {
                throw new AliPayException("支付宝交易号trade_no和商户订单号out_trade_no不能同时为空！");
            }
        }

        private static void PayValidation(AopDictionary data)
        {
            if (!data.ContainsKey("out_trade_no"))
            {
                throw new AliPayException("提交统一收单下单并支付页面接口API接口中，缺少必填参数out_trade_no！");
            }

            if (!data.ContainsKey("product_code"))
            {
                throw new AliPayException("提交统一收单下单并支付页面接口API接口中，缺少必填参数product_code！");
            }

            if (!data.ContainsKey("total_amount"))
            {
                throw new AliPayException("提交统一收单下单并支付页面接口API接口中，缺少必填参数total_amount！");
            }

            if (!data.ContainsKey("subject"))
            {
                throw new AliPayException("提交统一收单下单并支付页面接口API接口中，缺少必填参数subject！");
            }
        }

        private static AopDictionary RequestBase(AlipayFormSetting setting, string method)
        {
            if (string.IsNullOrEmpty(setting.APPId))
            {
                throw new AliPayException("您的支付宝配置未能通过检查，详细信息：商户ID未指定！");
            }

            //SignType私钥检查
            if (string.IsNullOrEmpty(setting.SignType))
            {
                throw new AliPayException("您的支付宝配置未能通过检查，详细信息：签名类型未指定！");
            }

            //RSA私钥检查
            if (string.IsNullOrEmpty(setting.PrivateKey))
            {
                throw new AliPayException("您的支付宝配置未能通过检查，详细信息：未能获取到商户私钥！");
            }

            //RSA私钥格式检查
            RSA rsaCsp = AlipaySignature.LoadCertificateString(setting.PrivateKey, setting.SignType);

            if (rsaCsp == null)
            {
                throw new AliPayException("您的支付宝配置未能通过检查，详细信息：商户私钥格式错误，未能导入！");
            }

            var dic = new AopDictionary();
            dic.Add("app_id", setting.APPId);
            dic.Add("method", method);
            dic.Add("format", setting.Format);
            dic.Add("charset", setting.Charset);
            dic.Add("sign_type", setting.SignType);
            dic.Add("timestamp", DateTime.Now);
            dic.Add("version", setting.Version);

            return dic;
        }
    }
}