//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Kooboo.Extensions;
using System.Linq.Expressions;

namespace Kooboo.Lib.Reflection
{
    public static class TypeHelper
    {

        public static List<MethodInfo> GetPublicMethods(Type type)
        {
            List<MethodInfo> result = new List<MethodInfo>();
            List<MethodInfo> list = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy).ToList();

            foreach (var item in list)
            {
                if (!item.IsSpecialName && !IsObjectName(item.Name))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private static bool IsObjectName(string Name)
        {
            if (Name == "GetType" || Name == "ToString" || Name == "GetHashCode" || Name == "Equals" || Name == "ReferenceEquals")
            {
                return true;
            }

            return false;
        }

        public static Type GetGenericType(Type typeinfo)
        {
            if (typeinfo == null)
            {
                return null;
            }
            var argus = typeinfo.GetGenericArguments();
            if (argus != null && argus.Length > 0)
            {
                return argus[0];
            }

            var basetype = typeinfo;

            while (argus == null || argus.Length == 0)
            {
                basetype = basetype.BaseType;

                if (basetype == null)
                {
                    break;
                }

                argus = basetype.GetGenericArguments();
                if (argus != null && argus.Length > 0)
                {
                    return argus[0];
                }
            }

            var allinterfaces = typeinfo.GetInterfaces();

            foreach (var item in allinterfaces)
            {
                argus = item.GetGenericArguments();
                if (argus != null && argus.Length > 0)
                {
                    return argus[0];
                }
            }

            return null;

        }


        public static List<Type> GetGenericTypes(Type typeinfo)
        {
            List<Type> Result = new List<Type>();

            if (typeinfo == null)
            {
                return null;
            }
            var argus = typeinfo.GetGenericArguments();
            if (argus != null && argus.Length > 0)
            {
                Result.Add(argus[0]);
            }

            var basetype = typeinfo;

            while (argus == null || argus.Length == 0)
            {
                basetype = basetype.BaseType;

                if (basetype == null)
                {
                    break;
                }

                argus = basetype.GetGenericArguments();
                if (argus != null && argus.Length > 0)
                {
                    Result.Add(argus[0]);
                }
            }

            var allinterfaces = typeinfo.GetInterfaces();

            foreach (var item in allinterfaces)
            {
                argus = item.GetGenericArguments();
                if (argus != null && argus.Length > 0)
                {
                    Result.Add(argus[0]);
                }
            }

            return Result;

        }

        public static MethodInfo GetRightMethodInfo(List<MethodInfo> MethodList, string MethodName, Guid MethodHash)
        {
            var methods = MethodList.FindAll(o => o.Name == MethodName);
            int count = methods.Count();
            if (count == 1)
            {
                return methods[0];
            }
            else if (count == 0)
            {
                return null;
            }
            else
            {
                foreach (var item in methods)
                {
                    if (TypeHelper.GetMethodSignatureHash(item) == MethodHash)
                    {
                        return item;
                    }
                }
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

            type = Type.GetType(typename, (AssemblyName o) => assemblies.First(a => a.FullName == o.FullName), null);

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

        public static bool IsComplexType(this Type type)
        {
            return !TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
        }

        public static Type GetGenericInterface(this Type queryType, Type interfaceGenericDefinition)
        {
            if (queryType.IsGenericType && queryType.GetGenericTypeDefinition() == interfaceGenericDefinition)
            {
                return queryType;
            }

            foreach (var @interface in queryType.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceGenericDefinition)
                {
                    return @interface;
                }
            }

            return null;
        }

        public static Guid GetMethodSignatureHash(MethodInfo method)
        {
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            string value = String.Format("{0}|{1}",
                method.Name, String.Join("|", parameterTypes.Select(p => p.Name)));

            value += method.DeclaringType.FullName;

            return value.ToHashGuid();
        }

        /// <summary>
        /// The the list of field or property of this object. 
        /// This is used to generate like property list from like config type. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, Type> GetPublicFieldOrProperties(this Type objectType)
        {
            Dictionary<string, Type> fieldlist = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            FieldInfo[] fieldInfo = objectType.GetFields();

            foreach (FieldInfo info in fieldInfo)
            {
                if (info.IsPublic)
                {
                    fieldlist.Add(info.Name, info.FieldType);
                }
            }

            foreach (var item in objectType.GetProperties())
            {
                if (item.CanRead && item.CanWrite && item.PropertyType.IsPublic)
                {
                    fieldlist.Add(item.Name, item.PropertyType);
                }
            }
            return fieldlist;
        }

        public static List<MemberInfo> GetPublicPropertyOrFields(Type ClassType)
        {

            List<MemberInfo> result = new List<MemberInfo>();

            foreach (var item in ClassType.GetProperties())
            {
                if (item.CanRead && item.CanWrite)
                {
                    result.Add(item);
                }
            }

            foreach (var item in ClassType.GetFields())
            {
                if (item.IsPublic && !item.IsStatic)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<MemberInfo> GetPublicMembers(Type objectType)
        {
            List<MemberInfo> result = new List<MemberInfo>();
            FieldInfo[] fieldInfo = objectType.GetFields();
            foreach (FieldInfo info in fieldInfo)
            {
                if (info.IsPublic && !info.IsStatic)
                {
                    result.Add(info);
                }
            }
            foreach (var item in objectType.GetProperties())
            {
                if (item.CanRead && item.CanWrite && item.PropertyType.IsPublic)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<MemberInfo> GetCanReadMembers(Type objectType)
        {
            List<MemberInfo> result = new List<MemberInfo>();
            FieldInfo[] fieldInfo = objectType.GetFields();
            foreach (FieldInfo info in fieldInfo)
            {
                if (info.IsPublic && !info.IsStatic)
                {
                    result.Add(info);
                }
            }
            foreach (var item in objectType.GetProperties())
            {
                if (item.CanRead && item.PropertyType.IsPublic)
                {
                    result.Add(item);
                }
            }
            return result;
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

        public static bool IsList(Type type)
        {
            if (!type.IsGenericType)
            { return false; }

            var EnumeGenericDefi = typeof(IList<>);

            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType && item.GetGenericTypeDefinition() == EnumeGenericDefi)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsGenericCollection(Type type)
        {
            if (!type.IsGenericType)
            { return false; }

            var EnumeGenericDefi = typeof(ICollection<>);
            var enumEnumable = typeof(IEnumerable<>);

            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType)
                {
                    var inter = item.GetGenericTypeDefinition();
                    if (inter == EnumeGenericDefi || inter == enumEnumable)
                    {
                        return true;
                    }
                }
            } 
            return false;
        }

        public static bool IsCollection(Type type)
        {
            if (type.IsGenericType)
            {
                return IsGenericCollection(type); 
            }
            else
            {
                if (HasInterface(type, typeof(System.Collections.IEnumerable)))
                {
                    return true; 
                }
            }
            return false; 
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

        public static bool HasGenericInterface(Type CheckType, Type GenericInterfaceType)
        {

            if (CheckType.IsGenericType && CheckType.GetGenericTypeDefinition() == GenericInterfaceType)
            {
                return true;
            }

            foreach (var item in CheckType.GetInterfaces())
            {
                if (item.IsGenericType)
                {
                    var genericdefinition = item.GetGenericTypeDefinition();

                    if (genericdefinition == GenericInterfaceType)
                    {
                        return true;
                    }

                }

            }
            return false;
        }

        public static bool HasBaseType(Type CheckType, Type BaseType)
        {
            Type currentbase = CheckType.BaseType;
            while (currentbase != null && currentbase != typeof(object))
            {
                if (currentbase == BaseType)
                {
                    return true;
                }
                currentbase = currentbase.BaseType;
            }
            return false;
        }

        public static bool IsOfBaseTypeOrInterface(Type CheckType, Type BaseOrInterface)
        {
            if (HasBaseType(CheckType, BaseOrInterface))
            {
                return true;
            }

            if (HasInterface(CheckType, BaseOrInterface))
            {
                return true;
            }

            if (CheckType == BaseOrInterface)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the datatype within the eg LIST<T>
        /// </summary>
        /// <param name="EnumberableType"></param>
        /// <returns></returns>
        public static Type GetEnumberableType(Type EnumberableType)
        {
            return EnumberableType.GetGenericArguments().Single();
        }

        public static Type GetDictionaryKeyType(Type DictionaryType)
        {
            return DictionaryType.GetGenericArguments()[0];
        }

        public static Type GetDictionaryValueType(Type DictionaryType)
        {
            return DictionaryType.GetGenericArguments()[1];
        }

        public static Type GetFieldType(Type ObjectType, string FieldName)
        {
            var property = ObjectType.GetProperty(FieldName);
            if (property != null)
            {
                return property.PropertyType;
            }

            var field = ObjectType.GetField(FieldName);
            if (field != null)
            {
                return field.FieldType;
            }
            return null;
        }

        public static Func<TValue, TFieldType> GetGetValue<TValue, TFieldType>(string FieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, FieldName);
            return Expression.Lambda<Func<TValue, TFieldType>>(expr, arg).Compile();
        }

        public static Action<TValue, TFieldType> GetSetValue<TValue, TFieldType>(string FieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, FieldName);
            var valueExp = Expression.Parameter(typeof(TFieldType));
            return Expression.Lambda<Action<TValue, TFieldType>>(Expression.Assign(expr, valueExp), arg, valueExp).Compile();
        }



        public static Action<TValue, object> GetSetObjectValue<TValue>(string FieldName, Type fieldtype)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));

            Expression expr = Expression.PropertyOrField(arg, FieldName);

            var objectpara = Expression.Parameter(typeof(object));

            var righttype = Expression.Convert(objectpara, fieldtype);

            var valueExp = Expression.Parameter(fieldtype);

            return Expression.Lambda<Action<TValue, object>>(Expression.Assign(expr, righttype), arg, objectpara).Compile();
        }

        public static Func<TValue, object> GetGetObjectValue<TValue>(string FieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, FieldName);
            return Expression.Lambda<Func<TValue, object>>(expr, arg).Compile();
        }

        public static Action<object, object> GetSetObjectValue(string FieldName, Type ObjectType, Type fieldtype)
        {
            var objectpara = Expression.Parameter(typeof(object));
            var fieldpara = Expression.Parameter(typeof(object));
            var rightObjectType = Expression.Convert(objectpara, ObjectType);
            Expression expr = Expression.PropertyOrField(rightObjectType, FieldName);
            var righttype = Expression.Convert(fieldpara, fieldtype);
            return Expression.Lambda<Action<object, object>>(Expression.Assign(expr, righttype), objectpara, fieldpara).Compile();

        }

        public static Func<object, object> GetGetObjectValue(string FieldName, Type objecttype)
        {
            ParameterExpression objectPara = Expression.Parameter(typeof(object));

            var expression = Expression.Convert(objectPara, objecttype);

            //Expression expr = Expression.PropertyOrField(expression, FieldName);

            Expression expr;
            if (objecttype.GetField(FieldName) != null)
            {
                expr = Expression.Field(expression, FieldName);
            }
            else
            {
                if (objecttype.GetProperty(FieldName) != null)
                {
                    expr = Expression.Property(expression, FieldName);
                }
                else
                {
                    var name = GetPropertyName(objecttype, FieldName);
                    if (name != null)
                    {
                        expr = Expression.PropertyOrField(expression, name);
                    }
                    else
                    {
                        return null;
                    }

                }
            }

            Expression convertobject = Expression.Convert(expr, typeof(object));

            return Expression.Lambda<Func<object, object>>(convertobject, objectPara).Compile();

        }

        private static string GetPropertyName(Type objType, string fieldname)
        {
            string lower = fieldname.ToLower();

            var ps = objType.GetProperties();

            foreach (var item in ps)
            {
                if (item.Name.ToLower() == lower)
                {
                    return item.Name;
                }
            }

            var fs = objType.GetFields();
            foreach (var item in fs)
            {
                if (item.Name.ToLower() == lower)
                {
                    return item.Name;
                }
            }

            return null;
        }

        public static Action<object, TFieldType> GetSetFieldValue<TFieldType>(string FieldName, Type ObjectType)
        {
            var objectpara = Expression.Parameter(typeof(object));
            var fieldpara = Expression.Parameter(typeof(TFieldType));
            var rightObjectType = Expression.Convert(objectpara, ObjectType);
            Expression expr = Expression.PropertyOrField(rightObjectType, FieldName);
            var righttype = Expression.Convert(fieldpara, typeof(TFieldType));
            return Expression.Lambda<Action<object, TFieldType>>(Expression.Assign(expr, righttype), objectpara, fieldpara).Compile();
        }

        public static Func<object, TFieldType> GetGetFieldValue<TFieldType>(string FieldName, Type objecttype)
        {
            ParameterExpression objectPara = Expression.Parameter(typeof(object));
            var expression = Expression.Convert(objectPara, objecttype);
            Expression expr = Expression.PropertyOrField(expression, FieldName);

            return Expression.Lambda<Func<object, TFieldType>>(expr, objectPara).Compile();
        }

        public static Func<object[], object> CompileStaticFunc(Type InvokeClassType, string MethodName, List<Type> paratypes)
        {
            var argsParam = Expression.Parameter(typeof(object[]));

            int counter = paratypes.Count;
            var castedArgs = new Expression[counter];

            for (var i = 0; i < counter; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (paratypes[i].IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, paratypes[i]);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, paratypes[i]);
                }
            }

