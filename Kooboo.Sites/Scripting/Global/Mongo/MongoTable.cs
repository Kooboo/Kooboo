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
            var option = new InsertOneOptions();
            option.BypassDocumentValidation = false;
            return dic["_id"];
        }

        public void createIndex(string fieldname)
        {
            throw new NotImplementedException();
        }

        public void delete(object id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IDynamicTableObject GetByLog(long LogId)
        {
            throw new NotImplementedException();
        }

        public List<ChangeLog> GetLogs(object id)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void update(object id, object newvalue)
        {
            throw new NotImplementedException();
        }
    }
}
