//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Data
{
    public static class TypeCache
    {
        private static readonly object Writelock = new object();

        private static Dictionary<string, Type> _typelist = new Dictionary<string, Type>();

        private static Dictionary<string, Action<object, object>> _setValues = new Dictionary<string, Action<object, object>>();

        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            lock (Writelock)
            {
                if (_typelist.TryGetValue(typeName, out var type))
                {
                    return type;
                }

                type = Lib.Reflection.TypeHelper.GetType(typeName);
                if (type != null)
                {
                    _typelist.Add(typeName, type);
                    return type;
                }

                //if (TypeName == "Kooboo.Sites.DataSources.FilterDefinition")
                //{
                //    return typeof(Kooboo.Data.Definition.Filter);
                //}
                return null;
            }
        }

        public static Action<object, object> GetSetValue(string fullTypeName, Type type, string fieldOrProperty)
        {
            string uniquekey = fullTypeName + fieldOrProperty;
            Action<object, object> setValue = null;

            if (_setValues.ContainsKey(uniquekey))
            {
                return _setValues[uniquekey];
            }

            var propertylist = Lib.Reflection.TypeHelper.GetPublicFieldOrProperties(type);

            if (propertylist.TryGetValue(fieldOrProperty, out var fieldtype))
            {
                setValue = Lib.Reflection.TypeHelper.GetSetObjectValue(fieldOrProperty, type, fieldtype);
            }

            _setValues[uniquekey] = setValue;

            return setValue;
        }
    }
}