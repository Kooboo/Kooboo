using Kooboo.Mail.Models;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Kooboo.Mail
{
    public static class Settings
    { 
        // the mta server ip. This should only defined in the Mail Server.
        public static string Mta { get; set; }

        public static int MtaPort { get; set; }

        private static bool HasDefineMta
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Mta) && MtaPort > 0;
            }
        }

        static Settings()
        {
            Mta = ConfigurationManager.AppSettings.Get("Mta");
            string strport = ConfigurationManager.AppSettings.Get("MtaPort");

            if (!string.IsNullOrWhiteSpace(strport))
            {
                MtaPort = int.Parse(strport);
            }
            else
            {
                MtaPort = 587;
            }

            nextcheck = System.DateTime.Now.AddHours(-10);
        }

        public static async Task<SendSetting> GetSendSetting(Data.Models.ServerSetting serverSetting, bool IsOnlineServer, string MailFrom, string RcptTo)
        { 
            SendSetting setting = new SendSetting();

            if (IsOnlineServer)
            {
                if (HasDefineMta)
                {
                    setting.UseKooboo = true;
                    setting.KoobooServerIp = Mta;
                    setting.Port = MtaPort;
                    setting.HostName = System.Net.Dns.GetHostName();
                    setting.LocalIp = System.Net.IPAddress.Any;
                    setting.OkToSend = true;
                }

                else if (!string.IsNullOrEmpty(serverSetting.SmtpServerIP))
                {
                    setting.UseKooboo = true;
                    setting.KoobooServerIp = serverSetting.SmtpServerIP;
                    setting.Port = serverSetting.SmtpPort;
                    if (setting.Port <= 0)
                    {
                        setting.Port = 587;
                    }
                    setting.HostName = System.Net.Dns.GetHostName();
                    setting.LocalIp = System.Net.IPAddress.Any;
                    setting.OkToSend = true;
                }
                else
                {
                    setting.OkToSend = false;
                    setting.ErrorMessage = "Email Sending is prevented";
                } 
            }
            else
            {
                var mxs = await Kooboo.Mail.Utility.SmtpUtility.GetMxRecords(RcptTo);
                if (mxs == null || mxs.Count() == 0)
                {
                    setting.OkToSend = false;
                    setting.ErrorMessage = "Mx records not found";
                }
                else
                {
                    setting.OkToSend = true;
                    setting.Mxs = mxs;

                    if (serverSetting.ReverseDns != null && serverSetting.ReverseDns.Count() > 0)
                    {
                        var dns = serverSetting.ReverseDns[0];  //TODO: random it.  
                        setting.LocalIp = System.Net.IPAddress.Parse(dns.IP);
                        setting.HostName = dns.HostName;
                    }
                    else
                    {
                        setting.LocalIp = System.Net.IPAddress.Any;
                        setting.HostName = System.Net.Dns.GetHostName();
                    }
                }
            }
            return setting;
        }

        public static bool ForwardRequired
        {
            get
            {
                //return false; 
                return !HasDefineMta && Data.AppSettings.IsOnlineServer;
            }
        }


        private static string _mailserverip;

        public static string MailServerIP
        {
            get
            {
                if (Kooboo.Mail.Settings.ForwardRequired)
                {
                    if (string.IsNullOrWhiteSpace(_mailserverip))
                    {
                        _mailserverip = GetMailServer();
                        nextcheck = DateTime.Now.AddHours(10);
                    }
                    else
                    {
                        if (nextcheck < DateTime.Now)
                        {
                            nextcheck = DateTime.Now.AddHours(10);
                            System.Threading.Thread t = new System.Threading.Thread(setAsync);
                            t.Start();
                        }
                    }
                    return _mailserverip;
                }
                return _mailserverip;
            }
        }


        private static void setAsync()
        {
            var mailserver = GetMailServer();
            if (!string.IsNullOrWhiteSpace(mailserver))
            {
                _mailserverip = mailserver;
            }
        }

        private static System.DateTime nextcheck { get; set; }

        private static string GetMailServer()
        {
            string url = Kooboo.Data.Helper.AccountUrlHelper.System("MailServer");
            var ip = Lib.Helper.HttpHelper.TryGet<string>(url);

            if (ip != null && Lib.Helper.IPHelper.IsIP(ip))
            {
                return ip;
            }  
            ip = Lib.Helper.HttpHelper.TryGet<string>(url); 
            if (ip != null && Lib.Helper.IPHelper.IsIP(ip))
            {
                return ip;
            } 
            return null;
        }
    }
}
