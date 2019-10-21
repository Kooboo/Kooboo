//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private Dictionary<Guid, TValue> _siteObjects = new Dictionary<Guid, TValue>();

        public void AddOrUpdate(TValue Value)
        {
            if (Value == null)
            {
                return;
            }
            lock (_locker)
            {
                _siteObjects[Value.Id] = Value;
            }
        }

        public TValue Get(Guid objectId)
        {
            if (_siteObjects.ContainsKey(objectId))
            {
                return (TValue)_siteObjects[objectId];
            }
            return default(TValue);
        }

        public TValue Get(string nameOrGuid)
        {
            Guid Key = default;

            if (System.Guid.TryParse(nameOrGuid, out Key))
            {
                return Get(Key);
            }

            var id = GetId(nameOrGuid);
            return Get(id);
        }

        public List<TValue> List()
        {
            return _siteObjects.Values.ToList();
        }

        public Guid GetId(string name)
        {
            string uniqu = ValueType.Name + name;
            return IDGenerator.GetId(uniqu);
        }
    }
}