using KScript;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Sqlite
{
    public class SqliteTableQuery : ITableQuery
    {
        readonly SQLiteConnection _connection;
        readonly string _tableName;

        public bool Ascending { get; set; }
        public string OrderByField { get; set; }
        public string SearchCondition { get; set; }
        public int skipcount { get; set; }

        public SqliteTableQuery(SQLiteConnection connection, string tableName)
        {
            _connection = connection;
            _tableName = tableName;
        }

        public int count()
        {
            return _connection.Count(_tableName, SearchCondition, null, skipcount);
        }

        public ITableQuery OrderBy(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = true;
            return this;
        }

        public ITableQuery OrderByDescending(string fieldname)
        {
            this.OrderByField = fieldname;
            this.Ascending = false;
            return this;
        }

        public ITableQuery skip(int skip)
        {
            this.skipcount = skip;
            return this;
        }

        public IDynamicTableObject[] take(int count)
        {
            var desc = Ascending ? string.Empty : "DESC";
            var orderBy = OrderByField == null ? null : $"{OrderByField} {desc}";
            var data = _connection.QueryData(_tableName, SearchCondition, count, skipcount, orderBy);
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _connection, _tableName);
        }

        public ITableQuery Where(string searchCondition)
        {
            this.SearchCondition = searchCondition;
            return this;
        }
    }
}
