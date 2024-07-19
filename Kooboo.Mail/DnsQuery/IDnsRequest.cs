using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DnsClient.Protocol;

namespace Kooboo.Mail.DnsQuery
{
    public interface IDnsRequest
    {
        Task<List<MxRecord>> GetMxRecordsAsync(string domain, CancellationToken cancellationToken);

        Task<List<string>> GetMailExchangesAsync(string domain, CancellationToken cancellationToken);

        Task<List<string>> GetNameServersAsync(string domain, CancellationToken cancellationToken);

        Task<IPHostEntry> GetReverseDnsAsync(string ip, CancellationToken cancellationToken);

        Task<List<IPAddress>> GetARecordAsync(string domain, CancellationToken cancellationToken);

        Task<List<IPAddress>> GetAAAARecordAsync(string domain, CancellationToken cancellationToken);

        List<string> GetTxtRecords(string domain);

        Task<List<ICollection<string>>> GetTxtRecordAsync(string domain, CancellationToken cancellationToken);

        Task<List<string>> GetPtrRecordAsync(string domain, CancellationToken cancellationToken);

        Task<DnsResolveResult<List<ICollection<string>>>> GetTxtResolveResultAsync(string domain, CancellationToken cancellationToken);

        Task<DnsResolveResult<List<string>>> GetPtrResolveResultAsync(string domain, CancellationToken cancellationToken);

        Task<DnsResolveResult<List<IPAddress>>> GetAResolveResultAsync(DomainName domain, CancellationToken cancellationToken);

        Task<DnsResolveResult<List<MxRecord>>> GetMxResolveResultAsync(DomainName domain, CancellationToken cancellationToken);

        Task<DnsResolveResult<List<IPAddress>>> GetAAAAResolveResultAsync(DomainName domain, CancellationToken cancellationToken);
    }

}

