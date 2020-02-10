//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Factory
{
    public static class DBFactory
    {
        private static Dictionary<Guid, MailDb> _maildbs = new Dictionary<Guid, MailDb>();
        private static Dictionary<Guid, OrgDb> _orgdbs = new Dictionary<Guid, OrgDb>();

        private static object _dbCreateLock = new object();

        public static MailDb UserMailDb(Guid userId, Guid OrganizationId)
        {
            string key = userId.ToString() + OrganizationId.ToString();
            Guid guidkey = Lib.Security.Hash.ComputeGuidIgnoreCase(key);

            MailDb result;
            if (_maildbs.TryGetValue(guidkey, out result))

                return result;

            lock (_dbCreateLock)
            {
                if (_maildbs.TryGetValue(guidkey, out result))
                    return result;

                result = new MailDb(userId, OrganizationId);
                _maildbs[guidkey] = result;
            }
            return result;
        }

        public static MailDb UserMailDb(User user)
        {
            return UserMailDb(user.Id, user.CurrentOrgId);
        }

        /// <summary>
        /// the name of database,normally it is the site name. 
        /// If database not exists, it will be created. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static OrgDb OrgDb(Guid organizationId)
        {
            OrgDb result;
            if (_orgdbs.TryGetValue(organizationId, out result))
                return result;

            lock (_dbCreateLock)
            {
                if (_orgdbs.TryGetValue(organizationId, out result))
                    return result;
                result = new OrgDb(organizationId);
                _orgdbs[organizationId] = result;
            }
            return result;
        }

        // only when prepare for moving. 
        public static void SetNull(Guid OrganizationId)
        {
            _orgdbs[OrganizationId] = null;
        }

        public static void RemoveNull(Guid OrganizationId)
        {
            _orgdbs.Remove(OrganizationId);
        }

        // this is for email. 
        public static OrgDb OrgDb(string emailAddress)
        {
            string domainName = GetDomain(emailAddress);

            var domain = Kooboo.Data.GlobalDb.Domains.Get(domainName);

            if (domain != null && domain.OrganizationId != default(Guid))
            {
                if (domain.IsKooboo && Data.AppSettings.IsOnlineServer)
                {
                    // for Kooboo subdomain, check if it is in 
                    var org = Kooboo.Data.GlobalDb.Organization.Get(domain.OrganizationId);

                    if (org != null)
                    {
                        var checker = Lib.IOC.Service.GetSingleTon<IMailServerProvider>(false);
                        if (checker != null)
                        {
                            var islocal = checker.IsLocal(org);
                            if (islocal)
                            {
                                return OrgDb(domain.OrganizationId);
                            }
                            else
                            {
                                return null;
                            }
                        }

                    }
                    else
                    {
                        return null;
                    }
                }

                return OrgDb(domain.OrganizationId);
            }
            else
            {
                return null;
            }
        }

        private static string GetDomain(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return null;

            int index = emailAddress.LastIndexOf("@");

            return index > 0 ? emailAddress.Substring(index + 1) : null;
        }

    }
}
