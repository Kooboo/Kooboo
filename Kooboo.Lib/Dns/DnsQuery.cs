using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using DNS.Client;
using DNS.Protocol;
using DNS.Protocol.ResourceRecords;

namespace Kooboo.Lib.DnsRequest
{
    public class DnsQuery
    {

        public static async Task<IEnumerable<IResourceRecord>> QueryRecords(string host, RecordType recordType)
        {
            foreach (var item in Servers)
            {
                try
                {
                    ClientRequest request = new ClientRequest(item);

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(host), recordType));
                    request.RecursionDesired = true;
                    request.OperationCode = OperationCode.Query;

                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(10000);

                    IResponse response = await request.Resolve(source.Token);
                    var answers = response.AnswerRecords.Where(r => r.Type == recordType);

                    return answers;

                }
                catch (Exception)
                {

                }
            }
            return null;
        }


        public static async Task<IPAddress> QueryAFromRootDnsAsync(string domain)
        {
            var dnsServer = Kooboo.Lib.Whois.Service.GetDnsServer(domain);
            foreach (var item in dnsServer)
            {
                try
                {

                    var ip = System.Net.Dns.GetHostAddresses(item);

                    if (ip == null || !ip.Any())
                    {
                        continue;
                    }

                    ClientRequest request = new ClientRequest(ip.FirstOrDefault());

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(domain), RecordType.A));
                    request.RecursionDesired = true;
                    request.OperationCode = OperationCode.Query;

                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(10000);

                    IResponse response = await request.Resolve(source.Token);
                    var answers = response.AnswerRecords.Where(r => r.Type == RecordType.A);

                    if (answers != null && answers.Any())
                    {
                        foreach (var answer in answers)
                        {
                            var record = answer as DNS.Protocol.ResourceRecords.IPAddressResourceRecord;
                            if (record != null)
                            {
                                return record.IPAddress;
                            }
                        }
                    }

                    var CNameAnswers = response.AnswerRecords.Where(r => r.Type == RecordType.CNAME);

                    if (CNameAnswers != null && CNameAnswers.Any())
                    {
                        foreach (var answer in CNameAnswers)
                        {
                            var record = answer as DNS.Protocol.ResourceRecords.CanonicalNameResourceRecord;
                            if (record != null)
                            {

                                var cDomain = record.CanonicalDomainName.ToString();
                                return await QueryAFromRootDnsAsync(cDomain);

                            }
                        }
                    }


                }
                catch (Exception)
                {

                }
            }
            return null;

        }

        public static async Task<string> QueryCName(string FullDomain)
        {
            var Answers = await Kooboo.Lib.DnsRequest.DnsQuery.QueryRecords(FullDomain, DNS.Protocol.RecordType.CNAME);

            if (Answers != null)
            {
                foreach (var cname in Answers)
                {
                    var record = cname as DNS.Protocol.ResourceRecords.CanonicalNameResourceRecord;
                    if (record != null)
                    {
                        var cDomain = record.CanonicalDomainName.ToString();
                        return cDomain;
                    }
                }
            }
            return null;
        }


        public static async Task<string> QueryTxt(string FullDomain)
        {
            var Answers = await Kooboo.Lib.DnsRequest.DnsQuery.QueryRecords(FullDomain, DNS.Protocol.RecordType.TXT);

            string text = string.Empty;

            if (Answers != null)
            {
                foreach (var txt in Answers)
                {
                    var record = txt as DNS.Protocol.ResourceRecords.TextResourceRecord;
                    if (record != null && record.Data != null && record.Data.Length > 0)
                    {
                        var RawValue = System.Text.Encoding.UTF8.GetString(record.Data);
                        text += RawValue + "\r\n";
                    }
                }
            }
            return !string.IsNullOrEmpty(text) ? text.Trim() : null;
        }

        public static async Task<List<MXRecord>> QueryMX(string FullDomain)
        {
            var Answers = await Kooboo.Lib.DnsRequest.DnsQuery.QueryRecords(FullDomain, DNS.Protocol.RecordType.MX);

            List<MXRecord> result = new List<MXRecord>();

            if (Answers != null)
            {
                foreach (var mx in Answers)
                {
                    var record = mx as DNS.Protocol.ResourceRecords.MailExchangeResourceRecord;
                    if (record != null)
                    {
                        result.Add(new MXRecord() { exchange = record.ExchangeDomainName.ToString().ToLower(), preference = record.Preference });
                    }
                }
            }

            return result.OrderBy(o => o.exchange).ToList();
        }


        private static List<IPAddress> _dns;
        public static List<IPAddress> Servers
        {
            get
            {
                if (_dns == null)
                {
                    var list = GetDnsServers();
                    _dns = list;
                }
                return _dns;
            }

            set
            {
                _dns = value;
            }

        }

        public static List<IPAddress> GetDnsServers()
        {
            List<IPAddress> DnsServer = new List<IPAddress>();

            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var type = networkInterface.NetworkInterfaceType;

                    if (type == NetworkInterfaceType.Ethernet || type == NetworkInterfaceType.Ethernet3Megabit || type == NetworkInterfaceType.FastEthernetFx || type == NetworkInterfaceType.FastEthernetT || type == NetworkInterfaceType.GigabitEthernet || type == NetworkInterfaceType.Wireless80211)
                    {

                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        if (ipProperties != null)
                        {
                            IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                            foreach (IPAddress dnsAddress in dnsAddresses)
                            {
                                if (dnsAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork || dnsAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                                {
                                    DnsServer.Add(dnsAddress);
                                }
                            }
                        }
                    }

                }
            }

            DnsServer.Add(System.Net.IPAddress.Parse("1.1.1.1"));

            DnsServer.Add(System.Net.IPAddress.Parse("8.8.8.8"));

            return DnsServer;
        }

    }



}
