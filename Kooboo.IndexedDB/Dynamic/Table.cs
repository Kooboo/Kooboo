//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kooboo.IndexedDB.Dynamic
{
    public class Table
    {
        public Setting Setting { get; set; }

        public Guid CurrentUserId { get; set; }

        internal object _Locker = new object();

        public Converter.ObjectConverter ObjectConverter { get; set; }

        private BlockFile _blockfile;
        private object _lockerBlockFile = new object();
        public BlockFile BlockFile
        {
            get
            {
                if (_blockfile == null)
                {
                    lock (_lockerBlockFile)
                    {
                        if (_blockfile == null)
                        {
                            string blockfileName = System.IO.Path.Combine(this.ObjectFolder, "table.block");
                            _blockfile = new BlockFile(blockfileName);
                            _blockfile.OpenOrCreate();
                        }
                    }
                }
                return _blockfile;
            }
        }

        internal Btree.BtreeIndex<Guid> PrimaryBtree
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
                return this.PrimaryBtree.Count(false);
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
                var keyconverter = ObjectContainer.GetConverter<Guid>();

                foreach (var item in this.PrimaryBtree.AllKeyBytesCollection(true))
                {
                    var key = keyconverter.FromByte(item);
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
                return this.PrimaryBtree.FirstKey;
            }
        }

        public Guid LastKey
        {
            get
            {
                return this.PrimaryBtree.LastKey;
            }
        }

        private string PrimaryKey { get; set; }

        public Table(string name, Database ownerdatabase, Setting setting)
        {
            this.Name = name;
            this.ObjectFolder = ownerdatabase.TableFolder(name);
            this.SettingFile = ownerdatabase.TableSetitingFile(name);

            setting = SettingHelper.GetOrSetTableSetting(this.SettingFile, setting);

            this.OwnerDatabase = ownerdatabase;
            Init(setting);
        }
        public Table(string name, Database ownerdatabase)
        {
            Setting setting = null;
            this.Name = name;
            this.ObjectFolder = ownerdatabase.TableFolder(name);
            this.SettingFile = ownerdatabase.TableSetitingFile(name);

            setting = SettingHelper.GetOrSetTableSetting(this.SettingFile, setting);

            this.OwnerDatabase = ownerdatabase;
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
                byte[] valuebyte = ObjectConverter.ToBytes(data);
                return this.BlockFile.Add(valuebyte, valuebyte.Length);
            }
        }

        public void RebuildTable(Setting newSetting)
        {
            var isenable = newSetting.EnableLog;

            string newname = "_koobootemp_" + System.Guid.NewGuid().ToString() + this.Name;
            var newtable = this.OwnerDatabase.GetOrCreateTable(newname, newSetting);

            newtable.Setting.EnableLog = false;

            var primaryindex = this.Indexs.Find(o => o.IsSystem);

            string errormsg = null;

            try
            {
                foreach (var item in primaryindex.AllItems(true))
                {
                    var value = this._getvalue(item);
                    newtable.Add(value);
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }

            if (errormsg != null)
            {
                this.OwnerDatabase.DeleteTable(newtable.Name);
                throw new Exception(errormsg);
            }

            string oldfolder = this.ObjectFolder;
            string newfolder = newtable.ObjectFolder;

            this.Close();
            newtable.Close();
            this.DelSelf();
            this._blockfile = null;
            System.IO.Directory.Move(newfolder, oldfolder);


            newtable.Setting.EnableLog = isenable;

            SettingHelper.WriteSetting(this.SettingFile, newSetting);
            Init(newSetting);

            if (System.IO.Directory.Exists(newfolder))
            {
                System.IO.Directory.Delete(newfolder, true);
            }

            this.OwnerDatabase.openTableList.Remove(newname);
        }

        public void UpdateSetting(Setting newsetting)
        {
            ensureincremental(newsetting);

            var check = SettingHelper.UpdateSetting(newsetting.Columns.ToList(), this.Setting);

            lock (_Locker)
            {
                if (check.ShouldRebuild)
                {
                    RebuildTable(check.NewSetting);
                }
                else if (check.HasChange)
                {
                    // reinit index... and try to find remove or create index. 

                    var newindexs = IndexHelper.CreatIndexs(check.NewSetting, this.ObjectFolder);

                    foreach (var item in newindexs)
                    {
                        item.Close();
                    }

                    List<string> toremove = new List<string>();

                    foreach (var item in this.Indexs)
                    {
                        var newer = newindexs.Find(o => o.FieldName == item.FieldName);
                        if (newer == null)
                        {
                            toremove.Add(item.FieldName);
                        }
                    }

                    Dictionary<string, bool> indexToRebuild = new Dictionary<string, bool>();

                    foreach (var item in newindexs)
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

                    foreach (var item in toremove)
                    {
                        this.RemoveIndex(item);
                    }

                    this.Setting = check.NewSetting;

                    SettingHelper.WriteSetting(this.SettingFile, this.Setting);

                    var primary = newindexs.Find(o => o.IsPrimaryKey);

                    this.ObjectConverter = new Dynamic.Converter.ObjectConverter(this.Setting.Columns.ToList(), primary.FieldName);

                    this.PrimaryKey = primary.FieldName;

                    foreach (var item in this.Indexs)
                    {
                        item.Close();
                    }

                    this.Indexs = newindexs;

                    foreach (var item in this.Indexs)
                    {
                        item.Close();
                    }

                }

                this.Close();
            }
        }

        private void ensureincremental(Setting newsetting)
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

        public Dictionary<string, object> PrepareData(object dataobj, bool Update = false)
        {
            // prepare key...
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            IDictionary<string, object> dynamicobj = null;
            Type objecttype = null;

            if (idict == null)
            {
                dynamicobj = dataobj as IDictionary<string, object>;
            }

            if (idict == null && dynamicobj == null)
            {
                objecttype = dataobj.GetType();
            }

            foreach (var item in this.Setting.Columns)
            {
                if (item.IsSystem)
                {
                    // the only system field is the id fields. 
                    object Value = null;

                    if (this.PrimaryKey != Dynamic.Constants.DefaultIdFieldName)
                    {

                        if (idict != null)
                        {
                            Value = Accessor.GetValueIDict(idict, this.PrimaryKey, item.ClrType);

                        }
                        else if (dynamicobj != null)
                        {
                            Value = Accessor.GetValue(dynamicobj, this.PrimaryKey, item.ClrType);
                        }
                        else
                        {
                            Value = Accessor.GetValue(dataobj, objecttype, this.PrimaryKey, item.ClrType);
                        }

                        if (Value == null)
                        {
                            var col = this.Setting.Columns.First(o => o.Name == this.PrimaryKey);
                            if (col != null)
                            {
                                Value = IndexHelper.DefaultValue(col.ClrType);
                            }
                        }
                    }
                    if (Value == null || _ParseKey(Value) == default(Guid))
                    {

                        if (idict != null)
                        {
                            Value = Accessor.GetValueIDict(idict, item.Name, item.ClrType);

                        }
                        else if (dynamicobj != null)
                        {
                            Value = Accessor.GetValue(dynamicobj, item.Name, item.ClrType);
                        }
                        else
                        {
                            Value = Accessor.GetValue(dataobj, objecttype, item.Name, item.ClrType);
                        }
                    }

                    if (Value == null || _ParseKey(Value) == default(Guid))
                    {
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

                    if (idict != null)
                    {
                        Value = Accessor.GetValueIDict(idict, item.Name, item.ClrType);

                    }
                    else if (dynamicobj != null)
                    {
                        Value = Accessor.GetValue(dynamicobj, item.Name, item.ClrType);
                    }
                    else
                    {
                        Value = Accessor.GetValue(dataobj, objecttype, item.Name, item.ClrType);
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

                    // var rightvalue = Accessor.ChangeType(Value, item.ClrType);  
                    data.Add(item.Name, Value);
                }
            }
            return data;
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

                        // throw new Exception("Value not provided for Index: " + item.FieldName);
                    }

                    //    if (IndexHelper.IsDefaultValue(value, item.keyType))
                    //   {
                    //  throw new Exception("Value not provided for Index: " + item.FieldName);
                    // TODO: think of whether this is needed or not. 
                    //  }

                    var block = item.Get(value);
                    if (block > 0)
                    {
                        throw new Exception("Uniqueness contraints validation failed. Index: " + item.FieldName);
                    }
                }
                else
                {
                    throw new Exception("Value not provided for index: " + item.FieldName);
                }
            }
        }

        public Guid Add(object Value, bool CheckCol = false, Action<long> CallBackPos = null)
        {
            lock (_Locker)
            {
                if (CheckCol)
                {
                    var check = SettingHelper.CompareSetting(Value, this.Setting);

                    if (check.ShouldRebuild)
                    {
                        RebuildTable(check.NewSetting);
                        return Add(Value);
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

                CheckAddConstraints(data);

                Int64 contentposition = _addBlock(data);

                if (CallBackPos != null)
                {
                    CallBackPos(contentposition);
                }


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

                    item.Add(key, contentposition);
                }

                this.Close();

                Guid ReturnId = default(Guid);

                if (data.ContainsKey(Constants.DefaultIdFieldName))
                {
                    ReturnId = (Guid)data[Constants.DefaultIdFieldName];

                    if (this.Setting.EnableLog)
                    {
                        var logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

                        var logpos = AddLogData(logid, data);

                        var log = new LogEntry()
                        {
                            Id = logid,
                            EditType = EditType.Add,
                            OldBlockPosition = logpos,
                            NewBlockPosition = logpos,
                            UserId = this.CurrentUserId,
                            TableName = this.Name,
                            UpdateTime = DateTime.UtcNow,
                            KeyBytes = ObjectContainer.GuidConverter.ToByte(ReturnId)
                        };

                        this.OwnerDatabase.Log.Add(log);
                    }
                }

                return ReturnId;
            }
        }

        public long AddLogData(long LogId, Dictionary<string, object> data)
        {
            byte[] valuebyte = Kooboo.IndexedDB.Serializer.Simple.TableDataLogConverter.Instance.ToBytes(data);
            int len = valuebyte.Length;

            var logidbytes = BitConverter.GetBytes(LogId);

            byte[] totalbytes = new byte[len + 8];
            System.Buffer.BlockCopy(logidbytes, 0, totalbytes, 0, 8);
            System.Buffer.BlockCopy(valuebyte, 0, totalbytes, 8, len);

            var LogPos = this.OwnerDatabase.TableLog.Add(totalbytes, totalbytes.Length);
            this.OwnerDatabase.TableLog.Close(); // relese to enable delete. 

            return LogPos;
        }

        public Dictionary<string, object> GetLogData(LogEntry log)
        {
            return GetLogData(log.Id, log.NewBlockPosition);
        }

        public Dictionary<string, object> GetLogData(long LogId, long DiskPosition)
        {
            var databytes = this.OwnerDatabase.TableLog.Get(DiskPosition);

            if (databytes != null)
            {
                var dbLogId = BitConverter.ToInt64(databytes, 0);
                if (dbLogId == LogId)
                {
                    var valuebytes = new byte[databytes.Length - 8];
                    System.Buffer.BlockCopy(databytes, 8, valuebytes, 0, databytes.Length - 8);

                    return Kooboo.IndexedDB.Serializer.Simple.TableDataLogConverter.Instance.FromBytes(valuebytes);
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
            string strkey = key.ToString();
            Guid guidkey;
            if (System.Guid.TryParse(strkey, out guidkey))
            {
                return guidkey;
            }
            else
            {
                return Helper.KeyHelper.ComputeGuid(strkey);
            }
        }

        public IDictionary<string, object> Get(Guid key)
        {
            Int64 blockposition;

            var primary = this.Indexs.Find(o => o.IsSystem);
            blockposition = primary.Get(key);

            if (blockposition > 0)
            {
                return _getvalue(blockposition);
            }
            return null;
        }

        public long GetDiskPos(object key)
        {
            var guidkey = _ParseKey(key);
            return GetDiskPos(guidkey);
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
            var guidkey = _ParseKey(key);
            return Get(guidkey);
        }


        public T Get<T>(object key)
        {
            var guidkey = _ParseKey(key);

            Int64 blockposition;

            var primary = this.Indexs.Find(o => o.IsSystem);
            blockposition = primary.Get(guidkey);

            if (blockposition > 0)
            {
                return _getvalue<T>(blockposition);
            }
            return default(T);
        }

        internal IDictionary<string, object> _getvalue(Int64 blockposition)
        {
            lock (_Locker)
            {
                byte[] contentbytes = this.BlockFile.Get(blockposition);
                if (contentbytes != null)
                {
                    return this.ObjectConverter.FromBytes(contentbytes);
                }
                return null;
            }
        }

        internal T _getvalue<T>(Int64 blockposition)
        {
            lock (_Locker)
            {
                byte[] contentbytes = this.BlockFile.Get(blockposition);
                if (contentbytes != null)
                {
                    return this.ObjectConverter.FromBytes<T>(contentbytes);
                }
                return default(T);
            }
        }

        public bool Delete(object key)
        {
            var guidkey = _ParseKey(key);

            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);
                List<Int64> blocklist = primary.Del(guidkey);

                if (blocklist.Count == 0)
                {
                    // key not found, not delete. 
                    return false;
                }

                var blockposition = blocklist[0];
                var value = this._getvalue(blockposition);

                if (value == null)
                {
                    return false;
                }

                var data = this.PrepareData(value, true);

                foreach (var item in this.Indexs)
                {
                    if (item.FieldName != primary.FieldName)
                    {
                        if (data.ContainsKey(item.FieldName))
                        {
                            var keyvalue = data[item.FieldName];
                            item.Del(keyvalue, blockposition);
                        }
                    }
                }



                if (this.Setting.EnableLog)
                {
                    var logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

                    var diskposition = this.AddLogData(logid, data);

                    var log = new LogEntry()
                    {
                        Id = logid,
                        EditType = EditType.Delete,
                        UserId = this.CurrentUserId,
                        TableName = this.Name,
                        UpdateTime = DateTime.UtcNow,
                        OldBlockPosition = diskposition,
                        NewBlockPosition = diskposition,
                        KeyBytes = ObjectContainer.GuidConverter.ToByte(guidkey)
                    };

                    this.OwnerDatabase.Log.Add(log);
                }


                this.Close();

                return true;
            }
        }

        private void CheckUpdateConstraints(IDictionary<string, object> olddata, Dictionary<string, object> newdata)
        {
            AssignIncremental(olddata, newdata);

            foreach (var item in this.Indexs.Where(o => o.IsUnique && o.FieldName != Constants.DefaultIdFieldName))
            {
                if (olddata.ContainsKey(item.FieldName))
                {
                    if (newdata.ContainsKey(item.FieldName))
                    {

                        var oldkey = olddata[item.FieldName];
                        var newkey = newdata[item.FieldName];

                        if (oldkey.ToString() != newkey.ToString())
                        {
                            var newobj = item.Get(newkey);
                            if (newobj > 0)
                            {
                                throw new Exception("Uniqueness contraints validation failed. Index: " + item.FieldName);
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


        public bool UpdateOrAdd(object newvalue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsPrimaryKey);
                if (primary.IsSystem)
                {
                    throw new Exception("Update requires a primary key");
                }

                var newdata = this.PrepareData(newvalue, true);

                if (newdata.ContainsKey(Constants.DefaultIdFieldName))
                {
                    var key = newdata[Constants.DefaultIdFieldName];
                    var id = _ParseKey(key);

                    var sys = this.Indexs.Find(o => o.IsSystem);

                    var blockposition = sys.Get(id);

                    if (blockposition <= 0)
                    {
                        var newid = this.Add(newvalue);
                        return newid != default(Guid);
                    }
                    else
                    {
                        var oldvalue = this._getvalue(blockposition);

                        if (IsPrimaryKeyChange(oldvalue, newdata))
                        {
                            throw new Exception("primary key can not be changed or null");
                        }

                        newdata[Dynamic.Constants.DefaultIdFieldName] = key;


                        if (oldvalue != null && newvalue is IDictionary<string, object>)
                        {
                            var newdict = newvalue as IDictionary<string, object>;

                            foreach (var item in oldvalue)
                            {
                                if (!newdict.ContainsKey(item.Key))
                                {
                                    newdata[item.Key] = item.Value;
                                }
                            }
                        }


                        CheckUpdateConstraints(oldvalue, newdata);

                        return UpdateNewData(newdata, id, blockposition, oldvalue);

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
            var primarykey = this.Indexs.Find(o => o.IsPrimaryKey);
            if (primarykey != null && !primarykey.IsSystem)
            {
                var field = primarykey.FieldName;

                if (old.ContainsKey(field))
                {
                    if (newer.ContainsKey(field))
                    {
                        var oldkey = old[field];
                        var newkey = newer[field];
                        if (oldkey.ToString() != newkey.ToString())
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

        public bool Update(Guid key, object newvalue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);

                var blockposition = primary.Get(key);

                if (blockposition <= 0)
                {
                    return false;
                }

                var oldvalue = this._getvalue(blockposition);

                long newblock = blockposition;

                var newdata = this.PrepareData(newvalue, true);

                // verify that update does not change primary key. 
                if (IsPrimaryKeyChange(oldvalue, newdata))
                {
                    throw new Exception("Primary key can not be changed or null");
                }

                newdata[Dynamic.Constants.DefaultIdFieldName] = key;

                if (oldvalue != null && newvalue is IDictionary<string, object>)
                {
                    var newdict = newvalue as IDictionary<string, object>;

                    foreach (var item in oldvalue)
                    {
                        if (!newdict.ContainsKey(item.Key))
                        {
                            newdata[item.Key] = item.Value;
                        }
                    }
                }

                CheckUpdateConstraints(oldvalue, newdata);

                return UpdateNewData(newdata, key, blockposition, oldvalue);
            }

        }


        public bool Update(object newvalue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsPrimaryKey);
                if (primary.IsSystem)
                {
                    throw new Exception("Update requires a primary key");
                }

                var newdata = this.PrepareData(newvalue, true);

                if (newdata.ContainsKey(Constants.DefaultIdFieldName))
                {
                    var key = newdata[Constants.DefaultIdFieldName];
                    var id = _ParseKey(key);

                    var sys = this.Indexs.Find(o => o.IsSystem);

                    var blockposition = sys.Get(id);

                    if (blockposition <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        var oldvalue = this._getvalue(blockposition);

                        // verify that update does not change primary key. 
                        if (IsPrimaryKeyChange(oldvalue, newdata))
                        {
                            throw new Exception("primary key can not be changed or null");
                        }

                        newdata[Dynamic.Constants.DefaultIdFieldName] = key;

                        if (oldvalue != null && newvalue is IDictionary<string, object>)
                        {
                            var newdict = newvalue as IDictionary<string, object>;

                            foreach (var item in oldvalue)
                            {
                                if (!newdict.ContainsKey(item.Key))
                                {
                                    newdata[item.Key] = item.Value;
                                }
                            }
                        }

                        CheckUpdateConstraints(oldvalue, newdata);

                        return UpdateNewData(newdata, id, blockposition, oldvalue);
                    }
                }
                else
                {
                    throw new Exception("Update requires a primary key");
                }
            }
        }

        private bool UpdateNewData(Dictionary<string, object> newdata, Guid id, long blockposition, IDictionary<string, object> oldvalue)
        {
            bool UpdateOk = false;

            long newblock = blockposition;

            byte[] valuebyte = ObjectConverter.ToBytes(newdata);

            int tolanrance = this.BlockFile.GetTolerance(blockposition);

            if (tolanrance > valuebyte.Length)
            {
                var oldvaluebytes = this.BlockFile.Get(blockposition);

                var oldhash = Helper.KeyHelper.ComputeGuid(oldvaluebytes);
                var newhash = Helper.KeyHelper.ComputeGuid(valuebyte);

                if (oldhash == newhash)
                {
                    return false;
                }
                else
                {
                    this.BlockFile.UpdateBlock(valuebyte, blockposition);
                    UpdateOk = true;
                }
            }
            else
            {
                newblock = this.BlockFile.Add(valuebyte, valuebyte.Length);
                UpdateOk = true;
            }

            foreach (var item in this.Indexs)
            {
                if (oldvalue.ContainsKey(item.FieldName))
                {
                    var old = oldvalue[item.FieldName];
                    var newer = newdata[item.FieldName];
                    item.Update(old, newer, blockposition, newblock);
                }
                else
                {
                    var newer = newdata[item.FieldName];
                    item.Update(newer, blockposition, newblock);
                }
            }

            this.Close();

            CreateUpdateLog(newdata, id);
            return UpdateOk;
        }

        private void CreateUpdateLog(Dictionary<string, object> data, Guid key, string colname = null)
        {
            if (!this.Setting.EnableLog)
            {
                return;
            }
            var logid = this.OwnerDatabase.Log.GetNewLogId(this.OwnerDatabase.Name);

            var LogPos = this.AddLogData(logid, data);

            var log = new LogEntry()
            {
                Id = logid,
                EditType = EditType.Update,
                OldBlockPosition = LogPos,
                NewBlockPosition = LogPos,
                UserId = this.CurrentUserId,
                TableName = this.Name,
                TableColName = colname,
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

                    var rightvalue = Convert.ChangeType(next, item.ClrType);

                    newer[item.Name] = rightvalue;
                }
            }
        }

        public void Update(object key, object newvalue)
        {
            var tkey = _ParseKey(key);
            Update(tkey, newvalue);
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
                var value = _getvalue<T>(item);
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

            var col = this.Setting.Columns.FirstOrDefault(o => o.Name == ColumnName);

            if (col == null)
            {
                throw new Exception("Column name not found or data type not match");
            }

            var coltype = Helper.TypeHelper.GetType(col.DataType);
            var colvalue = Convert.ChangeType(value, coltype);
            var fieldConverter = this.ObjectConverter.Fields.Find(o => o.FieldName == col.Name);


            if (coltype == null || fieldConverter == null)
            {
                throw new Exception(col.DataType + " data type or field converter not found");
            }

            if (col.Length == int.MaxValue)
            {
                throw new Exception("columns wiht unlimited length can not be update by this method");
            }

            var valuebytes = fieldConverter.ToBytes(colvalue);

            valuebytes = Helper.KeyHelper.AppendToKeyLength(valuebytes, coltype == typeof(string), col.Length);

            lock (_Locker)
            {
                var guidkey = _ParseKey(key);

                Int64 blockposition;

                var primary = this.Indexs.Find(o => o.IsSystem);
                blockposition = primary.Get(key);

                if (blockposition > 0)
                {
                    var ok = this.BlockFile.UpdateCol(blockposition, col.relativePosition, col.Length, valuebytes);

                    this.BlockFile.Close();

                    if (ok)
                    {

                        Dictionary<string, object> coldata = new Dictionary<string, object>();
                        coldata.Add(ColumnName, colvalue);
                        CreateUpdateLog(coldata, guidkey, ColumnName);
                    }
                    return ok;
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
                if (this._blockfile != null)
                {
                    this._blockfile.Close();
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
                if (this._blockfile != null)
                {
                    this._blockfile.Flush();
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

                if (this._blockfile != null)
                {
                    this._blockfile.Close();

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

