//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Kooboo.IndexedDB.Helper;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Table
    {
        public Setting Setting { get; set; }

        public Guid CurrentUserId { get; set; }

        internal object _Locker = new object();

        public Converter.ObjectConverter ObjectConverter { get; set; }

        private BlockFile _blockFile;
        private object _lockerBlockFile = new object();
        public BlockFile BlockFile
        {
            get
            {
                if (_blockFile == null)
                {
                    lock (_lockerBlockFile)
                    {
                        if (_blockFile == null)
                        {
                            string blockFileName = System.IO.Path.Combine(this.ObjectFolder, "table.block");
                            var blockFile = new BlockFile(blockFileName);
                            blockFile.OpenOrCreate();
                            _blockFile = blockFile;
                        }
                    }
                }
                return _blockFile;
            }
        }

        internal BTree.BTreeIndex<Guid> PrimaryBTree
        {
            get
            {
                var find = this.Indexs.Find(o => o.FieldName == Dynamic.Constants.DefaultIdFieldName);
                var right = find as TableIndexBase<Guid>;
                return right.index;
            }
        }

        public long length
        {
            get
            {
                return this.PrimaryBTree.Count(false);
            }
        }

        public long count
        {
            get
            {
                return this.length;
            }
        }

        public List<Guid> Keys
        {
            get
            {
                List<Guid> result = new List<Guid>();
                var keyConverter = ObjectContainer.GetConverter<Guid>();

                foreach (var item in this.PrimaryBTree.AllKeyBytesCollection(true))
                {
                    var key = keyConverter.FromByte(item);
                    result.Add(key);
                }
                return result;
            }
        }

        public List<ITableIndex> Indexs { get; set; }

        public string ObjectFolder { get; set; }

        internal string SettingFile { get; set; }

        /// <summary>
        /// The name of this object store.
        /// </summary>
        public string Name { get; set; }

        public Database OwnerDatabase { get; set; }

        public Guid FirstKey
        {
            get
            {
                return this.PrimaryBTree.FirstKey;
            }
        }

        public Guid LastKey
        {
            get
            {
                return this.PrimaryBTree.LastKey;
            }
        }

        private string PrimaryKey { get; set; }

        public Table(string name, Database ownerDatabase, Setting setting)
        {
            this.Name = name;
            this.ObjectFolder = ownerDatabase.TableFolder(name);
            this.SettingFile = ownerDatabase.TableSetitingFile(name);

            setting = SettingHelper.GetOrSetTableSetting(this.SettingFile, setting);

            this.OwnerDatabase = ownerDatabase;
            Init(setting);
        }
        public Table(string name, Database ownerDatabase)
        {
            Setting setting = null;
            this.Name = name;
            this.ObjectFolder = ownerDatabase.TableFolder(name);
            this.SettingFile = ownerDatabase.TableSetitingFile(name);

            setting = SettingHelper.GetOrSetTableSetting(this.SettingFile, setting);

            this.OwnerDatabase = ownerDatabase;
            Init(setting);
        }

        private void Init(Setting setting)
        {
            this.Indexs = IndexHelper.CreatIndexs(setting, this.ObjectFolder);
            this.Setting = setting;
            var primary = this.Indexs.Find(o => o.IsPrimaryKey);
            this.PrimaryKey = primary.FieldName;

            SettingHelper.EnsureColumnRelativePosition(this.Setting.Columns);

            ObjectConverter = new Dynamic.Converter.ObjectConverter(this.Setting.Columns.ToList(), this.PrimaryKey);
        }


        private long _addBlock(Dictionary<string, object> data)
        {
            lock (_lockerBlockFile)
            {
                byte[] valueByte = ObjectConverter.ToBytes(data);
                return this.BlockFile.Add(valueByte, valueByte.Length);
            }
        }

        public void RebuildTable(Setting newSetting)
        {
            var isEnable = newSetting.EnableLog;

            string newName = "_koobootemp_" + System.Guid.NewGuid().ToString() + this.Name;
            var newTable = this.OwnerDatabase.GetOrCreateTable(newName, newSetting);

            newTable.Setting.EnableLog = false;

            var primaryIndex = this.Indexs.Find(o => o.IsSystem);

            string errorMsg = null;

            try
            {
                foreach (var item in primaryIndex.AllItems(true))
                {
                    var value = this._getvalue(item);
                    newTable.Add(value);
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            if (errorMsg != null)
            {
                this.OwnerDatabase.DeleteTable(newTable.Name);
                throw new Exception(errorMsg);
            }

            string oldFolder = this.ObjectFolder;
            string newFolder = newTable.ObjectFolder;

            this.Close();
            newTable.Close();
            this.DelSelf();
            this._blockFile = null;
            System.IO.Directory.Move(newFolder, oldFolder);


            newTable.Setting.EnableLog = isEnable;

            SettingHelper.WriteSetting(this.SettingFile, newSetting);
            Init(newSetting);

            if (System.IO.Directory.Exists(newFolder))
            {
                System.IO.Directory.Delete(newFolder, true);
            }

            this.OwnerDatabase.openTableList.Remove(newName);
        }

        public void UpdateSetting(Setting newSetting)
        {
            ensureIncremental(newSetting);

            var check = SettingHelper.UpdateSetting(newSetting.Columns.ToList(), this.Setting);

            lock (_Locker)
            {
                if (check.ShouldRebuild)
                {
                    RebuildTable(check.NewSetting);
                }
                else if (check.HasChange)
                {
                    // reinit index...and try to find remove or create index. 
                    var newIndexes = IndexHelper.CreatIndexs(check.NewSetting, this.ObjectFolder);

                    foreach (var item in newIndexes)
                    {
                        item.Close();
                    }

                    List<string> toRemove = new List<string>();

                    foreach (var item in this.Indexs)
                    {
                        var newer = newIndexes.Find(o => o.FieldName == item.FieldName);
                        if (newer == null)
                        {
                            toRemove.Add(item.FieldName);
                        }
                    }

                    Dictionary<string, bool> indexToRebuild = new();

                    foreach (var item in newIndexes)
                    {
                        var older = this.Indexs.Find(o => o.FieldName == item.FieldName);
                        if (older == null)
                        {
                            indexToRebuild.Add(item.FieldName, item.IsUnique);
                        }
                    }

                    foreach (var item in indexToRebuild)
                    {
                        this.CreateIndex(item.Key, item.Value, check.NewSetting);
                    }

                    foreach (var item in toRemove)
                    {
                        this.RemoveIndex(item);
                    }

                    this.Setting = check.NewSetting;

                    SettingHelper.WriteSetting(this.SettingFile, this.Setting);

                    var primary = newIndexes.Find(o => o.IsPrimaryKey);

                    this.ObjectConverter = new Dynamic.Converter.ObjectConverter(this.Setting.Columns.ToList(), primary.FieldName);

                    this.PrimaryKey = primary.FieldName;

                    foreach (var item in this.Indexs)
                    {
                        item.Close();
                    }

                    this.Indexs = newIndexes;

                    foreach (var item in this.Indexs)
                    {
                        item.Close();
                    }

                }

                this.Close();
            }
        }

        private void ensureIncremental(Setting newsetting)
        {
            if (newsetting != null && newsetting.Columns != null)
            {
                foreach (var item in newsetting.Columns)
                {
                    if (item.IsIncremental)
                    {
                        item.IsUnique = true;
                        item.IsIndex = true;
                        if (item.Seed < 0)
                        {
                            item.Seed = 0;
                        }
                        if (item.Increment < 1)
                        {
                            item.Increment = 1;
                        }
                    }
                }
            }
        }


        public Dictionary<string, object> PrepareData(object dataObj, bool Update = false)
        {
            // prepare key...
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            System.Collections.IDictionary iDict = dataObj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicObj = null;
            Type objectType = null;

            if (iDict == null)
            {
                dynamicObj = dataObj as IDictionary<string, object>;
            }

            if (iDict == null && dynamicObj == null)
            {
                objectType = dataObj.GetType();
            }

            Guid DefaultId = default(Guid);
            var DefaultIdObj = _GetObjValue(iDict, dynamicObj, Dynamic.Constants.DefaultIdFieldName, typeof(Guid), dataObj, objectType);
            if (DefaultIdObj != null)
            {
                System.Guid.TryParse(DefaultIdObj.ToString(), out DefaultId);
            }

            foreach (var item in this.Setting.Columns)
            {
                if (item.IsSystem)
                {
                    // the only system field is the id fields. 
                    object Value = null;

                    if (this.PrimaryKey != Dynamic.Constants.DefaultIdFieldName)
                    {
                        if (iDict != null)
                        {
                            Value = Accessor.GetValueIDict(iDict, this.PrimaryKey, item.ClrType);

                        }
                        else if (dynamicObj != null)
                        {
                            Value = Accessor.GetValue(dynamicObj, this.PrimaryKey, item.ClrType);
                        }
                        else
                        {
                            Value = Accessor.GetValue(dataObj, objectType, this.PrimaryKey, item.ClrType);
                        }

                        if (Value == null)
                        {
                            var col = this.Setting.Columns.First(o => o.Name == this.PrimaryKey);
                            if (col != null)
                            {
                                if (col.ClrType == typeof(string))
                                {
                                    Value = default(Guid);
                                }
                                else
                                {
                                    Value = IndexHelper.DefaultValue(col.ClrType);
                                }
                            }
                        }

                        if (Update == false && Value != null)
                        {
                            DefaultId = _ParseKey(Value);
                        }
                    }

                    if (Value == null)
                    {
                        if (iDict != null)
                        {
                            Value = Accessor.GetValueIDict(iDict, item.Name, item.ClrType);
                        }
                        else if (dynamicObj != null)
                        {
                            Value = Accessor.GetValue(dynamicObj, item.Name, item.ClrType);
                        }
                        else
                        {
                            Value = Accessor.GetValue(dataObj, objectType, item.Name, item.ClrType);
                        }
                    }

                    if (Value == null)
                    {
                        // the only possible null value value for key is string.   
                        Value = Helper.IndexHelper.NewTimeGuid();
                    }
                    else
                    {
                        Value = _ParseKey(Value);
                    }
                    if (!data.ContainsKey(item.Name))
                    {
                        data.Add(item.Name, Value);
                    }
                }

                else
                {
                    object Value = null;

                    if (iDict != null)
                    {
                        Value = Accessor.GetValueIDict(iDict, item.Name, item.ClrType);

                    }
                    else if (dynamicObj != null)
                    {
                        Value = Accessor.GetValue(dynamicObj, item.Name, item.ClrType);
                    }
                    else
                    {
                        Value = Accessor.GetValue(dataObj, objectType, item.Name, item.ClrType);
                    }

                    if (item.IsIncremental && !Update)
                    {
                        if (Value == null || Accessor.ChangeType<long>(Value) == 0)
                        {
                            var index = this.Indexs.Find(o => o.FieldName == item.Name);
                            Value = index.NextIncrement();
                        }

                        if (item.IsPrimaryKey && Value != null)
                        {
                            var keyvalue = _ParseKey(Value);
                            data[Dynamic.Constants.DefaultIdFieldName] = keyvalue;
                        }
                    }

                    if (Value == null)
                    {
                        if (item.IsIncremental)
                        {
                            var index = this.Indexs.Find(o => o.FieldName == item.Name);
                            Value = index.NextIncrement();
                        }
                        else if (item.IsIndex || item.IsPrimaryKey)
                        {
                            Value = IndexHelper.DefaultValue(item.ClrType);
                        }
                    }

                    if (Value == null && (item.IsIndex || item.IsPrimaryKey))
                    {
                        Value = IndexHelper.DefaultValue(item.ClrType);
                    }
                    data.Add(item.Name, Value);
                }
            }


            if (DefaultId != default(Guid))
            {
                data[Dynamic.Constants.DefaultIdFieldName] = DefaultId;
            }

            return data;
        }


        private object _GetObjValue(System.Collections.IDictionary iDict, IDictionary<string, object> dynamicObj, string key, Type clrType, object dataObj, Type objectType)
        {
            if (iDict != null)
            {
                return Accessor.GetValueIDict(iDict, key, clrType);

            }
            else if (dynamicObj != null)
            {
                return Accessor.GetValue(dynamicObj, key, clrType);
            }
            else
            {
                return Accessor.GetValue(dataObj, objectType, key, clrType);
            }
        }

        private void CheckAddConstraints(Dictionary<string, object> data)
        {
            foreach (var item in this.Indexs.Where(o => o.IsUnique))
            {
                if (data.ContainsKey(item.FieldName))
                {
                    var value = data[item.FieldName];

                    if (value == null)
                    {
                        value = IndexHelper.DefaultValue(item.keyType);
                    }

                    var block = item.Get(value);
                    if (block > 0)
                    {
                        if (item.FieldName == Dynamic.Constants.DefaultIdFieldName)
                        {

                            throw new Exception("Primary key uniqueness constraints validation failed");
                        }
                        else
                        {

                            throw new Exception("Uniqueness constraints validation failed. Index: " + item.FieldName);
                        }

                    }
                }
                else
                {
                    throw new Exception("Value not provided for index: " + item.FieldName);
                }
            }
        }


        public Guid Add(object Value, bool CheckCol = false, TableActionResult result = null)
        {
            lock (_Locker)
            {
                if (CheckCol)
                {
                    var check = SettingHelper.CompareSetting(Value, this.Setting);

                    if (check.ShouldRebuild)
                    {
                        RebuildTable(check.NewSetting);
                        return Add(Value, CheckCol, result);
                    }
                    else if (check.HasChange)
                    {
                        this.Setting = check.NewSetting;
                        SettingHelper.WriteSetting(this.SettingFile, this.Setting);
                        var primary = this.Indexs.Find(o => o.IsPrimaryKey);
                        this.ObjectConverter = new Dynamic.Converter.ObjectConverter(this.Setting.Columns.ToList(), primary.FieldName);
                    }
                }

                var data = this.PrepareData(Value);
                //add version. 
                long Version = 0;
                if (this.Setting.EnableLog)
                {
                    Version = this.OwnerDatabase.Log.GetNewLogId();
                    data[Constants.VersionFieldName] = Version;
                }

                CheckAddConstraints(data);

                Int64 contentPosition = _addBlock(data);

                foreach (var item in this.Indexs)
                {
                    object key = null;

                    if (data.ContainsKey(item.FieldName))
                    {
                        key = data[item.FieldName];
                    }
                    else
                    {
                        // should not be, because there is a constraints checking. 
                        throw new Exception("data not provided for index: " + item.FieldName);
                    }

                    item.Add(key, contentPosition);
                }

                this.Close();

                Guid ReturnId = default(Guid);

                if (data.ContainsKey(Constants.DefaultIdFieldName))
                {
                    ReturnId = (Guid)data[Constants.DefaultIdFieldName];

                    if (this.Setting.EnableLog)
                    {
                        var logPos = AddLogData(Version, data);



                        var log = new LogEntry()
                        {
                            Id = Version,
                            EditType = EditType.Add,
                            OldBlockPosition = logPos,
                            NewBlockPosition = logPos,
                            UserId = this.CurrentUserId,
                            TableName = this.Name,
                            UpdateTime = DateTime.UtcNow,
                            KeyBytes = ObjectContainer.GuidConverter.ToByte(ReturnId)
                        };

                        this.OwnerDatabase.Log.Add(log);
                    }
                }

                if (result != null)
                {
                    result.Id = ReturnId;
                    result.Version = Version;
                }

                return ReturnId;
            }
        }

        internal long AddLogData(long LogId, Dictionary<string, object> data)
        {
            byte[] valueByte = Kooboo.IndexedDB.Serializer.Simple.TableDataLogConverter.Instance.ToBytes(data);
            int len = valueByte.Length;

            var logIdBytes = BitConverter.GetBytes(LogId);

            byte[] totalBytes = new byte[len + 8];
            System.Buffer.BlockCopy(logIdBytes, 0, totalBytes, 0, 8);
            System.Buffer.BlockCopy(valueByte, 0, totalBytes, 8, len);

            var LogPos = this.OwnerDatabase.TableLog.Add(totalBytes, totalBytes.Length);
            this.OwnerDatabase.TableLog.Close();

            return LogPos;
        }

        public Dictionary<string, object> GetLogData(LogEntry log)
        {
            return GetLogData(log.Id, log.NewBlockPosition);
        }

        public Dictionary<string, object> GetLogData(long LogId, long DiskPosition)
        {
            var dataBytes = this.OwnerDatabase.TableLog.Get(DiskPosition);

            if (dataBytes != null)
            {
                var dbLogId = BitConverter.ToInt64(dataBytes, 0);
                if (dbLogId == LogId)
                {
                    var valueBytes = new byte[dataBytes.Length - 8];
                    Buffer.BlockCopy(dataBytes, 8, valueBytes, 0, dataBytes.Length - 8);

                    return Serializer.Simple.TableDataLogConverter.Instance.FromBytes(valueBytes);
                }
            }

            return null;
        }

        internal Guid _ParseKey(object key)
        {
            if (key == null)
            {
                return default(Guid);
            }

            if (key is System.Guid)
            {
                return (Guid)key;
            }
            string strKey = key.ToString();
            Guid guidKey;
            if (System.Guid.TryParse(strKey, out guidKey))
            {
                return guidKey;
            }
            else
            {
                return Helper.KeyHelper.ComputeGuid(strKey);
            }
        }

        public IDictionary<string, object> Get(Guid key)
        {
            Int64 blockPosition;

            var primary = this.Indexs.Find(o => o.IsSystem);
            blockPosition = primary.Get(key);

            if (blockPosition > 0)
            {
                return _getvalue(blockPosition);
            }
            return null;
        }

        public long GetDiskPos(object key)
        {
            var guidKey = _ParseKey(key);
            return GetDiskPos(guidKey);
        }

        public long GetDiskPos(Guid key)
        {
            var primary = this.Indexs.Find(o => o.IsSystem);
            return primary.Get(key);
        }

        public IDictionary<string, object> GetValue(long diskposition)
        {
            return _getvalue(diskposition);
        }

        public IDictionary<string, object> Get(object key)
        {
            var guidKey = _ParseKey(key);
            return Get(guidKey);
        }


        public T Get<T>(object key)
        {
            var guidKey = _ParseKey(key);

            Int64 blockPosition;

            var primary = this.Indexs.Find(o => o.IsSystem);
            blockPosition = primary.Get(guidKey);

            if (blockPosition > 0)
            {
                return _getValue<T>(blockPosition);
            }
            return default(T);
        }

        internal IDictionary<string, object> _getvalue(Int64 blockPosition)
        {
            lock (_Locker)
            {
                byte[] contentBytes = this.BlockFile.Get(blockPosition);
                if (contentBytes != null)
                {
                    return this.ObjectConverter.FromBytes(contentBytes);
                }
                return null;
            }
        }

        internal T _getValue<T>(Int64 blockPosition)
        {
            lock (_Locker)
            {
                byte[] contentBytes = this.BlockFile.Get(blockPosition);
                if (contentBytes != null)
                {
                    return this.ObjectConverter.FromBytes<T>(contentBytes);
                }
                return default(T);
            }
        }

        public long Delete(object key)
        {
            var guidKey = _ParseKey(key);

            long Version = 0;

            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);
                List<Int64> blocklist = primary.Del(guidKey);

                if (blocklist.Count == 0)
                {
                    // key not found, not delete.
                    return Version;
                }

                var blockPosition = blocklist[0];
                var value = this._getvalue(blockPosition);

                if (value == null)
                {
                    return Version;
                }

                var data = this.PrepareData(value, true);

                foreach (var item in this.Indexs)
                {
                    if (item.FieldName != primary.FieldName)
                    {
                        if (data.ContainsKey(item.FieldName))
                        {
                            var keyValue = data[item.FieldName];
                            item.Del(keyValue, blockPosition);
                        }
                    }
                }

                if (this.Setting.EnableLog)
                {
                    Version = this.OwnerDatabase.Log.GetNewLogId();

                    var diskPosition = this.AddLogData(Version, data);

                    var log = new LogEntry()
                    {
                        Id = Version,
                        EditType = EditType.Delete,
                        UserId = this.CurrentUserId,
                        TableName = this.Name,
                        UpdateTime = DateTime.UtcNow,
                        OldBlockPosition = diskPosition,
                        NewBlockPosition = diskPosition,
                        KeyBytes = ObjectContainer.GuidConverter.ToByte(guidKey)
                    };

                    this.OwnerDatabase.Log.Add(log);
                }


                this.Close();

                return Version;
            }
        }

        private void CheckUpdateConstraints(IDictionary<string, object> oldData, Dictionary<string, object> newData)
        {
            AssignIncremental(oldData, newData);

            foreach (var item in this.Indexs.Where(o => o.IsUnique && o.FieldName != Constants.DefaultIdFieldName))
            {
                if (oldData.ContainsKey(item.FieldName))
                {
                    if (newData.ContainsKey(item.FieldName))
                    {

                        var oldKey = oldData[item.FieldName];
                        var newKey = newData[item.FieldName];

                        if (oldKey.ToString() != newKey.ToString())
                        {
                            var newObj = item.Get(newKey);
                            if (newObj > 0)
                            {
                                throw new Exception("Uniqueness constraints validation failed. Index: " + item.FieldName);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Value not provided for index: " + item.FieldName);
                    }
                }
            }

        }


        public bool UpdateOrAdd(object newValue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsPrimaryKey);
                if (primary.IsSystem)
                {
                    throw new Exception("Update requires a primary key");
                }

                var newData = this.PrepareData(newValue, true);

                if (newData.ContainsKey(Constants.DefaultIdFieldName))
                {
                    var key = newData[Constants.DefaultIdFieldName];
                    var id = _ParseKey(key);

                    var sys = this.Indexs.Find(o => o.IsSystem);

                    var blockPosition = sys.Get(id);

                    if (blockPosition <= 0)
                    {
                        var newId = this.Add(newValue);
                        return newId != default(Guid);
                    }
                    else
                    {
                        var oldValue = this._getvalue(blockPosition);

                        if (IsPrimaryKeyChange(oldValue, newData))
                        {
                            throw new Exception("primary key can not be changed or null");
                        }

                        newData[Dynamic.Constants.DefaultIdFieldName] = key;


                        if (oldValue != null && newValue is IDictionary<string, object>)
                        {
                            var newDict = newValue as IDictionary<string, object>;

                            foreach (var item in oldValue)
                            {
                                if (!newDict.ContainsKey(item.Key))
                                {
                                    newData[item.Key] = item.Value;
                                }
                            }
                        }

                        CheckUpdateConstraints(oldValue, newData);
                        long version;
                        return UpdateNewData(newData, id, blockPosition, oldValue, out version);

                    }
                }
                else
                {
                    throw new Exception("Update requires a primary key");
                }

            }
        }


        private bool IsPrimaryKeyChange(IDictionary<string, object> old, IDictionary<string, object> newer)
        {
            var primaryKey = this.Indexs.Find(o => o.IsPrimaryKey);
            if (primaryKey != null && !primaryKey.IsSystem)
            {
                var field = primaryKey.FieldName;

                if (old.ContainsKey(field))
                {
                    if (newer.ContainsKey(field))
                    {
                        var oldKey = old[field];
                        var newKey = newer[field];
                        if (oldKey.ToString() != newKey.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }

            }

            return false;
        }

        public bool Update(Guid key, object newValue, TableActionResult result = null)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);

                var blockPosition = primary.Get(key);

                if (blockPosition <= 0)
                {
                    return false;
                }

                var oldValue = this._getvalue(blockPosition);

                long newBlock = blockPosition;

                var newData = this.PrepareData(newValue, true);

                // verify that update does not change primary key. 
                if (IsPrimaryKeyChange(oldValue, newData))
                {
                    throw new Exception("Primary key can not be changed or null");
                }

                newData[Dynamic.Constants.DefaultIdFieldName] = key;

                if (oldValue != null && newValue is IDictionary<string, object>)
                {
                    var newDict = newValue as IDictionary<string, object>;

                    foreach (var item in oldValue)
                    {
                        if (!newDict.ContainsKey(item.Key))
                        {
                            newData[item.Key] = item.Value;
                        }
                    }
                }

                CheckUpdateConstraints(oldValue, newData);

                long version;

                var success = UpdateNewData(newData, key, blockPosition, oldValue, out version);

                if (result != null)
                {
                    result.Id = key;
                    result.Version = version;
                }
                return success;
            }

        }


        public bool Update(object newValue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsPrimaryKey);
                if (primary.IsSystem)
                {
                    throw new Exception("Update requires a primary key");
                }

                var newData = this.PrepareData(newValue, true);

                if (newData.ContainsKey(Constants.DefaultIdFieldName))
                {
                    var key = newData[Constants.DefaultIdFieldName];
                    var id = _ParseKey(key);

                    var sys = this.Indexs.Find(o => o.IsSystem);

                    var blockPosition = sys.Get(id);

                    if (blockPosition <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        var oldValue = this._getvalue(blockPosition);

                        // verify that update does not change primary key. 
                        if (IsPrimaryKeyChange(oldValue, newData))
                        {
                            throw new Exception("primary key can not be changed or null");
                        }

                        newData[Dynamic.Constants.DefaultIdFieldName] = key;

                        if (oldValue != null && newValue is IDictionary<string, object>)
                        {
                            var newDict = newValue as IDictionary<string, object>;

                            foreach (var item in oldValue)
                            {
                                if (!newDict.ContainsKey(item.Key))
                                {
                                    newData[item.Key] = item.Value;
                                }
                            }
                        }

                        CheckUpdateConstraints(oldValue, newData);

                        long version;
                        return UpdateNewData(newData, id, blockPosition, oldValue, out version);
                    }
                }
                else
                {
                    throw new Exception("Update requires a primary key");
                }
            }
        }

        private Guid NewValueHash(Dictionary<string, object> newData, IDictionary<string, object> oldValue)
        {
            Guid HashValue = default(Guid);

            if (this.Setting.EnableLog)
            {
                if (newData.TryGetValue(Constants.VersionFieldName, out object current))
                {
                    if (oldValue != null && oldValue.TryGetValue(Constants.VersionFieldName, out object oldVersion))
                    {
                        newData[Constants.VersionFieldName] = oldVersion;
                    }

                    HashValue = Helper.KeyHelper.ComputeGuid(ObjectConverter.ToBytes(newData));
                    newData[Constants.VersionFieldName] = current;
                }
            }

            if (HashValue == default(Guid))
            {
                HashValue = Helper.KeyHelper.ComputeGuid(ObjectConverter.ToBytes(newData));
            }

            return HashValue;
        }

        private bool UpdateNewData(Dictionary<string, object> newData, Guid id, long blockPosition, IDictionary<string, object> oldValue, out long version)
        {
            bool UpdateOk = false;

            if (this.Setting.EnableLog)
            {
                version = this.OwnerDatabase.Log.GetNewLogId();
                newData[Constants.VersionFieldName] = version;
            }
            else
            { version = 0; }

            long newBlock = blockPosition;

            byte[] valueByte = ObjectConverter.ToBytes(newData);

            int tolerance = this.BlockFile.GetTolerance(blockPosition);

            if (tolerance > valueByte.Length)
            {
                var oldValueBytes = this.BlockFile.Get(blockPosition);

                var oldHash = Helper.KeyHelper.ComputeGuid(oldValueBytes);
                //  var newHash = Helper.KeyHelper.ComputeGuid(valueByte);
                var newHash = NewValueHash(newData, oldValue);

                if (oldHash == newHash)
                {
                    return false;
                }
                else
                {
                    this.BlockFile.UpdateBlock(valueByte, blockPosition);
                    UpdateOk = true;
                }
            }
            else
            {
                newBlock = this.BlockFile.Add(valueByte, valueByte.Length);
                UpdateOk = true;
            }

            foreach (var item in this.Indexs)
            {
                if (oldValue.ContainsKey(item.FieldName))
                {
                    var old = oldValue[item.FieldName];
                    var newer = newData[item.FieldName];
                    item.Update(old, newer, blockPosition, newBlock);
                }
                else
                {
                    var newer = newData[item.FieldName];
                    item.Update(newer, blockPosition, newBlock);
                }
            }

            this.Close();

            if (this.Setting.EnableLog)
            {
                CreateUpdateLog(version, newData, id);
            }

            return UpdateOk;
        }

        private void CreateUpdateLog(long version, Dictionary<string, object> data, Guid key, string colName = null)
        {
            if (!this.Setting.EnableLog)
            {
                return;
            }

            var LogPos = this.AddLogData(version, data);

            var log = new LogEntry()
            {
                Id = version,
                EditType = EditType.Update,
                OldBlockPosition = LogPos,
                NewBlockPosition = LogPos,
                UserId = this.CurrentUserId,
                TableName = this.Name,
                TableColName = colName,
                UpdateTime = DateTime.UtcNow,
                KeyBytes = ObjectContainer.GuidConverter.ToByte(key)
            };

            this.OwnerDatabase.Log.Add(log);
        }

        private void AssignIncremental(IDictionary<string, object> old, IDictionary<string, object> newer)
        {
            foreach (var item in this.Setting.Columns.Where(o => o.IsIncremental))
            {
                if (old.ContainsKey(item.Name))
                {
                    newer[item.Name] = old[item.Name];
                }
                else
                {
                    var index = this.Indexs.Find(o => o.FieldName == item.Name);
                    var next = index.NextIncrement();

                    var rightValue = Convert.ChangeType(next, item.ClrType);

                    newer[item.Name] = rightValue;
                }
            }
        }

        public void Update(object key, object newValue)
        {
            var TKey = _ParseKey(key);
            Update(TKey, newValue);
        }



        public List<IDictionary<string, object>> All()
        {
            List<IDictionary<string, object>> result = new List<IDictionary<string, object>>();

            var primary = this.Indexs.Find(o => o.IsSystem);

            foreach (var item in primary.AllItems(false))
            {
                var value = _getvalue(item);
                result.Add(value);
            }
            return result;
        }

        public List<T> All<T>()
        {
            List<T> result = new List<T>();

            var primary = this.Indexs.Find(o => o.IsSystem);

            foreach (var item in primary.AllItems(false))
            {
                var value = _getValue<T>(item);
                result.Add(value);
            }
            return result;
        }

        public bool UpdateColumn(object key, string ColumnName, object value)
        {
            if (ColumnName == null)
            {
                throw new Exception("column name is required");
            }

            long version = 0;
            if (this.Setting.EnableLog)
            {
                version = this.OwnerDatabase.Log.GetNewLogId();
            }

            var col = this.Setting.Columns.FirstOrDefault(o => o.Name == ColumnName);

            if (col == null)
            {
                throw new Exception("Column name not found or data type not match");
            }

            var colType = Helper.TypeHelper.GetType(col.DataType);
            var colValue = Convert.ChangeType(value, colType);
            var fieldConverter = this.ObjectConverter.Fields.Find(o => o.FieldName == col.Name);


            if (colType == null || fieldConverter == null)
            {
                throw new Exception(col.DataType + " data type or field converter not found");
            }

            if (col.Length == int.MaxValue)
            {
                throw new Exception("columns with unlimited length can not be update by this method");
            }

            var valueBytes = fieldConverter.ToBytes(colValue);

            var keyVaries = KeyHelper.IsKeyLenVar(colType);

            valueBytes = Helper.KeyHelper.AppendToKeyLength(valueBytes, keyVaries, col.Length);

            lock (_Locker)
            {
                var guidKey = _ParseKey(key);

                Int64 blockPosition;

                var primary = this.Indexs.Find(o => o.IsSystem);
                blockPosition = primary.Get(key);

                if (blockPosition > 0)
                {
                    var success = this.BlockFile.UpdateCol(blockPosition, col.relativePosition, col.Length, valueBytes);

                    this.BlockFile.Close();

                    if (success)
                    {
                        Dictionary<string, object> colData = new Dictionary<string, object>();
                        colData.Add(ColumnName, colValue);
                        CreateUpdateLog(version, colData, guidKey, ColumnName);
                    }
                    return success;
                }
            }

            return false;
        }

        public void UpdateColumn<T>(object key, Expression<Func<T, object>> expression, object newvalue)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<T>(expression);

            if (!string.IsNullOrEmpty(fieldname))
            {
                this.UpdateColumn(key, fieldname, newvalue);
            }
        }

        public Query Query
        {
            get
            {
                return new Query(this);
            }
        }

        public void Close()
        {
            lock (_Locker)
            {
                if (this._blockFile != null)
                {
                    this._blockFile.Close();
                }
                foreach (var item in this.Indexs)
                {
                    item.Close();
                }
            }
        }

        public void Flush()
        {
            lock (_Locker)
            {
                if (this._blockFile != null)
                {
                    this._blockFile.Flush();
                }

                foreach (var item in this.Indexs)
                {
                    item.Flush();
                }
            }
        }

        public void DelSelf()
        {
            lock (_Locker)
            {
                foreach (var item in this.Indexs)
                {
                    item.DelSelf();
                }

                if (this._blockFile != null)
                {
                    this._blockFile.Close();

                }

                if (System.IO.Directory.Exists(this.ObjectFolder))
                {
                    System.IO.Directory.Delete(this.ObjectFolder, true);
                }
            }
        }

        #region TOCHECK

        public void CreateIndex(string fieldName, bool unique = false, Setting setting = null)
        {
            if (setting == null)
            {
                setting = this.Setting;
            }
            lock (_Locker)
            {
                if (this.Indexs.Find(o => o.FieldName == fieldName) != null)
                {
                    throw new Exception("the index " + fieldName + " already exits");
                }

                if (fieldName == Constants.DefaultIdFieldName)
                {
                    throw new Exception(Constants.DefaultIdFieldName + " is reserved");
                }

                var col = setting.Columns.FirstOrDefault(o => o.Name == fieldName);

                if (col == null || col.Length == int.MaxValue)
                {
                    throw new Exception("Index fieldname must be in the column with fixed length");
                }


                string indexfile = IndexHelper.GetIndexFile(this.ObjectFolder, fieldName);

                var newindex = IndexHelper.CreateIndex(fieldName, col.ClrType, indexfile, unique, col.Length);

                if (col.IsIncremental)
                {
                    newindex.IsIncremental = true;
                    newindex.Seed = col.Seed;
                    newindex.Increment = col.Increment;
                }

                var primaryindex = this.Indexs.Find(o => o.IsSystem);

                foreach (var item in primaryindex.AllItems(true))
                {
                    object fieldvalue = null;
                    if (col.IsIncremental)
                    {
                        fieldvalue = newindex.NextIncrement();
                    }
                    else
                    {
                        var value = this._getvalue(item);
                        // need to check uniquness constraints... 
                        fieldvalue = Accessor.GetValue(value, fieldName, col.ClrType);
                    }

                    if (unique)
                    {
                        if (fieldvalue == null)
                        {
                            newindex.Close();
                            newindex.DelSelf();
                            throw new Exception("Create index failed, Value not provided for Index: " + newindex.FieldName);
                        }
                        else
                        {
                            var oldblock = newindex.Get(fieldvalue);
                            if (oldblock > 0)
                            {
                                newindex.Close();
                                newindex.DelSelf();
                                throw new Exception("Violate uniqueness constraints for Index: " + newindex.FieldName);
                            }
                        }

                    }

                    if (fieldvalue == null)
                    {
                        fieldvalue = IndexHelper.DefaultValue(col.ClrType);
                    }
                    newindex.Add(fieldvalue, item);
                }


                col.IsIndex = true;
                col.IsUnique = unique;
                this.Indexs.Add(newindex);
            }
        }

        public void CreateIndex<T>(Expression<Func<T, object>> FieldNameExpression, bool unique = false)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<T>(FieldNameExpression);

            CreateIndex(fieldname, unique);
        }

        public void RemoveIndex<T>(Expression<Func<T, object>> FieldNameExpression)
        {
            string fieldname = Helper.ExpressionHelper.GetFieldName<T>(FieldNameExpression);

            RemoveIndex(fieldname);
        }

        public void RemoveIndex(string FieldName)
        {
            if (FieldName.ToLower() == Dynamic.Constants.DefaultIdFieldName)
            {
                return;
            }
            var index = this.Indexs.Find(o => o.FieldName == FieldName);

            if (index != null)
            {
                lock (_Locker)
                {
                    this.Indexs.Remove(index);
                    index.Close();
                    index.DelSelf();
                }
            }
        }

        /// <summary>
        /// Update the content to the record before that log. 
        /// </summary>
        /// <param name="log"></param>
        public void RollBack(LogEntry log)
        {
            //just double confirm. 
            if (log.TableName == this.Name)
            {
                Guid key = ObjectContainer.GuidConverter.FromByte(log.KeyBytes);

                if (log.EditType == EditType.Add)
                {
                    // undo of add == delete. 
                    this.Delete(key);
                }
                else if (log.EditType == EditType.Delete)
                {
                    var lastlog = this.OwnerDatabase.Log.GetPreviousTableLog(log);
                    if (lastlog == null || lastlog.EditType == EditType.Delete)
                    {
                        this.Delete(key);
                    }
                    else
                    {
                        Dictionary<string, object> item = GetLastUpdateLogItem(lastlog);

                        if (item != null)
                        {
                            if (item.ContainsKey(Constants.DefaultIdFieldName))
                            {
                                var idkey = item[Constants.DefaultIdFieldName];
                                var id = _ParseKey(idkey);

                                if (this.Get(id) != null)
                                {
                                    this.Update(id, item);
                                }
                                else
                                {
                                    this.Add(item);
                                }

                            }
                        }
                    }
                }

                else if (log.EditType == EditType.Update)
                {
                    var lastlog = this.OwnerDatabase.Log.GetPreviousTableLog(log);
                    if (lastlog != null)
                    {
                        if (lastlog.EditType == EditType.Delete)
                        {
                            this.Delete(key);
                        }
                        else if (lastlog.EditType == EditType.Add || lastlog.EditType == EditType.Update)
                        {
                            Dictionary<string, object> item = GetLastUpdateLogItem(lastlog);

                            if (item != null)
                            {

                                if (item.ContainsKey(Constants.DefaultIdFieldName))
                                {
                                    var idkey = item[Constants.DefaultIdFieldName];
                                    var id = _ParseKey(idkey);
                                    if (this.Get(id) != null)
                                    {
                                        this.Update(id, item);
                                    }
                                    else
                                    {
                                        this.Add(item);
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }


        public void RollBack(List<LogEntry> loglist)
        {
            HashSet<Guid> finished = new HashSet<Guid>();

            foreach (var item in loglist.OrderBy(o => o.TimeTick))
            {
                Guid key = Kooboo.IndexedDB.ObjectContainer.GuidConverter.FromByte(item.KeyBytes);

                if (!finished.Contains(key))
                {
                    RollBack(item);
                    finished.Add(key);
                }
            }
        }

        private Dictionary<string, object> GetLastUpdateLogItem(LogEntry lastlog)
        {
            if (lastlog.EditType == EditType.Update && !string.IsNullOrWhiteSpace(lastlog.TableColName))
            {
                var previousLog = this.OwnerDatabase.Log.GetPreviousTableLog(lastlog);
                if (previousLog == null)
                {
                    return null;
                }
                else
                {
                    var item = GetLastUpdateLogItem(previousLog);
                    var ColItem = this.GetLogData(lastlog);
                    if (item != null && ColItem != null)
                    {
                        foreach (var col in ColItem)
                        {
                            item[col.Key] = col.Value;
                        }

                        return item;
                    }

                }
            }
            else
            {
                return this.GetLogData(lastlog);
            }

            return null;

        }


        public void CheckOut(Int64 VersionId, Table destinationTable, bool SelfIncluded, bool UpdateSetting = true)
        {
            if (UpdateSetting)
            {
                UpdateCheckOutTableSetting(destinationTable);
            }

            List<LogEntry> logs;
            int namehash = this.Name.GetHashCode32();
            if (SelfIncluded)
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id > VersionId && o.TableNameHash == namehash).Take(99999);
            }
            else
            {
                logs = this.OwnerDatabase.Log.Store.Where(o => o.Id >= VersionId && o.TableNameHash == namehash).Take(99999);
            }

            CheckOutExcl(logs, destinationTable);
        }

        private void UpdateCheckOutTableSetting(Table desttable)
        {
            var newsetting = Dynamic.SettingHelper.Clone(this.Setting);
            desttable.UpdateSetting(newsetting);
        }

        internal void CheckOutExcl(List<LogEntry> ExclLogs, Table destTable)
        {
            var exclitems = GetExclItems(ExclLogs);

            var processed = new HashSet<Guid>();

            var all = this.All();

            foreach (var item in all)
            {
                Guid dataid = default(Guid);
                if (item.ContainsKey("_id"))
                {
                    var objid = item["_id"];
                    Guid.TryParse(objid.ToString(), out dataid);
                }

                if (dataid == default(Guid))
                {
                    continue;
                }

                if (!exclitems.ContainsKey(dataid))
                {
                    destTable.Add(item);
                }
                else
                {
                    var exclitem = exclitems[dataid];

                    if (exclitem.EditType == EditType.Update || exclitem.EditType == EditType.Delete)
                    {
                        var lastlog = this.OwnerDatabase.Log.GetPreviousTableLog(exclitem);

                        var value = this.GetLogData(lastlog);

                        var old = this.Get(dataid);
                        if (old == null)
                        {
                            destTable.Add(value);
                        }
                        else
                        {
                            destTable.Update(dataid, value);
                        }
                    }
                    processed.Add(dataid);
                }
            }



            foreach (var item in exclitems)
            {
                if (!processed.Contains(item.Key))
                {
                    var logitem = item.Value;

                    if (logitem.EditType == EditType.Update || logitem.EditType == EditType.Delete)
                    {
                        var lastlog = this.OwnerDatabase.Log.GetPreviousTableLog(logitem);

                        if (lastlog != null)
                        {
                            var value = this.GetLogData(lastlog);
                            if (value != null)
                            {
                                destTable.Update(value);
                            }
                        }
                    }
                }

            }
        }

        private Dictionary<Guid, LogEntry> GetExclItems(List<LogEntry> logs)
        {
            Dictionary<Guid, LogEntry> result = new Dictionary<Guid, LogEntry>();

            foreach (var item in logs.OrderBy(o => o.TimeTick))
            {
                var key = ObjectContainer.GuidConverter.FromByte(item.KeyBytes);

                if (!result.ContainsKey(key))
                {
                    result[key] = item;
                }
            }
            return result;
        }

    }

    #endregion
}

