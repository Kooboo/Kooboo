using System;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.Models;

namespace Kooboo.Mail
{
    public static class Settings
    {
        static Settings()
        {

        }

        public static async Task<SendSetting> GetSendSetting(bool IsOnlineServer, string mailFrom, string RCPTTO)
        {
            SendSetting setting = new();
            //#if DEBUG
            //            setting.KoobooServerIp = "127.0.0.1";
            //            // setting.KoobooServerIp = "mta3.kmailserver.com";
            //            setting.OkToSend = true;
            //            setting.HostName = System.Net.Dns.GetHostName();
            //            setting.LocalIp = System.Net.IPAddress.Any;
            //            setting.UseKooboo = true;
            //            setting.Port = 50025;
            //            return setting;
            //#endif

            if (IsOnlineServer)
            {
                setting.UseKooboo = true;
                setting.Port = 50025;
                setting.Server = "mta3.kmailserver.com";
                setting.OkToSend = true;
                setting.HostName = System.Net.Dns.GetHostName();
                setting.LocalIp = System.Net.IPAddress.Any;
            }
            else
            {
                var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(mailFrom);
                if (orgDb != null)
                {
                    var smtpSetting = orgDb.SmtpGet();
                    if (smtpSetting != null && !string.IsNullOrWhiteSpace(smtpSetting.Server) && smtpSetting.Port > 0)
                    {
                        return new SendSetting() { OkToSend = true, CustomSmtp = true, Server = smtpSetting.Server, UserName = smtpSetting.UserName, Password = smtpSetting.Password, HostName = System.Net.Dns.GetHostName() };
                    }
                }


                var mxs = await Kooboo.Mail.Utility.SmtpUtility.GetMxRecords(RCPTTO);
                if (mxs == null || mxs.Count() == 0)
                {
                    setting.OkToSend = false;
                    setting.ErrorMessage = "Mx records not found";
                }
                else
                {
                    setting.OkToSend = true;
                    setting.Mxs = mxs;

                    setting.LocalIp = System.Net.IPAddress.Any;
                    setting.HostName = System.Net.Dns.GetHostName();
                }
            }

            return setting;
        }

        public static string ImapDomain
        {
            get
            {
                //#if DEBUG
                //                {
                //                    return "mx.localkooboo.com"; 
                //                }
                //#endif
                return "mx.imapsetting.com";
            }
        }

        public static string SmtpDomain
        {
            get
            {
                //#if DEBUG
                //                {
                //                    return "mx.localkooboo.com";
                //                }
                //#endif
                return "mx.sitepapa.com";
            }
        }

        public static string Port587SmtpDomain
        {
            get
            {
                return "mx.imapsetting.com";
            }
        }

        public static bool ForwardRequired
        {
            get; set;

        }

    }
}