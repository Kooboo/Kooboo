//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting.Global;
using KScript;
using System;
using System.ComponentModel;

namespace KScript
{
    public class KTable
    {
        [KIgnore]
        public Table table { get; set; }

        [KIgnore]
        public RenderContext context { get; set; }

        public KTable(Table table, RenderContext context)
        {
            this.table = table;
            this.context = context;
        }

        public KTable()
        { }


        [Description(@"Add an object into database table. If the table does not have the fields, the table schema will be automatically updated with columns.
    var obj = {fieldone: ""value one"", fieldtwo: ""value two""};
    k.database.tablename.add(obj);")]
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

        [Description(@"Append an object to database table. This is the same as ""add"" except that it will not update table schema when the object contains fields not defined in the table schema.
var obj = {fieldone: ""value one"", fieldtwo: ""value two""};
        var returnid = k.database.tablename.append(obj);")]
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

        [Description(@"update an item, key must be the system default Guid key or the key value of primary key field.
var table = k.database.tablename;
var obj = {fieldone: ""value one""};
var id = table.add(obj);   
        obj.fieldone = ""new value"";   
        table.update(id, obj); ")]
        public void update(object id, object newvalue)
        {
            newvalue = CleanDynamicObject(newvalue);
            this.table.Update(id, newvalue);
        }


        [Description(@"update an item
var table = k.database.tablename;
var obj = {fieldone: ""value one""};
var id = table.add(obj);   
var item = table.get(id);
        item.fieldone = ""new value"";   
        table.update(item);")]
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

        [Description(@"Delete an item from database based on id or primary key
 k.database.tablename.delete(key);")]
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

        [Description(@"Return the first itme that match query condition
       // available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var table = k.database.tablename;
        var items = table.find(""name == 'matchedvalue'""); 
        var items = table.find(""number>=123""); 
        var items = table.find(""number >=123&&name=='matchedvalue'""); 
        var items = table.find(""name contains 'matchedvalue'""); 
        var items = table.find(""name startwith 'matchedvalue'""); ")]
        public DynamicTableObject find(string query)
        {
            var result = this.table.Query.Find(query);
            return DynamicTableObject.Create(result, this.table, this.context);
        }

        [Description(@"return the first item that has field value equal the match value
var item = k.database.tablename.find(""fieldname"", ""matchvalue"");")]
        public DynamicTableObject find(string fieldName, object matchValue)
        {
            var obj = this.table.Query.Where(fieldName, Kooboo.IndexedDB.Query.Comparer.EqualTo, matchValue).FirstOrDefault();
            return DynamicTableObject.Create(obj, this.table, this.context);
        }

        [Description(@"return all items that have the field value equal the match value
var items = k.database.tablename.findAll(""fieldname"", ""matchvalue"");")]
        public DynamicTableObject[] findAll(string field, object value)
        {
            var list = this.table.Query.Where(field, Kooboo.IndexedDB.Query.Comparer.EqualTo, value).SelectAll();
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }


        [Description(@"Search all items based on query condition
       // available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var table = k.database.tablename;
        var items = table.findAll(""name == 'matchedvalue'""); 
        var items = table.findAll(""number>=123""); 
        var items = table.findAll(""number >=123&&name=='matchedvalue'""); 
        var items = table.findAll(""name contains 'matchedvalue'""); 
        var items = table.findAll(""name startwith 'matchedvalue'""); ")]
        public DynamicTableObject[] findAll(string query)
        {
            var list = this.table.Query.FindAll(query);
            return DynamicTableObject.CreateList(list.ToArray(), this.table, this.context);
        }

        [Description("get an item based on Id or primary key")]
        public DynamicTableObject get(object id)
        {
            var obj = this.table.Get(id);
            return DynamicTableObject.Create(obj, this.table, this.context);
        }


        public TableQuery Query()
        {
            return new TableQuery(this);
        }

        [Description(@"Return the query object for further operations like paging.
use the same query syntax as find or findAll")]
        public TableQuery Query(string query)
        {
            var result = new TableQuery(this);
            result.Where(query);
            return result;
        }



        [Description("create additional index")]
        public void createIndex(string fieldname)
        {
            this.table.CreateIndex(fieldname, false);
        }


        [Description("Return all items")]
        public DynamicTableObject[] all()
        {
            var all = this.table.All();
            return DynamicTableObject.CreateList(all.ToArray(), this.table, this.context);
        }

    }
}
