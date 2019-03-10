//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Repository;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Cache
{
 
    public class SiteObjectCache<TValue> where TValue: ISiteObject
    {
        private static object _locker = new object();

        private static Dictionary<Guid, Dictionary<Guid, TValue>> SiteObjects = new Dictionary<Guid, Dictionary<Guid, TValue>>(); 

        private static Kooboo.IndexedDB.Serializer.Simple.SimpleConverter<TValue> serializer = new IndexedDB.Serializer.Simple.SimpleConverter<TValue>(); 

        private static TValue clone(TValue input)
        {
            var bytes = serializer.ToBytes(input);
            var back = serializer.FromBytes(bytes);
            return back; 
        }

        public static void Remove(SiteDb SiteDb, TValue value)
        {
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(SiteDb.Id))
                {
                    var siteobject = SiteObjects[SiteDb.Id];

                    siteobject.Remove(value.Id);
                }
            }
        }

        public static void AddOrUpdate(SiteDb SiteDb, TValue Value)
        {
            if (Value == null)
            {
                return;
            }

            lock (_locker)
            {
                Dictionary<Guid, TValue> siteobject = null;

                if (SiteObjects.ContainsKey(SiteDb.Id))
                {
                    siteobject = SiteObjects[SiteDb.Id];
                }
                else
                {/// should never come here... 
                    siteobject = new Dictionary<Guid, TValue>();
                    SiteObjects[SiteDb.Id] = siteobject;
                }

                siteobject[Value.Id] = clone(Value); 
            }
        }

        public static TValue Get(SiteDb SiteDb, string NameOrId)
        {
            Guid id;
            if (!System.Guid.TryParse(NameOrId, out id))
            {
                var consttype = ConstTypeContainer.GetConstType(typeof(TValue));
                id = Kooboo.Data.IDGenerator.Generate(NameOrId, consttype);
            }
            return Get(SiteDb, id);
        }

        public static TValue Get(SiteDb SiteDb, Guid ObjectId)
        {
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(SiteDb.Id))
                {
                    var siteobject = SiteObjects[SiteDb.Id];

                    if (siteobject.ContainsKey(ObjectId))
                    {
                        TValue value =  siteobject[ObjectId];
                        return clone(value); 
                    }
                }
            }
            return default(TValue);
        }
        
        public static List<TValue> List(SiteDb SiteDb, bool cloneObject = true)
        {
            List<TValue> result = null;
            lock (_locker)
            {
                if (SiteObjects.ContainsKey(SiteDb.Id))
                {
                    var siteobject = SiteObjects[SiteDb.Id];
                    result = siteobject.Values.ToList();
                }
                else
                {
                    var repo = SiteDb.GetRepository(typeof(TValue));
                    if (repo == null)
                    {
                        return new List<TValue>(); 
                    }

                    Dictionary<Guid, TValue> siteobject = new Dictionary<Guid, TValue>();

                    foreach (var item in repo.All())
                    {
                        siteobject[item.Id] = (TValue)item; 
                    }
                    SiteObjects[SiteDb.Id] = siteobject;

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
                    var newitem = clone(item);
                    cloned.Add(newitem);
                }
                return cloned;
            }
            else
            {
                return result;
            }
        }

        public static void RemoveSiteDb(Guid Id)
        {
            lock (_locker)
            {
                SiteObjects.Remove(Id);
            }
        }
        
    }
}
