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

        public SqliteDatabase(RenderContext renderContext)
        {
            var path = Path.Combine(AppSettings.GetFileIORoot(renderContext.WebSite), "sqlite.db");
            _connection = new SQLiteConnection($"Data source='{path}';Version=3");
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

        public object[] Query(string sql)
        {
            return _connection.Query<object>(sql).ToArray();
        }

        public int Execute(string sql)
        {
            return _connection.Execute(sql);
        }
    }
}
