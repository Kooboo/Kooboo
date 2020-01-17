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
    [KValueType(typeof(SqliteTable))]
    public class SqliteDatabase : ISqliteDatabase
    {
        readonly SQLiteConnection _connection;
        readonly ConcurrentDictionary<string, SqliteTable> _tables = new ConcurrentDictionary<string, SqliteTable>();

        public SqliteDatabase(SQLiteConnection connection)
        {
            _connection = connection;
        }

        ~SqliteDatabase()
        {
            _connection.Dispose();
        }

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
            return _tables.GetOrAdd(name, new SqliteTable(_connection, name));
        }

        public IDynamicTableObject[] Query(string sql)
        {
            var data = _connection.Query<object>(sql).ToArray();
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _connection, null);
        }

        public int Execute(string sql)
        {
            return _connection.Execute(sql);
        }
    }
}
