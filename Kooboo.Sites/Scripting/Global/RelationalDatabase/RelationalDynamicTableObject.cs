using Kooboo.Sites.Scripting.Global.Database;
using KScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationalDynamicTableObject<TExecuter, TSchema, TConnection> : DynamicTableObjectBase
        where TExecuter : SqlExecuter<TConnection>
        where TSchema : RelationalSchema
        where TConnection : IDbConnection
    {
        readonly RelationalTable<TExecuter, TSchema, TConnection> _table;

        public override string Source => _table.Database.Source;

        RelationalDynamicTableObject(IDictionary<string, object> orgObj, RelationalTable<TExecuter, TSchema, TConnection> table)
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

            if (relation != null && _table.Name != null && obj.ContainsKey(relation.To))
            {
                var data = _table.Database.SqlExecuter.QueryData(relation.TableA, $"{relation.From} == {obj[relation.To]}").Take(999);
                return CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _table.Database.GetTable(relation.TableA) as RelationalTable<TExecuter, TSchema, TConnection>);
            }

            return null;
        }

        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, RelationalTable<TExecuter, TSchema, TConnection> table)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], table);
            }
            return result;
        }

        public static IDynamicTableObject Create(IDictionary<string, object> item, RelationalTable<TExecuter, TSchema, TConnection> table)
        {
            if (item != null)
            {
                return new RelationalDynamicTableObject<TExecuter, TSchema, TConnection>(item, table);
            }
            return null;

        }

        public override IDictionary<string, string> GetTraceInfo()
        {
            return new Dictionary<string, string>
            {
                { "id", obj["_id"].ToString() },
                { "table", _table.Name }
            };
        }
    }
}
