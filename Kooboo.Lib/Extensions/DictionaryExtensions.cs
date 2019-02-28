//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
        {
            if (dic == null)
            {
                return defaultValue;
            }
            TValue value;
            if (dic.TryGetValue(key, out value) && value != null)
            {
                return value;
            }

            return defaultValue;
        }

        public static string GetValue(this IDictionary<string, string> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key]; 
            }
            return string.Empty; 
        }
   
        public static bool ContainsKeyIgnoreCase(this Dictionary<string, string> dict, string key)
        {
            if (key == null)
            {
                return false; 
            }

            key = key.ToLower(); 

            foreach (var item in dict)
            {
                if (item.Key.ToLower() == key)
                {
                    return true; 
                }
            }

            return false; 
        }

    }
}
