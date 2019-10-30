//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Cache
{
    public class SiteObjectCache<TValue> where TValue : ISiteObject
    {
        private static object _locker = new object();

        private static Dictionary<Guid, Dictionary<Guid, TValue>> SiteObjects = new Dictionary<Guid, Dictionary<Guid, TValue>>();

        private static Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<TValue> serializer = new IndexedDB.Serializer.Simple.SimpleConverter<TValue>();

        private static TValue Clone(TValue input)
        {
            var bytes = serializer.ToBytes(input);
            var back = serializer.FromBytes(bytes);
            return back;
        }

        public static void Remove(SiteDb siteDb, TValue value)
        {
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(siteDb.Id))
                {
                    var siteobject = SiteObjects[siteDb.Id];

                    siteobject.Remove(value.Id);
                }
            }
        }

        public static void AddOrUpdate(SiteDb siteDb, TValue value)
        {
            if (value == null)
            {
                return;
            }

            lock (_locker)
            {
                Dictionary<Guid, TValue> siteobject = null;

                if (SiteObjects.ContainsKey(siteDb.Id))
                {
                    siteobject = SiteObjects[siteDb.Id];
                }
                else
                {
                    // should never come here...
                    siteobject = new Dictionary<Guid, TValue>();
                    SiteObjects[siteDb.Id] = siteobject;
                }

                siteobject[value.Id] = Clone(value);
            }
        }

        public static TValue Get(SiteDb siteDb, string nameOrId)
        {
            if (!System.Guid.TryParse(nameOrId, out var id))
            {
                var consttype = ConstTypeContainer.GetConstType(typeof(TValue));
                id = Kooboo.Data.IDGenerator.Generate(nameOrId, consttype);
            }
            return Get(siteDb, id);
        }

        public static TValue Get(SiteDb siteDb, Guid objectId)
        {
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(siteDb.Id))
                {
                    var siteobject = SiteObjects[siteDb.Id];

                    if (siteobject.ContainsKey(objectId))
                    {
                        TValue value = siteobject[objectId];
                        return Clone(value);
                    }
                }
            }
            return default(TValue);
        }

        public static List<TValue> List(SiteDb siteDb, bool cloneObject = true)
        {
            List<TValue> result = null;
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(siteDb.Id))
                {
                    var siteobject = SiteObjects[siteDb.Id];
                    result = siteobject.Values.ToList();
                }
                else
                {
                    var repo = siteDb.GetRepository(typeof(TValue));
                    if (repo == null)
                    {
                        return new List<TValue>();
                    }

                    Dictionary<Guid, TValue> siteobject = new Dictionary<Guid, TValue>();

                    foreach (var item in repo.All())
                    {
                        siteobject[item.Id] = (TValue)item;
                    }
                    SiteObjects[siteDb.Id] = siteobject;

                    result = siteobject.Values.ToList();
                }
            }

            if (result == null || result.Count == 0)
            {
                return new List<TValue>();
            }
            if (cloneObject)
            {
                List<TValue> cloned = new List<TValue>();
                foreach (var item in result)
                {
                    var newitem = Clone(item);
                    cloned.Add(newitem);
                }
                return cloned;
            }
            else
            {
                return result;
            }
        }

        public static void RemoveSiteDb(Guid id)
        {
            lock (_locker)
            {
                SiteObjects.Remove(id);
            }
        }
    }
}