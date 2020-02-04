using KScript;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mongo
{
    public class MongoTable : ITable
    {
        private readonly IMongoCollection<object> _mongoCollection;

        public MongoTable(IMongoCollection<object> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public object add(object value)
        {
            var dic = value as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) dic["_id"] = ObjectId.GenerateNewId();
            _mongoCollection.InsertOne(value);
            return dic["_id"];
        }

        public IDynamicTableObject[] all()
        {
            var data = _mongoCollection.AsQueryable().ToArray();
            var list = data.Select(s => s as IDictionary<string, object>).ToArray();
            return MongoDynamicTableObject.CreateList(list);
        }

        public object append(object value)
        {
            var dic = value as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) dic["_id"] = ObjectId.GenerateNewId();
            _mongoCollection.InsertOne(value);
            return dic["_id"];
        }

        public void createIndex(string fieldname)
        {
            var keys = new IndexKeysDefinitionBuilder<object>().Ascending(fieldname);
            var model = new CreateIndexModel<object>(keys);
            _mongoCollection.Indexes.CreateOne(model);
        }

        public void delete(object id)
        {
            var filter = GetIdFilter(id);
            _mongoCollection.DeleteOne(filter);
        }

        private static FilterDefinition<object> GetIdFilter(object id)
        {
            object objectId;

            if (id is ObjectId)
            {
                objectId = id;
            }
            else
            {
                objectId = ObjectId.Parse(id.ToString());
            }

            var filter = Builders<object>.Filter.Eq("_id", objectId);
            return filter;
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
            var filter = GetIdFilter(id);
            var data = _mongoCollection.Find(filter).FirstOrDefault();
            return MongoDynamicTableObject.Create(data as IDictionary<string, object>);
        }

        public IDynamicTableObject GetByLog(long LogId)
        {
            return null;
        }

        public List<ChangeLog> GetLogs(object id)
        {
            return null;
        }

        public ITableQuery Query()
        {
            throw new NotImplementedException();
        }

        public ITableQuery Query(string query)
        {
            throw new NotImplementedException();
        }

        public void update(object newvalue)
        {
            var dic = newvalue as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) add(newvalue);
            else
            {
                update(dic["_id"], newvalue);
            }
        }

        public void update(object id, object newvalue)
        {
            var dic = newvalue as IDictionary<string, object>;
            if (dic.ContainsKey("_id")) dic.Remove("_id");
            _mongoCollection.FindOneAndReplace(GetIdFilter(id), dic);
        }
    }
}
