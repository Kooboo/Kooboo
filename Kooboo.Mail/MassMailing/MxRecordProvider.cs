using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNS.Client;
using DNS.Protocol;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail.MassMailing
{
    public static class MxRecordProvider
    {

        private static MemoryCache MXCache { get; set; } = new MemoryCache(new MemoryCacheOptions());

        private static object _locker = new object();


        private static ConcurrentDictionary<string, DomainQueryTask> DomainTasks { get; set; } = new();


        private static Task<List<MX>> GetDomainQueryTask(string Domain)
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

                var queryTask = QueryMxRecords(Domain);
                var DomainTask = new DomainQueryTask() { Task = queryTask, Domain = Domain };
                DomainTasks[Domain] = DomainTask;
                return queryTask;

            }

        }

        public static async Task<List<MX>> GetMx(string emailHost)
        {
            emailHost = emailHost.ToLower();

            if (MXCache.TryGetValue<List<MX>>(emailHost, out var value))
            {
                return value;
            }
            else
            {
                var result = await GetDomainQueryTask(emailHost);

                if (result != null && result.Any())
                {
                    MXCache.Set(emailHost, result, TimeSpan.FromHours(6));
                }
                else
                {
                    if (MXCache.TryGetValue<List<MX>>(emailHost, out var value2))
                    {
                        return value2;
                    }
                }
                return result;
            }

        }

        public static async Task<List<MX>> QueryMxRecords(string host)
        {
            List<MX> result = new();

            foreach (var item in DNSServerProvider.Servers)
            {
                try
                {
                    ClientRequest request = new ClientRequest(item);

                    request.Questions.Add(new Question(DNS.Protocol.Domain.FromString(host), RecordType.MX));
                    request.RecursionDesired = true;
                    request.OperationCode = OperationCode.Query;

                    CancellationTokenSource source = new CancellationTokenSource();
                    source.CancelAfter(10000);

                    IResponse response = await request.Resolve(source.Token);
                    var answers = response.AnswerRecords.Where(r => r.Type == RecordType.MX);
                    if (answers != null && answers.Any())
                    {
                        foreach (var answer in answers)
                        {
                            var mxAnswer = answer as DNS.Protocol.ResourceRecords.MailExchangeResourceRecord;
                            if (mxAnswer != null)
                            {
                                MX record = new MX();
                                record.Domain = mxAnswer.ExchangeDomainName.ToString();

                                if (record.Domain.EndsWith("."))
                                {
                                    record.Domain = record.Domain.TrimEnd('.');
                                }
                                record.Preference = mxAnswer.Preference;
                                result.Add(record);
                            }
                        }

                        return result;
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }
    }

    public class DomainQueryTask
    {
        public Task<List<MX>> Task { get; set; }

        public string Domain { get; set; }

        public DateTime LastModified { get; set; } = DateTime.Now;

    }
}
