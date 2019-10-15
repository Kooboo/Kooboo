//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Repository
{
    public class OrganizationRepository
    {
        public Dictionary<Guid, string> NameCache = new Dictionary<Guid, string>();

        private Dictionary<Guid, Organization> _cache = new Dictionary<Guid, Organization>();

        public Dictionary<Guid, DateTime> Lastfail = new Dictionary<Guid, DateTime>();

        // if failed for 5 times, not get any more.

        public Organization Get(Guid id)
        {
            var org = GetFromLocal(id);

            if (org == null)
            {
                org = GetFromAccount(id);
                if (org != null)
                {
                    AddOrUpdateLocal(org);
                }
            }
            return org;
        }

        public Organization GetFromLocal(Guid id)
        {
            if (this._cache.ContainsKey(id))
            {
                return this._cache[id];
            }

            var org = GlobalDb.LocalOrganization.Get(id);

            if (org != null)
            {
                _cache[id] = org;
                return org;
            }
            return null;
        }

        public Organization GetFromAccount(Guid id)
        {
            if (Lastfail.ContainsKey(id))
            {
                var lasttime = Lastfail[id];
                if (lasttime > DateTime.Now.AddHours(-4))
                {
                    return null;
                }
            }

            Dictionary<string, string> para = new Dictionary<string, string> {{"id", id.ToString()}};
            var org = HttpHelper.Post<Organization>(Account.Url.Org.GetUrl, para) ?? HttpHelper.Post<Organization>(Account.Url.Org.GetUrl, para);

            if (org != null)
            {
                return org;
            }

            Lastfail[id] = DateTime.Now;
            return null;
        }

        public Organization GetByUser(Guid userId)
        {
            Dictionary<string, string> para = new Dictionary<string, string> {{"userid", userId.ToString()}};
            return HttpHelper.Post<Organization>(Kooboo.Data.Helper.AccountUrlHelper.Org("GetByUser"), para);
        }

        public List<User> Users(Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                {"organizationId", organizationId.ToString()}
            };
            return HttpHelper.Post<List<User>>(Account.Url.Org.Users, para);
        }

        public string AddUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                {"organizationId", organizationId.ToString()}, {"userName", userName}
            };
            var result = HttpHelper.Post<string>(Account.Url.Org.AddUser, para);

            var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);
            Kooboo.Data.Cache.OrganizationUserCache.AddUser(organizationId, userid);

            return result;
        }

        public bool DeleteUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>
            {
                {"organizationId", organizationId.ToString()}, {"userName", userName}
            };
            var ok = HttpHelper.Post<bool>(Account.Url.Org.DeleteUser, para);

            if (ok)
            {
                var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);
                Kooboo.Data.Cache.OrganizationUserCache.RemoveUser(organizationId, userid);
            }
            return ok;
        }

        public string GetName(Guid orgId)
        {
            if (orgId == default(Guid))
            {
                return System.Guid.Empty.ToString();
            }

            var org = GetFromLocal(orgId);

            if (org != null)
            {
                return org.Name;
            }

            if (!NameCache.ContainsKey(orgId))
            {
                var user = GlobalDb.Users.GetLocalUserByOrgId(orgId);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(user.CurrentOrgName) && Lib.Security.Hash.ComputeGuidIgnoreCase(user.CurrentOrgName) == orgId)
                    {
                        NameCache[user.CurrentOrgId] = user.CurrentOrgName;
                        return user.CurrentOrgName;
                    }
                }
                var getorg = Get(orgId);
                if (getorg != null && !string.IsNullOrWhiteSpace(getorg.Name))
                {
                    NameCache[getorg.Id] = getorg.Name;
                    return getorg.Name;
                }
            }
            else
            {
                return NameCache[orgId];
            }

            return null;
        }

        public void RemoveOrgCache(Guid orgId)
        {
            GlobalDb.LocalOrganization.Delete(orgId);
            _cache.Remove(orgId);
        }

        public void AddOrUpdateLocal(Organization organization)
        {
            GlobalDb.LocalOrganization.AddOrUpdate(organization);
            _cache[organization.Id] = organization;
        }
    }
}