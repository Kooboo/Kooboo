using Kooboo.Data.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Data.SSL
{
    public static class SslService
    {
        static SslService()
        {
            Tokenurl = "http://www.sslgenerator.com";
        }

        public static string Tokenurl { get; set; }

        // this is used to self verify token.
        public static bool Verify(string domain, string code)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"domain", domain}, {"code", code}};
            string vefiryurl = Tokenurl + "/_api/validator/SelfCheck";
            return Lib.Helper.HttpHelper.TryGet<bool>(vefiryurl, para);
        }

        public static bool EnsureCheck(string domain)
        {
            string vefiryurl = Tokenurl + "/_api/validator/ensurecheck?domain=" + domain;
            return Lib.Helper.HttpHelper.TryGet<bool>(vefiryurl);
        }

        public static string GetToken(string domain)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"domain", domain}};
            var url = Tokenurl + "/_api/validator/HttpToken";
            return Lib.Helper.HttpHelper.TryGet<string>(url, para);
        }

        public static void SetSsl(string domain, Guid organizationid)
        {
            // check if exists.
            var cert = Kooboo.Data.GlobalDb.SslCertificate.GetByDomain(domain);
            if (cert != null && cert.Expiration > DateTime.Now.AddDays(10))
            {
                return;
            }

            Dictionary<string, string> para = new Dictionary<string, string> {{"domain", domain}};

            string url = Tokenurl + "/_api/SSL/GetPfx";

            try
            {
                var certbytes = Lib.Helper.HttpHelper.Get<Data.Models.DataString>(url, para);

                if (certbytes != null && certbytes.Validate())
                {
                    var orgbytes = Convert.FromBase64String(certbytes.Base64String);
                    Kooboo.Data.GlobalDb.SslCertificate.AddCert(organizationid, domain, orgbytes);
                }
                else
                {
                    Kooboo.Data.Log.Instance.Exception.Write("SSL generation failed: " + domain);
                }
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.Write(ex.Message + ex.StackTrace);
            }
        }

        public static void AddSslMiddleWare(List<IKoobooMiddleWare> middleWareList)
        {
            if (middleWareList == null)
            {
                return;
            }

            int len = middleWareList.Count();

            var pos = len - 1;
            if (pos <= 0)
            {
                pos = 0;
            }

            middleWareList.Insert(pos, new SslCertMiddleWare());
        }

        public static void EnsureServerHostDomain()
        {
            if (!Data.AppSettings.IsOnlineServer)
            {
                return;
            }

            var setting = Kooboo.Data.AppSettings.ServerSetting;
            if (setting != null && setting.MyIP != "127.0.0.1" && setting.ServerId > 0)
            {
                if (!string.IsNullOrWhiteSpace(setting.HostDomain))
                {
                    var domain = setting.ServerId + "." + setting.HostDomain;
                    Console.WriteLine("generating domain:" + domain);

                    SslService.SetSsl(domain, default(Guid));
                }
            }
        }
    }
}