using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail.DnsQuery
{
    public class DnsRequest : IDnsRequest
    {
        private const int CacheInHours = 6;
        private static readonly ILookupClient _lookupClient = new LookupClient();
        private static readonly MemoryCache _cacheProvider = new MemoryCache(new MemoryCacheOptions());

        public async Task<List<IPAddress>> GetAAAARecordAsync(string domain, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheEntry.AAAARecordPrefix}{domain}";
            return await _cacheProvider.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = await _lookupClient.QueryAsync(domain, QueryType.AAAA, cancellationToken: cancellationToken).ConfigureAwait(false);
                var aRecords = response.Answers.AaaaRecords().ToList();
                if (aRecords == null || !aRecords.Any()) return new List<IPAddress>();
                var aValues = aRecords.Select(it => it.Address).ToList();
                return aValues;
            });
        }

        public async Task<DnsResolveResult<List<IPAddress>>> GetAAAAResolveResultAsync(DomainName domain, CancellationToken cancellationToken)
        {
            try
            {
                var aRecords = await GetAAAARecordAsync(domain.ToString(), cancellationToken);
                return new DnsResolveResult<List<IPAddress>>(ReturnCode.NoError, aRecords!);
            }
            catch (Exception)
            {
                return new DnsResolveResult<List<IPAddress>>(ReturnCode.ServerFailure, default!);
            };
        }

        public async Task<List<IPAddress>> GetARecordAsync(string domain, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheEntry.ARecordPrefix}{domain}";
            return await _cacheProvider.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = await _lookupClient.QueryAsync(domain, QueryType.A, cancellationToken: cancellationToken).ConfigureAwait(false);
                var aRecords = response.Answers.ARecords().ToList();
                if (aRecords == null || !aRecords.Any()) return new List<IPAddress>();
                var aValues = aRecords.Select(it => it.Address).ToList();
                return aValues;
            });
        }

        public async Task<DnsResolveResult<List<IPAddress>>> GetAResolveResultAsync(DomainName domain, CancellationToken cancellationToken)
        {
            try
            {
                var aRecords = await GetARecordAsync(domain.ToString(), cancellationToken);
                return new DnsResolveResult<List<IPAddress>>(ReturnCode.NoError, aRecords!);
            }
            catch (Exception)
            {
                return new DnsResolveResult<List<IPAddress>>(ReturnCode.ServerFailure, default!);
            };
        }

        public async Task<List<string>> GetMailExchangesAsync(string domain, CancellationToken cancellationToken)
        {
            var mxRecords = await GetMxRecordsAsync(domain, cancellationToken).ConfigureAwait(false);
            var exchanges = new List<string>();
            if (mxRecords == null || !mxRecords.Any()) return exchanges;
            return mxRecords
                .OrderBy(o => o.Preference)
                .Select(o => o.Exchange.Value)
                .ToList();
        }

        public async Task<List<MxRecord>> GetMxRecordsAsync(string domain, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheEntry.MXRecordPrefix}{domain}";
            return await _cacheProvider.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = await _lookupClient.QueryAsync(domain, QueryType.MX, cancellationToken: cancellationToken).ConfigureAwait(false);
                var mxRecords = response?.Answers.MxRecords().ToList();
                if (mxRecords == null || !mxRecords.Any()) return new List<MxRecord>();
                return mxRecords;
            });
        }

        public async Task<DnsResolveResult<List<MxRecord>>> GetMxResolveResultAsync(DomainName domain, CancellationToken cancellationToken)
        {
            try
            {
                var mxRecords = await GetMxRecordsAsync(domain.ToString(), cancellationToken);
                return new DnsResolveResult<List<MxRecord>>(ReturnCode.NoError, mxRecords);
            }
            catch (Exception)
            {
                return new DnsResolveResult<List<MxRecord>>(ReturnCode.ServerFailure, default);
            }
        }

        public async Task<List<string>> GetNameServersAsync(string domain, CancellationToken cancellationToken)
        {
            var response = await _lookupClient.QueryAsync(domain, QueryType.NS, cancellationToken: cancellationToken).ConfigureAwait(false);
            var nsRecords = response?.Answers.NsRecords().ToList();
            if (nsRecords == null || !nsRecords.Any()) return new List<string>();
            var nameServers = nsRecords.Select(it => it.NSDName.Value).ToList();
            return nameServers;
        }

        public async Task<List<string>> GetPtrRecordAsync(string domain, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheEntry.PTRRecordPrefix}{domain}";
            return await _cacheProvider.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = await _lookupClient.QueryAsync(domain, QueryType.PTR, cancellationToken: cancellationToken).ConfigureAwait(false);
                var ptrRecords = response.Answers.PtrRecords().ToList();
                if (ptrRecords == null || !ptrRecords.Any()) return new List<string>();
                var ptrValues = ptrRecords.Select(it => it.PtrDomainName.ToString()).ToList();
                return ptrValues;
            });
        }

        public async Task<DnsResolveResult<List<string>>> GetPtrResolveResultAsync(string domain, CancellationToken cancellationToken)
        {
            try
            {
                var ptrRecords = await GetPtrRecordAsync(domain, cancellationToken);
                return new DnsResolveResult<List<string>>(ReturnCode.NoError, ptrRecords!);
            }
            catch (Exception)
            {
                return new DnsResolveResult<List<string>>(ReturnCode.ServerFailure, default!);
            }
        }

        public async Task<IPHostEntry> GetReverseDnsAsync(string ip, CancellationToken cancellationToken)
        {
            var result = await System.Net.Dns.GetHostEntryAsync(ip, cancellationToken);
            return result;
        }


        public List<string> GetTxtRecords(string domain)
        {
            var cacheKey = $"{CacheEntry.TXTRecordPrefix}{domain}";
            return _cacheProvider.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = _lookupClient.Query(domain, QueryType.TXT);
                var txtRecords = response.Answers.AsQueryable();
                if (txtRecords == null || !txtRecords.Any()) return new List<string>();
                var txtValues = new List<string>();
                foreach (var item in txtRecords)
                {
                    if (item is TxtRecord)
                    {
                        var txt = item as TxtRecord;
                        txtValues.AddRange(txt.Text);
                    }
                }
                return txtValues;
            });

        }

        public async Task<List<ICollection<string>>> GetTxtRecordAsync(string domain, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheEntry.TXTRecordPrefix}{domain}";
            return await _cacheProvider.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(CacheInHours);
                var response = await _lookupClient.QueryAsync(domain, QueryType.TXT, cancellationToken: cancellationToken).ConfigureAwait(false);
                var txtRecords = response.Answers.TxtRecords().ToList();
                if (txtRecords == null || !txtRecords.Any()) return new List<ICollection<string>>();
                var txtValues = txtRecords.Select(it => it.Text).ToList();
                return txtValues;
            });
        }

        public async Task<DnsResolveResult<List<ICollection<string>>>> GetTxtResolveResultAsync(string domain, CancellationToken cancellationToken)
        {
            try
            {
                var txtRecords = await GetTxtRecordAsync(domain, cancellationToken);
                return new DnsResolveResult<List<ICollection<string>>>(ReturnCode.NoError, txtRecords!);
            }
            catch (Exception)
            {
                return new DnsResolveResult<List<ICollection<string>>>(ReturnCode.ServerFailure, default!);
            }
        }
    }
}

