//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting.Global;
using System;
using System.ComponentModel;

namespace KScript
{
    public class KTable : ITable
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
            value = kHelper.CleanDynamicObject(value);
            var result = this.table.Add(value, true);
            if (result == default(Guid))
            {
                return null;
            }
            return result;
        }

        public object append(object value)
        {
            value = kHelper.CleanDynamicObject(value);
            var result = this.table.Add(value, false);
            if (result == default(Guid))
            {
                return null;
            }
            return result;
        }

        public void update(object id, object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
            this.table.Update(id, newvalue);
        }

        public void update(object newvalue)
        {
            newvalue = kHelper.CleanDynamicObject(newvalue);
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

        public IDynamicTableObject find(string query)
        {
            var result = this.table.Query.Find(query);
            return DynamicTableObject.Create(result, this.table, this.context);
        }

        public IDynamicTableObject find(string fieldName, object matchValue)
        {
            var obj = this.table.Query.Where(fieldName, Kooboo.IndexedDB.Query.Comparer.EqualTo, matchValue).FirstOrDefault();
            return DynamicTableObject.Create(obj, this.table, this.context);
        }

        public IDynamicTableObject[] findAll(string field, object value)
        {
            var list = this.table.Query.Where(field, Kooboo.IndexedDB.Query.Comparer.EqualTo, value).SelectAll();
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }

        public IDynamicTableObject[] findAll(string query)
        {
            var list = this.table.Query.FindAll(query);
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }

        public IDynamicTableObject get(object id)
        {
            var obj = this.table.Get(id);
            return DynamicTableObject.Create(obj, this.table, this.context);
        }

        public ITableQuery Query()
        {
            return new TableQuery(this);
        }

        public ITableQuery Query(string query)
        {
            var result = new TableQuery(this);
            result.Where(query);
            return result;
        }

        public void createIndex(string fieldname)
        {
            this.table.CreateIndex(fieldname, false);
        }

        public IDynamicTableObject[] all()
        {
            var all = this.table.All();
            return DynamicTableObject.CreateList(all.ToArray(), this.table, this.context);
        }

    }
}
