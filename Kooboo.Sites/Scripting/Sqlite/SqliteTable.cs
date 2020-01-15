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
        readonly static object _locker = new object();
        SqliteSchema _schema;

        public SqliteTable(SQLiteConnection connection, string name)
        {
            _connection = connection;
            _name = name;
        }

        public object Add(object value)
        {
            EnsureTableCreated();
            TryUpgradeSchema(value);
            EnsureHaveId(value);
            _connection.Insert(_name, value);
            return value;
        }

        public object[] All()
        {
            EnsureTableCreated();
            return _connection.All(_name);
        }

        public object Append(object value)
        {
            EnsureTableCreated();
            EnsureSchemaCompatible(value);
            EnsureHaveId(value);
            _connection.Append(_name, value, _schema);
            return value;
        }

        void TryUpgradeSchema(object value)
        {
            lock (_locker)
            {
                var newItems = EnsureSchemaCompatible(value);
                if (newItems.Count() > 0)
                {
                    _schema.AddItems(newItems);
                    _connection.UpgradeSchema(_name, newItems);
                }
            }
        }

        IEnumerable<SqliteSchema.Item> EnsureSchemaCompatible(object value)
        {
            var newSchema = new SqliteSchema(value as IDictionary<string, object>);
            var compatible = _schema.Compatible(newSchema, out var newItems);
            if (!compatible) throw new SqliteSchemaNotCompatibleException();
            return newItems;
        }

        void EnsureTableCreated()
        {
            lock (_locker)
            {
                if (_schema == null) _schema = SqliteConnectionExtensions.GetSchema(_connection, _name);
                if (!_schema.Created) _connection.CreateTable(_name);
            }
        }

        object EnsureHaveId(object value)
        {
            var dic = value as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) dic.Add("_id", Guid.NewGuid().ToString());
            return value;
        }
    }
}
