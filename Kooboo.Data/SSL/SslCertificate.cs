using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Server
{
 public  static  class SslCertificate
    {
        static SslCertificate()
        {
            var bytes = Kooboo.Data.Embedded.EmbeddedHelper.GetBytes("kooboo.pfx", typeof(Kooboo.Data.Server.WebServer));  

            var cert =  new X509Certificate2(bytes, "kooboo");

            DefaultCert = cert;
        }
             
        public static X509Certificate DefaultCert { get; set; }
                              
 
    }
}
