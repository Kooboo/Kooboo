//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Reflection
{
    public static class Dynamic
    {
        private static object _lock = new object();

        public static object GetObjectMember(object obj, string fullPropertyName)
        {
            if (obj == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(fullPropertyName))
            {
                return obj;
            }

            var dest = obj;

            var subProperties = fullPropertyName.Split('.');

            foreach (var item in subProperties)
            {
                if (dest == null)
                {
                    return null;
                }
                Type objtype = dest.GetType();

                string key = objtype.Name + "." + item;

                if (!GetValueFuncs.ContainsKey(key))
                {
                    lock (_lock)
                    {
                        if (!GetValueFuncs.ContainsKey(key))
                        {
                            var getValueFunc = Reflection.TypeHelper.GetGetObjectValue(item, objtype);
                            GetValueFuncs[key] = getValueFunc;
                        }
                    }
                }

                var function = GetValueFuncs[key];

                if (function == null)
                {
                    return null;
                }
                else
                {
                    dest = function(dest);
                }
            }

            return dest;
        }

        private static Dictionary<string, Func<object, object>> GetValueFuncs = new Dictionary<string, Func<object, object>>();
    }
}