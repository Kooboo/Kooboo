//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Dynamic
{
    public static class Accessor
    {
        internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null)
            {
                if (conversionType.IsValueType)
                {
                    return Activator.CreateInstance(conversionType);
                }
                else if (conversionType == typeof(string))
                {
                    return "";
                }
                else
                {
                    return null;
                }
            }

            object result;

            Type valuetype = value.GetType();
            if (valuetype == conversionType)
            {
                return value;
            }

            if (conversionType == typeof(String))
            {
                if (!(value is String))
                {
                    if (value == null)
                    {
                        result = null;
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
                else
                {
                    return Helper.KeyHelper.ComputeGuid(value.ToString());
                }
            }
            else if (conversionType == typeof(bool))
            {
                bool ok;
                if (bool.TryParse(value.ToString(), out ok))
                {
                    return ok;
                }
                return false;
            }
            else if (conversionType == typeof(DateTime))
            {
                if (valuetype == typeof(double))
                {
                    result = Epoch.AddMilliseconds((Double)value);
                }
                else if (valuetype == typeof(decimal) || valuetype == typeof(long))
                {
                    var douvalue = Convert.ToDouble(value);
                    result = Epoch.AddMilliseconds((Double)douvalue);
                }

                return DateTime.Now;
            }
            else
            {
                result = Convert.ChangeType(value, conversionType);
            }

            return result;
        }

        public static T ChangeType<T>(object value)
        {
            var type = typeof(T);
            var newvalue = ChangeType(value, type);
            if (newvalue != null)
            {
                return (T)newvalue;
            }
            return default(T);
        }

        public static object GetValue(IDictionary<string, object> dynamic, string fieldName, Type clrType)
        {
            if (dynamic.ContainsKey(fieldName))
            {
                var itemvalue = dynamic[fieldName];
                if (itemvalue != null)
                {
                    return ChangeType(itemvalue, clrType);
                }
            }
            return null;
        }

        public static object GetValueIDict(IDictionary dynamic, string fieldName, Type clrType)
        {
            if (dynamic.Contains(fieldName))
            {
                var itemvalue = dynamic[fieldName];
                if (itemvalue != null)
                {
                    return ChangeType(itemvalue, clrType);
                }
            }
            return null;
        }

        public static object GetValue(object obj, Type objectType, string fieldName, Type clrType)
        {
            var getter = GetGettter(objectType, fieldName);
            var value = getter?.Invoke(obj);
            if (value != null)
            {
                return ChangeType(value, clrType);
            }
            return null;
        }

        public static Guid _ParseKey(object key)
        {
            if (key is Guid guid)
            {
                return guid;
            }
            string strkey = key.ToString();
            if (System.Guid.TryParse(strkey, out var guidkey))
            {
                return guidkey;
            }
            else
            {
                return Helper.KeyHelper.ComputeGuid(strkey);
            }
        }

        private static object _locker = new object();

        private static Dictionary<string, Func<object, object>> GetValueFuncs { get; set; } = new Dictionary<string, Func<object, object>>();

        private static Func<object, object> GetGettter(Type objectType, string fieldName)
        {
            string key = objectType.FullName + fieldName;

            if (!GetValueFuncs.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!GetValueFuncs.ContainsKey(key))
                    {
                        var func = ObjectHelper.GetGetObjectValue(fieldName, objectType);
                        GetValueFuncs[key] = func;
                    }
                }
            }

            return GetValueFuncs[key];
        }

        private static object _setterLock = new object();

        private static Dictionary<string, Action<object, object>> SetValueActions { get; set; } = new Dictionary<string, Action<object, object>>();

        public static Action<object, object> GetSetter(Type objectType, string fieldName)
        {
            string key = objectType.FullName + fieldName;

            if (!SetValueActions.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!SetValueActions.ContainsKey(key))
                    {
                        var fieldtype = ObjectHelper.GetFieldType(objectType, fieldName);

                        if (fieldtype == null)
                        {
                            SetValueActions[key] = null;
                        }
                        else
                        {
                            var action = ObjectHelper.GetSetObjectValue(fieldName, objectType, fieldtype);
                            SetValueActions[key] = action;
                        }
                    }
                }
            }

            return SetValueActions[key];
        }
    }
}