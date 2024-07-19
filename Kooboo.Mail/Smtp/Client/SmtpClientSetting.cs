using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Kooboo.Mail.Smtp.Client
{
    public static class SmtpClientSetting
    {
        public static SslClientAuthenticationOptions Options()
        {
            return new SslClientAuthenticationOptions()
            {
                CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck,

                LocalCertificateSelectionCallback = (a, b, c, d, f) =>
                {

                    return null;
                },

                RemoteCertificateValidationCallback = (a, b, c, d) =>
                {
                    return true;
                }
            };
        }


        private static X509Certificate2Collection CertCol { get; set; }

        public static X509Certificate2Collection ClientCertificates(string clientHost)
        {
            X509Certificate2Collection result = new X509Certificate2Collection();
            //result.Add();
            return result;
        }
    }
}
