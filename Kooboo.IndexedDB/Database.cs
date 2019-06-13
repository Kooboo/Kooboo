//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

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

            string storesetting = SettingFile(objectStoreName);

            return System.IO.File.Exists(storesetting);
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
                            var primaryIndex = new Btree.BtreeIndex<Tkey>(oldsetting.primaryKeyFieldName, true, oldsetting.primaryKeyLength, Helper.IndexHelper.GetIndexFileName(oldfolder, oldsetting.primaryKeyFieldName), oldsetting.MaxCacheLevel);

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
                    string folder = this.objectFolder(name); 
                    if (System.IO.Directory.Exists(folder))
                    {
                        System.IO.Directory.Delete(folder, true);
                    }
                }
            }  
        }

        public Sequence<TValue> GetSequence<TValue>(string name)
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

                System.IO.Directory.Move(newfolder, oldfolder);

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

        internal string SettingFile(string storename, string settingfilename = null)
        {
            if (string.IsNullOrWhiteSpace(settingfilename))
            {
                settingfilename = "store.config";
            }

            string folder = objectFolder(storename);
            string filename = System.IO.Path.Combine(folder, settingfilename);

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
            string settingfile = this.SettingFile(storename);
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
           if (!System.IO.Directory.Exists(this.AbsolutePath))
            {
                return new List<string>(); 
            }

            var subfolders = System.IO.Directory.GetDirectories(this.AbsolutePath); 

            if (subfolders == null || subfolders.Count() ==0)
            {
                return new List<string>();
            }

            List<string> result = new List<string>(); 

            foreach (var item in subfolders)
            {
                // verify as subfolder.  
                var tablepath = System.IO.Path.Combine(this.AbsolutePath, item);

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
