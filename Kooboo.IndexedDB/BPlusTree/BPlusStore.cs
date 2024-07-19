//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.IndexedDB.BPlusTree;
using Kooboo.IndexedDB.ByteConverter;

namespace Kooboo.IndexedDB
{
    public class BPlusStore<TKey, TValue> where TValue : IBPlusTreeObject
    {

        internal object _Locker = new();

        internal IByteConverter<TKey> KeyConverter { get; set; }
        internal IByteConverter<TValue> ValueConverter { get; set; }


        private BlockFile _blockFile;
        private object _lockerBlockFile = new object();
        internal BlockFile BlockFile
        {
            get
            {
                if (_blockFile == null)
                {
                    lock (_lockerBlockFile)
                    {
                        if (_blockFile == null)
                        {
                            var blockFile = new BlockFile(this.BlockFileName);
                            blockFile.OpenOrCreate();
                            _blockFile = blockFile;
                        }
                    }
                }
                return _blockFile;
            }
        }

        internal BPlusTreeIndex<TKey> primaryIndex { get; set; }

        /// <summary>
        /// string key has varies length, the rest has fixed length.
        /// </summary>
        private bool IsStringKey = false;

        internal string ObjectFolder { get; set; }

        /// <summary>
        /// The name of this object store.
        /// </summary>
        public string Name { get; set; }

        internal Database OwnerDatabase { get; set; }

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

        internal string PrimaryKey { get; set; }

        private string IndexFileName;
        private string BlockFileName;

        internal int KeyLen { get; set; }

        private bool skipValueBlock { get; set; }

        private int BPlusLen { get; set; }

        public BPlusStore(string name, Database ownerDatabase, string primaryKeyField, int keyLen = 0)
        {
            var folder = OwnerDatabase.objectFolder(name);
            var fullIndexName = System.IO.Path.Combine(folder, "BPlusTree.index");
            InitByIndex(fullIndexName, primaryKeyField, keyLen);
        }

        public BPlusStore(string fullIndexFileName, string primaryKeyField, int keyLen = 0)
        {
            InitByIndex(fullIndexFileName, primaryKeyField, keyLen);
        }

        private void InitByIndex(string fullFileName, string primaryKey, int keyLen)
        {

            this.IndexFileName = fullFileName;
            var fileInfo = new System.IO.FileInfo(fullFileName);
            if (string.IsNullOrEmpty(fileInfo.Extension))
            {
                this.IndexFileName = this.IndexFileName + ".index";
                fileInfo = new System.IO.FileInfo(IndexFileName);
            }

            var file = fileInfo.FullName.Substring(0, fileInfo.FullName.Length - fileInfo.Extension.Length);

            this.BlockFileName = file + ".data";

            this.ObjectFolder = fileInfo.Directory.FullName;

            if (!System.IO.Directory.Exists(this.ObjectFolder))
            {
                System.IO.Directory.CreateDirectory(this.ObjectFolder);
            }

            this.PrimaryKey = primaryKey;

            var keytype = typeof(TKey);

            this.KeyLen = Helper.KeyHelper.GetKeyLen(keytype, keyLen);

            Helper.IndexHelper.VerifyIndexType(keytype);

            this.IsStringKey = keytype == typeof(string);

            this.skipValueBlock = Helper.TypeHelper.GetPropertyValue<bool, TValue>(nameof(IBPlusTreeObject.SkipValueBlock));
            this.BPlusLen = Helper.TypeHelper.GetPropertyValue<int, TValue>(nameof(IBPlusTreeObject.BPlusTreeLen));

            if (KeyConverter == null)
            {
                KeyConverter = ObjectContainer.GetConverter<TKey>();
            }

            if (ValueConverter == null)
            {
                ValueConverter = ObjectContainer.GetConverter<TValue>();
            }

            this.primaryIndex = new BPlusTreeIndex<TKey>(this.IndexFileName, this.KeyLen, this.BPlusLen);
        }


        private Func<TValue, TKey> _getKey;

        internal Func<TValue, TKey> GetKey
        {
            get
            {
                if (_getKey == null)
                {
                    if (string.IsNullOrWhiteSpace(this.PrimaryKey))
                    {
                        throw new Exception("Primary key field not found, you may need to define a field name _id or SetPrimaryKey for this store.");
                    }
                    else
                    {
                        _getKey = Helper.ObjectHelper.GetGetValue<TValue, TKey>(this.PrimaryKey);
                    }
                }
                return _getKey;
            }
        }

        private Action<TValue, TKey> _setKey;
        internal Action<TValue, TKey> SetKey
        {
            get
            {

                if (_setKey == null)
                {
                    if (string.IsNullOrWhiteSpace(this.PrimaryKey))
                    {
                        throw new Exception("Primary key field not found, you may need to define a field name _id or SetPrimaryKey for this store.");
                    }
                    else
                    {
                        _setKey = Helper.ObjectHelper.GetSetValue<TValue, TKey>(this.PrimaryKey);
                    }
                }
                return _setKey;

            }


        }

        public bool add(TKey key, TValue value)
        {
            long blockPosition = 0;
            if (!this.skipValueBlock)
            {
                blockPosition = addValueBlock(key, value);
            }
            var metaBytes = value.GetBPlusBytes();
            return this.primaryIndex.Add(key, blockPosition, metaBytes);

        }

