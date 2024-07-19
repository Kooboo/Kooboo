//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Mail.Transport;
using MimeKit;

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
            if (address == null)
            {
                return null;
            }

            var add = MailKitUtility.ParseAddress(address);

            if (add != null)
            {
                return add.GetAddress(false);
            }

            if (address.Contains("<") && address.Contains(">"))
            {
                var index = address.IndexOf('<');
                if (index > -1)
                {
                    var end = address.IndexOf('>', index + 1);

                    var InnerPart = address.Substring(index + 1, end - index - 1);

                    add = MailKitUtility.ParseAddress(InnerPart);
                    if (add != null)
                    {
                        return add.GetAddress(false);
                    }
                }
            }

            return null;

        }

        public static bool IsLocalEmailAddress(string input)
        {
            return GetLocalEmailAddress(input) != null;
        }

        public static string GetEmailDomain(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return null;

            emailAddress = GetAddress(emailAddress);

            int index = emailAddress.LastIndexOf("@");

            return index > -1 ? emailAddress.Substring(index + 1) : null;
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

            var domain = MailDomainCheck.Instance.GetByEmailAddress(Address);

            if (domain == null)
            {
                return null;
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(domain.OrganizationId);

            var add = orgdb.Email.Find(Address);
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


        public static bool WildcardMatch(string NormalAddress, string WildcardAddress)
        {
            if (WildcardAddress == null)
            {
                return false;
            }
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

        public static string GetDisplayName(string FullAddress)
        {
            if (string.IsNullOrEmpty(FullAddress))
            {
                return null;
            }

            if (FullAddress.Contains(";"))
            {
                FullAddress = FullAddress.Replace(";", ",");
                return GetDisplayNameList(FullAddress);
            }

            if (MimeKit.MailboxAddress.TryParse(FullAddress, out var mailbox))
            {
                var address = mailbox.GetAddress(false);

                if (string.IsNullOrWhiteSpace(address))
                {
                    return null;
                }

                string name = mailbox.Name;

                if (!string.IsNullOrEmpty(name))
                {
                    return name.Trim('\'').Trim('\"');
                }

                var segments = Kooboo.Mail.Utility.AddressUtility.ParseSegment(address);

                return segments.Address;
            }
            return null;
        }

        public static string GetDisplayNameList(string addressLine)
        {
            var addlist = MailKitUtility.GetMailKitAddressList(addressLine);
            if (addlist != null && addlist.Count > 0)
            {
                List<string> names = new List<string>();

                foreach (var item in addlist)
                {

                    var address = item.GetAddress(false);

                    if (string.IsNullOrWhiteSpace(address))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        names.Add(item.Name.Trim('\'').Trim('\"'));
                    }
                    else
                    {
                        var segments = Kooboo.Mail.Utility.AddressUtility.ParseSegment(address);

                        names.Add(segments.Address);
                    }
                }

                return String.Join(";", names);
            }

            return null;

        }


        public static string GetDisplayName(MailboxAddress add)
        {
            if (add == null)
            {
                return null;
            }

            string name = add.Name;

            if (string.IsNullOrEmpty(name))
            {
                var segments = Kooboo.Mail.Utility.AddressUtility.ParseSegment(add.Address);
                return segments.Address;
            }
            else
            {
                name = name.Trim(' ').Trim('\'').Trim('\"');
                name = name.Replace("\\", "");
                name = name.Trim('\"');
                name = name.Trim('\'');
                return name;
            }

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
