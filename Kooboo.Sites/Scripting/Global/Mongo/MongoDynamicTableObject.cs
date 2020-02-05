using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global.Database;
using KScript;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global.Mongo
{
    public class MongoDynamicTableObject : DynamicTableObjectBase
    {

        public MongoDynamicTableObject(IDictionary<string, object> orgObj)
        {
            obj = orgObj;
        }

        internal override object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key];
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i]);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item)
        {
            if (item != null)
            {
                return new MongoDynamicTableObject(item);
            }
            return null;
        }
    }
}
