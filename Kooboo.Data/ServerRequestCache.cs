//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data
{

    public static class ServerCache
    {
        public static ServerRequestCache<User> User = new ServerRequestCache<User>(); 
        public static ServerRequestCache<Domain> Domain = new ServerRequestCache<Models.Domain>(); 
    }


    public class ServerRequestCache<TValue> where TValue : IGolbalObject
    {
        private object _locker = new object();

        private Type _valuetype;
        private Type ValueType
        {
            get
            {
                if (_valuetype == null)
                {
                    _valuetype = typeof(TValue);
                }
                return _valuetype;
            }
        }

        private Dictionary<Guid, TValue> SiteObjects = new Dictionary<Guid, TValue>();
         

        public void AddOrUpdate(TValue Value)
        {
            if (Value == null)
            {
                return;
            }
            lock (_locker)
            {
                SiteObjects[Value.Id] = Value;
            }
        }

        public TValue Get(Guid ObjectId)
        {
            if (SiteObjects.ContainsKey(ObjectId))
            {
                return (TValue)SiteObjects[ObjectId];
            }
            return default(TValue);
        }

        public TValue Get(string NameOrGuid)
        {
            Guid Key = default(Guid);

            if (System.Guid.TryParse(NameOrGuid, out Key))
            {
                return Get(Key);
            }
            else
            {
                var id = GetId(NameOrGuid);
                return Get(id);
            }
        }

        public List<TValue> List()
        {
            return SiteObjects.Values.ToList();
        }

        public Guid GetId(string name)
        {
            string uniqu = ValueType.Name + name;
            return IDGenerator.GetId(uniqu);
        }
    }


}
