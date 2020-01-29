using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Payment.Methods
{
    public class WeChatSetting : IPaymentSetting
    {
        public string Name => "WeChatPay";

        public string AppId { get; set; }

        public string MerchantId { get; set; }

        public string Key { get; set; }

        public string AppSecret { get; set; }
        
        public string GetSSlCertPath()
        {
            return "";
        }
        public string GetSSlCertPassword()
        {
            return "";
        }
         
        //=======【支付结果通知url】===================================== 
        /* 支付结果通知回调url，用于商户接收支付结果
        */
        public string GetNotifyUrl()
        {
            return "http://www.kooboo.cn";
        }

        //=======【商户系统后台机器IP】===================================== 
        /* 此参数可手动配置也可在程序中自动获取
        */
        public string GetIp()
        {
            return "127.0.0.1";
        }


        //=======【代理服务器设置】===================================
        /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
        */
        public string GetProxyUrl()
        {
            return "";
        }


        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public int GetReportLevel()
        {
            return 1;
        }


        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public int GetLogLevel()
        {
            return 1;
        }


    }
}
