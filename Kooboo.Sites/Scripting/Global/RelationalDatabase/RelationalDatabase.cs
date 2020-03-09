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
    public abstract class RelationalDatabase<TExecuter, TSchema, TConnection> : IDatabase
        where TExecuter : SqlExecuter<TConnection>
        where TSchema : RelationalSchema
        where TConnection : IDbConnection
    {
        internal readonly ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema, TConnection>> _tables;

        public RelationalDatabase(string connectionString)
        {
            _tables = new ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema, TConnection>>();
            SqlExecuter = (TExecuter)Activator.CreateInstance(typeof(TExecuter), connectionString);
        }

        public abstract string Source { get; }

        [KIgnore]
        public SqlExecuter<TConnection> SqlExecuter { get; }

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
            return _tables.GetOrAdd(name, new RelationalTable<TExecuter, TSchema, TConnection>(name, this));
        }

        public IDynamicTableObject[] Query(string sql)
        {
            using (var connection = SqlExecuter.CreateConnection())
            {
                var data = connection.Query<object>(sql).ToArray();
                return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), null);
            }
        }

        public int Execute(string sql)
        {
            using (var connection = SqlExecuter.CreateConnection())
            {
                return connection.Execute(sql);
            }
        }
    }
}
