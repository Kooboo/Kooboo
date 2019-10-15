//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Cache
{
    public static class AccessTokenCache
    {
        private static object _locker = new object();

        public static Dictionary<string, UserTokenCache> UserTokenCache = new Dictionary<string, Cache.UserTokenCache>();

        public static string GetNewToken(Guid userId)
        {
            lock (_locker)
            {
                string token = Kooboo.Lib.Security.ShortGuid.GetNewShortId();

                while (UserTokenCache.ContainsKey(token))
                {
                    token = Lib.Security.ShortGuid.GetNewShortId();
                }

                UserTokenCache cache = new Cache.UserTokenCache {UserId = userId};

                UserTokenCache[token] = cache;
                return token;
            }
        }

        public static void SetToken(Guid userId, string token)
        {
            lock (_locker)
            {
                UserTokenCache cache = new Cache.UserTokenCache {UserId = userId};
                UserTokenCache[token] = cache;
            }
        }

        public static Guid GetUserId(string token)
        {
            lock (_locker)
            {
                if (UserTokenCache.ContainsKey(token))
                {
                    var cache = UserTokenCache[token];
                    UserTokenCache.Remove(token);
                    return cache.UserId;
                }
            }

            return default;
        }

        public static void RomoveToken(string token)
        {
            lock (_locker)
            {
                UserTokenCache.Remove(token);
            }
        }
    }

    public class UserTokenCache
    {
        public Guid UserId { get; set; }

        public DateTime AddTime { get; set; } = new DateTime();
    }
}