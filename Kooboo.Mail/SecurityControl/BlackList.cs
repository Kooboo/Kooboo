using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Data.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Kooboo.Mail.SecurityControl
{
    public static class BlackList
    {
        public static MemoryCache ViolationCache { get; set; } = new MemoryCache(new MemoryCacheOptions());

        public static MemoryCache BlackListCache { get; set; } = new MemoryCache(new MemoryCacheOptions());


        public static int KeepSeconds { get; set; } = 4 * 60 * 60;

        public static bool IsBlacklisted(string IP)
        {
            if (IP == null)
            {
                return false;
            }

            if (BlackListCache.TryGetValue(IP, out var time))
            {
                return true;
            }

            return false;
        }


        public static void AddViolation(string IP)
        {
            if (ViolationCache.TryGetValue(IP, out List<DateTime> item))
            {
                if (item == null)
                {
                    item = new List<DateTime>();
                }
                var lastOneHourCount = item.Count(o => o > DateTime.Now.AddHours(-1));

                if (lastOneHourCount >= 3)
                {
                    BlackListCache.Set(IP, DateTime.Now, TimeSpan.FromHours(12));
                }
                item.Add(DateTime.Now);
            }
            else
            {
                List<DateTime> newItem = new List<DateTime>();
                newItem.Add(DateTime.Now);
                ViolationCache.Set(IP, newItem, TimeSpan.FromSeconds(KeepSeconds));
            }
        }


        public static bool IsUserBanned(User user)
        {
            if (user != null && user.UserName.ToLower() == "qingbo")
            {
                return true;
            }
            return false;
        }

    }
}
