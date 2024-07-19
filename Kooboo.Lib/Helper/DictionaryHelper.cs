//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public static class DictionaryHelper
    {

        public static string GetString(Dictionary<string, string> dict, string key)
        {
            if (dict == null)
            {
                return null;
            }

            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            string lower = key.ToLower();
            foreach (var item in dict)
            {
                if (item.Key.ToLower() == lower)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static T GetValue<T>(Dictionary<string, string> dict, string key)
        {
            if (dict == null)
            {
                return default(T);
            }

            if (dict.ContainsKey(key))
            {
                return DataTypeHelper.ConvertType<T>(dict[key]);
            }

            string lower = key.ToLower();
            foreach (var item in dict)
            {
                if (item.Key.ToLower() == lower)
                {
                    return DataTypeHelper.ConvertType<T>(item.Value);
                }
            }
            return default(T);
        }


        public static string GetString(Dictionary<string, object> dict, string key)
        {
            if (dict == null)
            {
                return null;
            }

            if (dict.ContainsKey(key))
            {
                var value = dict[key];
                if (value != null)
                {
                    return value.ToString();
                }
            }

            string lower = key.ToLower();
            foreach (var item in dict)
            {
                if (item.Key.ToLower() == lower)
                {
                    if (item.Value != null)
                    {
                        return item.Value.ToString();
                    }
                }
            }
            return null;
        }

        public static T GetValue<T>(Dictionary<string, object> dict, string key)
        {
            if (dict == null)
            {
                return default(T);
            }

            if (dict.ContainsKey(key))
            {
                var item = dict[key];
                if (item != null)
                {
                    return DataTypeHelper.ConvertType<T>(item.ToString());
                }

            }

            string lower = key.ToLower();
            foreach (var item in dict)
            {
                if (item.Key.ToLower() == lower)
                {
                    if (item.Value != null)
                    {
                        return DataTypeHelper.ConvertType<T>(item.Value.ToString());
                    }

                }
            }
            return default(T);
        }



    }
}
