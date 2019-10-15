//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Cache
{
    public static class OrganizationUserCache
    {
        private static object _locker = new object();

        public static Dictionary<Guid, OrgUser> CacheItems { get; set; }

        static OrganizationUserCache()
        {
            CacheItems = new Dictionary<Guid, OrgUser>();
        }

        private static OrgUser GetOrg(Guid orgId)
        {
            if (!CacheItems.ContainsKey(orgId))
            {
                lock (_locker)
                {
                    if (!CacheItems.ContainsKey(orgId))
                    {
                        OrgUser orguser = GetOrgUserIds(orgId);
                        CacheItems[orgId] = orguser;
                    }
                }
            }

            return CacheItems[orgId];
        }

        private static OrgUser GetOrgUserIds(Guid orgId)
        {
            OrgUser orguser = new OrgUser();
            var users = GetOrgUsers(orgId);
            if (users != null)
            {
                foreach (var item in users)
                {
                    orguser.Users.Add(item.Id);
                }
            }
            orguser.LastModified = DateTime.Now;
            return orguser;
        }

        public static void AddUser(Guid orgId, Guid userId)
        {
            var orguser = GetOrg(orgId);
            orguser.Users.Add(userId);
            orguser.LastModified = DateTime.Now;
        }

        public static void RemoveUser(Guid orgId, Guid userId)
        {
            var orguser = GetOrg(orgId);
            orguser.Users.Remove(userId);
            orguser.LastModified = DateTime.Now;
        }

        public static bool HasUser(Guid orgId, Guid userId)
        {
            var orgusr = GetOrg(orgId);

            if (orgusr != null && orgusr.Users.Contains(userId))
            {
                return true;
            }

            if (orgusr != null && orgusr.LastModified < DateTime.Now.AddHours(-1))
            {
                var neworguser = GetOrgUserIds(orgId);
                if (neworguser != null)
                {
                    CacheItems[orgId] = neworguser;

                    if (neworguser.Users.Contains(userId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<Kooboo.Data.Models.User> GetOrgUsers(Guid orgId)
        {
            return GlobalDb.Organization.Users(orgId);
        }
    }

    public class OrgUser
    {
        public OrgUser()
        {
            Users = new HashSet<Guid>();
        }

        public Guid OrgId { get; set; }

        public HashSet<Guid> Users { get; set; }

        public DateTime LastModified { get; set; }
    }
}