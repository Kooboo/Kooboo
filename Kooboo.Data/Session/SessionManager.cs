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

        public static UserSession GetRemoteSessionId(User user, string remoteHost)
        {
            var session = GetSession(user.UserName, remoteHost, null);
            if (session != null)
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

        public static UserSession GetOrSetSession(User user, string remoteHost, string clientIp)
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

        public static UserSession GetSession(string userName, string remoteHost, string clientIp)
        {
            string unique = userName + remoteHost + clientIp;
            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);

            if (SessionCache.ContainsKey(key))
            {
                var session = SessionCache[key];
                if (session.LastUse > DateTime.Now.AddMinutes(-3))
                {
                    session.LastUse = DateTime.Now;
                    return session;
                }
            }
            return null;
        }
    }
}