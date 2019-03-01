using System;
using System.Collections.Generic;
using Kooboo.Data.Models;
using Kooboo.Data.Helper;
using Kooboo.Lib.Helper;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CertificateService
    {
        public static async Task<X509Certificate> GetCertificate(string host)
        {
            var certificate = await Get(host);
            if (certificate == null)
                return null;

            return new X509Certificate2(certificate.Content, "");
        }
        public static async Task<Certificate> Get(string host)
        {
            var url = AccountUrlHelper.Certificate("get");
            Dictionary<string, string> paraDic = new Dictionary<string, string>();
            paraDic.Add("domain", host);
            string para = JsonHelper.Serialize(paraDic);
            Certificate certificate = null;
            try
            {
                certificate =await HttpHelper.GetAsync<Certificate>(url, paraDic);
            }
            catch (Exception ex)
            {

            }
            return certificate;
            
        }
    }
}
