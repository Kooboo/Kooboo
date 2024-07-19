//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Kooboo.Data.Models;
using Kooboo.Mail.Transport;

namespace Kooboo.Mail.Factory
{
    public static class DBFactory
    {

        private static ConcurrentDictionary<Guid, MailDb> _maildbprivate;

        private static ConcurrentDictionary<Guid, MailDb> _maildbs
        {
            get
            {
                if (_maildbprivate == null)
                {
                    _maildbprivate = new ConcurrentDictionary<Guid, MailDb>();
                }
                return _maildbprivate;
            }
        }
        private static Dictionary<Guid, OrgDb> _orgdbs { get; set; } = new Dictionary<Guid, OrgDb>();

        private static object CreateLock = new object();

        public static IEnumerable<MailDb> OpenMailDbs()
        {
            return _maildbs.Values;
        }

        public static MailDb UserMailDb(Guid userId, Guid OrganizationId)
        {
            string key = userId.ToString() + OrganizationId.ToString();
            Guid guidkey = Lib.Security.Hash.ComputeGuidIgnoreCase(key);

            MailDb result;
            if (_maildbs.TryGetValue(guidkey, out result))
            {
                return result;
            }

            var locker = GetMailDbLocker(guidkey);

            lock (locker)
            {
                if (_maildbs.TryGetValue(guidkey, out result))
                {
                    return result;
                }

                result = new MailDb(userId, OrganizationId);

                _maildbs.TryAdd(guidkey, result);
                // _maildbs[guidkey] = result;
                return result;
            }
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
            {
                return result;
            }

            var orgdbLocker = GetOrgDbLocker(organizationId);

            lock (orgdbLocker)
            {
                if (_orgdbs.TryGetValue(organizationId, out result))
                {
                    return result;
                }

                result = new OrgDb(organizationId);

                _orgdbs[organizationId] = result;
                return result;
            }

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
            var domain = MailDomainCheck.Instance.GetByEmailAddress(emailAddress);

            if (domain != null && domain.OrganizationId != default(Guid))
            {

                return OrgDb(domain.OrganizationId);
            }
            else
            {
                return null;
            }
        }


        private static object _lockMailLocker = new object();
        private static object _lockOrgLocker = new object();


        private static Dictionary<Guid, object> mailDbLocker = new Dictionary<Guid, object>();
        private static Dictionary<Guid, object> orgLocker = new Dictionary<Guid, object>();

        public static object GetMailDbLocker(Guid Id)
        {
            if (mailDbLocker.ContainsKey(Id))
            {
                return mailDbLocker[Id];
            }

            lock (_lockMailLocker)
            {
                if (mailDbLocker.ContainsKey(Id))
                {
                    return mailDbLocker[Id];
                }
                else
                {
                    object newLock = new object();
                    mailDbLocker[Id] = newLock;
                    return newLock;
                }
            }
        }

        public static object GetOrgDbLocker(Guid Id)
        {
            if (orgLocker.ContainsKey(Id))
            {
                return orgLocker[Id];
            }

            lock (_lockOrgLocker)
            {
                if (orgLocker.ContainsKey(Id))
                {
                    return orgLocker[Id];
                }
                else
                {
                    object newLock = new object();
                    orgLocker[Id] = newLock;
                    return newLock;
                }
            }
        }

    }
}
