using Kooboo.Lib.Helper;
using KScript;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Global.Mongo
{
    public class MongoDatabase : IDatabase
    {
        readonly IMongoDatabase _database;

        public MongoDatabase(IMongoDatabase database)
        {
            _database = database;
        }
        public ITable this[string key] => GetTable(key);

        public ITable GetTable(string Name)
        {
            return new MongoTable(_database.GetCollection<object>(Name));
        }
    }
}
