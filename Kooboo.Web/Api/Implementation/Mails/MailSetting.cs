using System.Linq;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Mail.Models;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class MailSetting : IApi
    {
        public string ModelName => "mailsetting";
        public bool RequireSite => false;
        public bool RequireUser => true;


        private string GetDefaultEmailAdd(ApiCall call)
        {

            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(call.Context.User.CurrentOrgId);

            var mailAddress = orgDb.Email.ByUser(call.Context.User.Id);

            if (mailAddress == null || !mailAddress.Any())
            {
                return null;
            }
            var defaultAdd = mailAddress.Find(o => o.IsDefault);
            if (defaultAdd != null)
            {
                return defaultAdd.Address;
            }

            return mailAddress.FirstOrDefault().Address;
        }

        // only for local.
        public SmtpSetting SmtpGet(ApiCall call)
        {
            var OrgId = call.Context.User.CurrentOrgId;
            var OrgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(OrgId);
            return OrgDb.SmtpGet();
        }

        public void SmtpUpdate(SmtpSetting setting, ApiCall call)
        {
            var OrgId = call.Context.User.CurrentOrgId;
            var OrgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(OrgId);
            OrgDb.SmtpUpdate(setting);
        }

        public ImapSetting Imap(ApiCall call)
        {
            if (EmailForwardManager.RequireForward(call.Context))
            {
                return EmailForwardManager.Get<ImapSetting>(this.ModelName, nameof(MailSetting.Imap), call.Context.User);
            }
            var add = GetDefaultEmailAdd(call);

            var login = Hardcoded.GetValue("login password for user: ", call.Context);
            login = login.Replace("{0}", call.Context.User.UserName);

            var imaptitle = Hardcoded.GetValue("IMAP Setting", call.Context);
            var smtptitle = Hardcoded.GetValue("SMTP Setting", call.Context);

            var org = GlobalDb.Organization.Get(call.Context.User.CurrentOrgId);
            var server = org.Name + ".imapsetting.com";
            if (!AppSettings.IsOnlineServer)
            {
                server = "mx.localkooboo.com";
            }
            var setting = new ImapSetting();
            setting.Smtp = new MailboxSetting()
            {

                UserName = add,
                Password = "***" + "    (" + login + ")",
                Port = 587,
                SSL = "STARTTLS",
                Title = smtptitle,
                Url = server
            };

            setting.Imap = new MailboxSetting()
            {
                UserName = setting.Smtp.UserName,
                Password = setting.Smtp.Password,
                Port = 993,
                SSL = "SSL",
                Title = imaptitle,
                Url = server
            };

            return setting;
        }


        private BimiSetting temp = new BimiSetting()
        {
            Logo = "https://www.fakeurl.com/logo.svg",
            VMC = "https://www.fakeurl.com/vmc.pem"
        };

        public BimiSetting Bimi(ApiCall call)
        {

            var org = GlobalDb.Organization.GetFromAccount(call.Context.User.CurrentOrgId);


            BimiSetting setting = new BimiSetting();

            setting.Logo = org.BimiLogo;
            setting.VMC = org.BimiVMC;

            setting.LogoName = Hardcoded.GetValue("Inbox Logo", call.Context);
            setting.LogoTitle = Hardcoded.GetValue("Display your logo on email clients", call.Context);

            setting.LogoDescription = Hardcoded.GetValue("BIMI is an email specification that enables the use of brand logos within supporting email clients.", call.Context);

            setting.LogoDescription += "<br/>";

            setting.LogoDescription += Hardcoded.GetValue("Logo File must be in format of: SVG Tiny Portable/Secure", call.Context);

            setting.LogoDescription += "<br/>";
            setting.LogoDescription += Hardcoded.GetValue("Create and insert your logo URL below", call.Context);

            setting.LogoDescription += "<br/>" + Hardcoded.GetValue("See", call.Context) + ":";

            setting.LogoDescription += "<a href='https://www.bimigroup.org'>https://www.bimigroup.org</a>";


            setting.VMCName = Hardcoded.GetValue("VMC", call.Context);
            setting.VMCTitle = Hardcoded.GetValue("Verified Mark Certificates", call.Context);


            setting.VMCDescription = Hardcoded.GetValue("VMC asserts a verifiable and auditable binding between a logo and a domain.", call.Context);
            setting.VMCDescription += "<br/>";

            setting.VMCDescription += Hardcoded.GetValue("Some email providers such as Gmail require a VMC before displaying your logo.", call.Context);
            setting.VMCDescription += "<br/>";

            setting.VMCDescription += Hardcoded.GetValue("Apply and insert your VMC file URL below.", call.Context);
            setting.VMCDescription += "<br/>" + Hardcoded.GetValue("See", call.Context) + ":";

            setting.VMCDescription += "<a href='https://www.bimigroup.org/verified-mark-certificates-vmc-and-bimi/'>https://www.bimigroup.org/verified-mark-certificates-vmc-and-bimi/</a>";


            return setting;
        }

        //public bool UpdateBimiLogo(string Logo, Guid OrgId, ApiCall call)
        public void UpdateLogo(string url, ApiCall call)
        {
            if (!url.ToLower().StartsWith("https://"))
            {
                var error = Hardcoded.GetValue("URL must start with https://", call.Context);
            }

            var submitUrl = Kooboo.Data.Helper.AccountUrlHelper.Org("UpdateBimiLogo");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Logo", url);

            var submitOk = Kooboo.Lib.Helper.HttpHelper.Get2<bool>(
                submitUrl,
                data,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }

        //   public bool UpdateBimiVmc(string VMC, Guid OrgId, ApiCall call)
        public void UpdateVMC(string url, ApiCall call)
        {
            if (!url.ToLower().StartsWith("https://"))
            {
                var error = Hardcoded.GetValue("URL must start with https://", call.Context);
            }

            var submitUrl = Kooboo.Data.Helper.AccountUrlHelper.Org("UpdateBimiVmc");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("VMC", url);

            var submitOk = Kooboo.Lib.Helper.HttpHelper.Get2<bool>(
                submitUrl,
                data,
                Data.Helper.ApiHelper.GetAuthHeaders(call.Context)
            );
        }
    }
    public record BimiSetting
    {
        private string _logo;
        public string Logo
        {
            get
            {
                if (_logo == null)
                {
                    return "None";
                }
                return _logo;
            }
            set
            {
                _logo = value;
            }
        }
        public string LogoName { get; set; }
        public string LogoTitle { get; set; }
        public string LogoDescription { get; set; }

        private string _vmc;
        public string VMC
        {
            get
            {
                if (_vmc == null)
                {
                    return "None";
                }
                return _vmc;
            }
            set
            {
                _vmc = value;
            }
        }
        public string VMCName { get; set; }
        public string VMCTitle { get; set; }
        public string VMCDescription { get; set; }
    }
    public class ImapSetting
    {
        public MailboxSetting Imap { get; set; } = new MailboxSetting();
        public MailboxSetting Smtp { get; set; } = new MailboxSetting();
    }
    public class MailboxSetting
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SSL { get; set; }
    }

}
