
using System;
using System.Collections.Concurrent;
using System.Net;

namespace Kooboo.Mail.SecurityControl
{
    public static class ConnectionSpeed
    {
        public static ConcurrentDictionary<string, DateTime> IpConnection { get; set; } = new ConcurrentDictionary<string, DateTime>();

        public static bool CanConnect(string IPV4, IPAddress OriginalIP)
        {
            if (IpConnection.TryGetValue(IPV4, out DateTime LastTime))
            {
                var elaspe = DateTime.Now - LastTime;
                if (elaspe.TotalMilliseconds < 100)
                {
                    //violation
                    //rule 2.
                    return IsAcceptRDNS(OriginalIP);

                }
                else
                {
                    IpConnection[IPV4] = DateTime.Now;
                }
            }

            IpConnection[IPV4] = DateTime.Now;

            return true;

        }


        public static bool IsAcceptRDNS(System.Net.IPAddress ip)
        {
            var RDNS = Kooboo.Lib.DnsRequest.RequestManager.GetReverseDns(ip);

            if (!string.IsNullOrEmpty(RDNS))
            {
                var domain = Kooboo.Lib.Domain.DomainService.Parse(RDNS);
                if (domain != null && domain.Root != null)
                {
                    return true;
                }
            }

            Kooboo.Data.Log.Instance.EmailDebug.Write("IP Send Too Fast: " + ip.ToString());
            if (RDNS != null)
            {
                Kooboo.Data.Log.Instance.EmailDebug.Write(RDNS);
            }

            return false;
        }

    }



}
