using Kooboo.Sites.Scripting.Global.Database;
using KScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationalDynamicTableObject<TExecuter, TSchema> : DynamicTableObjectBase where TExecuter : SqlExecuter where TSchema : RelationalSchema
    {
        readonly RelationalTable<TExecuter, TSchema> _table;

        RelationalDynamicTableObject(IDictionary<string, object> orgObj, RelationalTable<TExecuter, TSchema> table)
        {
            this.obj = orgObj;
            _table = table;
        }

        internal override object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key];
            }

            var relation = _table.Database.SqlExecuter.GetRelation(key, _table.Name);

            if (relation != default && _table.Name != default && obj.ContainsKey(relation.To))
            {
                var data = _table.Database.SqlExecuter.QueryData(key, $"{relation.From} == {obj[relation.To]}").Take(999);
                return CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _table.Database.GetTable(key) as RelationalTable<TExecuter, TSchema>);
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, RelationalTable<TExecuter, TSchema> table)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], table);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item, RelationalTable<TExecuter, TSchema> table)
        {
            if (item != null)
            {
                return new RelationalDynamicTableObject<TExecuter, TSchema>(item, table);
            }
            return null;

        }
    }
}
