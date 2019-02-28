//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo
{
    public class TypeEntry<TMeta, TInterface>
    {
        private object _lock = new object();

        public TMeta Meta { get; set; }

        public Type Type { get; set; }

        private TInterface _instance;
        public TInterface Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = (TInterface)Activator.CreateInstance(Type);
                        }
                    }
                }
                return _instance;
            }
        }

        public TInterface CreateInstance()
        {
            return (TInterface)Activator.CreateInstance(Type);
        }
    }

    public static class TypeEntryExtensions
    {
        public static IEnumerable<TInterface> Instances<TMeta, TInterface>(this IEnumerable<TypeEntry<TMeta, TInterface>> types)
        {
            return types.Select(o => o.Instance).ToArray();
        }

        public static IEnumerable<TInterface> CreateInstances<TMeta, TInterface>(this IEnumerable<TypeEntry<TMeta, TInterface>> types)
        {
            return types.Select(o => o.CreateInstance()).ToArray();
        }
    }
}