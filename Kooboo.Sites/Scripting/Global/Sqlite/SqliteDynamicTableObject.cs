using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.Scripting.Helper;
using KScript;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace KScript
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public class SqliteDynamicTableObject : DynamicTableObjectBase, IDynamicTableObject
    {
        readonly SQLiteConnection _connection;
        readonly string _tableName;

        public SqliteDynamicTableObject(IDictionary<string, object> orgObj, SQLiteConnection connection, string tableName)
        {
            this.obj = orgObj;
            _connection = connection;
            _tableName = tableName;
        }

        internal override object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                if (obj[key] is long) return (long)obj[key] == 1;
                return obj[key];
            }

            var relation = _connection.GetRelation(key, _tableName);

            if (relation != default && _tableName != default && obj.ContainsKey(relation.To))
            {
                var data = _connection.QueryData(key, $"{relation.From} == {obj[relation.To]}").Take(999);
                return CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _connection, key);
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, SQLiteConnection connection, string tableName)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], connection, tableName);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item, SQLiteConnection connection, string tableName)
        {
            if (item != null)
            {
                return new SqliteDynamicTableObject(item, connection, tableName);
            }
            return null;

        }
    }
}
