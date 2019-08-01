using Kooboo.Data.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.SSL
{
    
    public static  class SslService
    {
        static SslService()
        {
            tokenurl ="http://www.sslgenerator.com";
        }

        public static string tokenurl { get; set; }
         

        // this is used to self verify token. 
        public static bool Verify(string domain, string code)
        { 
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);
            para.Add("code", code);
            string vefiryurl = tokenurl + "/_api/validator/SelfCheck";
            return Lib.Helper.HttpHelper.TryGet<bool>(vefiryurl, para);
        }

        public static bool EnsureCanGenerate(string domain)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain); 
            string vefiryurl = tokenurl + "/_api/validator/EnsureCheck";
            return Lib.Helper.HttpHelper.TryGet<bool>(vefiryurl, para);
        }


        public static string GetToken(string domain)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);
            var  url  = tokenurl + "/_api/validator/HttpToken";
            return Lib.Helper.HttpHelper.TryGet<string>(url, para);
        }
        
        public static void SetSsl (string domain, Guid Organizationid)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);

            string url = tokenurl + "/_api/SSL/GetPfx";
             
            var certbytes = Lib.Helper.HttpHelper.Get<Data.Models.DataString>(url, para); 

            if (certbytes != null && certbytes.Validate())
            {
                var orgbytes = Convert.FromBase64String(certbytes.Base64String); 
                Kooboo.Data.GlobalDb.SslCertificate.AddCert(Organizationid, domain, orgbytes); 
            } 
        }
         

        public static void AddSslMiddleWare(List<IKoobooMiddleWare> MiddleWareList)
        {
            if (MiddleWareList == null)
            {
                return;
            }

            int len = MiddleWareList.Count();

            var pos = len - 1;
            if (pos <= 0)
            {
                pos = 0;
            }

            MiddleWareList.Insert(pos, new SslCertMiddleWare());
        }


    }


}
