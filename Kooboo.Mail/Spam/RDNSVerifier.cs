using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Spam
{
    public class RDNSVerifier
    {

        private static Dictionary<string, RDNSRecord> cacheRecords = new Dictionary<string, RDNSRecord>();

        public static async Task<TestResult> IsValidRDNS(string FQDN, System.Net.IPAddress ip)
        {
            if (FQDN == null || ip == null)
            {
                return TestResult.NoResult;
            }

            string key = ip.ToString();
            string host = FQDN.ToLower();

            if (cacheRecords.TryGetValue(key, out var cache))
            {
                if (cache.Host == host)
                {
                    return TestResult.Pass;
                }

                if (cache.lastModified > DateTime.Now.AddHours(-12))
                {
                    return TestResult.Failed;
                }
            }

            var hostDNS = await Kooboo.Lib.DnsRequest.RequestManager.GetReverseDnsAsync(ip);

            if (hostDNS == null)
            {
                return TestResult.NoResult;
            }

            hostDNS = hostDNS.ToLower();
            cacheRecords[key] = new RDNSRecord() { Host = hostDNS, lastModified = DateTime.Now };
            return hostDNS == host ? TestResult.Pass : TestResult.Failed;
        }
    }

    public record RDNSRecord
    {
        public string Host { get; set; }

        public DateTime lastModified { get; set; }
    }


}
