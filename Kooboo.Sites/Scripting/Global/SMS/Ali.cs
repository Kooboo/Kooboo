using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.SMS
{
    public class Ali
    {
        public Ali(RenderContext context)
        {
            this.Context = context;
        }

        public RenderContext Context { get; set; }

        [Description("k.sms.aliSMS.send(\"your_ali_template_code\", \"+8615312345678\", \"MergeContent\");")]
        public bool Send(string templateCode, string ToPhoneNumber, string bindingkey, string bindingvalue)
        {
            var setting = this.Context.WebSite.SiteDb().CoreSetting.GetSetting<AliSMSSetting>();

            if (setting == null || string.IsNullOrWhiteSpace(setting.accessId) || string.IsNullOrWhiteSpace(setting.accessSecret) || string.IsNullOrWhiteSpace(setting.signName))
            {
                throw new Exception("Ali SMS service not configured");
            }

            if (string.IsNullOrWhiteSpace(setting.regionId))
            {
                setting.regionId = "cn-hangzhou";
            }

            if (!ToPhoneNumber.StartsWith("+"))
            {
                ToPhoneNumber = "+86" + ToPhoneNumber;
            }

            var profile = DefaultProfile.GetProfile(setting.regionId, setting.accessId, setting.accessSecret);
            profile.AddEndpoint(setting.regionId, setting.regionId, "Dysmsapi", "dysmsapi.aliyuncs.com");

            var acsClient = new DefaultAcsClient(profile);
            var request = new SendSmsRequest();

            string errormsg = null;
            try
            {
                request.PhoneNumbers = ToPhoneNumber;
                request.SignName = setting.signName;
                request.TemplateCode = templateCode;

                string bindingpara = "";

                if (!string.IsNullOrWhiteSpace(bindingkey) && !string.IsNullOrWhiteSpace(bindingvalue))
                {
                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add(bindingkey, bindingvalue);
                    bindingpara = JsonConvert.SerializeObject(para);

                }
                request.TemplateParam = bindingpara;
                 
                var res = acsClient.GetAcsResponse(request);

                if (res == null || res.Code != "OK")
                {
                    string err = "";
                    if (res == null)
                    {
                        err = "no response";
                    }
                    else
                    {
                        err = res.Message;
                    }

                    throw new Exception(err);
                }

            }
            catch (ServerException e)
            {
                errormsg = "sms ServerException";
            }
            catch (ClientException e)
            {
                errormsg = "sms ClientException";
            }

            if (errormsg == null)
            {
                return true;
            }
            else
            {
                throw new Exception(errormsg);
            }
        }

    }
}
