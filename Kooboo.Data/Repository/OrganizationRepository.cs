//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;
using Kooboo.Data.Helper;

namespace Kooboo.Data.Repository
{
    public class OrganizationRepository
    {
        public Dictionary<Guid, string> NameCache = new Dictionary<Guid, string>();


        private Dictionary<Guid, Organization> Cache = new Dictionary<Guid, Organization>();

        public Dictionary<Guid, DateTime> lastfail = new Dictionary<Guid, DateTime>();

        // if failed for 5 times, not get any more.  

        public Organization Get(Guid id)
        {
            var org = GetFromLocal(id); 

            if (org == null || org.LastModified == default(DateTime) || org.LastModified < DateTime.Now.AddHours(-12))
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
            if (this.Cache.ContainsKey(id))
            {
                return this.Cache[id];
            }

            var org = GlobalDb.LocalOrganization.Get(id);

            if (org != null)
            { 
                Cache[id] = org;
                return org; 
            } 
            return null;
        }


        public Organization GetFromAccount(Guid id)
        {
            if (lastfail.ContainsKey(id))
            {
                var lasttime = lastfail[id];
                if (lasttime > DateTime.Now.AddHours(-1))
                {
                    return null;
                }
            }

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", id.ToString());
            var org = HttpHelper.Post<Organization>(AccountUrlHelper.Org("get"), para);

            if (org == null)
            {
                // try one more time. 
                org = HttpHelper.Post<Organization>(AccountUrlHelper.Org("get"), para);
            }

            if (org != null)
            {
                return org;
            }
            else
            {
                lastfail[id] = DateTime.Now;
                return null;
            }
        }

        public Organization GetByUser(Guid UserId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("userid", UserId.ToString());
            return HttpHelper.Post<Organization>(Kooboo.Data.Helper.AccountUrlHelper.Org("GetByUser"), para);
        }

        public List<User> Users(Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("organizationId", organizationId.ToString());
            return HttpHelper.Post<List<User>>(AccountUrlHelper.Org("Users"), para);
        }

        public string AddUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("organizationId", organizationId.ToString());
            para.Add("userName", userName);
            var result = HttpHelper.Post<string>(AccountUrlHelper.Org("adduser"), para);

            var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);
            Kooboo.Data.Cache.OrganizationUserCache.AddUser(organizationId, userid);

            return result;
        }

        public bool DeleteUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("organizationId", organizationId.ToString());
            para.Add("userName", userName);
            var ok = HttpHelper.Post<bool>(AccountUrlHelper.Org("deleteuser"), para);

            if (ok)
            {
                var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);
                Kooboo.Data.Cache.OrganizationUserCache.RemoveUser(organizationId, userid);
            }
            return ok;
        }

        public string GetName(Guid OrgId)
        {
            if (OrgId == default(Guid))
            {
                return System.Guid.Empty.ToString();
            }

            var org = GetFromLocal(OrgId); 

            if (org !=null)
            {
                return org.Name; 
            }
            else
            {

            }

            if (!NameCache.ContainsKey(OrgId))
            {
                var user = GlobalDb.Users.GetLocalUserByOrgId(OrgId);
                if (user != null)
                {
                    if (!string.IsNullOrWhiteSpace(user.CurrentOrgName) && Lib.Security.Hash.ComputeGuidIgnoreCase(user.CurrentOrgName) == OrgId)
                    {
                        NameCache[user.CurrentOrgId] = user.CurrentOrgName;
                        return user.CurrentOrgName;
                    }
                }
                var getorg = Get(OrgId);
                if (getorg != null && !string.IsNullOrWhiteSpace(getorg.Name))
                {
                    NameCache[getorg.Id] = getorg.Name;
                    return getorg.Name;
                }
            }

            else
            {
                return NameCache[OrgId];
            }

            return null;
        }

        public void RemoveOrgCache(Guid orgId)
        {
            GlobalDb.LocalOrganization.Delete(orgId);
            Cache.Remove(orgId);
        }

        public void AddOrUpdateLocal(Organization Organization)
        {
            Organization.LastModified = DateTime.Now; 

            GlobalDb.LocalOrganization.AddOrUpdate(Organization);
            Cache[Organization.Id] = Organization;
        }

    }
}
