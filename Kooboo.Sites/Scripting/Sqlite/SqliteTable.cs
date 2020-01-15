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

        public object Get(object id)
        {
            EnsureTableCreated();
            return _connection.Get(_name, id.ToString());
        }

        public object Append(object value)
        {
            EnsureTableCreated();
            EnsureSchemaCompatible(value);
            EnsureHaveId(value);
            _connection.Append(_name, value, _schema);
            return value;
        }

        public void CreateIndex(string fieldname)
        {
            EnsureTableCreated();
            _connection.CreateIndex(_name, fieldname);
        }

        public void Update(object id, object newvalue)
        {
            EnsureTableCreated();
            TryUpgradeSchema(newvalue);
            EnsureHaveId(newvalue, id.ToString());
            _connection.UpdateData(_name, id.ToString(), newvalue);
        }

        public void Update(object newvalue)
        {
            var dic = newvalue as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) Update(dic["_id"], newvalue);
            else Add(newvalue);
        }

        public void Delete(object id)
        {
            EnsureTableCreated();
            _connection.Delete(_name, id.ToString());
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

        object EnsureHaveId(object value, string id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            var dic = value as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) dic.Add("_id", id);
            return value;
        }
    }
}
