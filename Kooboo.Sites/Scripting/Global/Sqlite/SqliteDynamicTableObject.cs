using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting.Helper;
using KScript;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace KScript
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonConverterDynamicObject))]
    public class SqliteDynamicTableObject : IDynamicTableObject
    {
        [KIgnore]
        public IDictionary<string, object> obj { get; set; }
        readonly SQLiteConnection _connection;
        readonly string _tableName;


        public Dictionary<string, object> Values
        {
            get
            {
                return this.obj.ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public SqliteDynamicTableObject(IDictionary<string, object> orgObj, SQLiteConnection connection, string tableName)
        {
            this.obj = orgObj;
            _connection = connection;
            _tableName = tableName;
        }

        [KIgnore]
        public object this[string key]
        {
            get
            {
                return GetValueFromDict(key);
            }
            set
            {
                this.obj[key] = value;
            }
        }

        private object GetValueFromDict(string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key];
            }

            var relation = _connection.GetRelation(_tableName, key);

            if (relation != default)
            {
                
            }

            return null;
        }

        [KIgnore]
        public static IDynamicTableObject[] CreateList(IDictionary<string, object>[] list, SQLiteConnection connection, string tableName)
        {
            int len = list.Length;

            IDynamicTableObject[] result = new IDynamicTableObject[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = Create(list[i], connection, tableName);
            }
            return result;
        }

        [KIgnore]
        public static IDynamicTableObject Create(IDictionary<string, object> item, SQLiteConnection connection, string tableName)
        {
            if (item != null)
            {
                return new SqliteDynamicTableObject(item, connection, tableName);
            }
            return null;

        }

        public object GetValue(string FieldName)
        {
            return GetValueFromDict(FieldName);
        }

        public object GetValue(string FieldName, RenderContext Context)
        {
            return GetValueFromDict(FieldName);
        }

        public void SetValue(string FieldName, object Value)
        {
            obj[FieldName] = Value;
        }
    }
}
