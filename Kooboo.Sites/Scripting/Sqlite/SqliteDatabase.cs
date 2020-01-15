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

namespace Kooboo.Sites.Scripting.Sqlite
{
    [KValueType(typeof(SqliteTable))]
    public class SqliteDatabase
    {
        readonly SQLiteConnection _connection;
        readonly RenderContext _renderContext;
        readonly ConcurrentDictionary<string, SqliteTable> _tables = new ConcurrentDictionary<string, SqliteTable>();

        public SqliteDatabase(RenderContext renderContext)
        {
            _renderContext = renderContext;
            var path = Path.Combine(AppSettings.GetFileIORoot(renderContext.WebSite), "sqlite.db");
            _connection = new SQLiteConnection($"Data source='{path}';Version=3");
        }

        [KIgnore]
        public SqliteTable this[string key]
        {
            get
            {
                return GetTable(key);
            }
        }

        public SqliteTable GetTable(string name)
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
