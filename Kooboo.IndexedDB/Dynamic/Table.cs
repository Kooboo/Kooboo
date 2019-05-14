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
            this.ObjectFolder = ownerdatabase.objectFolder(name);
            this.SettingFile = ownerdatabase.SettingFile(name, "table.config");

            setting = SettingHelper.GetOrSetTableSetting(this.SettingFile, setting);

            this.OwnerDatabase = ownerdatabase;
            Init(setting);
        }
        public Table(string name, Database ownerdatabase)
        {
            Setting setting = null;
            this.Name = name;
            this.ObjectFolder = ownerdatabase.objectFolder(name);
            this.SettingFile = ownerdatabase.SettingFile(name, "table.config");

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
            string newname = "_koobootemp_" + System.Guid.NewGuid().ToString() + this.Name;
            var newtable = this.OwnerDatabase.GetOrCreateTable(newname, newSetting);
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
                        if (Value == null || Accessor.ChangeType<long>(Value) ==0)
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

                if (data.ContainsKey(Constants.DefaultIdFieldName))
                {
                    return (Guid)data[Constants.DefaultIdFieldName];
                }

            }
            return default(Guid);
        }

        private Guid _ParseKey(object key)
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

        public void Delete(object key)
        {
            var guidkey = _ParseKey(key);

            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);
                List<Int64> blocklist = primary.Del(guidkey);

                if (blocklist.Count == 0)
                {
                    // key not found, not delete. 
                    return;
                }

                var blockposition = blocklist[0];
                var value = this._getvalue(blockposition);

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

                this.Close();
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

        public void Update(Guid key, object newvalue)
        {
            lock (_Locker)
            {
                var primary = this.Indexs.Find(o => o.IsSystem);

                var blockposition = primary.Get(key);

                if (blockposition <= 0)
                {
                    return;
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

                byte[] valuebyte = ObjectConverter.ToBytes(newdata);

                int tolanrance = this.BlockFile.GetTolerance(blockposition);

                if (tolanrance > valuebyte.Length)
                {
                    this.BlockFile.UpdateBlock(valuebyte, blockposition);
                }
                else
                {
                    newblock = this.BlockFile.Add(valuebyte, valuebyte.Length);
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
            }
        }

        public void UpdateOrAdd(object newvalue)
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
                        this.Add(newvalue);
                    }
                    else
                    {
                        var oldvalue = this._getvalue(blockposition);

                        long newblock = blockposition;

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

                        byte[] valuebyte = ObjectConverter.ToBytes(newdata);

                        int tolanrance = this.BlockFile.GetTolerance(blockposition);

                        if (tolanrance > valuebyte.Length)
                        {
                            this.BlockFile.UpdateBlock(valuebyte, blockposition);
                        }
                        else
                        {
                            newblock = this.BlockFile.Add(valuebyte, valuebyte.Length);
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
                    }
                }
                else
                {
                    throw new Exception("Update requires a primary key");
                }

                this.Close();
            }
        }

        public void Update(object newvalue)
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
                        throw new Exception("No record found to be updated");
                    }
                    else
                    {
                        var oldvalue = this._getvalue(blockposition);

                        long newblock = blockposition;

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

                        byte[] valuebyte = ObjectConverter.ToBytes(newdata);

                        int tolanrance = this.BlockFile.GetTolerance(blockposition);

                        if (tolanrance > valuebyte.Length)
                        {
                            this.BlockFile.UpdateBlock(valuebyte, blockposition);
                        }
                        else
                        {
                            newblock = this.BlockFile.Add(valuebyte, valuebyte.Length);
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
                    }
                }
                else
                {
                    throw new Exception("Update requires a primary key");
                }
            }
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

        public void UpdateColumn(object key, string ColumnName, object value)
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
                    this.BlockFile.UpdateCol(blockposition, col.relativePosition, col.Length, valuebytes);
                }
            }
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



        #endregion
    }
}
