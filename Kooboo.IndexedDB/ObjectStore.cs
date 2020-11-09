//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.IndexedDB.ByteConverter;
using Kooboo.IndexedDB.Columns;
using Kooboo.IndexedDB.Indexs;
using Kooboo.IndexedDB.Query;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Kooboo.IndexedDB
{
    public class ObjectStore<TKey, TValue> : IObjectStore
    {
        public Guid CurrentUserId { get; set; }

        public StoreSetting StoreSetting { get; set; }

        private Action<TValue, long> SetVersionNr;

        internal object _Locker = new object();

        public IByteConverter<TKey> KeyConverter { get; set; }
        public IByteConverter<TValue> ValueConverter { get; set; }

        private IEqualityComparer<byte[]> GenericBytesComparer;

        private BlockFile _blockfile;
        private object _lockerBlockFile = new object();
        private BlockFile BlockFile
        {
            get
            {
                if (_blockfile == null)
                {
                    lock (_lockerBlockFile)
                    {
                        if (_blockfile == null)
                        {
                            string blockfileName = System.IO.Path.Combine(this.ObjectFolder, "Data.block");
                            _blockfile = new BlockFile(blockfileName);
                            _blockfile.OpenOrCreate();
                        }
                    }
                }
                return _blockfile;
            }
        }

        internal Btree.BtreeIndex<TKey> primaryIndex { get; set; }

        public List<TKey> Keys
        {
            get
            {
                List<TKey> result = new List<TKey>();

                foreach (var item in this.primaryIndex.AllKeyBytesCollection(true))
                {
                    var key = this.KeyConverter.FromByte(item);
                    result.Add(key);
                }
                return result;
            }
        }

        private bool HasIndex { get; set; }

        public IndexInstanceList<TValue> Indexes { get; set; }

        /// <summary>
        /// string key has varies length, the rest has fixed length.
        /// </summary>
        private bool IsStringKey = false;

        public string ObjectFolder { get; set; }

        internal string StoreSettingFile { get; set; }

        /// <summary>
        /// The name of this object store.
        /// </summary>
        public string Name { get; set; }

        public Database OwnerDatabase { get; set; }

        public TKey FirstKey
        {
            get
            {
                return this.primaryIndex.FirstKey;
            }
        }

        public TKey LastKey
        {
            get
            {
                return this.primaryIndex.LastKey;
            }
        }

        public ObjectStore(string name, Database ownerdatabase, ObjectStoreParameters parameters)
        {
            this.Name = name;
            this.ObjectFolder = ownerdatabase.objectFolder(name);
            this.StoreSettingFile = ownerdatabase.StoreSetitingFile(name);
            this.OwnerDatabase = ownerdatabase;
            this.StoreSetting = Helper.SettingHelper.GetOrSetSetting<TValue, TKey>(this.StoreSettingFile, parameters);
            Init();
        }

        public ObjectStore(string name, Database ownerdatabase, StoreSetting setting)
        {
            this.Name = name;
            this.ObjectFolder = ownerdatabase.objectFolder(name);
            this.StoreSettingFile = ownerdatabase.StoreSetitingFile(name);
            this.OwnerDatabase = ownerdatabase;
            this.StoreSetting = setting;
            Init();
        }


        private void Init()
        {
            this.GenericBytesComparer = new Kooboo.IndexedDB.Btree.Comparer.EqualityComparer(this.StoreSetting.PrimaryKeyLen);

            this.TrySetVersionFunction();

            Type keytype = typeof(TKey);

            Helper.IndexHelper.VerifyIndexType(keytype);

            this.IsStringKey = keytype == typeof(string);

            // StoreVersionUpgrade.Upgrade(this);

            if (KeyConverter != null && ValueConverter != null && this.primaryIndex != null && this.Indexes != null)
            {
                return;
            }

            if (KeyConverter == null)
            {
                KeyConverter = ObjectContainer.GetConverter<TKey>();
            }

            if (ValueConverter == null)
            {
                if (this.StoreSetting.UseDefaultNETBinaryFormater)
                {
                    ValueConverter = ObjectContainer.GetBinaryConverter<TValue>();
                }
                else
                {
                    ValueConverter = ObjectContainer.GetConverter<TValue>(this.SettingColumns);
                }
            }

            this.primaryIndex = new Btree.BtreeIndex<TKey>(this.StoreSetting.PrimaryKey, true, this.StoreSetting.PrimaryKeyLen, Helper.IndexHelper.GetIndexFileName(this.ObjectFolder, this.StoreSetting.PrimaryKey), this.StoreSetting.MaxCacheLevel);

            IndexInstanceList<TValue> indexClassInstance = new IndexInstanceList<TValue>();

            indexClassInstance.ParseSetting(this.StoreSetting.Indexs, this.ObjectFolder, this.StoreSetting.MaxCacheLevel);

            this.Indexes = indexClassInstance;

            if (indexClassInstance.items.Count > 0)
            {
                this.HasIndex = true;
            }
        }

        private Dictionary<string, int> _settingColumns;

        public Dictionary<string, int> SettingColumns
        {
            get
            {
                if (_settingColumns == null)
                {
                    _settingColumns = new Dictionary<string, int>();

                    Dictionary<string, int> setcols = new Dictionary<string, int>();
                    if (this.StoreSetting.Columns != null)
                    {
                        foreach (var item in this.StoreSetting.Columns)
                        {
                            setcols[item.Key] = item.Value;
                        }
                    }
                    _settingColumns = Helper.SettingHelper.GetColumns<TValue>(setcols, this.StoreSetting.PrimaryKey, this.StoreSetting.PrimaryKeyLen);
                }

                return _settingColumns;
            }

        }

        public IColumn<TValue> GetColumn(string ColumnName)
        {
            var koobooconverter = this.ValueConverter as KoobooSimpleConverter<TValue>;
            if (koobooconverter != null)
            {
                return koobooconverter.GetColumn(ColumnName);
            }
            return null;
        }

        public bool CheckSameSetting(ObjectStoreParameters ParametersToCheck)
        {
            var parasetting = Helper.SettingHelper.GetSetting<TValue>(ParametersToCheck);
            return Helper.SettingHelper.IsSameSetting(this.StoreSetting, parasetting);
        }

        /// <summary>
        /// Create an additional index. The primary key index will be automatically created. 
        /// When create a new index, it will trigger a rebuild of that index. 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="unique"></param>
        /// <param name="maxkeylen">For string type of index, you can define a </param>
        private void CreateIndex(string fieldName, bool unique, int maxkeylen = 0)
        {
            lock (_Locker)
            {
                if (this.Indexes.HasIndex(fieldName))
                {
                    throw new Exception("the index " + fieldName + " already exits");
                }

                Type keytype = Helper.TypeHelper.GetFieldType<TValue>(fieldName);

                Helper.IndexHelper.VerifyIndexType(keytype);

                maxkeylen = Helper.KeyHelper.GetKeyLen(keytype, maxkeylen);

                var indexinstance = IndexInstanceList<TValue>.GetIndexInstance(ObjectFolder, fieldName, maxkeylen, this.StoreSetting.MaxCacheLevel);

                try
                {
                    foreach (var item in this.primaryIndex.allItemCollection(true))
                    {
                        var value = this.getValue(item);
                        indexinstance.Add(value, item);
                    }
                }
                catch (Exception)
                {
                    indexinstance.Close();
                    indexinstance.DelSelf();
                    throw new Exception("build index failed, can be uniqueness contraints or others.");
                }

                this.Indexes.items.Add(indexinstance);
                this.StoreSetting.Indexs[fieldName] = maxkeylen;

                Helper.SettingHelper.UpdateSetting(this.StoreSettingFile, this.StoreSetting);
            }
        }

        public void CreateIndex(Expression<Func<TValue, object>> FieldNameExpression, bool unique = false)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldNameExpression);
            CreateIndex(fieldname, unique);
        }

        public void RemoveIndex(Expression<Func<TValue, object>> FieldNameExpression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(FieldNameExpression);

            var index = this.Indexes.getIndex(fieldname);
            if (index != null)
            {
                this.Indexes.items.Remove(index);
                index.DelSelf();
                this.StoreSetting.Indexs.Remove(fieldname);
                Helper.SettingHelper.UpdateSetting(this.StoreSettingFile, this.StoreSetting);
            }
        }

        private Func<TValue, TKey> _getkeyValue;

        private Func<TValue, TKey> GetKeyValue
        {
            get
            {
                if (_getkeyValue == null)
                {
                    if (string.IsNullOrWhiteSpace(this.StoreSetting.PrimaryKey))
                    {
                        throw new Exception("Primary key field not found, you may need to define a field name _id or SetPrimaryKey for this store.");
                    }
                    else
                    {
                        _getkeyValue = Helper.ObjectHelper.GetGetValue<TValue, TKey>(this.StoreSetting.PrimaryKey);
                    }
                }
                return _getkeyValue;
            }
        }

        /// <summary>
        /// Add a new record, 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="EnableLog">set to false to disable the log per records</param>
        /// <returns></returns>
        public bool put(TKey key, TValue value, bool EnableLog = true)
        {
            lock (_Locker)
            {
                long logid = 0;

                if (this.StoreSetting.EnableLog && EnableLog)
                {
                    logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

                    if (this.StoreSetting.EnableVersion && this.SetVersionNr != null)
                    {
                        this.SetVersionNr(value, logid);
                    }
                }

                Int64 contentposition = addValueBlock(key, value);
                // add the primary index first. 
                bool ok = this.primaryIndex.Add(key, contentposition);
                // continue with the rest of indexes.
                // 
                if (ok)
                {
                    bool updateindex = true;

                    if (this.HasIndex)
                    {
                        updateindex = this.Indexes.Add(value, contentposition);
                    }

                    if (updateindex)
                    {
                        // if all ok, write to the log. 
                        if (this.StoreSetting.EnableLog && EnableLog)
                        {
                            var log = new LogEntry() { Id = logid, EditType = EditType.Add, OldBlockPosition = 0, NewBlockPosition = contentposition, UserId = this.CurrentUserId, StoreName = this.Name, UpdateTime = DateTime.UtcNow, KeyBytes = this.KeyConverter.ToByte(key) };
                            this.OwnerDatabase.Log.Add(log);
                        }
                    }
                    else
                    {
                        // should roll back primary index as well. 
                        this.primaryIndex.Del(key);
                    }
                    return updateindex;
                }
                else
                {
                    return false;
                }

            }
        }

        private void updateindex(TKey key, TValue value, Int64 blockposition)
        {
            // add the primary index first. 
            bool ok = this.primaryIndex.Add(key, blockposition);
            // continue with the rest of indexes. 
            bool updateindex = true;
            if (ok)
            {
                if (this.HasIndex)
                {
                    updateindex = this.Indexes.Add(value, blockposition);
                }
            }
            if (!updateindex || !ok)
            {
                throw new Exception("update index failed");
            }
        }

        /// <summary>
        /// Caution, this will only affect the query that use column data. does not update index or the real content body.. 
        /// </summary>
        /// <typeparam name="TColumnType"></typeparam>
        /// <param name="key"></param>
        /// <param name="ColumnName"></param>
        /// <param name="value"></param>
        private void UpdateColumn<TColumnType>(TKey key, string ColumnName, TColumnType value)
        {
            if (ColumnName == null)
            {
                throw new Exception("column name is required");
            }

            var col = this.GetColumn(ColumnName);
            if (col == null || col.DataType != typeof(TColumnType))
            {
                throw new Exception("Column name not found or data type not match");
            }

            IByteConverter<TColumnType> converter = ObjectContainer.GetConverter<TColumnType>();

            long blockpoisiton = this.getBlockPosition(key);

            var bytes = converter.ToByte(value);

            if (col.IsString)
            {
                bytes = Helper.KeyHelper.AppendToKeyLength(bytes, true, col.Length);
            }

            lock (_Locker)
            {
                this.BlockFile.UpdateCol(blockpoisiton, col.relativePosition, col.Length, bytes);
            }
        }

        public void UpdateColumn<TColumnType>(TKey key, Expression<Func<TValue, object>> expression, TColumnType value)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<TValue>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                UpdateColumn<TColumnType>(key, fieldname, value);
            }
        }

        public void RestoreIndexFromLog()
        {
            lock (_Locker)
            {
                var alllogs = this.OwnerDatabase.Log.GetByStoreName(this.Name);
                var equal = new Kooboo.IndexedDB.Btree.Comparer.EqualityComparer(this.primaryIndex.keylength);

                Dictionary<byte[], LogEntry> lastedit = new Dictionary<byte[], LogEntry>(equal);

                foreach (var item in alllogs.OrderByDescending(o => o.UpdateTime))
                {
                    if (!lastedit.ContainsKey(item.KeyBytes))
                    {
                        lastedit.Add(item.KeyBytes, item);
                    }
                }

                if (lastedit.Count == 0)
                {
                    return;
                }

                this.primaryIndex.DelSelf();
                foreach (var item in this.Indexes.items)
                {
                    item.DelSelf();
                }

                foreach (var item in lastedit)
                {

                    if (item.Value.EditType != EditType.Delete)
                    {
                        var key = this.KeyConverter.FromByte(item.Key);

                        var block = item.Value.NewBlockPosition;
                        if (block == 0)
                        {
                            block = item.Value.OldBlockPosition;
                        }
                        var value = this.getValue(block);
                        if (value != null)
                        {
                            updateindex(key, value, block);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// add a new record. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Enablelog">set to false to disable the log per records</param>
        /// <returns></returns>
        public bool add(TKey key, TValue value, bool Enablelog = true)
        {
            return put(key, value, Enablelog);
        }

        public bool add(TValue value, bool Enablelog = true)
        {
            TKey key = GetKeyValue(value);

            return put(key, value, Enablelog);
        }


        /// <summary>
        /// delete a record
        /// </summary>
        /// <param name="key"></param>
        /// <param name="EnableLog">set to false to disable the log per records</param>
        public void delete(TKey key, bool EnableLog = true)
        {
            lock (_Locker)
            {
                List<Int64> blocklist = this.primaryIndex.Del(key);

                if (blocklist.Count == 0)
                {
                    // key not found, not delete. 
                    return;
                }
                bool delok = true;
                foreach (Int64 item in blocklist)
                {
                    /// continue with the rest of indexes. 
                    TValue recordvalue = this.getValue(item);

                    delok = this.Indexes.Del(recordvalue, item);

                    if (!delok)
                    {
                        // if del failed any reason, roll back.
                        // TODO: this might be a problem without putting all del into one transaction. 
                        // However, it is not a problem, because blocklist will only contains 1 record  max for now. 
                        this.primaryIndex.Add(key, item);
                        break;
                    }
                    else
                    {
                        /// del ok, we need to insert the log. 
                        if (this.StoreSetting.EnableLog && EnableLog)
                        {
                            this.OwnerDatabase.Log.Add(new LogEntry() { EditType = EditType.Delete, OldBlockPosition = item, NewBlockPosition = item, UserId = this.CurrentUserId, StoreName = this.Name, UpdateTime = DateTime.UtcNow, KeyBytes = this.KeyConverter.ToByte(key) });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// update a record
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newvalue"></param>
        /// <param name="EnableLog">set to false to disable the log per records</param>
        public void update(TKey key, TValue newvalue, bool EnableLog = true)
        {
            lock (_Locker)
            {
                Int64 oldblockposition = this.primaryIndex.Get(key);

                if (oldblockposition < 1)
                {
                    return;
                }

                TValue oldrecord = getValue(oldblockposition);

                if (EqualityComparer<TValue>.Default.Equals(oldrecord, default(TValue)))
                {
                    /// if  this does not exists. 
                    this.put(key, newvalue);
                    return;
                }

                long logid = 0;

                if (this.StoreSetting.EnableLog && EnableLog)
                {
                    logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

                    if (this.StoreSetting.EnableVersion && this.SetVersionNr != null)
                    {
                        this.SetVersionNr(newvalue, logid);
                    }
                }

                Int64 newblockposition = addValueBlock(key, newvalue);

                //update the primary key. 
                this.primaryIndex.Update(key, oldblockposition, newblockposition);

                //now update the other indexes.
                this.Indexes.Update(oldrecord, newvalue, oldblockposition, newblockposition);

                if (this.StoreSetting.EnableLog && EnableLog)
                {
                    LogEntry log = new LogEntry() { Id = logid, EditType = EditType.Update, OldBlockPosition = oldblockposition, NewBlockPosition = newblockposition, UserId = this.CurrentUserId, StoreName = this.Name, UpdateTime = DateTime.UtcNow, KeyBytes = this.KeyConverter.ToByte(key) };

                    this.OwnerDatabase.Log.Add(log);
                }
            }
        }

        private long addValueBlock(TKey key, TValue value)
        {
            lock (_lockerBlockFile)
            {
                byte[] valuebyte = ValueConverter.ToByte(value);
                return this.BlockFile.Add(valuebyte, valuebyte.Length);
            }
        }


        public bool UpdateOnly(TKey key, TValue newvalue, bool EnableLog = true)
        {
            lock (_Locker)
            {
                Int64 oldblockposition = this.primaryIndex.Get(key);

                if (oldblockposition < 1)
                {
                    return false;
                }

                TValue oldrecord = getValue(oldblockposition);

                if (EqualityComparer<TValue>.Default.Equals(oldrecord, default(TValue)))
                {
                    return false;
                }

                long logid = 0;

                if (this.StoreSetting.EnableLog && EnableLog)
                {
                    logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

                    if (this.StoreSetting.EnableVersion && this.SetVersionNr != null)
                    {
                        this.SetVersionNr(newvalue, logid);
                    }
                }

                Int64 newblockposition = addValueBlock(key, newvalue);

                //update the primary key. 
                this.primaryIndex.Update(key, oldblockposition, newblockposition);

                //now update the other indexes.
                this.Indexes.Update(oldrecord, newvalue, oldblockposition, newblockposition);

                if (this.StoreSetting.EnableLog && EnableLog)
                {
                    LogEntry log = new LogEntry() { Id = logid, EditType = EditType.Update, OldBlockPosition = oldblockposition, NewBlockPosition = newblockposition, UserId = this.CurrentUserId, StoreName = this.Name, UpdateTime = DateTime.UtcNow, KeyBytes = this.KeyConverter.ToByte(key) };

                    this.OwnerDatabase.Log.Add(log);
                }
                return true;
            }
        }


        public Filter<TKey, TValue> Filter
        {
            get
            {
                Filter<TKey, TValue> Filter = new Filter<TKey, TValue>(this);
                return Filter;
            }
        }

        /// <summary>
        /// This query convert all data back to object and use brute force to search.
        /// For better performance, consider use manual query. 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public FullScan<TKey, TValue> FullScan(Predicate<TValue> query)
        {
            return new FullScan<TKey, TValue>(this, query);
        }

        public WhereFilter<TKey, TValue> Where(Expression<Predicate<TValue>> query)
        {
            return new WhereFilter<TKey, TValue>(this, query);
        }

        public WhereFilter<TKey, TValue> Where()
        {
            return new WhereFilter<TKey, TValue>(this);
        }

        public int Count()
        {
            return this.primaryIndex.Count(false);
        }

        public List<LogEntry> GetLogs(int take, int skip = 0)
        {
            return this.OwnerDatabase.Log.GetByStoreName(this.Name, take, skip);
        }

        /// <summary>
        /// Update the content to the record before that log. 
        /// </summary>
        /// <param name="log"></param>
        public void RollBack(LogEntry log)
        {
            TKey key = this.KeyConverter.FromByte(log.KeyBytes);

            if (log.EditType == EditType.Add)
            {
                this.delete(key);
            }
            else if (log.EditType == EditType.Update)
            {
                TValue oldvalue = this.getValue(log.OldBlockPosition);
                this.update(key, oldvalue);
            }
            else
            {
                // this is delete of one item. 
                TValue oldvalue = this.getValue(log.OldBlockPosition);
                this.add(key, oldvalue);
            }
        }

        /// <summary>
        /// roll back all the log entries. 
        /// </summary>
        /// <param name="loglist"></param>
        public void RollBack(List<LogEntry> loglist)
        {
            HashSet<byte[]> finished = new HashSet<byte[]>();

            foreach (var item in loglist.OrderBy(o => o.TimeTick))
            {
                byte[] fixedlengthkey = this.appendToKeyLength(item.KeyBytes);

                if (!finished.Contains(fixedlengthkey, this.GenericBytesComparer))
                {
                    RollBack(item);
                    finished.Add(fixedlengthkey);
                }
            }
        }

        /// <summary>
        /// roll back the object store to specified version number. 
        /// </summary>
        /// <param name="LastVersionId"></param>
        /// <param name="SelfIncluded"></param>
        public void RollBack(Int64 LastVersionId, bool SelfIncluded = true)
        {
            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();
            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id >= LastVersionId && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id > LastVersionId && o.StoreNameHash == namehash).Take(99999);
            }
            if (logs.Count > 0)
            {
                RollBack(logs);
            }

        }

        /// <summary>
        /// Roll back the object store to special time tick. 
        /// </summary>
        /// <param name="TimeTick"></param>
        /// <param name="SelfIncluded"></param>
        public void RollBackTimeTick(Int64 TimeTick, bool SelfIncluded = true)
        {
            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();

            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.TimeTick >= TimeTick && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.TimeTick > TimeTick && o.StoreNameHash == namehash).Take(99999);
            }

            if (logs.Count > 0)
            {
                RollBack(logs);
            }
        }

        /// <summary>
        /// Checkout a version to another object store. 
        /// </summary>
        /// <param name="TimeTick"></param>
        /// <param name="DestinationStore"></param>
        /// <param name="SelfIncluded"></param>
        public void CheckOutTimeTick(Int64 TimeTick, ObjectStore<TKey, TValue> DestinationStore, bool SelfIncluded = true)
        {
            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();

            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.TimeTick <= TimeTick && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.TimeTick < TimeTick && o.StoreNameHash == namehash).Take(99999);
            }

            if (logs.Count > 0)
            {
                CheckOut(logs, DestinationStore);
            }

        }

        public void CheckOut(Int64 VersionId, ObjectStore<TKey, TValue> DestinationStore, bool SelfIncluded = true)
        {
            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();
            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id <= VersionId && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id < VersionId && o.StoreNameHash == namehash).Take(99999);
            }
            if (logs.Count > 0)
            {
                CheckOut(logs, DestinationStore);
            }
        }

        /// <summary>
        /// Check out all the log entries to new object store. 
        /// </summary>
        /// <param name="logs"></param>
        /// <param name="DestinationStore"></param>
        public void CheckOut(List<LogEntry> logs, ObjectStore<TKey, TValue> DestinationStore)
        {
            HashSet<byte[]> KeysDone = new HashSet<byte[]>();

            foreach (var item in logs.OrderByDescending(o => o.TimeTick))
            {

                byte[] fixedlengthkey = this.appendToKeyLength(item.KeyBytes);

                if (KeysDone.Contains(fixedlengthkey, this.GenericBytesComparer))
                {
                    continue;
                }

                if (item.EditType == EditType.Add || item.EditType == EditType.Update)
                {
                    TValue value = this.getValue(item.NewBlockPosition);
                    DestinationStore.add(this.KeyConverter.FromByte(item.KeyBytes), value);
                    KeysDone.Add(fixedlengthkey);
                }
                else
                {
                    // this is delete of one item. 
                    KeysDone.Add(fixedlengthkey);
                }

            }
        }

        public List<LogEntry> getVersions(TKey key)
        {
            byte[] keys = this.appendToKeyLength(this.KeyConverter.ToByte(key));
            return this.OwnerDatabase.Log.GetByStoreNameAndKey(this.Name, keys, 9999);
        }

        public TValue getByVersion(TKey key, long versionid)
        {
            var log = this.OwnerDatabase.Log.Get(versionid);
            if (log != null)
            {
                long blockid = log.NewBlockPosition;
                return getValue(blockid);
            }
            return default(TValue);
        }

        public Int64 getBlockPosition(TKey key)
        {
            lock (_Locker)
            {
                return this.primaryIndex.Get(key);
            }
        }

        /// <summary>
        /// get object value, should use to replace getOjbect/getValue. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue get(TKey key)
        {
            Int64 blockposition;

            lock (_Locker)
            {
                blockposition = this.primaryIndex.Get(key);
            }

            if (blockposition > 0)
            {
                return getValue(blockposition);
            }
            else
            {
                return default(TValue);
            }
        }


        public async Task<TValue> getAsync(TKey key)
        {
            Int64 blockposition;

            lock (_Locker)
            {
                blockposition = this.primaryIndex.Get(key);
            }

            if (blockposition > 0)
            {
                return await getValueAsync(blockposition);
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// Get the object of a special version number. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="versionid"></param>
        /// <returns></returns>
        public TValue get(TKey key, Int64 versionid)
        {
            return getByVersion(key, versionid);
        }

        public TValue getValue(Int64 blockposition)
        {
            lock (_Locker)
            {
                byte[] contentbytes = this.BlockFile.Get(blockposition);
                if (contentbytes == null)
                {
                    return default(TValue);
                }
                else
                {
                    return this.ValueConverter.FromByte(contentbytes);
                }
            }
        }

        public async Task<TValue> getValueAsync(Int64 blockposition)
        {
            byte[] contentbytes = await this.BlockFile.GetAsync(blockposition);
            if (contentbytes == null)
            {
                return default(TValue);
            }
            else
            {
                return this.ValueConverter.FromByte(contentbytes);
            }
        }

        public int getLength(long blockposition)
        {
            return this.BlockFile.GetLength(blockposition);
        }

        public TValue GetFromColumns(TKey key)
        {
            Int64 blockposition;

            lock (_Locker)
            {
                blockposition = this.primaryIndex.Get(key);

            }
            if (blockposition > 0)
            {
                return getValueFromColumns(blockposition);
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// Get the TValue based on the column data only. 
        /// </summary>
        /// <param name="blockposition"></param>
        /// <returns></returns>
        internal TValue getValueFromColumns(Int64 blockposition)
        {
            lock (_Locker)
            {
                var converter = this.ValueConverter as KoobooSimpleConverter<TValue>;

                if (converter != null)
                {
                    byte[] allcolumns = this.GetColumnBytes(blockposition);

                    return converter.FromByte(allcolumns);
                }

                return default(TValue);
            }
        }

        /// <summary>
        /// get the collection of all items, sorted by primary key. 
        /// Warning, Thread-unsafe. used in a multithread sitaution is not guarantee. 
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public ValueCollection<TKey, TValue> ItemCollection(bool ascending = true)
        {
            ValueCollection<TKey, TValue> collection = new ValueCollection<TKey, TValue>(this, this.primaryIndex.allItemCollection(ascending).GetEnumerator());
            return collection;
        }

        /// <summary>
        /// get the collection of all items, sorted by the provided index. 
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        public ValueCollection<TKey, TValue> ItemCollection(string indexFieldOrPropertyName, bool ascending)
        {
            IIndex<TValue> index = this.Indexes.getIndex(indexFieldOrPropertyName);

            ValueCollection<TKey, TValue> collection = new ValueCollection<TKey, TValue>(this, index.AllItems(ascending).GetEnumerator());

            return collection;
        }


        internal byte[] getColumnsBytes(Int64 blockposition, int relativePosition, int length)
        {
            return this.BlockFile.GetCol(blockposition, relativePosition, length);
        }

        internal byte[] GetColumnBytes(Int64 blockposition)
        {
            int length = 0;
            foreach (var item in this.SettingColumns)
            {
                length += item.Value + 8;
            }
            return this.BlockFile.Get(blockposition, length);
        }

        internal byte[] getColumnsBytes(TKey key, int relativePosition, int length)
        {
            Int64 blockposition = this.primaryIndex.Get(key);
            if (blockposition > 0)
            {
                return getColumnsBytes(blockposition, relativePosition, length);
            }
            else
            {
                return null;
            }
        }


        [Obsolete]
        public byte[] GetColumnBytes(TKey key, string FieldName)
        {
            var column = GetColumn(FieldName);

            if (column == null)
            {
                return null;
            }
            else
            {
                return getColumnsBytes(key, column.relativePosition, column.Length);
            }
        }

        public FilePart GetFieldPart(TKey key, string FieldName)
        {
            var FieldNameHash = Helper.ObjectHelper.GetHashCode(FieldName);
            var fieldnamehashbytes = BitConverter.GetBytes(FieldNameHash);
            Int64 blockposition = this.primaryIndex.Get(key);

            if (blockposition <= 0)
            {
                return null;
            }

            FilePart info = new FilePart();
            info.FullFileName = this.BlockFile._fullfilename;
            info.BlockPosition = blockposition;

            var koobooconverter = this.ValueConverter as KoobooSimpleConverter<TValue>;

            int startpos = 0;

            // found in the column first. 
            foreach (var item in koobooconverter.converter.Columns)
            {
                if (item.FieldNameHash == FieldNameHash)
                {
                    info.RelativePosition =  item.relativePosition + 10 + 8; // TODO: move this to 
                    info.Length = item.Length;
                    return info;
                } 
                var next = item.relativePosition + item.Length + 8; 
                if (next > startpos)
                {
                    startpos = next; 
                }
            }

            // if not found, after the last column, check one field by one field. 

            var totallen = this.BlockFile.GetLength(blockposition);

            var maxstart = totallen - 8;

            while (startpos < maxstart)
            {
                var header = this.BlockFile.GetPartial(blockposition, startpos + 10, 8); 

                int itemNameHash  = BitConverter.ToInt32(header, 0);

                int len = BitConverter.ToInt32(header, 4);


                if (itemNameHash == FieldNameHash)
                {
                    var itemstart = startpos + 10 + 8;
                    info.RelativePosition = itemstart;
                    info.Length = len;
                    return info; 
                }

                startpos = startpos + 8 + len; 
                   
            }
              
            return null;
        }
         

        public void Close()
        {
            lock (_Locker)
            {
                if (this._blockfile != null)
                {
                    this._blockfile.Close();
                    this._blockfile = null;
                }

                if (this.primaryIndex != null)
                {
                    this.primaryIndex.Close();
                }

                if (this.Indexes != null)
                {
                    this.Indexes.CloseAll();
                }
            }
        }

        public void Flush()
        {
            lock (_Locker)
            {
                if (this._blockfile != null)
                {
                    this._blockfile.Flush();
                }

                if (this.primaryIndex != null)
                {
                    this.primaryIndex.Flush();
                }

                if (this.Indexes != null)
                {
                    this.Indexes.FlushAll();
                }
            }
        }


        /// <summary>
        /// delete this store object. Does not update the database config with the changes. 
        /// </summary>
        public void DelSelf()
        {
            //Additional indexes should be closed as well. 
            lock (_Locker)
            {
                Close();
                if (System.IO.Directory.Exists(this.ObjectFolder))
                {
                    System.IO.Directory.Delete(this.ObjectFolder, true);
                }
            }
        }

        private byte[] appendToKeyLength(byte[] input)
        {
            return Helper.KeyHelper.AppendToKeyLength(input, this.IsStringKey, this.StoreSetting.PrimaryKeyLen);
        }


        private void TrySetVersionFunction()
        {
            var VersionFieldType = Helper.TypeHelper.GetFieldType<TValue>(GlobalSettings.VersionFieldName);

            if (VersionFieldType != null && VersionFieldType == typeof(Int64))
            {
                this.SetVersionNr = Helper.ObjectHelper.GetSetValue<TValue, Int64>(GlobalSettings.VersionFieldName);
            }
        }

        public bool add(object key, object value)
        {
            TValue tvalue = (TValue)value;
            TKey tkey = (TKey)key;
            if (tvalue != null && tkey != null)
            {
                return this.add(tkey, tvalue, this.StoreSetting.EnableLog);
            }
            return false;
        }



        public bool update(object key, object value)
        {
            TValue tvalue = (TValue)value;
            TKey tkey = (TKey)key;

            if (tvalue != null && tkey != null)
            {
                update(tkey, tvalue, this.StoreSetting.EnableLog);
                return true;
            }
            return false;
        }

        public void delete(object id)
        {
            TKey tkey = (TKey)id;
            if (tkey != null)
            {
                this.delete(tkey);
            }
        }

        public object get(object key)
        {
            TKey tkey = (TKey)key;
            if (tkey != null)
            {
                return this.get(tkey);
            }
            return null;
        }

        public void CheckOut(long VersionId, IObjectStore DestinationStore, bool SelfIncluded = true)
        {
            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();
            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id <= VersionId && o.StoreNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id < VersionId && o.StoreNameHash == namehash).Take(99999);
            }
            if (logs.Count > 0)
            {
                CheckOut(logs, DestinationStore);
            }
        }

        public void CheckOut(List<LogEntry> logs, IObjectStore DestinationStore)
        {
            HashSet<TKey> tkeysdone = new HashSet<TKey>();

            foreach (var item in logs.OrderByDescending(o => o.TimeTick))
            {
                byte[] fixedlengthkey = this.appendToKeyLength(item.KeyBytes);

                TKey key = this.KeyConverter.FromByte(item.KeyBytes);

                if (tkeysdone.Contains(key))
                {
                    continue;
                }

                tkeysdone.Add(key);

                if (item.EditType == EditType.Add || item.EditType == EditType.Update)
                {
                    TValue value = this.getValue(item.NewBlockPosition);
                    DestinationStore.add(key, value);
                }

            }
        }

        public List<object> List(int count = 9999, int skip = 0)
        {
            List<object> result = new List<object>();

            var values = this.Filter.Skip(skip).Take(count);
            foreach (var item in values)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
