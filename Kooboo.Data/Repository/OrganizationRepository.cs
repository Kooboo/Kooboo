//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using Kooboo.Lib.Helper;

namespace Kooboo.Data.Repository
{
    public class OrganizationRepository
    {
        internal Dictionary<Guid, string> NameCache = new Dictionary<Guid, string>();


        public Organization Add(Organization org)
        {
            var json = Lib.Helper.JsonHelper.Serialize(org);
            var neworg = HttpHelper.Post<Organization>(Account.Url.Org.Add, json);

            if (!NameCache.ContainsKey(neworg.Id))
            {
                NameCache.Add(neworg.Id, neworg.Name);
            }
            else
            {
                if (NameCache[neworg.Id] != neworg.Name)
                {
                    NameCache[neworg.Id] = neworg.Name;
                }
            }
            return neworg;
        }

        public bool Update(Organization org)
        {
            if (!NameCache.ContainsKey(org.Id))
            {
                NameCache.Add(org.Id, org.Name);
            }
            else
            {
                if (NameCache[org.Id] != org.Name)
                {
                    NameCache[org.Id] = org.Name;
                }
            }
            var json = Lib.Helper.JsonHelper.Serialize(org);
            return HttpHelper.Post<bool>(Account.Url.Org.Update, json);
        }

        public bool Delete(Guid id)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", id.ToString());
            var paramStr = Lib.Helper.JsonHelper.Serialize(para);
            return HttpHelper.Post<bool>(Account.Url.Org.Delete, paramStr);
        }

        public Organization Get(Guid id)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("id", id.ToString());
            return HttpHelper.Post<Organization>(Account.Url.Org.GetUrl, para);
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
            return HttpHelper.Post<List<User>>(Account.Url.Org.Users, para);
        }

        public string AddUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("organizationId", organizationId.ToString());
            para.Add("userName", userName);
            var result =  HttpHelper.Post<string>(Account.Url.Org.AddUser, para);

            var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName);
            Kooboo.Data.Cache.OrganizationUserCache.AddUser(organizationId, userid); 

            return result; 
        }

        public bool DeleteUser(string userName, Guid organizationId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("organizationId", organizationId.ToString());
            para.Add("userName", userName);
            var ok =  HttpHelper.Post<bool>(Account.Url.Org.DeleteUser, para);

            if (ok)
            {
                var userid = Lib.Security.Hash.ComputeGuidIgnoreCase(userName); 
                Kooboo.Data.Cache.OrganizationUserCache.RemoveUser(organizationId, userid); 
            }
            return ok; 
        }

        public bool AddProposalUserBalance(Guid proposalUserId, Guid proposalId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("proposalUserId", proposalUserId.ToString());
            para.Add("proposalId", proposalId.ToString());
            return HttpHelper.Post<bool>(Account.Url.Org.AddProposalUserBalance, para);
        }

        public bool ChangeDemandUserBalance(Guid demandUserId, Guid proposalId)
        {
            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("demandUserId", demandUserId.ToString());
            para.Add("proposalId", proposalId.ToString());
            return HttpHelper.Post<bool>(Account.Url.Org.ChangeDemandUserBalance, para);
        }

        public string GetName(Guid OrgId)
        {
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

        public bool ChangeDataCenter(Guid OrganizationId, string DataCenter)
        {
            string url = Kooboo.Data.Helper.AccountUrlHelper.OnlineDataCenter("ChangeDataCenter");

            Dictionary<string, string> para = new Dictionary<string, string>();
            para.Add("OrganizationId", OrganizationId.ToString());
            para.Add("DataCenter", DataCenter);

            var org = HttpHelper.Post<Organization>(url, para);

            if (org != null)
            {
                GlobalDb.Organization.NameCache[org.Id] = org.Name;
                return true;
            }
            return false;
        }

        public void RemoveOrgCache(Guid orgId)
        {
            if (NameCache.ContainsKey(orgId))
            {
                NameCache.Remove(orgId);
            }
        }

    }
}
