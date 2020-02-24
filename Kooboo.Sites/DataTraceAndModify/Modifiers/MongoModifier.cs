using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using KScript;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class MongoModifier : DataBaseModifier
    {
        public override string Source => "mongo";

        internal override ITable GetTable(k kInstance)
        {
            return kInstance.Mongo.GetTable(Table);
        }

        internal override object GetRealId(string id)
        {
            return ObjectId.Parse(id);
        }

        internal override void SaveUpdate(ITable table, IDynamicTableObject entity)
        {
            ((MongoTable)table).MongoCollection.ReplaceOne(MongoTable.GetIdFilter(Id), entity.obj);
        }

    }
}
