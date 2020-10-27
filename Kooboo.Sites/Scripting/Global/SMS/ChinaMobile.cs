using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.SMS.ChinaMobile;
using Kooboo.Sites.SMS.ChinaMobile.Models;
using System;
using System.ComponentModel;

namespace Kooboo.Sites.Scripting.Global.SMS
{
    public class ChinaMobile
    {
        public ChinaMobile(RenderContext context)
        {
            this.Context = context;
        }

        public RenderContext Context { get; set; }

        [Description("Send SMS using ChinaMobile, to multiple phone numbers")]
        public bool Send(string[] toPhoneNumbers, string content)
        {
            return SendSMS(toPhoneNumbers, content);
        }

        [Description("Send SMS using ChinaMobile, to single phone number")]
        public bool Send(string toPhoneNumber, string content)
        {
            return SendSMS(new string[] { toPhoneNumber }, content);
        }

        private bool SendSMS(string[] toPhoneNumbers, string content)
        {
            var setting = this.Context.WebSite.SiteDb().CoreSetting.GetSetting<ChinaMobileSMSSetting>();
            if (setting == null ||
                string.IsNullOrWhiteSpace(setting.ecName) ||
                string.IsNullOrWhiteSpace(setting.apId) ||
                string.IsNullOrWhiteSpace(setting.secretKey) ||
                string.IsNullOrWhiteSpace(setting.sign))
            {
                throw new Exception("China Mobile SMS service not configured");
            }

            var smsConfig = new SMSConfig
            {
                EcName = setting.ecName,
                ApId = setting.apId,
                SecretKey = setting.secretKey,
                Sign = setting.sign
            };

            var smsClient = new SMSClient(smsConfig);
            var smsResult = smsClient.SendAsync(toPhoneNumbers, content).Result;
            if (smsResult.IsFailure)
            {
                throw new Exception(smsResult.Message);
            }

            return true;
        }
    }
}
