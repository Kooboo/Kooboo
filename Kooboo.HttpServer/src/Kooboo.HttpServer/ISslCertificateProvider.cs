using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Kooboo.HttpServer
{
    public interface ISslCertificateProvider
    {
         X509Certificate SelectCertificate(string hostName);
    }
}
