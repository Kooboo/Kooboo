using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;

namespace Kooboo.Mail.Spam
{
    public class KDkimVerifier
    {
        List<string> DnsLookupText(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;
            return Kooboo.Lib.DnsRequest.RequestManager.GetText(FullDomain);
        }

        private string GetPublicKey(List<string> textRecords)
        {
            /*
 *  two._domainkey.abc.com subdomain  
 v=DKIM1;t=s;p=MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCWE3zy2aS/rFl4S+E1vBqj6YMqaohp7wpyxP4RzpuZ4epNp9ZOO7AncdllGFTRDCeQhenZh7EzW3/T/fqwYsAYg61tWdlQbEVqKVX1E0rkctlg5wTcGEkxeQguvsH9K/35WT6DgXlQE/AzteePk9BhANZHj2yW2ELxWriWkUH0zwIDAQAB
 */

            foreach (var item in textRecords)
            {
                if (item != null && item.Contains("DKIM", StringComparison.InvariantCultureIgnoreCase))
                {
                    var parts = item.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        var PartWithoutSpace = part.Replace(" ", "");
                        if (PartWithoutSpace.StartsWith("p=") || PartWithoutSpace.StartsWith("P="))
                        {
                            var key = PartWithoutSpace.Substring(2);
                            if (key != null && key.Length > 10)
                            {
                                return key;
                            }
                        }
                    }

                }

            }

            return null;
        }

        public TestResult Verify(string messagebody)
        {
            System.IO.MemoryStream mo = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(messagebody));
            var message = MimeMessage.Load(mo);
            return Verify(message);
        }

        public TestResult Verify(MimeMessage message)
        {
            var verifier = new DkimVerifier(new KeyLocator());
            var index = message.Headers.IndexOf(HeaderId.DkimSignature);
            if (index == -1)
            {
                return TestResult.NoResult;
            }
            var dkim = message.Headers[index];
            if (verifier.Verify(message, dkim))
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Failed;
            }
        }
    }

    public class KeyLocator : DkimPublicKeyLocatorBase
    {
        List<string> DnsLookupText(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;
            return Kooboo.Lib.DnsRequest.RequestManager.GetText(FullDomain);
        }

        public string GetDnsText(string domain, string selector)
        {
            var result = DnsLookupText(domain, selector);
            return string.Join("\r\n", result);
        }

        public override AsymmetricKeyParameter LocatePublicKey(string methods, string domain, string selector, CancellationToken cancellationToken = default)
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

        public AsymmetricKeyParameter ExtractPublicKey(string DnsText)
        {
            return GetPublicKey(DnsText);
        }

        public override Task<AsymmetricKeyParameter> LocatePublicKeyAsync(string methods, string domain, string selector, CancellationToken cancellationToken = default)
        {
            var result = LocatePublicKey(methods, domain, selector, cancellationToken);

            return Task.FromResult<AsymmetricKeyParameter>(result);
        }
    }


    public static class PublicKeyFinder
    {
        public static MemoryCache KeyCache = new MemoryCache(new MemoryCacheOptions() { });



        private static List<string> DnsLookupText(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;
            return Kooboo.Lib.DnsRequest.RequestManager.GetText(FullDomain);
        }

        private static string GetDnsText(string domain, string selector)
        {
            var result = DnsLookupText(domain, selector);
            return string.Join("\r\n", result);
        }


        public static AsymmetricKeyParameter GetKey(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;

            if (KeyCache.TryGetValue<AsymmetricKeyParameter>(FullDomain, out var key))
            {
                return key;
            }

            var text = GetDnsText(domain, selector);

            var newKey = new KeyLocator().ExtractPublicKey(text);

            if (newKey != null)
            {
                KeyCache.Set<AsymmetricKeyParameter>(FullDomain, newKey, DateTimeOffset.Now.AddDays(2));
            }
            else
            {
                KeyCache.Set<AsymmetricKeyParameter>(FullDomain, newKey, DateTimeOffset.Now.AddMinutes(20));
            }

            return newKey;
        }
    }

}