using Kooboo.IndexedDB.Dynamic;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global
{
  public  class KTable
    {
        internal Table table { get; set; }

        public KTable(Table table)
        {
            this.table = table;  
        }

        public KTable()
        {   }

        public object add(object value)
        {
          return  this.table.Add(value, true); 
        } 

        public object append(object value)
        {
            return this.table.Add(value, false); 
        } 

        public void delete(object id)
        {
            this.table.Delete(id); 
        }

        public IDictionary<string, object> find(string searchCondition)
        { 
            return this.table.Query.Find(searchCondition);  
        }

        public IDictionary<string, object> find(string field, object value)
        {
            return this.table.Query.Where(field, IndexedDB.Query.Comparer.EqualTo, value).FirstOrDefault(); 
        }

        public IDictionary<string, object>[] findAll(string field, object value)
        {
            var list= this.table.Query.Where(field, IndexedDB.Query.Comparer.EqualTo, value).SelectAll();
            return list.ToArray();
        }
         
        public IDictionary<string, object>[] findAll(string condition)
        {
            var list= this.table.Query.FindAll(condition);
            return list.ToArray();
        }

        public object get(object id)
        {  
            return this.table.Get(id); 
        }
        
        public TableQuery Query()
        {
            return new TableQuery(this); 
        }
         
        public TableQuery Query(string searchCondition)
        {
            var result =  new TableQuery(this);
            result.Where(searchCondition);
            return result; 
        }

        public void update(object id, object newvalue)
        {
            this.table.Update(id, newvalue);  
        }

        public void update(object newvalue)
        {
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

        public void createIndex(string fieldname)
        {
              this.table.CreateIndex(fieldname, false);  
        }
         
        public IDictionary<string, object>[] all()
        {
            var all= this.table.All();
            return all.ToArray();
        }

    }
}
