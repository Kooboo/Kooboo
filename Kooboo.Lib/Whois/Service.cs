using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Lib.Whois
{
    public static class Service
    {
        public static WhoisRecord Lookup(string Host)
        {
            try
            {
                var result = WhoisServer.instance.GetRecord(Host);
                if (result == null)
                {
                    result = new WhoisRecord();
                }

                if (result.NameServers == null || !result.NameServers.Any())
                {
                    result.NameServers = Lib.DnsRequest.RequestManager.GetNameServers(Host);
                }
                return result;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public static async Task<WhoisRecord> LookupAsync(string Host)
        {
            try
            {
                var result = await WhoisServer.instance.GetRecordAsync(Host);
                if (result == null)
                {
                    result = new WhoisRecord();
                }

                if (result.NameServers == null || !result.NameServers.Any())
                {
                    result.NameServers = await Lib.DnsRequest.RequestManager.GetNameServersAsync(Host);
                }
                return result;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public static DateTime GetDomainExpires(string domain)
        {
            var server = new WhoisServer();
            return server.GetExpires(domain);
        }

        public static List<string> GetDnsServer(string host)
        {
            var records = Lookup(host);
            if (records != null)
            {
                return records.NameServers;
            }
            return null;
        }


        public static async Task<List<string>> GetDnsServerAsync(string host)
        {
            var records = Lookup(host);
            if (records != null)
            {
                return records.NameServers;
            }
            return null;
        }


    }





}
