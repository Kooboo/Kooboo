using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Kooboo.HttpServer;

namespace Kooboo.Web.Security
{
    public class SslProvider : Kooboo.HttpServer.ISslCertificateProvider
    {

        public SslProvider()
        {
            diskcache = LoadDisk(); 
        }

        private Dictionary<string, X509Certificate2> diskcache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);


        private Dictionary<string, X509Certificate2> cache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        X509Certificate ISslCertificateProvider.SelectCertificate(string hostName)
        {
            if (hostName != null)
            {
                if (cache.ContainsKey(hostName))
                {
                    var item = cache[hostName];
                    if (item.NotAfter > DateTime.Now)
                    {
                        return item;
                    }
                }

                var cert = Kooboo.Data.GlobalDb.SslCertificate.GetByDomain(hostName);

                if (cert != null)
                {
                    var certificate = new X509Certificate2(cert.Content, "kooboo");
                    if (certificate != null)
                    {
                        cache[hostName] = certificate;
                    }

                    return certificate;
                }

                if (diskcache.ContainsKey(hostName))
                {
                    var diskcert = diskcache[hostName]; 
                    if (diskcert !=null)
                    {
                        cache[hostName] = diskcert;
                    }

                    return diskcert; 
                } 

            }

            return Kooboo.Data.Server.SslCertificate.DefaultCert;
            // return null;    
        }

        public Dictionary<string, X509Certificate2> LoadDisk()
        {
            Dictionary<string, X509Certificate2> result = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase); 

            string folder = Kooboo.Data.AppSettings.RootPath;
            folder = Lib.Helper.IOHelper.CombinePath(folder, "ssl");

            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);
            var allfiles = System.IO.Directory.GetFiles(folder, "*.pfx");

            foreach (var item in allfiles)
            {             
                var info = new System.IO.FileInfo(item);   
                string hostname = info.Name;   

                var allbytes = System.IO.File.ReadAllBytes(item);   
                var cert =  new X509Certificate2(allbytes, "");
                if (cert == null)
                {
                    cert = new X509Certificate2(allbytes, "kooboo"); 
                }
                if (cert !=null)
                {
                    string hostName = cert.GetNameInfo(X509NameType.DnsName, false);

                    result[hostName] = cert; 
                }
            }             
            return result; 

        }


    }
}