        public bool add(TValue value)
        {
            TKey key = GetKey(value);
            return add(key, value);
        }


        /// <summary>
        /// delete a record
        /// </summary>
        /// <param name="key"></param>
        /// <param name="EnableLog">set to false to disable the log per records</param>
        public void delete(TKey key)
        {
            var blockPosition = this.primaryIndex.GetPosition(key);

            var ok = this.primaryIndex.Del(key);
            if (ok && blockPosition > 0)
            {
                this.BlockFile.Delete(blockPosition);
            }
        }

        public bool update(TKey key, TValue newValue)
        {
            var pointer = this.primaryIndex.GetPointer(key);

            if (pointer == null)
            {
                // not found.
                return false;
            }

            Int64 newBlockPosition = 0;

            if (!this.skipValueBlock)
            {
                newBlockPosition = addValueBlock(key, newValue);
            }

            var metaBytes = newValue.GetBPlusBytes();
            this.primaryIndex.Update(key, metaBytes, newBlockPosition);
            return true;
        }

        public bool UpdateBplus(TKey key, TValue newValue)
        {
            var pointer = this.primaryIndex.GetPointer(key);

            if (pointer == null)
            {
                // not found.
                return false;
            }

            var metaBytes = newValue.GetBPlusBytes();

            this.primaryIndex.UpdateBplus(key, metaBytes);

            if (!this.skipValueBlock && pointer.PositionPointer > 0)
            {
                this.BlockFile.UpdatePart(pointer.PositionPointer, metaBytes);
            }

            return true;

        }

        private long addValueBlock(TKey key, TValue value)
        {
            byte[] valueByte = ValueConverter.ToByte(value);

            var metaBytes = value.GetBPlusBytes();

            var totalLen = this.BPlusLen + valueByte.Length;

            var totalBytes = new byte[totalLen];

            var copyLen = BPlusLen;
            if (metaBytes.Length < copyLen)
            {
                copyLen = metaBytes.Length;
            }

            System.Buffer.BlockCopy(metaBytes, 0, totalBytes, 0, copyLen);
            System.Buffer.BlockCopy(valueByte, 0, totalBytes, BPlusLen, valueByte.Length);

            return this.BlockFile.Add(totalBytes, totalLen);

        }



        public int Count()
        {
            return this.primaryIndex.Count();
        }

        public TValue get(TKey key)
        {
            var pointer = this.primaryIndex.GetPointer(key);

            if (pointer != null)
            {
                if (pointer.PositionPointer > 0)
                {
                    return this.getValue(pointer.PositionPointer);
                }
                else
                {
                    var value = Activator.CreateInstance<TValue>();
                    value.SetBPlusBytes(pointer.BPlusBytes);

                    if (this.SetKey != null)
                    {
                        this.SetKey(value, key);
                    }
                    return value;
                }
            }

            return default;

        }


        public TValue GetByBplus(TKey key)
        {
            var pointer = this.primaryIndex.GetPointer(key);
            if (pointer != null)
            {
                var value = Activator.CreateInstance<TValue>();
                value.SetBPlusBytes(pointer.BPlusBytes);

                SetKey?.Invoke(value, key);

                return value;
            }
            return default;
        }

        public async Task<TValue> getAsync(TKey key)
        {
            Int64 blockPosition = this.primaryIndex.GetPosition(key);

            if (blockPosition > 0)
            {
                return await getValueAsync(blockPosition);
            }
            else
            {
                return default;
            }
        }


        internal TValue getValue(Int64 blockPosition)
        {
            byte[] contentBytes = this.BlockFile.Get(blockPosition);
            if (contentBytes == null)
            {
                return default;
            }
            else
            {
                var bytes = contentBytes.Skip(this.BPlusLen).ToArray();

                return ValueConverter.FromByte(bytes);
            }
        }

        internal async Task<TValue> getValueAsync(Int64 blockPosition)
        {
            byte[] contentBytes = await this.BlockFile.GetAsync(blockPosition);
            if (contentBytes == null)
            {
                return default;
            }
            else
            {
                var bytes = contentBytes.Skip(this.BPlusLen).ToArray();

                return ValueConverter.FromByte(bytes);
            }
        }

        private BPlusQuery<TKey, TValue> _query;
        public BPlusQuery<TKey, TValue> Query
        {
            get
            {
                if (_query == null)
                {
                    _query = new BPlusQuery<TKey, TValue>(this, false);
                }
                return _query;
            }
        }

        private BPlusQuery<TKey, TValue> _metaQuery;
        /// <summary>
        /// query using BPlus record..
        /// </summary>
        public BPlusQuery<TKey, TValue> BPlusQuery
        {
            get
            {
                if (_metaQuery == null)
                {
                    _metaQuery = new BPlusQuery<TKey, TValue>(this, true);
                }
                return _metaQuery;
            }
        }


        public void RestoreFromDisk()
        {
            this.OwnerDatabase.RestoreFromDisk<TKey, TValue>(this.Name);
        }

        public void Close()
        {
            lock (_Locker)
            {
                if (this._blockFile != null)
                {
                    this._blockFile.Close();
                    this._blockFile = null;
                }

                if (this.primaryIndex != null)
                {
                    this.primaryIndex.Close();
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

                if (this.primaryIndex != null)
                {
                    this.primaryIndex.Flush();
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


    }
}
