using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB
{
    public class EditLog
    {
        public ObjectStore<long, LogEntry> Store;

        private string LogKey { get; set; }
 
        private static ObjectStore<long, LogEntry> GetLog(Database db)
        {
                string storename = GlobalSettings.EditLogUniqueName;
    
                ObjectStoreParameters paras = new ObjectStoreParameters();
                paras.EnableLog = false;
                paras.EnableVersion = false;
                paras.AddIndex<LogEntry>(o => o.KeyHash);
                paras.AddColumn<LogEntry>(o => o.StoreNameHash);
                paras.AddColumn<LogEntry>(o => o.TimeTick);
                paras.SetPrimaryKeyField<LogEntry>(o => o.Id); 
                return db.GetOrCreateObjectStore<Int64, LogEntry>(storename, paras);
          
        }

        public EditLog(Database db)
        {
            this.Store = GetLog(db);
            Int64 initid = this.Store.LastKey;
            this.LogKey = "editlog" +  db.AbsolutePath;
            SetLogId(this.Store.OwnerDatabase.Name, initid);  
        }


        /// <summary>
        /// get the new database log entry id. 
        /// </summary>
        /// <param name="databasename"></param>
        /// <returns></returns>
        public  long GetNewLogId(string databasename)
        { 
            return Config.SequenceId.GetNewLongId(this.LogKey);
        }

        public   void SetLogId(string databasename, long id)
        { 
            Config.SequenceId.SetLong(this.LogKey, id);
        }
         
        public void Add(LogEntry entry)
        {
            if (entry.Id == default(Int64))
            {
                entry.Id = GetNewLogId(this.Store.OwnerDatabase.Name); 
            }
            this.Store.add(entry.Id, entry);  
        }

        public void DelSelf()
        {
            this.Store.DelSelf();
            this.SetLogId(this.Store.OwnerDatabase.Name, 0); 
        }

        public List<LogEntry> List(int take, int skip = 0, bool ascending = false)
        {
            var filter = Store.Where();
            if (!ascending)
            {
                filter.OrderByDescending(); 
            }

            if (skip > 0)
            {
                filter.Skip(skip); 
            }
            return  filter.Take(take); 
        }

        public LogEntry Get(long VersionId)
        {
            return this.Store.get(VersionId); 
        }

        public List<LogEntry> GetByStoreName(string StoreName, int take, int skip = 0, bool ascending = false)
        {
            int namehash = StoreName.GetHashCode32(); 
            if (ascending)
            {
                return this.Store.Where(o => o.StoreNameHash == namehash).OrderByAscending().Skip(skip).Take(take); 
            }
            else
            {
                return this.Store.Where(o => o.StoreNameHash == namehash).OrderByDescending().Skip(skip).Take(take); 
            }            
        }
         
        public List<LogEntry> GetByStoreName(string StoreName)
        {
            int namehash = StoreName.GetHashCode32(); 
            return this.Store.Where(o => o.StoreNameHash == namehash).SelectAll(); 
        }

        public void DeleteByStoreName(string StoreName)
        {
            int namehash = StoreName.GetHashCode32();
            var allitems = this.Store.Where(o => o.StoreNameHash == namehash).SelectAll();
            foreach (var item in allitems)
            {
                this.Store.delete(item.Id); 
            }
        }

        public List<LogEntry> GetByStoreNameAndKey(string StoreName, byte[] Keys, int take, int skip = 0, bool ascending = false)
        {
            Guid HashKey = LogEntry.ToHashGuid(Keys);
            int namehash = StoreName.GetHashCode32(); 
            if (ascending)
            {
                return this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByAscending().Skip(skip).Take(take);
            }
            else
            {
                return this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().Skip(skip).Take(take);
            } 
        }

        public LogEntry GetLastLogByStoreNameAndKey(string StoreName, byte[] Keys)
        {
            Guid HashKey = LogEntry.ToHashGuid(Keys);
            int namehash = StoreName.GetHashCode32();

            return this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().FirstOrDefault(); 
        }

        public long GetJustDeletedVersion(string StoreName,  byte[] key)
        { 
            int namehash = StoreName.GetHashCode32();
            Guid HashKey = LogEntry.ToHashGuid(key);

           var log = this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().FirstOrDefault();

            if (log !=null && log.EditType == EditType.Delete)
            {
                return log.Id; 
            }
             
            var last =  this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().Take(10);

            foreach (var item in last)
            {
                if (item.EditType == EditType.Delete)
                {
                    return item.Id; 
                }
            } 
          
            return -1;
        }


        public IEnumerable<LogEntry> GetCollection(bool ascending)
        {
            return this.Store.ItemCollection(ascending);
        }

        public IEnumerable<LogEntry> GetCollection(bool ascending, int skip, int take)
        {
            return this.Store.ItemCollection(ascending).Skip(skip).Take(take);
        }

    }
      
}
