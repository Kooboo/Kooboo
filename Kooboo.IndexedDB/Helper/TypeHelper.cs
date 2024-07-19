//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using Kooboo.IndexedDB.CustomAttributes;

namespace Kooboo.IndexedDB.Helper
{
    public static class TypeHelper
    {
        public static Dictionary<string, Type> GetPublicPropertyOrFields(Type ClassType)
        {
            Dictionary<string, Type> result = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in ClassType.GetProperties())
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

            foreach (var item in ClassType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    result.Add(item.Name, item.FieldType);
                }
            }
            return result;
        }

        public static Type GetFieldType<TValue>(string FieldName)
        {
            return GetFieldType(typeof(TValue), FieldName);
        }

        public static Type GetFieldType(Type ObjectType, string FieldName)
        {
            var Fields = GetPublicPropertyOrFields(ObjectType);
            if (Fields.ContainsKey(FieldName))
            {
                return Fields[FieldName];
            }
            return null;
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

        public static bool IsDictIgnoreCase(Type declareType, string DictFieldName)
        {
            foreach (var item in declareType.GetProperties())
            {
                if (item.Name == DictFieldName)
                {
                    return (item.IsDefined(typeof(KoobooKeyIgnoreCase), false));
                }
            }

            foreach (var item in declareType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    if (item.Name == DictFieldName)
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

        // is the type of Dictionary<string, object>. 

        public static bool IsDictionaryStrObj(Type type)
        {
            if (IsDictionary(type))
            {
                var keytype = GetDictionaryKeyType(type);
                var valuetype = GetDictionaryValueType(type);

                return (keytype == typeof(string) && valuetype == typeof(object));

            }
            return false;
        }

        public static bool IsDictionary(Type type)
        {
            if (!type.IsGenericType)
            {
                return HasInterface(type, typeof(System.Collections.IDictionary));
            }
            else
            {
                return (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) || (type.GetGenericTypeDefinition() == typeof(IDictionary<,>));
            }
        }

        public static bool HasInterface(Type CheckType, Type InterfaceType)
        {
            foreach (var item in CheckType.GetInterfaces())
            {
                if (item == InterfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        public static Type GetDictionaryKeyType(Type DictionaryType)
        {
            return DictionaryType.GetGenericArguments()[0];
        }

        public static Type GetDictionaryValueType(Type DictionaryType)
        {
            return DictionaryType.GetGenericArguments()[1];
        }


        public static TField GetPropertyValue<TField, TValue>(string PropertyName)
        {
            var type = typeof(TValue);

            var method = type.GetProperty(PropertyName).GetGetMethod();
            var dynamicMethod = new DynamicMethod("meide", typeof(TField),
                                                  Type.EmptyTypes);
            var generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldnull);
            generator.Emit(OpCodes.Call, method);
            generator.Emit(OpCodes.Ret);
            var funcCall = (Func<TField>)dynamicMethod.CreateDelegate(
                           typeof(Func<TField>));
            return funcCall();
        }

    }
}
