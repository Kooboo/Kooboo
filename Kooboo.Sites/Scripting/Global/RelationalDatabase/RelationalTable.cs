using KScript;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kooboo.Sites.Scripting.Global.RelationalDatabase
{
    public class RelationalTable<TExecuter, TSchema, TConnection> : ITable
        where TExecuter : SqlExecuter<TConnection>
        where TSchema : RelationalSchema
        where TConnection : IDbConnection
    {
        readonly static object _locker = new object();
        RelationalSchema _schema;

        public string Name { get; set; }
        public RelationalDatabase<TExecuter, TSchema, TConnection> Database { get; set; }

        public RelationalTable(string name, RelationalDatabase<TExecuter, TSchema, TConnection> database)
        {
            Database = database;
            Name = name;
        }

        void TryUpgradeSchema(IDictionary<string, object> value)
        {
            lock (_locker)
            {
                var newItems = GetNewSchemaItems(value);

                if (newItems.Count() > 0)
                {
                    _schema.AddItems(newItems);
                    Database.SqlExecuter.UpgradeSchema(Name, newItems);
                }
            }
        }

        List<RelationalSchema.Item> GetNewSchemaItems(IDictionary<string, object> value)
        {
            var newSchema = (TSchema)Activator.CreateInstance(typeof(TSchema), value);
            return _schema.Compatible(newSchema);
        }

        void EnsureTableCreated()
        {
            lock (_locker)
            {
                if (_schema == null) _schema = Database.SqlExecuter.GetSchema(Name);

                if (!_schema.Created)
                {
                    Database.SqlExecuter.CreateTable(Name);
                    _schema = Database.SqlExecuter.GetSchema(Name);
                }
            }
        }

        object EnsureHaveId(IDictionary<string, object> value, string id = null)
        {
            if (id == null) id = Guid.NewGuid().ToString();
            if (!value.ContainsKey("_id")) value.Add("_id", id);
            return value;
        }

        /// <summary>
        /// because we can't know null field type
        /// </summary>
        /// <param name="value"></param>
        void ClearNullField(IDictionary<string, object> value)
        {
            foreach (var item in value.Where(w => w.Value == null).Select(s => s.Key).ToArray())
            {
                value.Remove(item);
            }
        }

        public object add(object value)
        {
            var dic = kHelper.CleanDynamicObject(value);
            ClearNullField(dic);
            EnsureTableCreated();
            TryUpgradeSchema(dic);
            object newId = null;

            if (_schema.PrimaryKey == "_id")
            {
                newId = Guid.NewGuid().ToString();
                EnsureHaveId(dic, newId.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(_schema.PrimaryKey) && dic.ContainsKey(_schema.PrimaryKey))
            {
                newId = dic[_schema.PrimaryKey];
            }

            Database.SqlExecuter.Insert(Name, dic);
            return newId;
        }

        public IDynamicTableObject[] all()
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name);
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public object append(object value)
        {
            var dic = kHelper.CleanDynamicObject(value);
            EnsureTableCreated();
            GetNewSchemaItems(dic);
            object newId = null;

            if (_schema.PrimaryKey == "_id")
            {
                newId = Guid.NewGuid().ToString();
                EnsureHaveId(dic, newId.ToString());
            }
            else if (!string.IsNullOrWhiteSpace(_schema.PrimaryKey) && dic.ContainsKey(_schema.PrimaryKey))
            {
                newId = dic[_schema.PrimaryKey];
            }

            Database.SqlExecuter.Append(Name, dic, _schema);
            return newId;
        }

        public void createIndex(string fieldname)
        {
            EnsureTableCreated();
            Database.SqlExecuter.CreateIndex(Name, fieldname);
        }

        public void delete(object id)
        {
            EnsureTableCreated();
            if (_schema.PrimaryKey == "_id") id = kHelper.GetId(id.ToString());
            Database.SqlExecuter.Delete(Name, _schema.PrimaryKey, id);
        }

        public IDynamicTableObject find(string query)
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name, query).FirstOrDefault();
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.Create(data as IDictionary<string, object>, this);
        }

        public IDynamicTableObject find(string fieldName, object matchValue)
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name, $"{fieldName} == '{matchValue}'").FirstOrDefault();
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.Create(data as IDictionary<string, object>, this);
        }

        public IDynamicTableObject[] findAll(string query)
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name, query);
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public IDynamicTableObject[] findAll(string field, object value)
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name, $"{field} == '{value}'");
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.CreateList(data.Select(s => s as IDictionary<string, object>).ToArray(), this);
        }

        public IDynamicTableObject get(object id)
        {
            EnsureTableCreated();
            var data = Database.SqlExecuter.QueryData(Name, $"{_schema.PrimaryKey} == '{id}'").FirstOrDefault();
            return RelationalDynamicTableObject<TExecuter, TSchema, TConnection>.Create(data as IDictionary<string, object>, this);
        }

        public ITableQuery Query()
        {
            return new RelationalTableQuery<TExecuter, TSchema, TConnection>(this);
        }

        public ITableQuery Query(string query)
        {
            var result = new RelationalTableQuery<TExecuter, TSchema, TConnection>(this);
            result.Where(query);
            return result;
        }

        public void update(object newvalue)
        {
            var dic = kHelper.CleanDynamicObject(newvalue);
            if (_schema.PrimaryKey != null && dic.ContainsKey(_schema.PrimaryKey)) update(dic[_schema.PrimaryKey], dic);
            else add(dic);
        }

        public void update(object id, object newvalue)
        {
            var dic = kHelper.CleanDynamicObject(newvalue);
            ClearNullField(dic);
            if (_schema.PrimaryKey != null && dic.ContainsKey(_schema.PrimaryKey)) dic.Remove(_schema.PrimaryKey);
            EnsureTableCreated();
            TryUpgradeSchema(dic);
            if (_schema.PrimaryKey == "_id") id = kHelper.GetId(id.ToString());
            Database.SqlExecuter.UpdateData(Name, _schema.PrimaryKey, id, dic);
        }

        public List<ChangeLog> GetLogs(object id)
        {
            return null;
        }

        public IDynamicTableObject GetByLog(long LogId)
        {
            return null;
        }
    }
}
