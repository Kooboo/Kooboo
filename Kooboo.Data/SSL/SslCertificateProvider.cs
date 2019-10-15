using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Kooboo.Data.Server
{
    public static class SslCertificateProvider
    {
        static SslCertificateProvider()
        {
            _diskcache = LoadDisk();
        }

        private static Dictionary<string, X509Certificate2> _diskcache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, X509Certificate2> _cache = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

        public static X509Certificate SelectCertificate(string hostName)
        {
            if (hostName != null)
            {
                if (_cache.ContainsKey(hostName))
                {
                    var item = _cache[hostName];
                    if (item.NotAfter > DateTime.Now)
                    {
                        return item;
                    }
                }

                var cert = Kooboo.Data.GlobalDb.SslCertificate.GetByDomain(hostName);

                if (cert != null)
                {
                    var certificate = TryGetCert(cert.Content);
                    if (certificate != null)
                    {
                        _cache[hostName] = certificate;
                    }

                    return certificate;
                }

                if (_diskcache.ContainsKey(hostName))
                {
                    var diskcert = _diskcache[hostName];
                    if (diskcert != null)
                    {
                        _cache[hostName] = diskcert;
                    }
                    return diskcert;
                }
            }

            return Kooboo.Data.Server.SslCertificate.DefaultCert;
            // return null;
        }

        public static X509Certificate2 SelectCertificate2(string hostName)
        {
            if (hostName != null)
            {
                if (_cache.ContainsKey(hostName))
                {
                    var item = _cache[hostName];
                    if (item.NotAfter > DateTime.Now)
                    {
                        return item;
                    }
                }

                var cert = Kooboo.Data.GlobalDb.SslCertificate.GetByDomain(hostName);

                if (cert != null)
                {
                    var certificate = TryGetCert(cert.Content);
                    if (certificate != null)
                    {
                        _cache[hostName] = certificate;
                    }

                    return certificate;
                }

                if (_diskcache.ContainsKey(hostName))
                {
                    var diskcert = _diskcache[hostName];
                    if (diskcert != null)
                    {
                        _cache[hostName] = diskcert;
                    }
                    return diskcert;
                }
            }

            return Kooboo.Data.Server.SslCertificate.DefaultCert;
            // return null;
        }

        public static Dictionary<string, X509Certificate2> LoadDisk()
        {
            Dictionary<string, X509Certificate2> result = new Dictionary<string, X509Certificate2>(StringComparer.OrdinalIgnoreCase);

            string folder = Kooboo.Data.AppSettings.RootPath;
            folder = System.IO.Path.Combine(folder, "AppData");
            folder = Lib.Helper.IOHelper.CombinePath(folder, "ssl");

            Kooboo.Lib.Helper.IOHelper.EnsureDirectoryExists(folder);

            var allfiles = System.IO.Directory.GetFiles(folder, "*.pfx");

            foreach (var item in allfiles)
            {
                var info = new System.IO.FileInfo(item);
                string hostname = info.Name;

                var allbytes = System.IO.File.ReadAllBytes(item);
                var cert = TryGetCert(allbytes);
                if (cert != null)
                {
                    string hostName = cert.GetNameInfo(X509NameType.DnsName, false);

                    result[hostName] = cert;
                }
            }
            return result;
        }

        public static X509Certificate2 TryGetCert(byte[] content)
        {
            List<string> trypass = new List<string> {"kooboo", ""};

            foreach (var item in trypass)
            {
                try
                {
                    var cert = new X509Certificate2(content, item);
                    if (cert != null)
                    {
                        return cert;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return null;
        }
    }
}