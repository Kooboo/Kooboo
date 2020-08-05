using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;

namespace Kooboo.Sites.Scripting.Global.SMS
{
 public   class Tencent
    {

        public Tencent(RenderContext context)
        {
            this.Context = context; 
        }

        public RenderContext Context { get; set; }

        [Description("Send SMS using TencentCloud")]
        public bool Send(string templateId, string ToPhoneNumber, object Parameters) 
        {

            var obj = kHelper.CleanDynamicObject(Parameters);  

            var setting = this.Context.WebSite.SiteDb().CoreSetting.GetSetting<TencentSMSSetting>();
            
            if (setting== null || string.IsNullOrWhiteSpace(setting.secretId) || string.IsNullOrWhiteSpace(setting.secretKey) || string.IsNullOrWhiteSpace(setting.appId) || string.IsNullOrWhiteSpace(setting.sign))
            {
                throw new Exception("Tencent SMS service not configured"); 
            }

            if (string.IsNullOrWhiteSpace(setting.regionId))
            {
                setting.regionId = "ap-guangzhou";
            } 

            if (!ToPhoneNumber.StartsWith("+"))
            {
                ToPhoneNumber = "+86" + ToPhoneNumber; 
            } 
            var client = new SmsClient(new Credential
            {
                SecretId = setting.secretId,
                SecretKey = setting.secretKey
            }, setting.regionId);
            var req = new SendSmsRequest();

            req.SmsSdkAppid =  setting.appId;
            /* 短信签名内容: 使用 UTF-8 编码，必须填写已审核通过的签名，可登录 [短信控制台] 查看签名信息 */
            req.Sign = setting.sign;
            req.SenderId = string.Empty;

            req.TemplateID = templateId;
            req.PhoneNumberSet = new[] { ToPhoneNumber };

            if (obj !=null)
            {
                var list = new List<string>();
                foreach (var item in obj)
                {
                    if (item.Value !=null)
                    {
                        list.Add(item.Value.ToString());
                    } 
                }
                req.TemplateParamSet = list.ToArray(); 
            }
            
            var resp = client.SendSms(req).Result;
            var first = resp.SendStatusSet.FirstOrDefault();
            return first != null && first.Code.Equals("Ok", StringComparison.OrdinalIgnoreCase);
        }


    }





}
