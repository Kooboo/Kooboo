//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using Kooboo.Sites.Scripting.KscriptConfig;
using System.Linq;

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
                                var instance = Activator.CreateInstance(item) as IkScript;

                                if (instance != null)
                                {
                                    _list.Add(instance.Name, item);
                                }

                            }

                            foreach(var item in ExtensionKscriptConfigContainer.List)
                            {
                                _list[item.Key] = item.Value;
                            }

                        }
                    }

                }
                return _list;
            }

        }

        public static object Get(string name, RenderContext context)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (List.ContainsKey(name))
            {
                var type = List[name];
                var instance = Activator.CreateInstance(type);

                if (instance !=null)
                {
                    
                    var kscriptInstance = instance as IkScript;
                    if (kscriptInstance!=null)
                    {
                        kscriptInstance.context = context;
                        return instance;
                    }
                    else
                    {
                        var properties = type.GetProperties().ToList();
                        var datacontextProperty = properties.Find(p => typeof(IDataContext).IsAssignableFrom(p.PropertyType));
                        if (datacontextProperty == null) return instance;

                        var datacontext=  Activator.CreateInstance(datacontextProperty.PropertyType, context);
                        datacontextProperty.SetValue(instance, datacontext);
                    }

                    return instance;
                    
                } 
            }
            return null;
        }

        private static void SetDataContext(Type type,RenderContext context)
        {
            
        }

        public static void Set(object script)
        {

        }

    }
}
