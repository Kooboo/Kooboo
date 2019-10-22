//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        private static bool IsObjectName(string name)
        {
            return name == "GetType" || name == "ToString" || name == "GetHashCode" || name == "Equals" || name == "ReferenceEquals";
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
            List<Type> result = new List<Type>();

            if (typeinfo == null)
            {
                return null;
            }
            var argus = typeinfo.GetGenericArguments();
            if (argus != null && argus.Length > 0)
            {
                result.Add(argus[0]);
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
                    result.Add(argus[0]);
                }
            }

            var allinterfaces = typeinfo.GetInterfaces();

            foreach (var item in allinterfaces)
            {
                argus = item.GetGenericArguments();
                if (argus != null && argus.Length > 0)
                {
                    result.Add(argus[0]);
                }
            }

            return result;
        }

        public static MethodInfo GetRightMethodInfo(List<MethodInfo> methodList, string methodName, Guid methodHash)
        {
            var methods = methodList.FindAll(o => o.Name == methodName);
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
                    if (TypeHelper.GetMethodSignatureHash(item) == methodHash)
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
        /// <param name="objectType"></param>
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

        public static List<MemberInfo> GetPublicPropertyOrFields(Type classType)
        {
            List<MemberInfo> result = new List<MemberInfo>();

            foreach (var item in classType.GetProperties())
            {
                if (item.CanRead && item.CanWrite)
                {
                    result.Add(item);
                }
            }

            foreach (var item in classType.GetFields())
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

            var enumeGenericDefi = typeof(IList<>);

            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType && item.GetGenericTypeDefinition() == enumeGenericDefi)
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

            var enumeGenericDefi = typeof(ICollection<>);
            var enumEnumable = typeof(IEnumerable<>);

            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType)
                {
                    var inter = item.GetGenericTypeDefinition();
                    if (inter == enumeGenericDefi || inter == enumEnumable)
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

        public static bool HasInterface(Type checkType, Type interfaceType)
        {
            foreach (var item in checkType.GetInterfaces())
            {
                if (item == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasGenericInterface(Type checkType, Type genericInterfaceType)
        {
            if (checkType.IsGenericType && checkType.GetGenericTypeDefinition() == genericInterfaceType)
            {
                return true;
            }

            foreach (var item in checkType.GetInterfaces())
            {
                if (item.IsGenericType)
                {
                    var genericdefinition = item.GetGenericTypeDefinition();

                    if (genericdefinition == genericInterfaceType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HasBaseType(Type checkType, Type baseType)
        {
            Type currentbase = checkType.BaseType;
            while (currentbase != null && currentbase != typeof(object))
            {
                if (currentbase == baseType)
                {
                    return true;
                }
                currentbase = currentbase.BaseType;
            }
            return false;
        }

        public static bool IsOfBaseTypeOrInterface(Type checkType, Type baseOrInterface)
        {
            if (HasBaseType(checkType, baseOrInterface))
            {
                return true;
            }

            if (HasInterface(checkType, baseOrInterface))
            {
                return true;
            }

            return checkType == baseOrInterface;
        }

        /// <summary>
        /// Get the datatype within the eg LIST<T>
        /// </summary>
        /// <param name="enumberableType"></param>
        /// <returns></returns>
        public static Type GetEnumberableType(Type enumberableType)
        {
            return enumberableType.GetGenericArguments().Single();
        }

        public static Type GetDictionaryKeyType(Type dictionaryType)
        {
            return dictionaryType.GetGenericArguments()[0];
        }

        public static Type GetDictionaryValueType(Type dictionaryType)
        {
            return dictionaryType.GetGenericArguments()[1];
        }

        public static Type GetFieldType(Type objectType, string fieldName)
        {
            var property = objectType.GetProperty(fieldName);
            if (property != null)
            {
                return property.PropertyType;
            }

            var field = objectType.GetField(fieldName);
            if (field != null)
            {
                return field.FieldType;
            }
            return null;
        }

        public static Func<TValue, TFieldType> GetGetValue<TValue, TFieldType>(string fieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, fieldName);
            return Expression.Lambda<Func<TValue, TFieldType>>(expr, arg).Compile();
        }

        public static Action<TValue, TFieldType> GetSetValue<TValue, TFieldType>(string fieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, fieldName);
            var valueExp = Expression.Parameter(typeof(TFieldType));
            return Expression.Lambda<Action<TValue, TFieldType>>(Expression.Assign(expr, valueExp), arg, valueExp).Compile();
        }

        public static Action<TValue, object> GetSetObjectValue<TValue>(string fieldName, Type fieldtype)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));

            Expression expr = Expression.PropertyOrField(arg, fieldName);

            var objectpara = Expression.Parameter(typeof(object));

            var righttype = Expression.Convert(objectpara, fieldtype);

            var valueExp = Expression.Parameter(fieldtype);

            return Expression.Lambda<Action<TValue, object>>(Expression.Assign(expr, righttype), arg, objectpara).Compile();
        }

        public static Func<TValue, object> GetGetObjectValue<TValue>(string fieldName)
        {
            ParameterExpression arg = Expression.Parameter(typeof(TValue));
            Expression expr = Expression.PropertyOrField(arg, fieldName);
            return Expression.Lambda<Func<TValue, object>>(expr, arg).Compile();
        }

        public static Action<object, object> GetSetObjectValue(string fieldName, Type objectType, Type fieldtype)
        {
            var objectpara = Expression.Parameter(typeof(object));
            var fieldpara = Expression.Parameter(typeof(object));
            var rightObjectType = Expression.Convert(objectpara, objectType);
            Expression expr = Expression.PropertyOrField(rightObjectType, fieldName);
            var righttype = Expression.Convert(fieldpara, fieldtype);
            return Expression.Lambda<Action<object, object>>(Expression.Assign(expr, righttype), objectpara, fieldpara).Compile();
        }

        public static Func<object, object> GetGetObjectValue(string fieldName, Type objecttype)
        {
            ParameterExpression objectPara = Expression.Parameter(typeof(object));

            var expression = Expression.Convert(objectPara, objecttype);

            //Expression expr = Expression.PropertyOrField(expression, FieldName);

            Expression expr;
            if (objecttype.GetField(fieldName) != null)
            {
                expr = Expression.Field(expression, fieldName);
            }
            else
            {
                if (objecttype.GetProperty(fieldName) != null)
                {
                    expr = Expression.Property(expression, fieldName);
                }
                else
                {
                    var name = GetPropertyName(objecttype, fieldName);
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

        public static Action<object, TFieldType> GetSetFieldValue<TFieldType>(string fieldName, Type objectType)
        {
            var objectpara = Expression.Parameter(typeof(object));
            var fieldpara = Expression.Parameter(typeof(TFieldType));
            var rightObjectType = Expression.Convert(objectpara, objectType);
            Expression expr = Expression.PropertyOrField(rightObjectType, fieldName);
            var righttype = Expression.Convert(fieldpara, typeof(TFieldType));
            return Expression.Lambda<Action<object, TFieldType>>(Expression.Assign(expr, righttype), objectpara, fieldpara).Compile();
        }

        public static Func<object, TFieldType> GetGetFieldValue<TFieldType>(string fieldName, Type objecttype)
        {
            ParameterExpression objectPara = Expression.Parameter(typeof(object));
            var expression = Expression.Convert(objectPara, objecttype);
            Expression expr = Expression.PropertyOrField(expression, fieldName);

            return Expression.Lambda<Func<object, TFieldType>>(expr, objectPara).Compile();
        }

        public static Func<object[], object> CompileStaticFunc(Type invokeClassType, string methodName, List<Type> paratypes)
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

            var methodCall = Expression.Call(invokeClassType, methodName, null, castedArgs);

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

        public static Action<object[]> CompileStaticAction(Type invokeClassType, string methodName, List<Type> paratypes)
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

            var methodCall = Expression.Call(invokeClassType, methodName, null, castedArgs);

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

        public static Func<object, object[], object> CompileFunc(Type invokeClassType, string methodName, List<Type> paratypes)
        {
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, invokeClassType);

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

            var methodCall = Expression.Call(castedInstance, methodName, null, castedArgs);

            Expression lambdaBody = Expression.Convert(methodCall, typeof(object));

            return Expression.Lambda<Func<object, object[], object>>(lambdaBody, instanceParam, argsParam).Compile();
        }

        public static Action<object, object[]> CompileAction(Type invokeClassType, string methodName, List<Type> paratypes)
        {
            var argsParam = Expression.Parameter(typeof(object[]));
            var instanceParam = Expression.Parameter(typeof(object));
            var castedInstance = Expression.TypeAs(instanceParam, invokeClassType);

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

            var methodCall = Expression.Call(castedInstance, methodName, null, castedArgs);

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

        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null)
            {
                if (conversionType.IsValueType)
                {
                    return Activator.CreateInstance(conversionType);
                }
            }

            object result;

            if (conversionType == typeof(String))
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
            else if (conversionType == typeof(Guid))
            {
                Guid id;
                if (Guid.TryParse(value.ToString(), out id))
                {
                    return id;
                }
                return default(Guid);
            }
            else if (conversionType == typeof(bool))
            {
                if (bool.TryParse(value.ToString(), out var ok))
                {
                    return ok;
                }
                return false;
            }
            else
            {
                result = Convert.ChangeType(value, conversionType);
            }

            return result;
        }

        public static T ToObject<T>(IDictionary<string, object> source)
        {
            var result = ToObject(source, typeof(T));
            return (T)result;
        }

        public static object ToObject(IDictionary<string, object> source, Type objectType)
        {
            var result = Activator.CreateInstance(objectType);

            var allproperties = _GetProperties(objectType);
            var allfields = _GetFields(objectType);

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