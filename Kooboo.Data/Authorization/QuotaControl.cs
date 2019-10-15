//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Authorization
{
    public static class QuotaControl
    {
        static QuotaControl()
        {
            Init();
        }

        public static void Init()
        {
            if (Data.AppSettings.IsOnlineServer)
            {
                string url = Kooboo.Data.Helper.AccountUrlHelper.System("Quota");
                var result = Lib.Helper.HttpHelper.Get<Dictionary<int, ServiceLevel>>(url);
                ServiceLevel = result ?? new Dictionary<int, ServiceLevel>();
            }
        }

        private static object _locker = new object();

        private static DateTime LastCheck { get; set; }

        public static Dictionary<int, ServiceLevel> ServiceLevel
        {
            get; set;
        }

        public static bool CanSendEmail(Organization org, int emailnumber = 1)
        {
            if (!Data.AppSettings.IsOnlineServer)
            {
                return true;
            }
            return GetOpenEmails(org) > emailnumber - 1;
        }

        public static bool CanSendEmail(Guid orgId, int emailnumber = 1)
        {
            if (!Data.AppSettings.IsOnlineServer || orgId == default)
            {
                return true;
            }

            var org = Kooboo.Data.GlobalDb.Organization.Get(orgId);
            if (org != null)
            {
                return CanSendEmail(org, emailnumber);
            }

            return false;
        }

        private static long GetOpenEmails(Organization org)
        {
            var sent = GetEmailSent(org.Id);

            if (ServiceLevel.ContainsKey(org.ServiceLevel))
            {
                var lev = ServiceLevel[org.ServiceLevel];

                return lev.Email.Count - sent;
            }
            return 9999;
        }

        public static int GetEmailSent(Guid orgid)
        {
            var current = LogStorage.EmailLog.Get<EmailCounter>(orgid);
            return current?.Count ?? 0;
        }

        public static void AddSendEmailCount(Guid orgId, int emailnumber = 1)
        {
            if (orgId != default)
            {
                var current = LogStorage.EmailLog.Get<EmailCounter>(orgId);
                if (current == null)
                {
                    current = new EmailCounter {OrgId = orgId, Count = emailnumber};
                    LogStorage.EmailLog.Add(current);
                }
                else
                {
                    current.Count += emailnumber;
                    LogStorage.EmailLog.Update(current);
                }
            }
        }

        public static int MaxPages(Guid orgId)
        {
            if (!Data.AppSettings.IsOnlineServer || orgId == default)
            {
                return int.MaxValue;
            }
            var org = Kooboo.Data.GlobalDb.Organization.Get(orgId);
            if (org != null)
            {
                if (ServiceLevel.ContainsKey(org.ServiceLevel))
                {
                    var lev = ServiceLevel[org.ServiceLevel];

                    return lev.MaxPages;
                }
            }
            return int.MaxValue;
        }

        public static int MaxSites(Guid orgId)
        {
            if (!Data.AppSettings.IsOnlineServer || orgId == default)
            {
                return int.MaxValue;
            }
            var org = Kooboo.Data.GlobalDb.Organization.Get(orgId);
            if (org != null)
            {
                if (ServiceLevel.ContainsKey(org.ServiceLevel))
                {
                    var lev = ServiceLevel[org.ServiceLevel];

                    return lev.MaxSites;
                }
            }
            return int.MaxValue;
        }

        public static bool CanHaveMoreSsl(Guid orgId)
        {
            var org = Kooboo.Data.GlobalDb.Organization.Get(orgId);
            if (org != null)
            {
                if (ServiceLevel.ContainsKey(org.ServiceLevel))
                {
                    var lev = ServiceLevel[org.ServiceLevel];
                    int sslcount = lev.MaxSsl;

                    var currents = Data.GlobalDb.SslCertificate.Query.Where(o => o.OrganizationId == orgId);

                    if (currents != null && currents.Count() > sslcount)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}