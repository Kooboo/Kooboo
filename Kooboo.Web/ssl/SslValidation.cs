using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.ssl
{
    
    public static  class Sslvalidation
    {
        static Sslvalidation()
        {
            tokenurl ="http://www.sslgenerator.com";
        }

        public static string tokenurl { get; set; }
         
        public static bool Verify(string domain, string code)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);
            para.Add("code", code);
            string vefiryurl = tokenurl + "/_api/validator/SelfCheck";
            return Lib.Helper.HttpHelper.TryGet<bool>(vefiryurl, para);
        }


        public static string GetToken(string domain)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("domain", domain);
            var  url  = tokenurl + "/_api/validator/HttpToken";
            return Lib.Helper.HttpHelper.TryGet<string>(url, para);
        }
          
    }


}
