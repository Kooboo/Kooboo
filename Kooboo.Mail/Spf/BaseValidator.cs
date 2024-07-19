using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DnsClient;
using Kooboo.Mail.Extension;

namespace Kooboo.Mail.Spf
{
    public abstract class BaseValidator<T>
        where T : BaseSpfRecord
    {
        private static readonly Regex _marcoRegex = new Regex(@"(%%|%_|%-|%\{(?<letter>[slodiphcrtv])(?<count>\d*)(?<reverse>r?)(?<delimiter>[\.\-+,/=]*)})", RegexOptions.Compiled);
        private static readonly ISpfDnsRequest _koobooDnsRequest = new SpfDnsRequest();

        /// <summary>
        /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-7"/>
        /// </summary>

        public DomainName HeloDomain { get; set; }

        public IPAddress LocalIP { get; set; }

        public DomainName LocalDomain { get; set; }

        public int DnsLookupLimit { get; set; } = 20;

        public async Task<SPFValidationResult> CheckHost(IPAddress ip, DomainName domainName, string sender, bool expandExplanation = false)
        {
            return await CheckHostInternalAsync(ip, domainName, sender, expandExplanation, new State(), default);
        }

        protected abstract Task<LoadRecordResult> LoadRecordsAsync(DomainName domainName, CancellationToken cancellationToken);

        private async Task<SPFValidationResult> CheckHostInternalAsync(IPAddress ip, DomainName domainName, string sender, bool expandExplanation, State state, CancellationToken cancellationToken)
        {
            if (domainName == null || domainName.IsRoot)
            {
                return new SPFValidationResult() { Result = ResultsOfEvaluation.None, Explanation = string.Empty };
            }

            // see href="https://datatracker.ietf.org/doc/html/rfc7208#section-7"
            if (string.IsNullOrEmpty(sender))
            {
                sender = "postmaster@unknown";
            }
            else if (!sender.Contains("@"))
            {
                sender = $"postmaster@" + sender;
            }

            var loadResult = await LoadRecordsAsync(domainName, cancellationToken);
            if (!loadResult.CouldBeLoaded)
            {
                return new SPFValidationResult() { Result = loadResult.ErrorResult, Explanation = string.Empty };
            }

            T record = loadResult.Record!;
            if (!record.Terms!.Any())
                return new SPFValidationResult() { Result = ResultsOfEvaluation.Neutral, Explanation = string.Empty };
            if (record.Terms!.OfType<Modifier>()
                .GroupBy(x => x.ModifierType)
                .Where(x => x.Key == ModifierDefinitions.Exp || x.Key == ModifierDefinitions.Redirect)
                .Any(x => x.Count() > 1))
            {
                return new SPFValidationResult { Result = ResultsOfEvaluation.Permerror, Explanation = string.Empty };
            }

            var result = new SPFValidationResult() { Result = loadResult.ErrorResult };

            // Evaluate Mechanism
            foreach (Mechanism mechanism in record.Terms!.OfType<Mechanism>())
            {
                if (state.DnsLookupCount > DnsLookupLimit)
                    return new SPFValidationResult() { Result = ResultsOfEvaluation.Permerror, Explanation = string.Empty };
                ResultsOfEvaluation resultsOfEvaluation = await CheckMechanismAsync(mechanism, ip, domainName, sender, state, cancellationToken);
                if (resultsOfEvaluation != ResultsOfEvaluation.None)
                {
                    result.Result = resultsOfEvaluation;
                    break;
                }
            }

            // Evaluate modifier
            if (result.Result == ResultsOfEvaluation.None)
            {
                var redirectModifier = record.Terms!.OfType<Modifier>().FirstOrDefault(x => x.ModifierType == ModifierDefinitions.Redirect);
                if (redirectModifier != null)
                {
                    if (++state.DnsLookupCount > 10)
                        return new SPFValidationResult() { Result = ResultsOfEvaluation.Permerror, Explanation = string.Empty };

                    var redirectDomain = ExpandDomainAsync(redirectModifier.Domain ?? string.Empty, ip, domainName, sender, cancellationToken);
                    if (redirectDomain != null || redirectDomain!.IsRoot || redirectDomain.Equals(domainName))
                    {
                        result.Result = ResultsOfEvaluation.Permerror;
                    }
                    else
                    {
                        result = await CheckHostInternalAsync(ip, redirectDomain, sender, expandExplanation, state, cancellationToken);
                        if (result.Result == ResultsOfEvaluation.None)
                            result.Result = ResultsOfEvaluation.Permerror;
                    }
                }
            }
            else if (result.Result == ResultsOfEvaluation.Fail && expandExplanation)
            {
                var expModifier = record?.Terms?.OfType<Modifier>().FirstOrDefault(x => x.ModifierType == ModifierDefinitions.Exp);
                if (expModifier != null)
                {
                    var target = ExpandDomainAsync(expModifier.Domain, ip, domainName, sender, cancellationToken);
                    if (target.IsRoot)
                    {
                        result.Explanation = string.Empty;
                    }
                    else
                    {
                        var txtResolveResult = await _koobooDnsRequest.GetTxtResolveResultAsync(target.ToString(), cancellationToken);
                        if (txtResolveResult != null && txtResolveResult.ReturnCode == ReturnCode.NoError)
                        {
                            var txtRecords = txtResolveResult.Records?.FirstOrDefault();
                            if (txtRecords!.Any())
                            {
                                result.Explanation = ExpandMacroAsync(txtRecords!.FirstOrDefault()!, ip, domainName, sender, cancellationToken).ToString();
                            }
                        }
                    }
                }
            }

            if (result.Result == ResultsOfEvaluation.None)
                result.Result = ResultsOfEvaluation.Neutral;
            return result;
        }

        private DomainName ExpandDomainAsync(string pattern, IPAddress ip, DomainName domainName, string sender, CancellationToken cancellationToken)
        {
            var expanded = ExpandMacroAsync(pattern, ip, domainName, sender, cancellationToken);
            return string.IsNullOrEmpty(expanded) ? DomainName.Root : DomainName.Parse(expanded);
        }

        private string ExpandMacroAsync(string pattern, IPAddress ip, DomainName domainName, string sender, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(pattern))
                return string.Empty;

            var marcoMatch = _marcoRegex.Match(pattern);
            if (!marcoMatch.Success)
                return pattern;
            var stringBuilder = new StringBuilder();
            int position = 0;
            do
            {
                if (marcoMatch.Index != position)
                {
                    stringBuilder.Append(pattern, position, marcoMatch.Index - position);
                }

                position = marcoMatch.Index + marcoMatch.Length;
                stringBuilder.Append(ExpandMacroAsync(marcoMatch, ip, domainName, sender, cancellationToken));
                marcoMatch = marcoMatch.NextMatch();
            }
            while (marcoMatch.Success);

            if (position < pattern.Length)
                stringBuilder.Append(pattern, position, pattern.Length - position);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// <see href="https://datatracker.ietf.org/doc/html/rfc7208#section-7"/>
        /// </summary>
        private string? ExpandMacroAsync(Match pattern, IPAddress ip, DomainName domain, string sender, CancellationToken cancellationToken)
        {
            switch (pattern.Value)
            {
                case "%%":
                    return "%";
                case "%_":
                    return "_";
                case "%-":
                    return "-";
                default:
                    string letter;
                    switch (pattern.Groups["letter"].Value)
                    {
                        case "s":
                            letter = sender;
                            break;
                        case "l":
                            // no boundary check needed, sender is validated on start of CheckHost
                            letter = sender.Split('@')[0];
                            break;
                        case "o":
                            // no boundary check needed, sender is validated on start of CheckHost
                            letter = sender.Split('@')[1];
                            break;
                        case "d":
                            letter = domain.ToString(false);
                            break;
                        case "i":
                            letter = string.Join(".", ip.GetAddressBytes().Select(b => b.ToString()));
                            break;

                        // case "p" not implement
                        case "v":
                            letter = ip.AddressFamily == AddressFamily.InterNetworkV6 ? "ip6" : "in-addr";
                            break;
                        case "h":
                            letter = HeloDomain?.ToString(false) ?? "unknown";
                            break;
                        case "c":
                            IPAddress address =
                                LocalIP
                                ?? NetworkInterface.GetAllNetworkInterfaces()
                                    .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                    .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                                    .Select(u => u.Address)
                                    .FirstOrDefault(a => a.AddressFamily == ip.AddressFamily)
                                ?? (ip.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Loopback : IPAddress.IPv6Loopback);
                            letter = address.ToString();
                            break;
                        case "r":
                            letter = LocalDomain?.ToString(false) ?? Dns.GetHostName();
                            break;
                        case "t":
                            letter = ((int)(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) - DateTime.Now).TotalSeconds).ToString();
                            break;
                        default:
                            return null;
                    }

                    // only letter
                    if (pattern.Value.Length == 4)
                        return letter;

                    char[] delimiters = pattern.Groups["delimiter"].Value.ToCharArray();
                    if (delimiters.Length == 0)
                        delimiters = new[] { '.' };

                    string[] parts = letter.Split(delimiters);

                    if (pattern.Groups["reverse"].Value == "r")
                        parts = parts.Reverse().ToArray();

                    int count = int.MaxValue;
                    if (!string.IsNullOrEmpty(pattern.Groups["count"].Value))
                    {
                        count = int.Parse(pattern.Groups["count"].Value);
                    }

                    if (count < 1)
                        return null;

                    count = Math.Min(count, parts.Length);

                    return string.Join(".", parts, parts.Length - count, count);
            }
        }

