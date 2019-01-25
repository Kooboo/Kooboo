//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data
{
    public static class TypeCache
    {

        private static readonly object writelock = new object();

        private static Dictionary<string, Type> typelist = new Dictionary<string, Type>();

        private static Dictionary<string, Action<object, object>> SetValues = new Dictionary<string, Action<object, object>>();

        public static Type GetType(string TypeName)
        {
            if (string.IsNullOrEmpty(TypeName))
            {
                return null;
            }
            Type type;
            lock (writelock)
            {
                if (typelist.TryGetValue(TypeName, out type))
                {
                    return type;
                }
                else
                {
                    type = Lib.Reflection.TypeHelper.GetType(TypeName);
                    if (type != null)
                    {
                        typelist.Add(TypeName, type);
                        return type; 
                    }

                    //if (TypeName == "Kooboo.Sites.DataSources.FilterDefinition")
                    //{
                    //    return typeof(Kooboo.Data.Definition.Filter); 
                    //} 
                    return type;

                }
            }
        }

        public static Action<object, object> GetSetValue(string FullTypeName, Type type, string FieldOrProperty)
        {
            string uniquekey = FullTypeName + FieldOrProperty;
            Action<object, object> SetValue = null;

            if (SetValues.ContainsKey(uniquekey))
            {
                return SetValues[uniquekey];
            }

            var propertylist = Lib.Reflection.TypeHelper.GetPublicFieldOrProperties(type);

            Type fieldtype;

            if (propertylist.TryGetValue(FieldOrProperty, out fieldtype))
            {
                SetValue = Lib.Reflection.TypeHelper.GetSetObjectValue(FieldOrProperty, type, fieldtype);
            }

            SetValues[uniquekey] = SetValue;

            return SetValue;
        }
    }
}
