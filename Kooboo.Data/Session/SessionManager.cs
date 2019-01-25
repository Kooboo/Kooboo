//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic; 

namespace Kooboo.Data.Session
{
  public static class SessionManagerold
    { 
        private static DateTime LastCleanTime { get; set; } = DateTime.Now;

        private static Dictionary<Guid, UserSession> SessionCache = new Dictionary<Guid, UserSession>(); 
         
        public static Guid CreateSession(User user)
        { 
            return default(Guid); 
        }

        public static User ValidateSession(string key)
        { 
            return null;    
        } 

        public static UserSession GetRemoteSessionId(User User, string remoteHost)
        {
            var session = GetSession(User.UserName, remoteHost, null); 
            if (session !=null)
            {
                return session; 
            } 
            //create new session. 
            return null; 
        }
        

        public static Session.Requirement GetRequirement(string remotehost)
        {
            string url = "http://" + remotehost + "/_api/session/requirement";
            return Lib.Helper.HttpHelper.Get<Requirement>(url); 
        }

        public static UserSession GetOrSetSession(User user, string RemoteHost, string ClientIp)
        {
            return null;   
        } 
         

        public static UserSession CreateClientSession(User user)
        {
            return null;
        }

        public static UserSession CreateServerSession(User user)
        {
            return null;
        }

        public static UserSession GetSession(string UserName, string RemoteHost, string ClientIP)
        {
            string unique = UserName + RemoteHost + ClientIP; 
            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(unique); 

            if (SessionCache.ContainsKey(key))
            {
                var session = SessionCache[key];  
                if (session.LastUse >DateTime.Now.AddMinutes(-3))
                {
                    session.LastUse = DateTime.Now;
                    return session; 
                } 
            } 
            return null; 
        }
         
    }

    public class UserSession
    {  
        public static User User { get; set; } 

        public string RSAPublicKey { get; set; } 

        public string RSAPrivateKey { get; set; }  

        public bool Rsa { get; set; }
            
        public Guid SessionKey { get; set; }

        public DateTime  LastUse { get; set; }
         
        public string  RemoteHost { get; set; }
         
        public string ClientIp { get; set; }

        private HashSet<Guid> _usedkey;
        public HashSet<Guid> UsedKey
        {
            get
            {
                if (_usedkey == null)
                {
                    _usedkey = new HashSet<Guid>(); 
                }
                return _usedkey; 
            }
            set { _usedkey = value;  }
        }
        
    }
}
