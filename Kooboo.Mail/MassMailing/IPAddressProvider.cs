using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DNS.Client;
using DNS.Protocol;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail.MassMailing
{


    public static class IPAddressProvider
    {

        private static MemoryCache IPCache { get; set; } = new MemoryCache(new MemoryCacheOptions());

        private static object _locker = new object();


        public static ConcurrentDictionary<string, IPQueryTask> DomainTasks = new();


        public static Task<List<IPAddress>> GetIPQueryTask(string Domain)
        {
            if (DomainTasks.TryGetValue(Domain, out var value))
            {
                if (value != null && value.Task != null && value.LastModified > DateTime.Now.AddMinutes(-3))
                {
                    return value.Task;
                }
            }

            lock (_locker)
            {
                if (DomainTasks.TryGetValue(Domain, out var value2))
                {
                    if (value2 != null && value2.Task != null && value2.LastModified > DateTime.Now.AddMinutes(-3))
                    {
                        return value2.Task;
                    }
                }

                var queryTask = QueryIPRecords(Domain);
                var DomainTask = new IPQueryTask() { Task = queryTask, Domain = Domain };
                DomainTasks[Domain] = DomainTask;
                return queryTask;

            }

        }

        public static async Task<List<IPAddress>> GetIP(string host)
        {
            host = host.ToLower();

            if (IPCache.TryGetValue<List<IPAddress>>(host, out var value))
            {
                return value;
            }
            else
            {
                var result = await GetIPQueryTask(host);

                if (result != null && result.Any())
                {
                    IPCache.Set(host, result, TimeSpan.FromHours(6));
                }
                else
                {
                    if (IPCache.TryGetValue<List<IPAddress>>(host, out var value2))
                    {
                        return value2;
                    }
                }

                return result;
            }

        }

        public static async Task<List<IPAddress>> QueryIPRecords(string host)
        {
            List<IPAddress> result = new();

            foreach (var item in DNSServerProvider.Servers)
            {
                try
                {
                    ClientRequest request = new ClientRequest(item);

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(host), RecordType.A));
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
                            var AAnswer = answer as DNS.Protocol.ResourceRecords.IPAddressResourceRecord;
                            if (AAnswer != null)
                            {
                                result.Add(AAnswer.IPAddress);
                            }
                        }
                        return result;
                    }

                    var cNames = response.AnswerRecords.Where(o => o.Type == RecordType.CNAME);

                    if (cNames != null && cNames.Any())
                    {
                        foreach (var cName in cNames)
                        {
                            var cNameAnswer = cName as DNS.Protocol.ResourceRecords.CanonicalNameResourceRecord;
                            if (cNameAnswer != null)
                            {
                                var IPs = await QueryIPRecords(cNameAnswer.CanonicalDomainName.ToString());

                                if (IPs != null)
                                {
                                    return IPs;
                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {

                }
            }
            return result;
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


    }

    public class IPQueryTask
    {
        public Task<List<IPAddress>> Task { get; set; }

        public string Domain { get; set; }

        public DateTime LastModified { get; set; } = DateTime.Now;

    }




}
