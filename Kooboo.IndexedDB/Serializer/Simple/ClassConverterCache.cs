//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Serializer.Simple
{
    public static class ClassConverterCache
    {
        private static object _locker = new object();

        private static Dictionary<string, ClassConverter> Cache = new Dictionary<string, ClassConverter>();

        public static ClassConverter Get(Type classType)
        {
            lock (_locker)
            {
                var key = GetKey(classType);
                if (Cache.ContainsKey(key))
                {
                    return Cache[key];
                }
                return null;
            }
        }

        public static void Add(Type ClassType, ClassConverter converter)
        {
            lock (_locker)
            {
                var key = GetKey(ClassType);
                if (Cache.ContainsKey(key))
                {
                    return;
                }
                Cache[key] = converter;
            }
        }


        private static string GetKey(Type type)
        {
            return type.FullName;
        }


    }
}
