//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.IndexedDB
{
    public class Database
    {
        private EditLog _log;
        public EditLog Log
        {
            get
            {
                if (_log == null)
                {
                    _log = new EditLog(this);
                }
                return _log;
            }
        }

        private BlockFile _tablelog;

        public BlockFile TableLog
        {
            get
            {
                if (_tablelog == null)
                {
                    lock (_locker)
                    {
                        if (_tablelog == null)
                        {
                            string tablefolder = this.AbsolutePath;
                            if (!string.IsNullOrWhiteSpace(TablePath))
                            {
                                tablefolder = System.IO.Path.Combine(tablefolder, TablePath);
                            }

                            string folder = System.IO.Path.Combine(tablefolder, GlobalSettings.TableLogName);

                            string blockfileName = System.IO.Path.Combine(folder, "table.log");

                            var tablelog = new BlockFile(blockfileName);
                            tablelog.OpenOrCreate();
                            _tablelog = tablelog;
                        }
                    }
                }
                return _tablelog;
            }
        }

        private string _tablePrefixFolder;
        public string TablePath
        {
            get
            {
                if (_tablePrefixFolder == null)
                {
                    return "Tables";
                }
                return _tablePrefixFolder;
            }
            set
            {
                _tablePrefixFolder = value;
            }
        }

        public Database(string databaseName)
        {
            this.Name = databaseName.ToValidPath();
        }

        public string Name { get; private set; }

        private string _absolutepath;

        /// <summary>
        /// the absolute path/folder of this database. 
        /// </summary>
        public string AbsolutePath
        {
            get
            {
                if (_absolutepath == null)
                {
                    if (System.IO.Path.IsPathRooted(this.Name))
                    {
                        _absolutepath = this.Name;
                    }
                    else
                    {
                        _absolutepath = System.IO.Path.Combine(GlobalSettings.RootPath, Name);
                    }
                }
                return _absolutepath;
            }
        }

        internal Dictionary<string, IObjectStore> openStoreList { get; set; } = new Dictionary<string, IObjectStore>(StringComparer.OrdinalIgnoreCase);

        internal Dictionary<string, ISequence> openSequenceList { get; set; } = new Dictionary<string, ISequence>(StringComparer.OrdinalIgnoreCase);

        internal Dictionary<string, Dynamic.Table> openTableList { get; set; } = new Dictionary<string, Dynamic.Table>(StringComparer.OrdinalIgnoreCase);

        private object _locker = new object();

        public bool Exists
        {
            get
            {
                return System.IO.Directory.Exists(this.AbsolutePath);
            }
        }

        public bool HasObjectStore(string objectStoreName)
        {
            objectStoreName = objectStoreName.ToValidPath();

            if (openStoreList.ContainsKey(objectStoreName))
            {
                return true;
            }

            string folder = objectFolder(objectStoreName);

            if (!System.IO.Directory.Exists(folder))
            {
                return false;
            }

            string storesetting = StoreSetitingFile(objectStoreName);

            return System.IO.File.Exists(storesetting);
        }


        public bool HasTable(string talbeName)
        {
            talbeName = talbeName.ToValidPath();

            if (this.openTableList.ContainsKey(talbeName))
            {
                return true;
            }

            string folder = TableFolder(talbeName);

            if (!System.IO.Directory.Exists(folder))
            {
                return false;
            }

            string tableSetting = TableSetitingFile(talbeName);

            return System.IO.File.Exists(tableSetting);
        }


        public ObjectStore<Tkey, TValue> GetOrCreateObjectStore<Tkey, TValue>(string StoreName, ObjectStoreParameters Parameters = null)
        {
            StoreName = StoreName.ToValidPath();

            if (this.HasObjectStore(StoreName))
            {
                return this.GetObjectStore<Tkey, TValue>(StoreName);
            }
            else
            {
                lock (_locker)
                {
                    if (!HasObjectStore(StoreName))
                    {
                        // Should be removed. 
                        #region TempUpgrade2017end

                        var oldsettingfile = objectSettingFile(StoreName);
                        if (System.IO.File.Exists(oldsettingfile))
                        {
                            var oldsetting = SettingSerializer.DeserializeObjectStoreSetting(oldsettingfile);

                            if (oldsetting.UseDefaultNETBinaryFormater)
                            {
                                //clean everything. does not want to upgrade those....  
                                var folder = objectFolder(StoreName);
                                System.IO.Directory.Delete(folder, true);
                                return GetOrCreateObjectStore<Tkey, TValue>(StoreName, Parameters);
                            }

                            int keyoffset = oldsetting.primaryKeyLength;
                            foreach (var item in oldsetting.ColumnList)
                            {

                                if (item.KeyType == typeof(string))
                                {
                                    keyoffset += item.length;
                                }
                                else
                                {
                                    keyoffset += Helper.KeyHelper.GetKeyLen(item.KeyType, item.length);
                                }
                            }

                            var oldfolder = this.objectFolder(StoreName);

                            // primary key. 
                            var primaryIndex = new BTree.BTreeIndex<Tkey>(oldsetting.primaryKeyFieldName, true, oldsetting.primaryKeyLength, Helper.IndexHelper.GetIndexFileName(oldfolder, oldsetting.primaryKeyFieldName), oldsetting.MaxCacheLevel);

                            string blockfileName = System.IO.Path.Combine(oldfolder, "Data.block");
                            var blockfile = new BlockFile(blockfileName);
                            blockfile.OpenOrCreate();

                            var converter = new Serializer.Simple.SimpleConverter<TValue>();

                            string newname = "_koobootemp" + StoreName;

                            var newfolder = this.objectFolder(newname);

                            var upStore = this.GetOrCreateObjectStore<Tkey, TValue>(newname, Parameters);

                            lock (_locker)
                            {
                                foreach (var item in primaryIndex.allItemCollection(true))
                                {
                                    var bytes = blockfile.GetContent(item, keyoffset);
                                    var newobj = converter.FromBytes(bytes);
                                    upStore.add(newobj);
                                }

                                blockfile.Close();
                                primaryIndex.Close();
                                upStore.Close();

                                this.DeleteObjectStore(StoreName);

                                if (System.IO.Directory.Exists(oldfolder))
                                {
                                    System.IO.Directory.Delete(oldfolder, true);
                                }

                                System.IO.Directory.Move(newfolder, oldfolder);

                                if (this.openStoreList.ContainsKey(StoreName))
                                {
                                    this.openStoreList.Remove(StoreName);
                                }
                            }

                            return this.GetOrCreateObjectStore<Tkey, TValue>(StoreName, Parameters);
                        }

                        #endregion

                        clearOpenStoreCache();
                        if (Parameters == null)
                        {
                            Parameters = new ObjectStoreParameters();
                        }

                        ObjectStore<Tkey, TValue> newstore = new ObjectStore<Tkey, TValue>(StoreName, this, Parameters);

                        this.openStoreList.Add(StoreName, newstore);
                    }
                }

                return this.openStoreList[StoreName] as ObjectStore<Tkey, TValue>;
            }
        }

        public Dynamic.Table GetOrCreateTable(string name, Dynamic.Setting setting = null)
        {
            if (!Kooboo.IndexedDB.Helper.CharHelper.IsValidTableName(name))
            {
                throw new Exception("Only Alphanumeric are allowed to use as table name");
            }

            if (!this.openTableList.ContainsKey(name))
            {
                lock (_locker)
                {
                    if (!this.openTableList.ContainsKey(name))
                    {
                        var table = new Dynamic.Table(name, this, setting);
                        this.openTableList[name] = table;
                    }
                }
            }
            return this.openTableList[name];
        }

        public Dynamic.Table GetTable(string name)
        {
            if (!HasTable(name))
            {
                return null;
            }

            return GetOrCreateTable(name);
        }


        public void DeleteTable(string name)
        {
            if (!Kooboo.IndexedDB.Helper.CharHelper.IsValidTableName(name))
            {
                throw new Exception("Only Alphanumeric are allowed to use as table name");
            }

            lock (_locker)
            {
                if (openTableList.ContainsKey(name))
                {
                    var table = openTableList[name];
                    table.DelSelf();
                    openTableList.Remove(name);
                }
                else
                {
                    string folder = this.TableFolder(name);
                    if (System.IO.Directory.Exists(folder))
                    {
                        System.IO.Directory.Delete(folder, true);
                    }
                }
            }
        }

        public Sequence<TValue> GetSequence<TValue>(string name)
        {
            name = GetSeqScheduleFolder(name);

            if (!this.openSequenceList.ContainsKey(name))
            {
                lock (_locker)
                {
                    if (!this.openSequenceList.ContainsKey(name))
                    {
                        Sequence<TValue> log = new Sequence<TValue>(name);

                        this.openSequenceList.Add(name, log);
                    }
                }
            }
            return this.openSequenceList[name] as Sequence<TValue>;
        }
        [Obsolete]
        public Sequence<TValue> GetSequenceOld<TValue>(string name)
        {
            if (!this.openSequenceList.ContainsKey(name))
            {
                lock (_locker)
                {
                    if (!this.openSequenceList.ContainsKey(name))
                    {
                        Sequence<TValue> log = new Sequence<TValue>(name);

                        this.openSequenceList.Add(name, log);
                    }
                }
            }
            return this.openSequenceList[name] as Sequence<TValue>;
        }

        private string GetSeqScheduleFolder(string FolderName)
        {
            if (System.IO.Path.IsPathRooted(FolderName))
            {
                return FolderName;
            }
            else
            {
                string dbfolder = System.IO.Path.Combine(this.AbsolutePath, GlobalSettings.SequencePath);

                if (!System.IO.Directory.Exists(dbfolder))
                {
                    System.IO.Directory.CreateDirectory(dbfolder);
                }

                return System.IO.Path.Combine(dbfolder, FolderName + ".seq");
            }
        }


        /// <summary>
        /// get an existing object store. 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public ObjectStore<TKey, TValue> GetObjectStore<TKey, TValue>(string name)
        {
            string storename = name.ToValidPath();

            if (!HasObjectStore(storename))
            {
                throw new Exception("store: " + name + " does not exists");
            }

            if (!this.openStoreList.ContainsKey(storename))
            {
                lock (_locker)
                {
                    if (!this.openStoreList.ContainsKey(storename))
                    {
                        this.clearOpenStoreCache();

                        ObjectStore<TKey, TValue> newstore = new ObjectStore<TKey, TValue>(storename, this, new ObjectStoreParameters());
                        this.openStoreList.Add(storename, newstore);
                    }
                }
            }

            return this.openStoreList[storename] as ObjectStore<TKey, TValue>;
        }

        /// <summary>
        /// Get an new instance of the ObjectStore.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public ObjectStore<TKey, TValue> GetReadingStore<TKey, TValue>(string name, ObjectStoreParameters paras = null)
        {
            string storename = name.ToValidPath();

            if (paras == null)
            {
                paras = new ObjectStoreParameters();
            }
            if (!HasObjectStore(storename))
            {
                throw new Exception("store: " + name + " does not exists");
            }
            return new ObjectStore<TKey, TValue>(storename, this, paras);
        }

        public ObjectStore<OKey, OValue> RebuildObjectStore<OKey, OValue>(ObjectStore<OKey, OValue> currentStore, ObjectStoreParameters newparas)
        {
            string storename = currentStore.Name;
            string newname = "_koobootemp" + storename;

            if (currentStore == null)
            {
                throw new FileNotFoundException("store does not exists, name: " + Name);
            }

            if (string.IsNullOrEmpty(newparas.PrimaryKey))
            {
                throw new Exception("rebuild index requires setting the primary key field in ObjectStoreParameters.");
            }
            else
            {
                var valuetype = typeof(OValue);
                var fieldtype = valuetype.GetField(newparas.PrimaryKey);
                if (fieldtype == null)
                {
                    var property = valuetype.GetProperty(newparas.PrimaryKey);
                    if (property == null)
                    {
                        throw new Exception("rebuild index requires setting the primary key field in ObjectStoreParameters.");
                    }
                }
            }

            var newstore = this.GetOrCreateObjectStore<OKey, OValue>(newname, newparas);
            lock (_locker)
            {
                foreach (var item in currentStore.ItemCollection())
                {
                    newstore.add(item, false);
                }

                string oldfolder = currentStore.ObjectFolder;
                string newfolder = newstore.ObjectFolder;

                currentStore.Close();
                currentStore.DelSelf();
                newstore.Close();

                this.Log.DeleteByStoreName(storename);

                Directory.Move(newfolder, oldfolder);

                if (this.openStoreList.ContainsKey(storename))
                {
                    this.openStoreList.Remove(storename);
                }
            }

            newstore.Close();
            newstore = null;

            if (this.openStoreList.ContainsKey(newname))
            {
                this.openStoreList.Remove(newname);
            }

            return this.GetObjectStore<OKey, OValue>(storename);
        }


        public void RestoreFromDisk<TKey, TValue>(string StoreName)
        {
            var store = this.GetObjectStore<TKey, TValue>(StoreName);
            if (store != null)
            {
                RestoreFromDisk<TKey, TValue>(store);
            }
        }

        public ObjectStore<TKey, TValue> RestoreFromDisk<TKey, TValue>(ObjectStore<TKey, TValue> store)
        {
            string newname = "_koobootemp" + "_rebuile_" + store.Name;

            if (!store.HasPrimaryKeyDefined)
            {
                throw new Exception("restore requires setting the primary key field in ObjectStoreParameters.");
            }


            StoreRestore.RestoreTask<TKey, TValue> task = new StoreRestore.RestoreTask<TKey, TValue>(store);

            var newstore = task.RestoreTo(newname);

            lock (_locker)
            {
                if (this.openStoreList.ContainsKey(store.Name))
                {
                    this.openStoreList.Remove(store.Name);
                }

                store.Close();
                store.DelSelf();
                newstore.Close();

                this.Log.DeleteByStoreName(store.Name);

                Directory.Move(newstore.ObjectFolder, store.ObjectFolder);

                return this.GetObjectStore<TKey, TValue>(store.Name);
            }

        }

        public void DeleteObjectStore(string name)
        {

            lock (_locker)
            {
                string storename = name.ToValidPath();

                if (this.openStoreList.ContainsKey(storename))
                {
                    this.openStoreList[storename].DelSelf();
                    this.openStoreList.Remove(storename);
                }

                string storeroot = this.objectFolder(storename);

                if (System.IO.Directory.Exists(storeroot))
                {
                    System.IO.Directory.Delete(storeroot, true);
                }
            }
        }

        internal string objectFolder(string storename)
        {
            return System.IO.Path.Combine(this.AbsolutePath, storename.ToValidPath());
        }

        internal string TableFolder(string TableName)
        {
            string basefolder = null;
            string tablename = TableName.ToValidPath();

            string lowername = tablename.ToLower();

            if (string.IsNullOrWhiteSpace(TablePath))
            {
                basefolder = this.AbsolutePath;
            }
            else
            {
                basefolder = System.IO.Path.Combine(this.AbsolutePath, TablePath);
            }

            string fullpath = System.IO.Path.Combine(basefolder, tablename);


            if (!System.IO.Directory.Exists(fullpath))
            {
                // check capital letter etc.
                if (System.IO.Directory.Exists(basefolder))
                {
                    var dirinfo = new System.IO.DirectoryInfo(basefolder);
                    var subs = dirinfo.GetDirectories();
                    foreach (var item in subs)
                    {
                        if (item.Name != null && item.Name.ToLower() == lowername)
                        {
                            return item.FullName;
                        }
                    }
                }
            }

            // this directory does not existrs. 

            if (!System.IO.Directory.Exists(fullpath))
            {
                System.IO.Directory.CreateDirectory(fullpath);
            }

            return fullpath;
        }

        [Obsolete]
        internal string objectSettingFile(string storename)
        {
            string folder = objectFolder(storename);
            string filename = System.IO.Path.Combine(folder, "setting.config");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return filename;
        }

        internal string StoreSetitingFile(string storename)
        {
            string folder = objectFolder(storename);
            string filename = System.IO.Path.Combine(folder, "store.config");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return filename;
        }

        internal string TableSetitingFile(string tableName)
        {
            string folder = TableFolder(tableName);
            string filename = System.IO.Path.Combine(folder, "table.config");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return filename;
        }


        /// <summary>
        /// delete current database
        /// </summary>
        public void deleteDatabase()
        {
            lock (_locker)
            {
                this.Log.DelSelf();

                this.TableLog.DelSelf();

                foreach (var item in this.openStoreList)
                {
                    item.Value.DelSelf();
                }
                this.openStoreList.Clear();

                foreach (var item in this.openSequenceList)
                {
                    item.Value.DelSelf();
                }
                this.openSequenceList.Clear();

                foreach (var item in this.openTableList)
                {
                    item.Value.DelSelf();
                }

                this.openTableList.Clear();
                deletefolder(this.AbsolutePath);
            }
        }

        private void deletefolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                var subfolders = Directory.GetDirectories(folder);
                if (subfolders != null)
                {
                    foreach (var item in subfolders)
                    {
                        deletefolder(item);
                    }
                }

                var allfiles = Directory.GetFiles(folder);
                if (allfiles != null)
                {
                    foreach (var item in allfiles)
                    {
                        System.IO.File.Delete(item);
                    }
                }
                Directory.Delete(folder, true);
            }
        }

        /// <summary>
        ///  remove some items when too many object stores open.
        /// </summary>
        private void clearOpenStoreCache()
        {
            HashSet<string> toBeRemovedItems = new HashSet<string>();

            if (this.openStoreList.Count() > 800)
            {
                int count = 0;

                foreach (var item in this.openStoreList)
                {
                    // item.Value.Close();
                    toBeRemovedItems.Add(item.Key);
                    count += 1;
                    if (count > 300)
                    {
                        break;
                    }
                }

            }

            foreach (var item in toBeRemovedItems)
            {
                this.openStoreList.Remove(item);
            }

        }

        private List<string> _storelist;
        /// <summary>
        /// Get the list of store names within this database. 
        /// </summary>
        /// <param name="update">update the store list before return, otherwise return cached version</param>
        /// <returns></returns>
        public List<string> StoreList(bool update)
        {
            if (update)
            {
                _storelist = null;
            }

            if (_storelist == null)
            {
                _storelist = new List<string>();
                foreach (var item in System.IO.Directory.GetDirectories(this.AbsolutePath))
                {
                    string subdir = item;
                    int lastindex = Helper.PathHelper.GetLastSlash(item);
                    if (lastindex > 0)
                    {
                        subdir = subdir.Substring(lastindex + 1);
                    }

                    if (this.HasObjectStore(subdir))
                    {
                        _storelist.Add(subdir);
                    }
                }
            }

            return _storelist;

        }

        public IObjectStore GetStore(string Name)
        {
            string storename = Name.ToValidPath();

            if (!this.openStoreList.ContainsKey(storename))
            {
                lock (_locker)
                {
                    if (!this.openStoreList.ContainsKey(storename))
                    {
                        var setting = this.GetStoreSetting(storename);
                        if (setting == null)
                        {
                            return null;
                        }

                        var keytype = Helper.TypeHelper.GetFieldType(setting.ValueType, setting.PrimaryKey);

                        Type genericListType = typeof(ObjectStore<,>).MakeGenericType(keytype, setting.ValueType);

                        List<object> paras = new List<object>();
                        paras.Add(storename);
                        paras.Add(this);
                        paras.Add(setting);
                        var store = Activator.CreateInstance(genericListType, paras.ToArray()) as IObjectStore;

                        if (store == null)
                        {
                            return null;
                        }

                        this.openStoreList[storename] = store;
                    }
                }
            }

            return this.openStoreList[storename];

        }

        /// <summary>
        /// get the setting of current object store.
        /// </summary>
        /// <param name="storename"></param>
        /// <returns></returns>
        internal StoreSetting GetStoreSetting(string storename)
        {
            string settingfile = this.StoreSetitingFile(storename);
            return Helper.SettingHelper.ReadSetting(settingfile);
        }

        public void Close()
        {
            lock (_locker)
            {
                foreach (var item in this.openStoreList)
                {
                    item.Value.Close();
                }

                this.openStoreList.Clear();

                foreach (var item in this.openSequenceList)
                {
                    item.Value.Close();
                }
                this.openSequenceList.Clear();

                foreach (var item in this.openTableList)
                {
                    item.Value.Close();
                }

                this.openTableList.Clear();

                this.Log.Store.Close();
            }
        }

        public void Flush()
        {
            lock (_locker)
            {
                foreach (var item in this.openStoreList)
                {
                    item.Value.Flush();
                }

                foreach (var item in this.openSequenceList)
                {
                    item.Value.Flush();
                }

                foreach (var item in this.openTableList)
                {
                    item.Value.Flush();
                }

            }
        }

        // list of dynamic tables. 
        public List<string> GetTables()
        {
            string tablefolder = this.AbsolutePath;
            if (!string.IsNullOrWhiteSpace(TablePath))
            {
                tablefolder = System.IO.Path.Combine(tablefolder, TablePath);
            }

            if (!System.IO.Directory.Exists(tablefolder))
            {
                return new List<string>();
            }

            var subfolders = System.IO.Directory.GetDirectories(tablefolder);

            if (subfolders == null || subfolders.Count() == 0)
            {
                return new List<string>();
            }

            List<string> result = new List<string>();

            foreach (var item in subfolders)
            {
                if (item.Contains("_koobootemp_"))
                {
                    continue;
                }

                // verify as subfolder.  
                var tablepath = System.IO.Path.Combine(tablefolder, item);

                string tableSetting = System.IO.Path.Combine(tablepath, "table.config");
                if (System.IO.File.Exists(tableSetting))
                {
                    System.IO.DirectoryInfo info = new DirectoryInfo(tablepath);
                    if (info != null)
                    {
                        result.Add(info.Name);
                    }
                }
            }
            return result;
        }

    }

}
