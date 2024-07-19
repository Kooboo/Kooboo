using System;
using System.Threading;
using System.Threading.Tasks;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;

namespace Kooboo.Mail.Dkim
{
    public class KeyLocator : DkimPublicKeyLocatorBase
    {
        public override AsymmetricKeyParameter? LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = default)
        {
            var methodList = methods.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < methodList.Length; i++)
            {
                if (methodList[i] == "dns/txt")
                {
                    return PublicKeyFinder.GetKey(domain, selector);
                }
            }
            return null;
        }

        public AsymmetricKeyParameter ExtractPublicKey(string DnsText) => GetPublicKey(DnsText);

        public override Task<AsymmetricKeyParameter?> LocatePublicKeyAsync(string methods, string domain, string selector, CancellationToken cancellationToken = default)
        {
            var result = LocatePublicKey(methods, domain, selector, cancellationToken);

            return Task.FromResult(result);
        }
    }
}

