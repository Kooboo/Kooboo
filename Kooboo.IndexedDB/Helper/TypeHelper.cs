//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB.CustomAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace Kooboo.IndexedDB.Helper
{
  public  static class TypeHelper
    { 
        public static Dictionary<string, Type> GetPublicPropertyOrFields(Type classType)
        {
            Dictionary<string, Type> result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase); 

            foreach (var item in classType.GetProperties())
            {
                if (item.IsDefined(typeof(KoobooIgnore), false))
                {
                    continue;
                }

                if (item.CanRead && item.CanWrite)
                {
                    result.Add(item.Name, item.PropertyType);
                } 
            }

            foreach (var item in classType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    result.Add(item.Name, item.FieldType); 
                }
            }
            return result; 
        } 

        public static Type GetFieldType<TValue>(string fieldName)
        {
            return GetFieldType(typeof(TValue), fieldName); 
        }

        public static Type GetFieldType(Type objectType, string fieldName)
        {
            var fields = GetPublicPropertyOrFields(objectType);
            return fields.ContainsKey(fieldName) ? fields[fieldName] : null;
        }

        public static Type GetType(string typename)
        {
            Type type = Type.GetType(typename);

            if (type != null)
            {
                return type;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            type = Type.GetType(typename, (AssemblyName o) => assemblies.FirstOrDefault(a => a.FullName == o.FullName), null);

            if (type != null)
            {
                return type;
            }

            foreach (var item in assemblies)
            {
                type = item.GetType(typename);

                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        public static bool IsDictIgnoreCase(Type declareType, string dictFieldName)
        { 
            foreach (var item in declareType.GetProperties())
            {
                if (item.Name == dictFieldName)
                {
                    return (item.IsDefined(typeof(KoobooKeyIgnoreCase), false)); 
                }  
            }

            foreach (var item in declareType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    if (item.Name == dictFieldName)
                    {
                        return (item.IsDefined(typeof(KoobooKeyIgnoreCase), false));
                    }
                }
            }
            return false;

        }


        public static T DeepCopy<T>(T input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, input);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

    }
}
