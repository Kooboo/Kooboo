//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.IndexedDB.Dynamic;
using Kooboo.Sites.Scripting;
using System.Collections.Generic;
using System.ComponentModel;

namespace KScript
{
    public interface ITable
    {
        [Description(@"Add an object into database table. If the table does not have the fields, the table schema will be automatically updated with columns.
    var obj = {fieldone: ""value one"", fieldtwo: ""value two""};
    k.database.tablename.add(obj);")]
        object add(object value);

        [Description("Return all items")]
        IDynamicTableObject[] all();

        [Description(@"Append an object to database table. This is the same as ""add"" except that it will not update table schema when the object contains fields not defined in the table schema.
var obj = {fieldone: ""value one"", fieldtwo: ""value two""};
        var returnid = k.database.tablename.append(obj);")]
        object append(object value);

        [Description("create additional index")]
        void createIndex(string fieldname);

        [Description(@"Delete an item from database based on id or primary key
 k.database.tablename.delete(key);")]
        void delete(object id);

        [Description(@"Return the first itme that match query condition
       // available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var table = k.database.tablename;
        var items = table.find(""name == 'matchedvalue'""); 
        var items = table.find(""number>=123""); 
        var items = table.find(""number >=123&&name=='matchedvalue'""); 
        var items = table.find(""name contains 'matchedvalue'""); 
        var items = table.find(""name startwith 'matchedvalue'""); ")]
        IDynamicTableObject find(string query);

        [Description(@"return the first item that has field value equal the match value
var item = k.database.tablename.find(""fieldname"", ""matchvalue"");")]
        IDynamicTableObject find(string fieldName, object matchValue);

        [Description(@"Search all items based on query condition
       // available operators: ==,  >=,  >,  <,  <=, contains, startwith 
        var table = k.database.tablename;
        var items = table.findAll(""name == 'matchedvalue'""); 
        var items = table.findAll(""number>=123""); 
        var items = table.findAll(""number >=123&&name=='matchedvalue'""); 
        var items = table.findAll(""name contains 'matchedvalue'""); 
        var items = table.findAll(""name startwith 'matchedvalue'""); ")]
        IDynamicTableObject[] findAll(string query);

        [Description(@"return counter based on the query condition")]
        long Count(string query);
         
        [Description(@"return all items that have the field value equal the match value
var items = k.database.tablename.findAll(""fieldname"", ""matchvalue"");")]
        IDynamicTableObject[] findAll(string field, object value);

        [Description("get an item based on Id or primary key")]
        IDynamicTableObject get(object id);
        ITableQuery Query();

        [Description(@"Return the query object for further operations like paging.
use the same query syntax as find or findAll")]
        ITableQuery Query(string query);

        [Description(@"update an item
var table = k.database.tablename;
var obj = {fieldone: ""value one""};
var id = table.add(obj);   
var item = table.get(id);
        item.fieldone = ""new value"";   
        table.update(item);")]
        void update(object newvalue);

        [Description(@"update an item, key must be the system default Guid key or the key value of primary key field.
var table = k.database.tablename;
var obj = {fieldone: ""value one""};
var id = table.add(obj);   
        obj.fieldone = ""new value"";   
        table.update(id, obj); ")]
        void update(object id, object newvalue);

        [Description("Return editing history of this object")]
        List<ChangeLog> GetLogs(object id);

        [Description("Get object based on the log id")]
        IDynamicTableObject GetByLog(long LogId);
    }
}