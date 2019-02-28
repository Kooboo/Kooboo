//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Cache;

namespace Kooboo.Lib
{
    public class CacheUtility
    {
        private static readonly MemoryCache Cache = new MemoryCache(AppDomain.CurrentDomain.FriendlyName);

        private static readonly CacheEntryRemovedCallback CacheExpiredNotify = (arguments) =>
        {
            if (arguments.RemovedReason == CacheEntryRemovedReason.Removed)
            {
                CacheExpiredNotification.Notify(arguments.Source.Name, arguments.CacheItem.Key);
            }
        };

        private static readonly CacheItemPolicy DefaultCacheItemPolicy = new CacheItemPolicy
        {
            SlidingExpiration = TimeSpan.Parse("00:30:00"),
            RemovedCallback = CacheExpiredNotify
        };

        public static void Add(string key, object value)
        {
            Cache.Add(key, value, DefaultCacheItemPolicy);
        }

        public static object Get(string key)
        {
            return Cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            var value = Get(key);
            return value != null ? (T)value : default(T);
        }
         

        public static T GetOrAdd<T>(string key, Func<T> creator)
        {
            T value = Get<T>(key);
            if (value == null)
            {
                value = creator();
                if (value != null)
                {
                    Add(key, value);
                }
            }
            return value;
        }

 
        public static void Remove(string key)
        {
            Cache.Remove(key);
            CacheExpiredNotification.Notify("___GlobalCache___", key);
        }
 
    }
}
