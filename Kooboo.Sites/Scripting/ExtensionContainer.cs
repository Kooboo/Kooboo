//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using KScript.KscriptConfig;
using Kooboo.Lib.Reflection;
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
                            if (KscriptConfigContainer.List != null)
                            {
                                foreach (var item in KscriptConfigContainer.List)
                                {
                                    _list[item.Key] = item.Value;
                                }
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
                    else if(instance is KScriptConfigType)
                    {
                        var kscriptConfigType = instance as KScriptConfigType;
                        kscriptConfigType.SetKscriptnameNContext(name, context);
                    }

                    return instance;
                    
                } 
            }
            return null;
        }

        //private static void Clear()
        //{
        //    _list = null;
        //    KscriptConfigContainer.Clear();
        //}

        private static void SetDataContext(Type type,RenderContext context)
        {
            
        }

        public static void Set(object script)
        {

        }

    }
}
