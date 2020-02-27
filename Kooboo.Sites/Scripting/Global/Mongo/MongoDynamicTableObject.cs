using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global.Database;
using KScript;
using System;
using System.Collections.Generic;

namespace KScript
{
    public class MongoDynamicTableObject : DynamicTableObjectBase
    {
        public override string Source => "mongo";
        readonly string _collection;

        public MongoDynamicTableObject(IDictionary<string, object> orgObj, string collection)
        {
            obj = orgObj;
            _collection = collection;
        }

        internal override object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key];
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, string collection)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], collection);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item, string collection)
        {
            if (item != null)
            {
                return new MongoDynamicTableObject(item, collection);
            }
            return null;
        }

        public override IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>
            {
                { "id", obj["_id"].ToString() },
                { "table", _collection }
            };
        }
    }
}
