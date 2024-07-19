using System;
using System.Collections.Generic;
using Kooboo.Mail.DnsQuery;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Crypto;

namespace Kooboo.Mail.Dkim
{
    public static class PublicKeyFinder
    {
        public static MemoryCache KeyCache = new MemoryCache(new MemoryCacheOptions() { });

        public static IDnsRequest _dnsRequest = new DnsRequest();

        private static List<string> DnsLookupText(string domain, string selector)
        {
            var FullDomain = selector + "._domainkey." + domain;
            return _dnsRequest.GetTxtRecords(FullDomain);
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
                return key!;
            }

            var text = GetDnsText(domain, selector);
            var newKey = new KeyLocator().ExtractPublicKey(text);
            if (newKey != null)
            {
                KeyCache.Set(FullDomain, newKey, DateTimeOffset.Now.AddDays(2));
            }
            else
            {
                KeyCache.Set(FullDomain, newKey, DateTimeOffset.Now.AddMinutes(20));
            }

            return newKey!;
        }
    }
}

