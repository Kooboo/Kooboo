//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using KScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global
{
    public static class kHelper
    {
        public static IDictionary<string, object> CleanDynamicObject(object Value)
        {
            if (Value is IDynamicTableObject)
            {
                var dynamictable = Value as IDynamicTableObject;
                return dynamictable.obj;
            }

            return Value as IDictionary<string, object>;
        }

        public static object PrepareData(object dataobj, Type modelType)
        {
            Dictionary<string, object> data = GetData(dataobj);

            var result = Lib.Reflection.TypeHelper.ToObject(data, modelType);

            return result;
        }
        public static string GetId(string key)
        {
            if (!Guid.TryParse(key, out var guid))
            {
                guid = IndexedDB.Helper.KeyHelper.ComputeGuid(key);
            }

            return guid.ToString();
        }

        public static Dictionary<string, object> GetData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            if (idict != null)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                var dynamicobj = dataobj as IDictionary<string, object>;
                if (dynamicobj != null)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            return data;
        }

    }
}
