using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using KScript;
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

        public override void Modify(RenderContext context)
        {
            var kInstance = new k(context);
            var table = kInstance.Mongo.GetTable(Table) as MongoTable;
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
