using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace Kooboo.Lib.DnsRequest
{
    public static class RequestManager
    {
        public static List<MXRecord> GetMxRecords(string host)
        {
            List<MXRecord> result = new List<MXRecord>();
            var lookup = new LookupClient(NameServer.Cloudflare);

            var response = lookup.QueryAsync(host, QueryType.MX).Result;
            var mxs = response.Answers.MxRecords();
            if (mxs != null)
            {
                foreach (var item in mxs)
                {
                    MXRecord record = new MXRecord();
                    record.exchange = item.Exchange;
                    record.preference = item.Preference;

                    if (record.exchange != null && record.exchange.EndsWith("."))
                    {
                        record.exchange = record.exchange.Substring(0, record.exchange.Length - 1);
                    }

                    result.Add(record);
                }
            }
            return result;
        }

        private static LookupClientOptions _options;
        private static LookupClientOptions options
        {
            get
            {
                if (_options == null)
                {
                    LookupClientOptions options = new LookupClientOptions();
                    options.Timeout = new System.TimeSpan(0, 0, 10);
                    _options = options;
                }

                return _options;
            }
        }

        public static List<string> GetMx(string dnsServer, string Host)
        {
            LookupClientOptions localOption = GetDnsServer(dnsServer);
            return GetMx(Host, localOption);
        }

        public static List<string> GetMx(string host, LookupClientOptions _options = default)
        {
            List<MXRecord> result = new List<MXRecord>();
            var lookup = new LookupClient(_options ?? options);
            lookup.Timeout = new System.TimeSpan(0, 0, 10);


            var response = lookup.QueryAsync(host, QueryType.MX).Result;
            if (response == null)
            {
                return new List<string>();
            }
            var mxs = response.Answers.MxRecords();
            if (mxs != null)
            {
                foreach (var item in mxs)
                {
                    MXRecord record = new MXRecord();
                    record.exchange = item.Exchange;
                    if (record.exchange != null && record.exchange.EndsWith('.'))
                    {
                        record.exchange = record.exchange.TrimEnd('.');
                    }
                    record.preference = item.Preference;

                    result.Add(record);
                }
            }
            return result.OrderBy(o => o.preference).Select(o => o.exchange).ToList();

        }


        public static List<string> GetText(string host)
        {
            var lookup = new LookupClient(options);

            var response = lookup.QueryAsync(host, QueryType.TXT).Result;
            if (response == null)
            {
                return new List<string>();
            }
            var records = response.Answers.AsQueryable();
            if (records != null)
            {
                List<string> result = new List<string>();

                foreach (var item in records)
                {
                    if (item is TxtRecord)
                    {
                        var txt = item as TxtRecord;
                        result.AddRange(txt.Text);
                    }
                }

                return result;
            }

            return null;

        }


        public static List<ARecord> GetARecords(string host, LookupClientOptions _options = null)
        {

            List<ARecord> result = new List<ARecord>();
            LookupClient lookup;
            if (_options == null)
            {
                lookup = new LookupClient(options);
            }
            else
            {
                lookup = new LookupClient(_options);
            }



            var response = lookup.QueryAsync(host, QueryType.A).Result;
            if (response == null)
            {
                return result;
            }
            var aRecords = response.Answers.ARecords();
            if (aRecords != null)
            {
                foreach (var item in aRecords)
                {
                    ARecord record = new ARecord();
                    record.IpAddress = item.Address;
                    record.Host = item.DomainName;
                    result.Add(record);
                }
            }

            if (result.Count > 0)
            {
                return result;
            }

            while (!result.Any() && response.Answers != null && response.Answers.CnameRecords != null)
            {
                var cnames = response.Answers.CnameRecords();
                if (cnames != null && cnames.Any())
                {
                    string newName = cnames.FirstOrDefault().CanonicalName;
                    response = lookup.QueryAsync(newName, QueryType.A).Result;

                    aRecords = response.Answers.ARecords();
                    if (aRecords != null)
                    {
                        foreach (var item in aRecords)
                        {
                            ARecord record = new ARecord();
                            record.IpAddress = item.Address;
                            record.Host = item.DomainName;
                            result.Add(record);
                        }

                        if (result.Count > 0)
                        {
                            return result;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static List<ARecord> GetARecords(string dnsServer, string Host)
        {
            LookupClientOptions localOption = GetDnsServer(dnsServer);
            return GetARecords(Host, localOption);
        }

        private static LookupClientOptions GetDnsServer(string dnsServer)
        {
            System.Net.IPAddress dnsIP = null;

            if (System.Net.IPAddress.TryParse(dnsServer, out System.Net.IPAddress ip))
            {
                dnsIP = ip;
            }
            else
            {
                var aRecords = GetARecords(dnsServer);
                if (aRecords != null && aRecords.Any())
                {
                    dnsIP = aRecords[0].IpAddress;
                }
            }

            List<System.Net.IPAddress> ips = new()
            {
                dnsIP
            };

            LookupClientOptions localOption = new(ips.ToArray())
            {
                Timeout = new System.TimeSpan(0, 0, 10)
            };

            return localOption;
        }

        public static List<string> GetNameServers(string host)
        {
            List<string> nameServers = new List<string>();

            var lookup = new LookupClient(DnsClient.NameServer.Cloudflare);

            var response = lookup.QueryAsync(host, QueryType.NS).Result;
            var ns = response.Answers.NsRecords();
            if (ns != null)
            {
                foreach (var item in ns)
                {
                    if (item != null && item.NSDName != null && item.NSDName.Value != null)
                    {
                        var nsDomain = item.NSDName.Value.ToString();
                        if (nsDomain.EndsWith("."))
                        {
                            nsDomain = nsDomain.TrimEnd('.');
                        }
                        nameServers.Add(nsDomain);
                    }


                }
            }
            return nameServers;
        }
        public static async Task<List<string>> GetNameServersAsync(string host)
        {
            List<string> nameServers = new List<string>();

            var lookup = new LookupClient(DnsClient.NameServer.Cloudflare);

            var response = await lookup.QueryAsync(host, QueryType.NS);
            var ns = response.Answers.NsRecords();
            if (ns != null)
            {
                foreach (var item in ns)
                {
                    if (item != null && item.NSDName != null && item.NSDName.Value != null)
                    {
                        var nsDomain = item.NSDName.Value.ToString();
                        if (nsDomain.EndsWith("."))
                        {
                            nsDomain = nsDomain.TrimEnd('.');
                        }
                        nameServers.Add(nsDomain);
                    }


                }
            }
            return nameServers;
        }

        public static string GetReverseDns(System.Net.IPAddress ip)
        {
            try
            {
                var result = System.Net.Dns.GetHostEntry(ip);
                if (result != null && result.HostName != null)
                {
                    return result.HostName;
                }
            }
            catch (System.Exception)
            {

            }

            return null;
        }

        public static async Task<string> GetReverseDnsAsync(System.Net.IPAddress ip)
        {
            try
            {
                var result = await System.Net.Dns.GetHostEntryAsync(ip);
                if (result != null && result.HostName != null)
                {
                    return result.HostName;
                }
            }
            catch (System.Exception)
            {

            }

            return null;
        }
    }
}