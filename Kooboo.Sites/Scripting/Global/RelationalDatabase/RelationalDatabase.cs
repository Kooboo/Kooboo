using Dapper;
using Kooboo.Data.Attributes;
using KScript;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationalDatabase<TExecuter, TSchema> : IDatabase where TExecuter : SqlExecuter where TSchema : RelationalSchema
    {
        internal readonly ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema>> _tables;

        public RelationalDatabase(IDbConnection dbConnection)
        {
            _tables = new ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema>>();
            SqlExecuter = (TExecuter)Activator.CreateInstance(typeof(TExecuter), dbConnection);
        }

        [KIgnore]
        public SqlExecuter SqlExecuter { get; }

        [KIgnore]
        public ITable this[string key]
        {
            get
            {
                return GetTable(key);
            }
        }

        public ITable GetTable(string name)
        {
            return _tables.GetOrAdd(name, new RelationalTable<TExecuter, TSchema>(name, this));
        }

        public IDynamicTableObject[] Query(string sql)
        {
            var data = SqlExecuter.Connection.Query<object>(sql).ToArray();
            return RelationalDynamicTableObject<TExecuter, TSchema>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), null);
        }

        public int Execute(string sql)
        {
            return SqlExecuter.Connection.Execute(sql);
        }
    }
}
