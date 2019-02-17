﻿using Kooboo.Lib.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Kooboo.Module
{ 
    public static class ModuleApiContainer
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
                             
                            var alldefinedTypes = AssemblyLoader.LoadTypeByInterface(typeof(ISiteModuleApi));
                            foreach (var item in alldefinedTypes)
                            {
                                var name = GetNameProperty(item, "ModelName");
                                if (!string.IsNullOrWhiteSpace(name))
                                {
                                    _list.Add(name, item);
                                } 
                               // Activator.CreateInstance(item, )
                               // AddApi(_list, instance);
                            } 
                        }
                    }
                }
                return _list;
            }
        }
      
        public static Type GetType(string ModelName)
        {  
            if (List.ContainsKey(ModelName))
            {
                return List[ModelName];
            }
            return null;
        }

        public static string GetNameProperty(Type objType, string PropertyName)
        { 
            var method = objType.GetProperty(PropertyName).GetGetMethod();
            var dynamicMethod = new DynamicMethod("meide", typeof(string),
                                                  Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Call, method);
            generator.Emit(OpCodes.Ret);
            var silly = (Func<string>)dynamicMethod.CreateDelegate(
                           typeof(Func<string>)); 
            return silly(); 
        }  
        
    }
}
