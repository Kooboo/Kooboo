//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using System;

namespace Kooboo.Sites.Scripting.Global
{
    public class KTable
    {
        public Table table { get; set; }

        public RenderContext context { get; set; }

        public KTable(Table table, RenderContext context)
        {
            this.table = table;
            this.context = context;
        }

        public KTable()
        { }

        public object add(object value)
        {
            value = CleanDynamicObject(value);
            var result = this.table.Add(value, true);
            if (result == default(Guid))
            {
                return null;
            }
            return result;
        }

        public object append(object value)
        {
            value = CleanDynamicObject(value);
            var result = this.table.Add(value, false);
            if (result == default(Guid))
            {
                return null;
            }
            return result;
        }

        public void update(object id, object newvalue)
        {
            newvalue = CleanDynamicObject(newvalue);
            this.table.Update(id, newvalue);
        }

        public void update(object newvalue)
        {
            newvalue = CleanDynamicObject(newvalue);
            var newdata = this.table.PrepareData(newvalue);
            if (newdata.ContainsKey("_id"))
            {
                var key = newdata["_id"];
                this.update(key, newvalue);
            }
            else
            {
                this.table.UpdateOrAdd(newvalue);
            }
        }

        public void delete(object id)
        {
            this.table.Delete(id);
        }

        private object CleanDynamicObject(object Value)
        {
            if (Value is DynamicTableObject)
            {
                var dynamictable = Value as DynamicTableObject;
                return dynamictable.obj;
            }
            return Value;
        }

        public DynamicTableObject find(string searchCondition)
        {
            var result = this.table.Query.Find(searchCondition);
            return DynamicTableObject.Create(result, this.table, this.context);
        }

        public DynamicTableObject find(string field, object value)
        {
            var obj = this.table.Query.Where(field, IndexedDB.Query.Comparer.EqualTo, value).FirstOrDefault();
            return DynamicTableObject.Create(obj, this.table, this.context);
        }

        public DynamicTableObject[] findAll(string field, object value)
        {
            var list = this.table.Query.Where(field, IndexedDB.Query.Comparer.EqualTo, value).SelectAll();
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }

        public DynamicTableObject[] findAll(string condition)
        {
            var list = this.table.Query.FindAll(condition);
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }

        public DynamicTableObject get(object id)
        {
            var obj = this.table.Get(id);
            return DynamicTableObject.Create(obj, this.table, this.context);
        }

        public TableQuery Query()
        {
            return new TableQuery(this);
        }

        public TableQuery Query(string searchCondition)
        {
            var result = new TableQuery(this);
            result.Where(searchCondition);
            return result;
        }


        public void createIndex(string fieldname)
        {
            this.table.CreateIndex(fieldname, false);
        }

        public DynamicTableObject[] all()
        {
            var all = this.table.All();
            return DynamicTableObject.CreateList(all.ToArray(), this.table, this.context);
        }

    }
}
