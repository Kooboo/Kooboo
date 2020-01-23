//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Kooboo.Data.Interface;

namespace Kooboo.Mail.Utility
{
    public static class AddressUtility
    {
        public static bool IsValidEmailAddress(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var seg = ParseSegment(input);
            if (seg.Address == null || seg.Host == null)
            {
                return false;
            }
            return IsValidEmailDomain(seg.Host) && IsValidEmailChar(seg.Address);

        }

        internal static bool IsValidEmailDomain(string domain)
        {
            var result = Kooboo.Data.Helper.DomainHelper.Parse(domain);
            return result != null && !string.IsNullOrEmpty(result.Domain);
        }

        internal static bool IsValidEmailChar(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var currentchar = input[i];
                ///  48 - 57   0x30 - 0x39   0 - 9 OK Allowed without restrictions.
                if (currentchar <= 57 && currentchar >= 48)
                {
                    continue;
                }
                //  97 - 122   0x61 - 0x7a   a - z OK Allowed without restrictions. 
                else if (currentchar <= 122 && currentchar >= 97)
                {
                    continue;
                }
                //65 - 90   0x41 - 0x5a   A - Z OK Allowed without restrictions.
                else if (currentchar >= 65 && currentchar <= 90)
                {
                    continue;
                }
                else if (currentchar == '.' || currentchar == '+' || currentchar == '_' || currentchar == '*' || currentchar == '-' || currentchar == '=' || currentchar == '&')
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public static string GetAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return null;
            }

            int start = address.IndexOf("<");
            int end = address.IndexOf(">");

            if (start > -1 && end > -1 && end > start + 1)
            {
                return address.Substring(start + 1, end - start - 1);
            }

            return address;
        }

        public static bool IsLocalEmailAddress(string input)
        {
            return GetLocalEmailAddress(input) != null;
        }


        public static bool IsOrganizationOk(string emailaddress)
        {
            if (string.IsNullOrEmpty(emailaddress))
            {
                return false;
            }

            var segs = ParseSegment(emailaddress);
            if (segs.Address == null || segs.Host == null)
            {
                return false;
            }

            var domain = Kooboo.Data.GlobalDb.Domains.Get(segs.Host);

            if (domain == null || domain.OrganizationId == default(Guid))
            {
                return false;
            }

            return true;
        }


        public static EmailAddress GetLocalEmailAddress(string Address)
        {
            if (string.IsNullOrEmpty(Address))
            {
                return null;
            }

            var segs = ParseSegment(Address);
            if (segs.Address == null || segs.Host == null)
            {
                return null;
            }

            var domain = Kooboo.Data.GlobalDb.Domains.Get(segs.Host);

            if (domain == null || domain.OrganizationId == default(Guid))
            {
                return null;
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(domain.OrganizationId);

            var add = orgdb.EmailAddress.Find(Address);
            if (add != null)
            {
                add.OrgId = orgdb.OrganizationId;
            }
            return add;
        }


        public static EmailAddress GetEmailAddress(string emailaddress)
        {
            string rightaddress = GetAddress(emailaddress);

            return GetLocalEmailAddress(rightaddress);

        }


        public static bool IsOnlineDomainLocal(Kooboo.Data.Models.Domain domain)
        {
            var org = Kooboo.Data.GlobalDb.Organization.Get(domain.OrganizationId);

            if (org != null)
            {
                var checker = Lib.IOC.Service.GetSingleTon<IMailServerProvider>(false);
                if (checker != null)
                {
                    return checker.IsLocal(org);
                }
            }

            return false;
        }


        public static bool WildcardMatch(string NormalAddress, string WildcardAddress)
        {
            var normal = ParseSegment(NormalAddress);
            return WildcardMatch(normal, WildcardAddress);
        }

        public static bool WildcardMatch(EmailSegment NormalAddress, string WildcardAddress)
        {
            var wildcardseg = ParseSegment(WildcardAddress);
            if (wildcardseg.Address == null)
            {
                return false;
            }

            if (!Lib.Helper.StringHelper.IsSameValue(NormalAddress.Host, wildcardseg.Host))
            {
                return false;
            }

            if (wildcardseg.Address == "*")
            {
                return true;
            }

            string wildcard = wildcardseg.Address;
            string normal = NormalAddress.Address;
            int index = wildcard.IndexOf("*");
            if (index == -1)
            {
                return Lib.Helper.StringHelper.IsSameValue(normal, wildcard);
            }
            else
            {
                var begin = wildcard.Substring(0, index);
                if (!string.IsNullOrEmpty(begin))
                {
                    begin = begin.ToLower();

                    if (normal.ToLower().StartsWith(begin))
                    {
                        normal = normal.ToLower().Substring(begin.Length);
                    }
                    else
                    {
                        return false;
                    }
                }

                var end = wildcard.Substring(index + 1);

                if (!string.IsNullOrEmpty(end))
                {
                    if (string.IsNullOrEmpty(normal))
                    {
                        return false;
                    }
                    end = end.ToLower();
                    return normal.ToLower().EndsWith(end);
                }

                return true;
            }
        }

        public static EmailSegment ParseSegment(string emailaddress)
        {
            if (emailaddress == null)
            {
                return default(EmailSegment);
            }
            int index = emailaddress.LastIndexOf("@");
            if (index == -1)
            {
                return default(EmailSegment);
            }
            return new EmailSegment() { Address = emailaddress.Substring(0, index), Host = emailaddress.Substring(index + 1) };
        }


        internal static HashSet<string> GetExternalRcpts(Smtp.SmtpSession session)
        {
            HashSet<string> tos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in session.Log)
            {
                if (item.Key.Name == Smtp.SmtpCommandName.RCPTTO && item.Value.Code == 250)
                {
                    if (!string.IsNullOrEmpty(item.Key.Value))
                    {
                        var address = GetAddress(item.Key.Value);

                        var islocal = IsLocalEmailAddress(address);

                        Kooboo.Data.Log.Instance.Email.Write("--islocal: " + islocal.ToString() + address); 

                        if (!islocal)
                        {
                            tos.Add(item.Key.Value);
                        }
                    }
                }
            }

            return tos;
        }
    }

    public struct EmailSegment
    {
        public string Address { get; set; }

        public string Host { get; set; }
    }
}
