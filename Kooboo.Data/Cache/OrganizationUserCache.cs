//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Cache
{
  public static  class OrganizationUserCache
    {
        private static object _locker = new object(); 
        
        public static Dictionary<Guid, OrgUser> CacheItems { get; set; }

        static OrganizationUserCache()
        {
            CacheItems = new Dictionary<Guid, OrgUser>(); 
        }

        private static OrgUser GetOrg(Guid OrgId)
        {
            if (!CacheItems.ContainsKey(OrgId))
            {   
                lock(_locker)
                {
                    if (!CacheItems.ContainsKey(OrgId))
                    {
                        OrgUser orguser = getOrgUserIds(OrgId);
                        CacheItems[OrgId] = orguser;
                    }
                }
            }

            return CacheItems[OrgId]; 
        }

        private static OrgUser getOrgUserIds(Guid OrgId)
        {
            OrgUser orguser = new OrgUser();
            var users = GetOrgUsers(OrgId);
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

        public static void AddUser(Guid OrgId, Guid UserId)
        {
            var orguser = GetOrg(OrgId);
            orguser.Users.Add(UserId);
            orguser.LastModified = DateTime.Now; 
        }

        public static void RemoveUser(Guid OrgId, Guid UserId)
        {
            var orguser = GetOrg(OrgId);
            orguser.Users.Remove(UserId);
            orguser.LastModified = DateTime.Now; 
        }

        public static bool HasUser(Guid OrgId, Guid UserId)
        {  
            var orgusr = GetOrg(OrgId); 

            if (orgusr !=null && orgusr.Users.Contains(UserId))
            {
                return true; 
            } 

            if (orgusr.LastModified < DateTime.Now.AddHours(-1))
            {
                var neworguser = getOrgUserIds(OrgId); 
                if (neworguser !=null)
                {
                    CacheItems[OrgId] = neworguser;    

                    if (neworguser.Users.Contains(UserId))
                    {
                        return true; 
                    }
                }
            }

            return false;
        }

        public static List<Kooboo.Data.Models.User> GetOrgUsers(Guid OrgId)
        {
            return GlobalDb.Organization.Users(OrgId); 
        }   

    }


    public class OrgUser
    {
        public OrgUser()
        {
            this.Users = new HashSet<Guid>(); 
        }

        public Guid OrgId { get; set; }

        public HashSet<Guid> Users { get; set; }

        public DateTime LastModified { get; set; }
    }


}