        private async Task<ResultsOfEvaluation> CheckMechanismAsync(Mechanism mechanism, IPAddress ip, DomainName domainName, string sender, State state, CancellationToken cancellationToken)
        {
            switch (mechanism.MechanismType)
            {
                case MechanismDefinitions.All:
                    return mechanism.EvaluationType;
                case MechanismDefinitions.A:
                    if (++state.DnsLookupCount > 10)
                        return ResultsOfEvaluation.Permerror;
                    var mechanismDomain = string.IsNullOrEmpty(mechanism.Domain) ? domainName : ExpandDomainAsync(mechanism.Domain, ip, domainName, sender, cancellationToken);
                    bool? isAMatch = await IsIpMatchAsync(mechanismDomain, ip, mechanism.Prefix, mechanism.Prefix6, cancellationToken);
                    if (!isAMatch.HasValue)
                        return ResultsOfEvaluation.Temperror;

                    if (isAMatch.Value)
                    {
                        return mechanism.EvaluationType;
                    }

                    break;
                case MechanismDefinitions.Mx:
                    if (++state.DnsLookupCount > 10)
                        return ResultsOfEvaluation.Permerror;

                    var mxMechanismDomain = string.IsNullOrEmpty(mechanism.Domain) ? domainName : ExpandDomainAsync(mechanism.Domain, ip, domainName, sender, cancellationToken);
                    var dnsMxResult = await _koobooDnsRequest.GetMxResolveResultAsync(mxMechanismDomain, cancellationToken);
                    if (dnsMxResult == null || dnsMxResult.ReturnCode != ReturnCode.NoError && dnsMxResult.ReturnCode != ReturnCode.NxDomain)
                        return ResultsOfEvaluation.Temperror;
                    int mxCheckedCount = 0;
                    foreach (var mxRecord in dnsMxResult.Records!)
                    {
                        if (++mxCheckedCount == 10)
                            break;
                        if (DomainName.TryParse(mxRecord.Exchange!, out var exchangeDomainName))
                        {
                            bool? isMxMatch = await IsIpMatchAsync(exchangeDomainName!, ip, mechanism.Prefix, mechanism.Prefix6, cancellationToken);
                            if (!isMxMatch.HasValue)
                                return ResultsOfEvaluation.Temperror;

                            if (isMxMatch.Value)
                            {
                                return mechanism.EvaluationType;
                            }
                        }
                    }

                    break;
                case MechanismDefinitions.Ip4:
                case MechanismDefinitions.Ip6:
                    IPAddress? compareAddress;
                    if (IPAddress.TryParse(mechanism.Domain, out compareAddress))
                    {
                        if (ip.AddressFamily != compareAddress.AddressFamily)
                            return ResultsOfEvaluation.None;

                        if (mechanism.Prefix.HasValue)
                        {
                            if (mechanism.Prefix.Value < 0 || mechanism.Prefix.Value > (compareAddress.AddressFamily == AddressFamily.InterNetworkV6 ? 128 : 32))
                                return ResultsOfEvaluation.Permerror;

                            if (ip.GetNetworkAddress(mechanism.Prefix.Value).Equals(compareAddress.GetNetworkAddress(mechanism.Prefix.Value)))
                            {
                                return mechanism.EvaluationType;
                            }
                        }
                        else if (ip.Equals(compareAddress))
                        {
                            return mechanism.EvaluationType;
                        }
                    }
                    else
                    {
                        return ResultsOfEvaluation.Permerror;
                    }

                    break;
                case MechanismDefinitions.Exists:
                    if (++state.DnsLookupCount > 10)
                        return ResultsOfEvaluation.Permerror;

                    if (string.IsNullOrEmpty(mechanism.Domain))
                        return ResultsOfEvaluation.Permerror;

                    DomainName existsMechanismDomain = string.IsNullOrEmpty(mechanism.Domain) ? domainName : ExpandDomainAsync(mechanism.Domain, ip, domainName, sender, cancellationToken);

                    var dnsAResult = await _koobooDnsRequest.GetAResolveResultAsync(existsMechanismDomain, cancellationToken);
                    if (dnsAResult == null || dnsAResult.ReturnCode != ReturnCode.NoError && dnsAResult.ReturnCode != ReturnCode.NxDomain)
                        return ResultsOfEvaluation.Temperror;

                    if (dnsAResult.Records?.Count > 0)
                    {
                        return mechanism.EvaluationType;
                    }

                    break;
                case MechanismDefinitions.Include:
                    if (++state.DnsLookupCount > 10)
                        return ResultsOfEvaluation.Permerror;

                    if (string.IsNullOrEmpty(mechanism.Domain))
                        return ResultsOfEvaluation.Permerror;

                    DomainName includeMechanismDomain = string.IsNullOrEmpty(mechanism.Domain) ? domainName : ExpandDomainAsync(mechanism.Domain, ip, domainName, sender, cancellationToken);

                    if (includeMechanismDomain.Equals(domainName))
                        return ResultsOfEvaluation.Permerror;

                    var includeResult = await CheckHostInternalAsync(ip, includeMechanismDomain, sender, false, state, cancellationToken);
                    switch (includeResult.Result)
                    {
                        case ResultsOfEvaluation.Pass:
                            return mechanism.EvaluationType;
                        case ResultsOfEvaluation.Fail:
                        case ResultsOfEvaluation.Softfail:
                        case ResultsOfEvaluation.Neutral:
                            return ResultsOfEvaluation.None;
                        case ResultsOfEvaluation.Temperror:
                            return ResultsOfEvaluation.Temperror;
                        case ResultsOfEvaluation.Permerror:
                        case ResultsOfEvaluation.None:
                            return ResultsOfEvaluation.Permerror;
                    }

                    break;

                default:
                    return ResultsOfEvaluation.Permerror;
            }

            return ResultsOfEvaluation.None;
        }

