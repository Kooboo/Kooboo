using Kooboo.Sites.Scripting.Global;
using KScript;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace KScript
{
    public class SqliteTable : ITable
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

        public object add(object value)
        {
            value = Helper.CleanDynamicObject(value);
            EnsureTableCreated();
            TryUpgradeSchema(value);
            var newId = Guid.NewGuid().ToString();
            EnsureHaveId(value, newId);
            _connection.Insert(_name, value);
            return newId;
        }

        public IDynamicTableObject[] all()
        {
            EnsureTableCreated();
            var data = _connection.All(_name);
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), _connection, _name);
        }

        public object append(object value)
        {
            value = Helper.CleanDynamicObject(value);
            EnsureTableCreated();
            EnsureSchemaCompatible(value);
            var newId = Guid.NewGuid().ToString();
            EnsureHaveId(value, newId);
            _connection.Append(_name, value, _schema);
            return newId;
        }

        public void createIndex(string fieldname)
        {
            EnsureTableCreated();
            _connection.CreateIndex(_name, fieldname);
        }

        public void delete(object id)
        {
            EnsureTableCreated();
            _connection.Delete(_name, id.ToString());
        }

        public IDynamicTableObject find(string query)
        {
            throw new NotImplementedException();
        }

        public IDynamicTableObject find(string fieldName, object matchValue)
        {
            throw new NotImplementedException();
        }

        public IDynamicTableObject[] findAll(string query)
        {
            throw new NotImplementedException();
        }

        public IDynamicTableObject[] findAll(string field, object value)
        {
            throw new NotImplementedException();
        }

        public IDynamicTableObject get(object id)
        {
            EnsureTableCreated();
            var data = _connection.Get(_name, id.ToString());
            return SqliteDynamicTableObject.Create(data as IDictionary<string, object>, _connection, _name);
        }

        public TableQuery Query()
        {
            throw new NotImplementedException();
        }

        public TableQuery Query(string query)
        {
            throw new NotImplementedException();
        }

        public void update(object newvalue)
        {
            var dic = newvalue as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) update(dic["_id"], newvalue);
            else add(newvalue);
        }

        public void update(object id, object newvalue)
        {
            EnsureTableCreated();
            TryUpgradeSchema(newvalue);
            EnsureHaveId(newvalue, id.ToString());
            _connection.UpdateData(_name, id.ToString(), newvalue);
        }

    }
}
