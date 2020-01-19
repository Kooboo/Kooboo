using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.Sqlite;
using KScript;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace KScript
{
    public class SqliteTable : ITable
    {
        readonly static object _locker = new object();
        SqliteSchema _schema;

        public string Name { get; set; }
        public SqliteDatabase Database { get; set; }

        public SqliteTable(string name, SqliteDatabase database)
        {
            Database = database;
            Name = name;
        }

        void TryUpgradeSchema(object value)
        {
            lock (_locker)
            {
                var newItems = EnsureSchemaCompatible(value);
                if (newItems.Count() > 0)
                {
                    _schema.AddItems(newItems);
                    Database.Connection.UpgradeSchema(Name, newItems);
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
                if (_schema == null) _schema = SqliteConnectionExtensions.GetSchema(Database.Connection, Name);
                if (!_schema.Created) Database.Connection.CreateTable(Name);
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
            value = kHelper.CleanDynamicObject(value);
            EnsureTableCreated();
            TryUpgradeSchema(value);
            var newId = Guid.NewGuid().ToString();
            EnsureHaveId(value, newId);
            Database.Connection.Insert(Name, value);
            return newId;
        }

        public IDynamicTableObject[] all()
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name);
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public object append(object value)
        {
            value = kHelper.CleanDynamicObject(value);
            EnsureTableCreated();
            EnsureSchemaCompatible(value);
            var newId = Guid.NewGuid().ToString();
            EnsureHaveId(value, newId);
            Database.Connection.Append(Name, value, _schema);
            return newId;
        }

        public void createIndex(string fieldname)
        {
            EnsureTableCreated();
            Database.Connection.CreateIndex(Name, fieldname);
        }

        public void delete(object id)
        {
            EnsureTableCreated();
            Database.Connection.Delete(Name, id.ToString());
        }

        public IDynamicTableObject find(string query)
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name, query).FirstOrDefault();
            return SqliteDynamicTableObject.Create(data as IDictionary<string, object>, this);
        }

        public IDynamicTableObject find(string fieldName, object matchValue)
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name, $"{fieldName} == '{matchValue}'").FirstOrDefault();
            return SqliteDynamicTableObject.Create(data as IDictionary<string, object>, this);
        }

        public IDynamicTableObject[] findAll(string query)
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name, query);
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public IDynamicTableObject[] findAll(string field, object value)
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name, $"{field} == '{value}'");
            return SqliteDynamicTableObject.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public IDynamicTableObject get(object id)
        {
            EnsureTableCreated();
            var data = Database.Connection.QueryData(Name, $"_id == '{id}'").FirstOrDefault();
            return SqliteDynamicTableObject.Create(data as IDictionary<string, object>, this);
        }

        public ITableQuery Query()
        {
            return new SqliteTableQuery(this);
        }

        public ITableQuery Query(string query)
        {
            var result = new SqliteTableQuery(this);
            result.Where(query);
            return result;
        }

        public void update(object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
            var dic = newvalue as IDictionary<string, object>;
            if (dic.ContainsKey("_id")) update(dic["_id"], newvalue);
            else add(newvalue);
        }

        public void update(object id, object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
            var dic = newvalue as IDictionary<string, object>;
            if (dic.ContainsKey("_id")) dic.Remove("_id");
            EnsureTableCreated();
            TryUpgradeSchema(newvalue);
            Database.Connection.UpdateData(Name, id.ToString(), newvalue);
        }

    }
}
