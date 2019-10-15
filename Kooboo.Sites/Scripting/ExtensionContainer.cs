//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting
{
    public static class ExtensionContainer
    {
        private static object _locker = new object();

        private static Dictionary<string, Type> _list;

        public static Dictionary<string, Type> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            _list = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                            foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IkScript)))
                            {
                                if (Activator.CreateInstance(item) is IkScript instance)
                                {
                                    _list.Add(instance.Name, item);
                                }
                            }
                        }
                    }
                }
                return _list;
            }
        }

        public static IkScript Get(string name, RenderContext context)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (List.ContainsKey(name))
            {
                var type = List[name];

                if (Activator.CreateInstance(type) is IkScript instance)
                {
                    instance.Context = context;
                    return instance;
                }
            }
            return null;
        }

        public static void Set(IkScript script)
        {
        }
    }
}