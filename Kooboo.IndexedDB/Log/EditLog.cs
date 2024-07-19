//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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
            string storeName = GlobalSettings.EditLogUniqueName;

            ObjectStoreParameters paras = new ObjectStoreParameters();
            paras.EnableLog = false;
            paras.EnableVersion = false;
            paras.AddIndex<LogEntry>(o => o.KeyHash);
            paras.AddColumn<LogEntry>(o => o.StoreNameHash);
            paras.AddColumn<LogEntry>(o => o.TableNameHash);
            paras.AddColumn<LogEntry>(o => o.TimeTick);
            paras.SetPrimaryKeyField<LogEntry>(o => o.Id);
            return db.GetOrCreateObjectStore<Int64, LogEntry>(storeName, paras);
        }

        public EditLog(Database db)
        {
            this.Store = GetLog(db);
            Int64 initId = this.Store.LastKey;
            this.LogKey = "editlog" + db.AbsolutePath;
            SetLogId(initId);
        }

        /// <summary>
        /// get the new database log entry id. 
        /// </summary>
        /// <param name="databasename"></param>
        /// <returns></returns>
        public long GetNewLogId()
        {
            return Config.SequenceId.GetNewLongId(this.LogKey);
        }

        public void SetLogId(long id)
        {
            Config.SequenceId.SetLong(this.LogKey, id);
        }

        public void Add(LogEntry entry)
        {
            if (entry.Id == default(Int64))
            {
                entry.Id = GetNewLogId();
            }
            this.Store.add(entry.Id, entry);
            this.Store.Close();
        }

        public void DelSelf()
        {
            this.Store.DelSelf();
            this.SetLogId(0);
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
            return filter.Take(take);
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

        public List<LogEntry> GetByTableName(string tableName, int take, int skip = 0, bool ascending = false)
        {
            int namehash = tableName.GetHashCode32();
            if (ascending)
            {
                return this.Store.Where(o => o.TableNameHash == namehash).OrderByAscending().Skip(skip).Take(take);
            }
            else
            {
                return this.Store.Where(o => o.TableNameHash == namehash).OrderByDescending().Skip(skip).Take(take);
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

        public List<LogEntry> GetByTableNameAndKey(string TableName, Guid Id, int take, int skip = 0, bool ascending = false)
        {
            var keys = ObjectContainer.GuidConverter.ToByte(Id);
            return GetByTableNameAndKey(TableName, keys, take, skip, ascending);
        }

        public List<LogEntry> GetByTableNameAndKey(string TableName, byte[] Keys, int take, int skip = 0, bool ascending = false)
        {
            Guid HashKey = LogEntry.ToHashGuid(Keys);
            int nameHash = TableName.GetHashCode32();
            if (ascending)
            {
                return this.Store.Where(o => o.TableNameHash == nameHash && o.KeyHash == HashKey).OrderByAscending().Skip(skip).Take(take);
            }
            else
            {
                return this.Store.Where(o => o.TableNameHash == nameHash && o.KeyHash == HashKey).OrderByDescending().Skip(skip).Take(take);
            }
        }

        public LogEntry GetLastLogByTableNameAndKey(string TableName, Guid Id)
        {
            var keys = ObjectContainer.GuidConverter.ToByte(Id);
            return GetLastLogByTableNameAndKey(TableName, keys);
        }

        public LogEntry GetLastLogByTableNameAndKey(string TableName, byte[] Keys)
        {
            Guid HashKey = LogEntry.ToHashGuid(Keys);
            int nameHash = TableName.GetHashCode32();
            return this.Store.Where(o => o.TableNameHash == nameHash && o.KeyHash == HashKey).OrderByDescending().FirstOrDefault();
        }

        public LogEntry GetPreviousTableLog(LogEntry current)
        {
            Guid HashKey = LogEntry.ToHashGuid(current.KeyBytes);
            return this.Store.Where(o => o.TableNameHash == current.TableNameHash && o.KeyHash == HashKey && o.Id < current.Id).OrderByDescending().FirstOrDefault();
        }

        public long GetJustDeletedVersion(string StoreName, byte[] key)
        {
            int namehash = StoreName.GetHashCode32();
            Guid HashKey = LogEntry.ToHashGuid(key);

            var log = this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().FirstOrDefault();

            if (log != null && log.EditType == EditType.Delete)
            {
                return log.Id;
            }

            var last = this.Store.Where(o => o.StoreNameHash == namehash && o.KeyHash == HashKey).OrderByDescending().Take(10);

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