        private Task<bool?> IsIpMatchAsync(DomainName domainName, IPAddress ipAddress, int? prefix4, int? prefix6, CancellationToken cancellationToken)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (prefix6.HasValue)
                    ipAddress = ipAddress.GetNetworkAddress(prefix6.Value);

                return IsIpMatchAsync(domainName, ipAddress, prefix6, QueryType.AAAA, cancellationToken);
            }
            else
            {
                if (prefix4.HasValue)
                    ipAddress = ipAddress.GetNetworkAddress(prefix4.Value);

                return IsIpMatchAsync(domainName, ipAddress, prefix4, QueryType.A, cancellationToken);
            }
        }

        private async Task<bool?> IsIpMatchAsync(DomainName domain, IPAddress ipAddress, int? prefix, QueryType queryType, CancellationToken cancellationToken)
        {
            DnsResolveResult<List<IPAddress>> dnsResolveResult;
            switch (queryType)
            {
                case QueryType.AAAA:
                    dnsResolveResult = await _koobooDnsRequest.GetAAAAResolveResultAsync(domain, cancellationToken);
                    break;
                case QueryType.A:
                    dnsResolveResult = await _koobooDnsRequest.GetAResolveResultAsync(domain, cancellationToken);
                    break;
                default:
                    dnsResolveResult = await _koobooDnsRequest.GetAResolveResultAsync(domain, cancellationToken);
                    break;
            }

            if (dnsResolveResult == null || dnsResolveResult.ReturnCode != ReturnCode.NoError && dnsResolveResult.ReturnCode != ReturnCode.NxDomain)
                return null;

            foreach (var record in dnsResolveResult.Records!)
            {
                if (prefix.HasValue)
                {
                    if (ipAddress.Equals(record.GetNetworkAddress(prefix.Value)))
                        return true;
                }
                else
                {
                    if (ipAddress.Equals(record))
                        return true;
                }
            }

            return false;
        }

        protected class LoadRecordResult
        {
            public bool CouldBeLoaded { get; internal set; }

            public T Record { get; internal set; }

            public ResultsOfEvaluation ErrorResult { get; internal set; }
        }

        protected class State
        {
            public int DnsLookupCount { get; set; }
        }
    }
}

