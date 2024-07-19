//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail
{
    public class NullableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDictionary<TKey, TValue>
        where TValue : class
    {
        public NullableDictionary()
            : base()
        {
        }

        public NullableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public new TValue this[TKey key]
        {
            get
            {
                if (this.ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (this.ContainsKey(key))
                {
                    if (value == null)
                    {
                        this.Remove(key);
                    }
                    else
                    {
                        base[key] = value;
                    }
                }
                else if (value != null)
                {
                    this.Add(key, value);
                }
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                this[key] = value;
            }
        }
    }


    public static class DictionaryExtensions
    {
        public static T GetValue<T>(this IDictionary<string, string> dict, string key, T defaultValue = default(T))
        {
            var value = dict[key];
            if (String.IsNullOrEmpty(value))
                return defaultValue;

            var type = typeof(T);
            if (type.IsGenericType
               && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return (T)(value == null ? null : ChangeType(value, type.GetGenericArguments().First()));
            }
            else
            {
                return (T)ChangeType(value, type);
            }
        }

        public static void SetValue<T>(this IDictionary<string, string> dict, string key, T value)
        {
            if ((object)value == null)
            {
                dict[key] = null;
            }
            else
            {
                dict[key] = value.ToString();
            }
        }

        private static object ChangeType(string value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }
    }

}
