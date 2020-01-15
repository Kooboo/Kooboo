using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Kooboo.Sites.Scripting.Sqlite
{
    public class SqliteTable
    {
        readonly SQLiteConnection _connection;
        readonly string _name;
        SqliteSchema _schema;

        public SqliteTable(SQLiteConnection connection, string name)
        {
            _connection = connection;
            _name = name;
        }

        public object Add(object value)
        {
            var dic=value as IDictionary<string,object>;
            if (!dic.ContainsKey("_id")) dic.Add("_id", Guid.NewGuid().ToString());
            TryCreateOrUpdateTable(value);

            return value;
        }

        void TryCreateOrUpdateTable(object value)
        {
            lock (_name)
            {
                if (_schema == null) _schema = SqliteConnectionExtensions.GetSchema(_connection, _name);
                if (!_schema.Created) _connection.CreateTable(_name);
                var newSchema = new SqliteSchema(value as IDictionary<string, object>);
                var compatible = _schema.Compatible(newSchema, out var newItems);
                if (!compatible) throw new SqliteSchemaNotCompatibleException();
                if (newItems.Count > 0) {
                    _schema.AddItems(newItems);
                    _connection.UpgradeSchema(_name, newItems);
                }
            }
        }
    }
}
