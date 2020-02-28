using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Scripting.Global;
using Kooboo.Sites.Scripting.Global.RelationalDatabase;
using KScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public abstract class DataBaseModifier : ModifierBase
    {

        public string Path => GetValue("path");
        public string Table => GetValue("table");
        public string NewId => GetValue("new");

        public override void Modify(RenderContext context)
        {
            if (Id == null || Table == null) return;
            var kInstance = new k(context);
            var table = GetTable(kInstance);

            switch (Action)
            {
                case ActionType.update:
                    Update(table);
                    break;
                case ActionType.delete:
                    table.delete(GetRealId(Id));
                    break;
                case ActionType.copy:
                    if (NewId == null) return;
                    var entity = table.get(GetRealId(Id));
                    if (entity == null) return;
                    entity.SetValue("_id", GetRealId(NewId));
                    table.add(entity);
                    break;
                default:
                    break;
            }
        }

        internal abstract ITable GetTable(k kInstance);

        internal virtual object GetRealId(string id)
        {
            return kHelper.GetId(id);
        }

        internal void Update(ITable table)
        {
            var entity = table.get(GetRealId(Id));
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

            SaveUpdate(table, entity);
        }

        internal virtual void SaveUpdate(ITable table, IDynamicTableObject entity)
        {
            table.update(entity);
        }
    }
}
