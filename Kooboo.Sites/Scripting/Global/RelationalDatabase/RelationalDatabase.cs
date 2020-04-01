using Dapper;
using Kooboo.Data.Attributes;
using Kooboo.Sites.Scripting.Interfaces;
using KScript;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public abstract class RelationalDatabase<TExecuter, TSchema, TConnection> : IRelationalDatabase
        where TExecuter : SqlExecuter<TConnection>
        where TSchema : RelationalSchema
        where TConnection : IDbConnection
    {
        internal readonly ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema, TConnection>> _tables;

        protected RelationalDatabase(string connectionString)
        {
            _tables = new ConcurrentDictionary<string, RelationalTable<TExecuter, TSchema, TConnection>>();
            SqlExecuter = (TExecuter)Activator.CreateInstance(typeof(TExecuter), connectionString);
        }

        public abstract string Source { get; }

        [KIgnore]
        public ISqlExecuter SqlExecuter { get; }

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

        public IDynamicTableObject[] Query(string sql) => Query(sql, null);

        public IDynamicTableObject[] Query(string sql, object param = null)
        {
            using (var connection = SqlExecuter.CreateConnection())
            {
                var data = connection.Query<object>(sql, param).ToArray();
                return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), null);
            }
        }

        public int Execute(string sql) => Execute(sql, null);

        public int Execute(string sql, object param = null)
        {
            using (var connection = SqlExecuter.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var affectedRows = connection.Execute(sql, param, transaction);
                    transaction.Commit();
                    return affectedRows;
                }
            }
        }

        public object Procedure(string sql) => Procedure(sql, null);
        public object Procedure(string sql, object param = null)
        {
            using (var connection = SqlExecuter.CreateConnection())
            {
                connection.Open();
                return connection.Query(sql, param, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
