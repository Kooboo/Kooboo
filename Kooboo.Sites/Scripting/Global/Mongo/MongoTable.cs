using Kooboo.Data.Attributes;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.IndexedDB.Query;
using Kooboo.Sites.Scripting;
using Kooboo.Sites.Scripting.Global;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KScript
{
    public class MongoTable : ITable
    {
        [KIgnore]
        public IMongoCollection<object> MongoCollection { get; }

        public MongoTable(IMongoCollection<object> mongoCollection)
        {
            MongoCollection = mongoCollection;
        }

        public object add(object value)
        {
            value = kHelper.CleanDynamicObject(value);
            var dic = value as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) dic["_id"] = ObjectId.GenerateNewId();
            MongoCollection.InsertOne(value);
            return dic["_id"];
        }

        public IDynamicTableObject[] all()
        {
            var data = MongoCollection.AsQueryable().ToArray();
            var list = data.Select(s => s as IDictionary<string, object>).ToArray();
            return MongoDynamicTableObject.CreateList(list, MongoCollection.CollectionNamespace.CollectionName);
        }

        public object append(object value)
        {
            value = kHelper.CleanDynamicObject(value);
            var existFields = GetAllField();

            var dic = value as IDictionary<string, object>;
            var removedKeys = dic.Where(w => !existFields.Contains(w.Key)).Select(s => s.Key).ToArray();

            foreach (var item in removedKeys)
            {
                dic.Remove(item);
            }

            if (!dic.ContainsKey("_id")) dic["_id"] = ObjectId.GenerateNewId();
            MongoCollection.InsertOne(value);
            return dic["_id"];
        }

        [KIgnore]
        public IEnumerable<object> GetAllField()
        {
            var option = new MapReduceOptions<object, object>();
            option.OutputOptions = MapReduceOutputOptions.Inline;
            return MongoCollection.MapReduce(
                            "function() { for (var key in this) { emit(key, 1); } }",
                            "function(key, values) { return Array.sum(values); }",
                            option)
                            .ToList()
                            .Select(s =>
                            {
                                var obj = s as IDictionary<string, object>;
                                return obj["_id"];
                            });
        }

        public void createIndex(string fieldname)
        {
            var keys = new IndexKeysDefinitionBuilder<object>().Ascending(fieldname);
            var model = new CreateIndexModel<object>(keys);
            MongoCollection.Indexes.CreateOne(model);
        }

        public void delete(object id)
        {
            var filter = GetIdFilter(id);
            MongoCollection.DeleteOne(filter);
        }

        public static FilterDefinition<object> GetIdFilter(object id)
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
            var filter = QueryToFilter(query);
            var data = MongoCollection.Find(filter).FirstOrDefault();
            return MongoDynamicTableObject.Create(data as IDictionary<string, object>, MongoCollection.CollectionNamespace.CollectionName);
        }

        public static FilterDefinition<object> QueryToFilter(string query)
        {
            var conditions = QueryPraser.ParseConditoin(query);
            var filters = new List<FilterDefinition<object>>();
            foreach (var condition in conditions)
            {
                var filter = ConditionToFilter(condition);
                filters.Add(filter);
            }
            return Builders<object>.Filter.And(filters);
        }

        public static FilterDefinition<object> ConditionToFilter(ConditionItem condition)
        {
            switch (condition.Comparer)
            {
                case Comparer.Contains:
                    var containsReg = new Regex($"{Regex.Escape(condition.Value)}");
                    return Builders<object>.Filter.Regex(condition.Field, new BsonRegularExpression(containsReg));
                case Comparer.EqualTo:
                    return GetEqFilter(condition);
                case Comparer.GreaterThan:
                    return Builders<object>.Filter.Gt(condition.Field, double.Parse(condition.Value));
                case Comparer.GreaterThanOrEqual:
                    return Builders<object>.Filter.Gte(condition.Field, double.Parse(condition.Value));
                case Comparer.LessThan:
                    return Builders<object>.Filter.Lt(condition.Field, double.Parse(condition.Value));
                case Comparer.LessThanOrEqual:
                    return Builders<object>.Filter.Lte(condition.Field, double.Parse(condition.Value));
                case Comparer.NotEqualTo:
                    var eqFilter = GetEqFilter(condition);
                    return Builders<object>.Filter.Not(eqFilter);
                case Comparer.StartWith:
                    var startWithReg = new Regex($"^{Regex.Escape(condition.Value)}");
                    return Builders<object>.Filter.Regex(condition.Field, new BsonRegularExpression(startWithReg));
                default:
                    throw new NotSupportedException();
            }
        }

        private static FilterDefinition<object> GetEqFilter(ConditionItem condition)
        {
            var filters = new List<FilterDefinition<object>>();
            filters.Add(Builders<object>.Filter.Eq(condition.Field, condition.Value));

            if (bool.TryParse(condition.Value, out var boolean))
            {
                filters.Add(Builders<object>.Filter.Eq(condition.Field, boolean));
            }

            if (double.TryParse(condition.Value, out var number))
            {
                filters.Add(Builders<object>.Filter.Eq(condition.Field, number));
            }

            return Builders<object>.Filter.Or(filters);
        }

        public IDynamicTableObject find(string fieldName, object matchValue)
        {
            return find($"{fieldName} == {matchValue}");
        }

        public IDynamicTableObject[] findAll(string query)
        {
            var filter = QueryToFilter(query);
            var data = MongoCollection.Find(filter).ToList();
            var list = data.Select(s => s as IDictionary<string, object>).ToArray();
            return MongoDynamicTableObject.CreateList(list, MongoCollection.CollectionNamespace.CollectionName);
        }

        public IDynamicTableObject[] findAll(string field, object value)
        {
            return findAll($"{field} == {value}");
        }

        public IDynamicTableObject get(object id)
        {
            var filter = GetIdFilter(id);
            var data = MongoCollection.Find(filter).FirstOrDefault();
            return MongoDynamicTableObject.Create(data as IDictionary<string, object>, MongoCollection.CollectionNamespace.CollectionName);
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
            return new MongoTableQuery(this);
        }

        public ITableQuery Query(string query)
        {
            return Query().Where(query);
        }

        public void update(object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
            var dic = newvalue as IDictionary<string, object>;
            if (!dic.ContainsKey("_id")) add(newvalue);
            else
            {
                update(dic["_id"], newvalue);
            }
        }

        public void update(object id, object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
            var dic = newvalue as IDictionary<string, object>;
            if (dic.ContainsKey("_id")) dic.Remove("_id");
            if (dic.Count > 0)
            {
                var first = dic.First();
                var update = new UpdateDefinitionBuilder<object>().Set(first.Key, first.Value);

                foreach (var item in dic.Skip(1))
                {
                    update = update.Set(item.Key, item.Value);
                }

                MongoCollection.FindOneAndUpdate(GetIdFilter(id), update);
            }
        }

        public long Count(string query)
        {
            return this.Query(query).count();  
        }
    }
}
