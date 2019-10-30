//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Extensions;
using Kooboo.Sites.DataSources;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Cache
{
    public static class CompileMethodCache
    {
        private static object _lock = new object();

        public static Dictionary<Guid, DataMethodCompiled> MethodCache = new Dictionary<Guid, DataMethodCompiled>();

        public static DataMethodCompiled GetGlobalCompiledMethod(Guid methodId)
        {
            lock (_lock)
            {
                DataMethodCompiled compiled;
                if (MethodCache.TryGetValue(methodId, out compiled))
                {
                    return compiled;
                }
                IDataMethodSetting dataMethod = Data.GlobalDb.DataMethodSettings.Get(methodId);
                compiled = new DataMethodCompiled(dataMethod);
                MethodCache[methodId] = compiled;
                return compiled;
            }
        }

        public static DataMethodCompiled GetCompiledMethod(SiteDb siteDb, Guid methodId)
        {
            Guid id = GetId(siteDb, methodId);
            lock (_lock)
            {
                DataMethodCompiled compiled;
                if (MethodCache.TryGetValue(id, out compiled))
                {
                    return compiled;
                }

                // Global does not have site id.

                if (MethodCache.TryGetValue(methodId, out compiled))
                {
                    return compiled;
                }

                IDataMethodSetting dataMethod = siteDb.DataMethodSettings.Get(methodId);
                if (dataMethod != null)
                {
                    compiled = new DataMethodCompiled(dataMethod);
                    MethodCache[id] = compiled;
                    return compiled;
                }

                dataMethod = Data.GlobalDb.DataMethodSettings.Get(methodId);
                if (dataMethod != null)
                {
                    compiled = new DataMethodCompiled(dataMethod);
                    MethodCache[methodId] = compiled;
                    return compiled;
                }
            }
            return null;
        }

        public static void Remove(SiteDb siteDb, Guid methodId)
        {
            Guid id = GetId(siteDb, methodId);

            lock (_lock)
            {
                if (MethodCache.ContainsKey(id))
                {
                    MethodCache.Remove(id);
                }
            }
        }

        private static Guid GetId(SiteDb sitedb, Guid methodId)
        {
            string unique = sitedb.WebSite.Id.ToString() + methodId.ToString();
            return unique.ToHashGuid();
        }
    }
}