            var methodCall = Expression.Call(InvokeClassType, MethodName, null, castedArgs);

            Expression lambdaBody = Expression.Convert(methodCall, typeof(object));

            return Expression.Lambda<Func<object[], object>>(lambdaBody, argsParam).Compile();
        }

        public static Func<object[], object> CompileStaticFunc(MethodInfo method)
        {
            var argsParam = Expression.Parameter(typeof(object[]));

            var methodArgs = method.GetParameters();
            var castedArgs = new Expression[methodArgs.Length];

            for (var i = 0; i < methodArgs.Length; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (methodArgs[i].ParameterType.IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, methodArgs[i].ParameterType);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, methodArgs[i].ParameterType);
                }
            }

            var methodCall = Expression.Call(method, castedArgs);
            Expression lambdaBody = Expression.Convert(methodCall, typeof(object));

            return Expression.Lambda<Func<object[], object>>(lambdaBody, argsParam).Compile();
        }

        public static Action<object[]> CompileStaticAction(MethodInfo method)
        {
            var argsParam = Expression.Parameter(typeof(object[]), "arguments");

            var methodArgs = method.GetParameters();

            var castedArgs = new Expression[methodArgs.Length];

            for (var i = 0; i < methodArgs.Length; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (methodArgs[i].ParameterType.IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, methodArgs[i].ParameterType);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, methodArgs[i].ParameterType);
                }
            }

            var methodCall = Expression.Call(method, castedArgs);

            return Expression.Lambda<Action<object[]>>(methodCall, argsParam).Compile();
        }

        public static Action<object[]> CompileStaticAction(Type InvokeClassType, string MethodName, List<Type> paratypes)
        {
            var argsParam = Expression.Parameter(typeof(object[]), "arguments");

            int counter = paratypes.Count;
            var castedArgs = new Expression[counter];

            for (var i = 0; i < counter; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (paratypes[i].IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, paratypes[i]);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, paratypes[i]);
                }
            }

            var methodCall = Expression.Call(InvokeClassType, MethodName, null, castedArgs);

            return Expression.Lambda<Action<object[]>>(methodCall, argsParam).Compile();
        }

        public static Func<object, object[], object> CompileFunc(MethodInfo method)
        {
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, method.ReflectedType);

            var argsParam = Expression.Parameter(typeof(object[]));
            var methodArgs = method.GetParameters();
            var castedArgs = new Expression[methodArgs.Length];

            for (var i = 0; i < methodArgs.Length; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (methodArgs[i].ParameterType.IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, methodArgs[i].ParameterType);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, methodArgs[i].ParameterType);
                }
            }

            var methodCall = Expression.Call(castedInstance, method, castedArgs);
            Expression lambdaBody = Expression.Convert(methodCall, typeof(object));

            return Expression.Lambda<Func<object, object[], object>>(lambdaBody, instanceParam, argsParam).Compile();
        }

        public static Action<object, object[]> CompileAction(MethodInfo method)
        {
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, method.ReflectedType);

            var argsParam = Expression.Parameter(typeof(object[]));
            var methodArgs = method.GetParameters();
            var castedArgs = new Expression[methodArgs.Length];

            for (var i = 0; i < methodArgs.Length; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (methodArgs[i].ParameterType.IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, methodArgs[i].ParameterType);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, methodArgs[i].ParameterType);
                }
            }

            var methodCall = Expression.Call(castedInstance, method, castedArgs);

            return Expression.Lambda<Action<object, object[]>>(methodCall, instanceParam, argsParam).Compile();
        }

        public static Func<object, object[], object> CompileFunc(Type InvokeClassType, string MethodName, List<Type> paratypes)
        {
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, InvokeClassType);

            var argsParam = Expression.Parameter(typeof(object[]));

            int counter = paratypes.Count;
            var castedArgs = new Expression[counter];

            for (var i = 0; i < counter; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (paratypes[i].IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, paratypes[i]);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, paratypes[i]);
                }
            }

            var methodCall = Expression.Call(castedInstance, MethodName, null, castedArgs);

            Expression lambdaBody = Expression.Convert(methodCall, typeof(object));

            return Expression.Lambda<Func<object, object[], object>>(lambdaBody, instanceParam, argsParam).Compile();
        }

        public static Action<object, object[]> CompileAction(Type InvokeClassType, string MethodName, List<Type> paratypes)
        {
            var argsParam = Expression.Parameter(typeof(object[]));
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, InvokeClassType);

            int counter = paratypes.Count;
            var castedArgs = new Expression[counter];

            for (var i = 0; i < counter; i++)
            {
                var argParam = Expression.ArrayIndex(argsParam, Expression.Constant(i));

                if (paratypes[i].IsValueType)
                {
                    castedArgs[i] = Expression.Convert(argParam, paratypes[i]);
                }
                else
                {
                    castedArgs[i] = Expression.TypeAs(argParam, paratypes[i]);
                }
            }

            var methodCall = Expression.Call(castedInstance, MethodName, null, castedArgs);

            return Expression.Lambda<Action<object, object[]>>(methodCall, instanceParam, argsParam).Compile();
        }
         


        public static bool IsFieldType(Type type)
        {
            if (type == typeof(string) || type == typeof(Guid) || type == typeof(DateTime) || type == typeof(bool) || type == typeof(byte))
            {
                return true;
            }
            else if (type == typeof(decimal) || type == typeof(float) || type == typeof(double) || type == typeof(int) || type == typeof(long) || type == typeof(short))
            {
                return true;
            }
            else if (type == typeof(byte))
            {
                return true;
            }

            return false;
        }


        public static object ChangeType(object value, Type ConversionType)
        {
            if (value == null)
            {
                if (ConversionType.IsValueType)
                {
                    return Activator.CreateInstance(ConversionType);
                }
            }

            object result;

            if (ConversionType == typeof(String))
            {
                if (!(value is String))
                {
                    if (value == null)
                    {
                        result = "";
                    }
                    else
                    {
                        result = value.ToString();
                    }
                }
                else
                {
                    result = value;
                }
            }
            else if (ConversionType == typeof(Guid))
            {
                Guid id;
                if (Guid.TryParse(value.ToString(), out id))
                {
                    return id;
                }
                return default(Guid);
            }
            else if (ConversionType == typeof(bool))
            {
                bool ok;
                if (bool.TryParse(value.ToString(), out ok))
                {
                    return ok;
                }
                return false;
            }
            else
            {
                result = Convert.ChangeType(value, ConversionType);
            }

            return result;
        }


        public static T ToObject<T>(IDictionary<string, object> source)
        {
            var result = ToObject(source, typeof(T));
            return (T)result; 
        }

        public static object ToObject(IDictionary<string, object> source, Type ObjectType)
        {
            var result = Activator.CreateInstance(ObjectType); 

            var allproperties = _GetProperties(ObjectType);
            var allfields = _GetFields(ObjectType);

            foreach (var item in source)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }

                var lowerkey = item.Key.ToLower();

                var member = allproperties.Find(o => o.Name.ToLower() == lowerkey);

                if (member != null)
                {
                    var rightvalue = ChangeType(item.Value, member.PropertyType);
                    if (rightvalue != null)
                    {
                        member.SetValue(result, rightvalue);
                    }
                }
                else
                {
                    var field = allfields.Find(o => o.Name.ToLower() == lowerkey);

                    if (field != null)
                    {
                        var rightvalue = ChangeType(item.Value, field.FieldType);
                        if (rightvalue != null)
                        {
                            field.SetValue(result, rightvalue);
                        }
                    }
                }

            }

            return result;

        }

        private static List<PropertyInfo> _GetProperties(Type type)
        {      
            var props = type.GetProperties();
            if (props != null)
            {
                return props.ToList();
            }
            else    
            {
                return new List<PropertyInfo>();
            }   
        }

        private static List<FieldInfo> _GetFields(Type type)
        {
            var props = type.GetFields();
            if (props != null)
            {
                return props.ToList();
            }
            else
            {
                return new List<FieldInfo>(); 
            }
        }


        public static MethodInfo GetMethodInfo(Type type, string methodname)
        {
            var method = type.GetMethod(methodname);
            if (method != null)
            {
                return method;
            }

            var lowername = methodname.ToLower();

            var allmethods = Kooboo.Lib.Reflection.TypeHelper.GetPublicMethods(type);
            foreach (var item in allmethods)
            {
                if (item.Name.ToLower() == lowername)
                {
                    return item;
                }
            }

            return null;
        }

         

    }
}
