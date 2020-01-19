using Kooboo.Sites.Scripting.Global.Database;
using Kooboo.Sites.Scripting.Helper;
using System.Collections.Generic;
using System.Linq;

namespace KScript
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public class SqliteDynamicTableObject : DynamicTableObjectBase, IDynamicTableObject
    {
        readonly SqliteTable _table;

        SqliteDynamicTableObject(IDictionary<string, object> orgObj, SqliteTable table)
        {
            this.obj = orgObj;
            _table = table;
        }

        internal override object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                if (obj[key] is long) return (long)obj[key] == 1;
                return obj[key];
            }

            var relation = _table.Database.Connection.GetRelation(key, _table.Name);

            if (relation != default && _table.Name != default && obj.ContainsKey(relation.To))
            {
                var data = _table.Database.Connection.QueryData(key, $"{relation.From} == {obj[relation.To]}").Take(999);
                return CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _table.Database.GetTable(key) as SqliteTable);
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, SqliteTable table)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], table);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item, SqliteTable table)
        {
            if (item != null)
            {
                return new SqliteDynamicTableObject(item, table);
            }
            return null;

        }
    }
}
