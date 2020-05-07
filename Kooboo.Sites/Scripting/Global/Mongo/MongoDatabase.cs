using Kooboo.Data.Attributes;
using Kooboo.Lib.Helper;
using KScript;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KScript
{
    public class MongoDatabase : IDatabase
    {
        readonly IMongoDatabase _database;

        public MongoDatabase(IMongoDatabase database)
        {
            _database = database;
        }

        [KIgnore]
        public IEnumerable<string> Tables => _database.ListCollectionNames().ToList();

        [KIgnore]
        public ITable this[string key] => GetTable(key);

        public ITable GetTable(string Name)
        {
            return new MongoTable(_database.GetCollection<object>(Name));
        }

        [Description(@"
about mongo command you can see 
https://docs.mongodb.com/manual/reference/command/

For example:

// insert
k.mongo.runCommand({
    insert:'user',
    documents:[
        {name:'jobs',age:23}
    ]
})

// find
var result= k.mongo.runCommand({
   find:'user'
});
k.response.write(result);
")]
        [KDefineType(Params = new[] { typeof(CommandObject) }, Return = typeof(CommandResult))]
        public object RunCommand(object commandObject)
        {
            var jsonCommand = new JsonCommand<object>(JsonHelper.Serialize(commandObject));
            return _database.RunCommand(jsonCommand);
        }
    }
}
