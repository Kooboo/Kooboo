//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Cache
{
   public static  class AccessTokenCache
    {
        private static object _locker = new object(); 

        public static Dictionary<string, UserTokenCache> UserTokenCache = new Dictionary<string, Cache.UserTokenCache>(); 
         
       public static string GetNewToken(Guid UserId)
        {
            lock (_locker)
            {
                string token = Kooboo.Lib.Security.ShortGuid.GetNewShortId();

                while (UserTokenCache.ContainsKey(token))
                {
                    token = Lib.Security.ShortGuid.GetNewShortId();
                }

                UserTokenCache cache = new Cache.UserTokenCache();
                cache.UserId = UserId;

                UserTokenCache[token] = cache;
                return token;
            }
        }

        public static void SetToken(Guid UserId, string token)
        {
            lock (_locker)
            {   
                UserTokenCache cache = new Cache.UserTokenCache();
                cache.UserId = UserId; 
                UserTokenCache[token] = cache; 
            }
        }

        public static Guid GetUserId(string token)
        {
            if (UserTokenCache.ContainsKey(token))
            {
                var cache = UserTokenCache[token];
                UserTokenCache.Remove(token);
                return cache.UserId; 
            }
            return default(Guid); 
        }

        public static void RomoveToken(string token)
        {
            UserTokenCache.Remove(token); 
        }
    }

    public class UserTokenCache
    {
        public Guid UserId { get; set; }

        public DateTime AddTime { get; set; } = new DateTime(); 
    }

}
