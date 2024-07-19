using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Lib.Domain
{
    public static class DomainService
    {
        public static bool IsValidDomain(string domain)
        {
            if (!IsValidDomainChar(domain))
            {
                return false;
            }
            var suffix = StringDomainTree.Instance.GetMatchSuffic(domain);

            return suffix != null && domain.Length > suffix.Length + 1;
        }

        public static DomainResult Parse(string domain)
        {
            if (!IsValidDomainChar(domain))
            {
                return null;
            }

            domain = domain.ToLower();

            var Suffix = StringDomainTree.Instance.GetMatchSuffic(domain);

            if (Suffix != null)
            {
                DomainResult result = new DomainResult();
                result.FullName = domain;
                result.Tld = Suffix;  // domain.Substring(index);

                var lastIndex = domain.Length - Suffix.Length;

                if (domain[lastIndex - 1] != '.')
                {
                    // not match.
                    return null;
                }

                // index -1 must be the .   
                var lastdot = domain.LastIndexOf('.', lastIndex - 2);

                result.Root = domain.Substring(lastdot + 1);

                if (lastdot > 0)
                {
                    var sub = domain.Substring(0, lastdot);

                    sub = sub.Trim('.');

                    result.FullSubDomain = sub;

                    int subDot = sub.LastIndexOf(".");

                    if (subDot > 0)
                    {
                        result.SubDomainRoot = sub.Substring(subDot + 1);
                    }
                    else
                    {
                        result.SubDomainRoot = sub;
                    }
                }

                return result;


            }
            return null;

        }


        public static bool IsValidDomainChar(string domain)
        {
            for (int i = 0; i < domain.Length; i++)
            {
                var current = domain[i];

                if (Lib.Helper.CharHelper.isAlphanumeric(current))
                {
                    continue;
                }
                else if (current == '.' || current == '_' || current == '-')
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public static string GetRootDomain(string domain)
        {
            var result = Parse(domain);
            if (result != null)
            {
                return result.Root;
            }
            return null;
        }


        public static bool IsDomainUsingKoobooDns(string domain)
        {
            var rootDomain = GetRootDomain(domain);
            if (string.IsNullOrEmpty(rootDomain))
            {
                return false;
            }

            var result = Kooboo.Lib.Whois.Service.Lookup(rootDomain);

            if (result != null && result.NameServers != null && result.NameServers.Any())
            {
                return IsUseKoobooDns(result.NameServers);
            }

            return false;
        }

        private static bool IsUseKoobooDns(List<string> DNSServers)
        {
            if (DNSServers == null || !DNSServers.Any())
            {
                return false;
            }

            foreach (var item in DNSServers)
            {
                if (IsUseKoobooDns(item))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsUseKoobooDns(string NSServer)
        {
            if (NSServer == null)
            {
                return false;
            }

            if (NSServer.Contains(",") || NSServer.Contains(";"))
            {
                var parts = NSServer.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in parts)
                {
                    if (_IsOneServerUseKoobooDns(item))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return _IsOneServerUseKoobooDns(NSServer);
            }
            return false;
        }

        private static bool _IsOneServerUseKoobooDns(string oneNSServer)
        {
            if (oneNSServer == null)
            {
                return false;
            }
            oneNSServer = oneNSServer.ToLower();

            var nameResult = Parse(oneNSServer);

            if (nameResult != null && nameResult.Root != null)
            {
                var root = nameResult.Root.ToLower();

                return (root.Contains("dnscall.org") || root.Contains("kooboo"));

            }
            return false;
        }

        public static string QueryCName(string FullDomain)
        {
            var Answers = Kooboo.Lib.DnsRequest.DnsQuery.QueryRecords(FullDomain, DNS.Protocol.RecordType.CNAME).Result;

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
    }
}
