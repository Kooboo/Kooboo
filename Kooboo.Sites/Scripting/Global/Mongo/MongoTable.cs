using KScript;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mongo
{
    public class MongoTable : ITable
    {
        private IMongoCollection<object> mongoCollection;

        public MongoTable(IMongoCollection<object> mongoCollection)
        {
            this.mongoCollection = mongoCollection;
        }

        public object add(object value)
        {
            mongoCollection.InsertOne(value);
            return value;
        }

        public IDynamicTableObject[] all()
        {
            throw new NotImplementedException();
        }

        public object append(object value)
        {
            throw new NotImplementedException();
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
