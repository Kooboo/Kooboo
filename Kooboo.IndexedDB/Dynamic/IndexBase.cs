//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.IndexedDB.BTree;

namespace Kooboo.IndexedDB.Dynamic
{
    public class TableIndexBase<T> : ITableIndex
    {
        public int Length { get; set; }

        private Type _keytype;
        public Type keyType
        {
            get
            {
                if (_keytype == null)
                {
                    _keytype = typeof(T);
                }
                return _keytype;
            }
            set
            {
                _keytype = value;
            }
        }

        public string FieldName { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsSystem { get; set; }

        public bool IsUnique { get; set; }

        private int KeyLen { get; set; }

        public TableIndexBase(string FieldName, string indexfile, bool IsUnique, int keyLen)
        {
            this.IndexFile = indexfile;
            this.IsUnique = IsUnique;
            this.FieldName = FieldName;
            this.KeyLen = Helper.KeyHelper.GetKeyLen(this.keyType, keyLen);

            this.Seed = 1;
            this.Increment = 1;
        }

        private BTreeIndex<T> _index;

        private object _locker = new object();

        public BTreeIndex<T> index
        {
            get
            {
                if (_index == null)
                {
                    lock (_locker)
                    {
                        if (_index == null)
                        {
                            if (!string.IsNullOrEmpty(IndexFile))
                            {
                                _index = new BTreeIndex<T>(null, IsUnique, Constants.DefaultKeyLen, this.IndexFile);
                            }
                        }
                    }
                }
                return _index;
            }
        }

        public string IndexFile { get; set; }
        public bool IsIncremental { get; set; }
        public long Seed { get; set; }

        private long _increment;
        public long Increment
        {
            get
            {
                return _increment;
            }
            set { _increment = value; if (_increment <= 1) { _increment = 1; } }
        }

        public bool Add(object key, long blockPosition)
        {
            var Tkey = ParseKey(key);
            return this.index.Add(Tkey, blockPosition);
        }

        public long Get(object key)
        {
            var tkey = ParseKey(key);
            return this.index.Get(tkey);
        }

        public List<long> List(object key)
        {
            var tkey = ParseKey(key);
            return this.index.List(tkey);
        }

        private T ParseKey(object key)
        {
            var Tkey = (T)key;
            if (Tkey == null)
            {
                var newvalue = Convert.ChangeType(key, this.keyType);
                Tkey = (T)newvalue;
            }
            return Tkey;
        }

        public ItemCollection AllItems(bool ascending)
        {
            return this.index.allItemCollection(ascending);
        }

        public KeyBytesCollection AllKeys(bool ascending)
        {
            return this.index.AllKeyBytesCollection(ascending);
        }

        public void Close()
        {
            this.index.Close();
        }

        public int Count(bool distinct)
        {
            return this.index.Count(false);
        }

        public bool Del(object key, long blockPosition)
        {
            var tkey = ParseKey(key);
            return this.index.Del(tkey, blockPosition);
        }

        public List<Int64> Del(object key)
        {
            var tkey = ParseKey(key);
            return this.index.Del(tkey);
        }

        public void DelSelf()
        {
            this.index.DelSelf();
        }

        public void Flush()
        {
            this.index.Flush();
        }

        public ItemCollection GetCollection(byte[] startBytes, byte[] endBytes, bool lowerOpen, bool upperOpen, bool ascending)
        {
            return this.index.getCollection(startBytes, endBytes, lowerOpen, upperOpen, ascending);
        }

        public void Update(object oldKey, long oldBlockPosition, long newBlockPosition)
        {
            var old = ParseKey(oldKey);
            this.index.Update(old, oldBlockPosition, newBlockPosition);
        }

        public void Update(object oldKey, object newkey, long oldBlockPosition, long newBlockPosition)
        {
            var old = ParseKey(oldKey);
            var newer = ParseKey(newkey);
            this.index.Update(old, newer, oldBlockPosition, newBlockPosition);
        }

        private long CurrentSeed { get; set; } = -1;


        private object _seedlock = new object();

        public long NextIncrement()
        {
            lock (_seedlock)
            {
                if (CurrentSeed == -1)
                {
                    CurrentSeed = this.Seed;
                    // incremental must be the data type of Long. 
                    long first = (long)Convert.ChangeType(this.index.FirstKey, typeof(long));
                    if (first > CurrentSeed)
                    {
                        CurrentSeed = first;
                    }
                    long last = (long)Convert.ChangeType(this.index.LastKey, typeof(long));

                    if (last > CurrentSeed)
                    {
                        CurrentSeed = last;
                    }

                }
                CurrentSeed += this.Increment;

                long nextvalue = CurrentSeed;

                return nextvalue;
            }
        }
    }
}
