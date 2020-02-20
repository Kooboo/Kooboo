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
    public class MongoModifier : ModifierBase
    {
        public override string Source => "mongo";

        public string Path => GetValue("path");
        public string Table => GetValue("table");

        public string NewId => GetValue("new");

        public override void Modify(RenderContext context)
        {
            if (Id == null) return;
            var kInstance = new k(context);
            var table = kInstance.Mongo.GetTable(Table) as MongoTable;

            switch (Action)
            {
                case ActionType.update:
                    Update(table);
                    break;
                case ActionType.delete:
                    table.delete(Id);
                    break;
                case ActionType.copy:
                    if (NewId == null) return;
                    var entity = table.get(Id);
                    if (entity == null) return;
                    entity.SetValue("_id", ObjectId.Parse(NewId));
                    table.add(entity);
                    break;
                default:
                    break;
            }
        }

        private void Update(MongoTable table)
        {
            var entity = table.get(Id);
            if (entity == null) return;
            var stacks = Path.Split('.');
            object obj = entity;

            foreach (var item in stacks.Take(stacks.Length - 1))
            {
                if (obj == null) return;

                if (obj is IDynamic @dynamic)
                {
                    obj = dynamic.GetValue(item);
                }
                else if (obj is IDictionary<string, object> dic)
                {
                    if (dic.TryGetValue(item, out var value))
                    {
                        obj = value;
                    }
                    else return;
                }
                else return;
            }

            var field = stacks.Last();

            if (obj is IDynamic finalDynamic)
            {
                finalDynamic.SetValue(field, Value);
            }
            else if (obj is IDictionary<string, object> finalDic)
            {
                if (finalDic.ContainsKey(field))
                {
                    finalDic[field] = Value;
                }
                else return;
            }

            table.MongoCollection.ReplaceOne(MongoTable.GetIdFilter(Id), entity.obj);
        }
    }
}
