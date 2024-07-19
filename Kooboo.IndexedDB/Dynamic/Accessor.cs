//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Dynamic
{
    public static class Accessor
    {
        internal static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static object ChangeType(object value, Type ConversionType)
        {
            if (value == null)
            {
                if (ConversionType.IsValueType)
                {
                    return Activator.CreateInstance(ConversionType);
                }
                else if (ConversionType == typeof(string))
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
            if (valuetype == ConversionType)
            {
                return value;
            }

            if (ConversionType == typeof(String))
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
            else if (ConversionType == typeof(Guid))
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
            else if (ConversionType == typeof(bool))
            {
                bool ok;
                if (bool.TryParse(value.ToString(), out ok))
                {
                    return ok;
                }
                return false;
            }
            else if (ConversionType == typeof(DateTime))
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
                result = Convert.ChangeType(value, ConversionType);
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

        public static object GetValue(IDictionary<string, object> Dynamic, string FieldName, Type ClrType)
        {
            if (Dynamic.ContainsKey(FieldName))
            {
                var itemvalue = Dynamic[FieldName];
                if (itemvalue != null)
                {
                    return ChangeType(itemvalue, ClrType);
                }
            }
            return null;
        }

        public static object GetValueIDict(IDictionary Dynamic, string FieldName, Type ClrType)
        {
            if (Dynamic.Contains(FieldName))
            {
                var itemvalue = Dynamic[FieldName];
                if (itemvalue != null)
                {
                    return ChangeType(itemvalue, ClrType);
                }
            }
            return null;
        }


        public static object GetValue(object obj, Type objectType, string FieldName, Type clrType)
        {
            var getter = GetGettter(objectType, FieldName);
            if (getter != null)
            {
                var value = getter(obj);
                if (value != null)
                {
                    return ChangeType(value, clrType);
                }
            }
            return null;
        }

        public static Guid _ParseKey(object key)
        {
            if (key is System.Guid)
            {
                return (Guid)key;
            }
            string strkey = key.ToString();
            Guid guidkey;
            if (System.Guid.TryParse(strkey, out guidkey))
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

        private static Func<object, object> GetGettter(Type objectType, string FieldName)
        {
            string key = objectType.FullName + FieldName;

            if (!GetValueFuncs.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!GetValueFuncs.ContainsKey(key))
                    {
                        var func = ObjectHelper.GetGetObjectValue(FieldName, objectType);
                        GetValueFuncs[key] = func;
                    }
                }
            }

            return GetValueFuncs[key];
        }

        private static object _SetterLock = new object();

        private static Dictionary<string, Action<object, object>> SetValueActions { get; set; } = new Dictionary<string, Action<object, object>>();

        public static Action<object, object> GetSetter(Type objectType, string FieldName)
        {
            string key = objectType.FullName + FieldName;

            if (!SetValueActions.ContainsKey(key))
            {
                lock (_locker)
                {
                    if (!SetValueActions.ContainsKey(key))
                    {
                        var fieldtype = ObjectHelper.GetFieldType(objectType, FieldName);

                        if (fieldtype == null)
                        {
                            SetValueActions[key] = null;
                        }
                        else
                        {
                            var action = ObjectHelper.GetSetObjectValue(FieldName, objectType, fieldtype);
                            SetValueActions[key] = action;
                        }
                    }
                }
            }

            return SetValueActions[key];

        }

    }
}
