//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using Kooboo.Sites.DataSources;

namespace Kooboo.Sites.Cache
{
    public static class CompileMethodCache
    { 
        private static object _lock = new object(); 

        public static Dictionary<Guid, DataMethodCompiled> MethodCache = new Dictionary<Guid, DataMethodCompiled>();

        public static DataMethodCompiled GetGlobalCompiledMethod(Guid MethodId)
        {
            lock(_lock)
            {
                DataMethodCompiled compiled;
                if (MethodCache.TryGetValue(MethodId, out compiled))
                {
                    return compiled;
                }
                IDataMethodSetting DataMethod = Data.GlobalDb.DataMethodSettings.Get(MethodId);
                compiled = new DataMethodCompiled(DataMethod);
                MethodCache[MethodId] = compiled;
                return compiled;
            }
        }

        public static DataMethodCompiled GetCompiledMethod(SiteDb SiteDb, Guid MethodId)
        {
            Guid id = GetId(SiteDb, MethodId); 
            lock(_lock)
            { 
                DataMethodCompiled compiled;
                if (MethodCache.TryGetValue(id, out compiled))
                {
                    return compiled;
                }

                // Global does not have site id. 

                if (MethodCache.TryGetValue(MethodId, out compiled))
                {
                    return compiled; 
                }

                IDataMethodSetting DataMethod = SiteDb.DataMethodSettings.Get(MethodId);
                if (DataMethod != null)
                {
                    compiled = new DataMethodCompiled(DataMethod);
                    MethodCache[id] = compiled;
                    return compiled;
                }

                DataMethod = Data.GlobalDb.DataMethodSettings.Get(MethodId);
                if (DataMethod != null)
                {
                    compiled = new DataMethodCompiled(DataMethod);
                    MethodCache[MethodId] = compiled;
                    return compiled;
                } 
            } 
            return null;
        }

        public static void Remove(SiteDb SiteDb, Guid MethodId)
        {
            Guid id = GetId(SiteDb, MethodId); 

            lock(_lock)
            {
                if (MethodCache.ContainsKey(id))
                {
                    MethodCache.Remove(id);
                }
            }
        }

        private static Guid GetId(SiteDb sitedb, Guid MethodId)
        {
            string unique =  sitedb.WebSite.Id.ToString() + MethodId.ToString();
            return unique.ToHashGuid();
        }

    }
}
