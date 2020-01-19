using Dapper;
using Kooboo.Data;
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using KScript;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace KScript
{
    public class SqliteDatabase : ISqliteDatabase
    {
        readonly ConcurrentDictionary<string, SqliteTable> _tables = new ConcurrentDictionary<string, SqliteTable>();

        public SQLiteConnection Connection { get; }

        public SqliteDatabase(SQLiteConnection connection)
        {
            Connection = connection;
        }

        ~SqliteDatabase()
        {
            Connection.Dispose();
        }

        public ITable this[string key]
        {
            get
            {
                return GetTable(key);
            }
        }

        public ITable GetTable(string name)
        {
            return _tables.GetOrAdd(name, new SqliteTable(name, this));
        }

        public IDynamicTableObject[] Query(string sql)
        {
            var data = Connection.Query<object>(sql).ToArray();
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), null);
        }

        public int Execute(string sql)
        {
            return Connection.Execute(sql);
        }
    }
}
