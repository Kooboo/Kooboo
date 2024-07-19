//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Kooboo.Cache
{
    public static class CacheExpiredNotification
    {
        public static List<INotifyCacheExpired> Notifiactions = new List<INotifyCacheExpired>();

        public static void Notify(string objectCacheName, string cacheKey)
        {
            if (Notifiactions != null)
            {
                try
                {
                    foreach (var item in Notifiactions)
                    {
                        item.Notify(objectCacheName, cacheKey);
                    }
                }
                catch (Exception e)
                {
                    //log exception
                    Debug.WriteLine(e);
                }
            }
        }
    }
}
